using System;
using NUnit.Framework;
using Boomi.Erp.Sage.Webapi.Data.Helpers;
using System.Collections.Generic;

namespace Boomi.Erp.Sage.Webapi.Data.Tests.Helpers
{
    [TestFixture()]
    public class ParserTest
    {
        private class Data
        {
            public string Input { get; }

            public List<string> Expected { get; }

            public Data(string input, List<string> output)
            {
                this.Input = input;
                this.Expected = output;
            }
        }

        private List<Data> sentences { get; }

        public ParserTest()
        {
            this.sentences = new List<Data>()
            {
                new Data(
                    "",
                    new List<string>()
                ),
                new Data(
                    "greatbiglongunbrokenemailstringdesignedtotrickintegrationAaron.Williams2@Halliburton.com",
                    new List<string>()
                    {
                        "greatbiglongunbrokenemailstringdesignedt",
                        "otrickintegrationAaron.Williams2@Hallibu",
                        "rton.com"
                    }
                ),
                new Data(
                    "As per Altus Intervention Frame Agreement EU-GEN-2016-735 and Altus Amendment 1",
                    new List<string>() {
                        "As per Altus Intervention Frame",
                        "Agreement EU-GEN-2016-735 and Altus",
                        "Amendment 1"
                    }
                ),
                new Data(
                    "100% paid up front before launch of manufacture",
                    new List<string>() {
                        "100% paid up front before launch of",
                        "manufacture"
                    }
                ),
                new Data(
                    "Agreement EU-GEN-2016-735 and Altus Amendment 121321321fdfd31232131232231232131",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735 and Altus",
                        "Amendment",
                        "121321321fdfd31232131232231232131"
                    }
                ),
                new Data(
                    "FooBarFooBarFooBarFooBarFooBarFooBarFooB FooBarFooBarFooBarFooBarFooBarFooBarFooB FooBarFooBarFooBarFooBarFooBarFooBarFooB",
                    new List<string>() {
                        "FooBarFooBarFooBarFooBarFooBarFooBarFooB",
                        "FooBarFooBarFooBarFooBarFooBarFooBarFooB",
                        "FooBarFooBarFooBarFooBarFooBarFooBarFooB"
                    }
                ),
                new Data(
                    "Agreement EU-GEN-2016-735 and Altus Amendment FooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBar",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735 and Altus",
                        "Amendment",
                        "FooBarFooBarFooBarFooBarFooBarFooBarFooB",
                        "arFooBarFooBar"
                    }
                ),
                new Data(
                    "Agreement EU-GEN-2016-735 and Altus Amendment FooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFo",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735 and Altus",
                        "Amendment",
                        "FooBarFooBarFooBarFooBarFooBarFooBarFooB",
                        "arFooBarFooBarFooBarFooBarFooBarFooBarFo"
                    }
                )
            };
        }

        [Test()]
        public void ParseSentence()
        {
            this.sentences.ForEach((sentence) => {
                try
                {
                    var actual = Parser.SplitSentence(sentence.Input);
                    CollectionAssert.AreEqual(sentence.Expected, actual);
                } catch (Exception ex)
                {
                    throw new Exception(sentence.Input, ex);
                }
            });
        }
    }
}
