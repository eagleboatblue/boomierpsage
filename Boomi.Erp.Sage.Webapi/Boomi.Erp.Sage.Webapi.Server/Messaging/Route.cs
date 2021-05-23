using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Models.Common;
using Boomi.Erp.Sage.Webapi.Server.Interfaces;
using Boomi.Erp.Sage.Webapi.Server.Messaging.Core;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace Boomi.Erp.Sage.Webapi.Server.Messaging
{
    public class Route<TIn, TOut>
    {
        private readonly ILogger logger;
        private readonly JsonSerializer serializer;
        private readonly RouteInfo info;
        private readonly IHandler<TIn, TOut> handler;

        public Route(ILogger logger, JsonSerializer serializer, RouteInfo info, IHandler<TIn, TOut> handler)
        {
            this.logger = logger;
            this.serializer = serializer;
            this.info = info;
            this.handler = handler;
        }

        public Task Use(IConnection connection, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                using (var requestCh = connection.CreateModel())
                {
                    requestCh.BasicQos(0, 1, false);

                    var consumer = new AsyncEventingBasicConsumer(requestCh);

                    consumer.Received += async (object _, BasicDeliverEventArgs ea) =>
                    {
                        ErpRequest<TIn> payload = null;

                        // Try to deserialize body first
                        string bodyStr = string.Empty;

                        try
                        {
                            this.logger.Information("Received a new message {routingKey} {@properties}", ea.RoutingKey, ea.BasicProperties);

                            var body = ea.Body;

                            bodyStr = System.Text.Encoding.UTF8.GetString(body.ToArray());

                            using (var sr = new StringReader(bodyStr))
                            {
                                payload = this.serializer.Deserialize<ErpRequest<TIn>>(new JsonTextReader(sr));
                            }
                        }
                        catch (Exception e)
                        {
                            this.logger.Error("Failed to deserialize message body {@error} {@properties} {@body}", e, ea.BasicProperties, bodyStr);

                            requestCh.BasicAck(ea.DeliveryTag, false);

                            var jobID = ea.BasicProperties.MessageId;

                            if (!string.IsNullOrWhiteSpace(jobID))
                            {
                                this.sendToDeadletter(
                                    ea,
                                    new Failure(
                                        new Error("Failed to deserialize message body", e),
                                        new ErpRequest<dynamic>(Metadata.FromJobID(jobID), bodyStr)
                                    ),
                                    connection.CreateModel());
                            }
                            else
                            {
                                this.logger.Information("Message does not contain message id. Unable to send error to the deadletter exchange.");
                            }

                            return;
                        }

                        // If body is successfully serialized
                        // Try to call handler
                        try
                        {
                            TOut output = await this.handler.Handle(payload);
                            var response = new ErpRequest<TOut>(payload.Metadata, output);

                            var serialized = this.serialize(payload);

                            using (var responseCh = connection.CreateModel())
                            {
                                var exchangeName = this.info.Response.Exchange;
                                var queueName = ea.BasicProperties.ReplyTo ?? this.info.Response.Queue;

                                ea.BasicProperties.ReplyTo = "";

                                responseCh.BasicPublish(
                                    exchangeName,
                                    queueName,
                                    ea.BasicProperties,
                                    serialized
                                );

                                requestCh.BasicAck(ea.DeliveryTag, false);
                            }

                            this.logger.Information("Successfully handled message from {queue}", this.info.Request.Queue);
                        }
                        catch (Exception e)
                        {
                            this.logger.Error("Failed to execute event handler {@error} {@message}", payload, e);

                            requestCh.BasicAck(ea.DeliveryTag, false);
                            this.sendToDeadletter(
                                ea,
                                new Failure(
                                    new Error(e.Message, e.InnerException),
                                    new ErpRequest<dynamic>(payload.Metadata, payload.Data)
                                ),
                                connection.CreateModel()
                            );
                        }
                    };

                    try
                    {
                        this.logger.Information("Started to consume messages from a queue {@queue}", this.info.Request);

                        requestCh.BasicConsume(
                            this.info.Request.Queue,
                            false,
                            consumer
                        );

                        cancellation.WaitHandle.WaitOne();

                        this.logger.Information("Received a cancellation signal {@queue}", this.info.Request);
                    }
                    catch (Exception ex)
                    {
                        this.logger.Error("Failed to consume messages from a queue {@queue} {@error}", this.info.Request, ex);
                    }
                }
            }, cancellation);
        }

        private void sendToDeadletter(BasicDeliverEventArgs args, Failure failure, IModel ch)
        {
            try
            {
                var serialized = this.serialize(failure);

                using (ch)
                {
                    args.BasicProperties.ReplyTo = "";

                    ch.BasicPublish(
                        this.info.Deadletter.Exchange,
                        this.info.Deadletter.Queue,
                        args.BasicProperties,
                        serialized
                    );
                }
            }
            catch (Exception e)
            {
                this.logger.Error("Failed to send a failure to the dead letter queue {@error} {@message}", e, failure);
            }
        }

        private byte[] serialize<T>(T input)
        {
            using (var writer = new System.IO.StringWriter())
            {
                this.serializer.Serialize(writer, input);

                return System.Text.Encoding.UTF8.GetBytes(writer.ToString());
            }
        }
    }
}
