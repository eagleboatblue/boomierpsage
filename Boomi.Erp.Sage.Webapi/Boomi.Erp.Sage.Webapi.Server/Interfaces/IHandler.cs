using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Server.Messaging.Core;

namespace Boomi.Erp.Sage.Webapi.Server.Interfaces
{
    public interface IHandler<TIn, TOut>
    {
        Task<TOut> Handle(ErpRequest<TIn> payload);
    }
}
