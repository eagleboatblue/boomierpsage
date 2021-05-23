using System;
using NUnit.Framework;
using Boomi.Erp.Sage.Webapi.Data.Helpers;
using System.Collections.Generic;

namespace Boomi.Erp.Sage.Webapi.Data.Tests.Helpers
{
    [TestFixture()]
    public class MapperTest
    {
        private class Data
        {
            public string Input1 { get; }
            public string Input2 { get; }

            public List<string> Expected { get; }

            public Data(string input1, string input2, List<string> output)
            {
                this.Input1 = input1;
                this.Input2 = input2;
                this.Expected = output;
            }
        }

        private List<Data> sentences { get; }

        public MapperTest()
        {
            this.sentences = new List<Data>()
            {
                 new Data(
                    null,
                    " ",
                    new List<string>() {
                        null,
                        null
                    }
                ),
                new Data(
                    "Agreement EU-GEN-2016-735 and Altus Amendment Foo Bar",
                    " ",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735 and",
                        "Altus Amendment Foo Bar"
                    }
                ),
                new Data(
                    "Agreement EU-GEN-2016-735 and Altus Amendment Foo Bar FooBar Fo oBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFooBarFo",
                    " ",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735 and",
                        "Altus Amendment Foo Bar FooBar"
                    }
                ),
                new Data(
                    null,
                    "Agreement EU-GEN-2016-735 and Altus Amendment Foo Bar Foo Bar Fo oBarFooBarFoo",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735 and",
                        "Altus Amendment Foo Bar Foo Bar"
                    }
                ),
                new Data(
                    null,
                    "Agreement EU-GEN-2016-735 Altus",
                    new List<string>() {
                        null,
                        "Agreement EU-GEN-2016-735 Altus"
                    }
                ),
                new Data(
                    "Agreement EU-GEN-2016-735",
                    " ",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735",
                        null
                    }
                ),
                new Data(
                    "Agreement EU-GEN-2016-735 Altus Amendment",
                    "Foo Bar Foo Bar Fo oBarFooBarFoo",
                    new List<string>() {
                        "Agreement EU-GEN-2016-735 Altus",
                        "Amendment Foo Bar Foo Bar Fo"
                    }
                )
            };
        }

        [Test()]
        public void SplitAddress()
        {
            this.sentences.ForEach((sentence) => {
                try
                {
                    //CRM value correction
                    String addressLine2 = sentence.Input2;
                    if (addressLine2.Equals(" "))
                    {
                        addressLine2 = null;
                    }

                    //Split address if address line 1 or 2 has more than 32 characters
                    if ((!String.IsNullOrEmpty(sentence.Input1) && sentence.Input1.Length > 32) || (!String.IsNullOrEmpty(addressLine2)) && addressLine2.Length > 32)
                    {
                        //Making address line 1 and 2 into a single address
                        var address = String.Empty;
                        if (!String.IsNullOrEmpty(addressLine2))
                        {
                            address = sentence.Input1 + " " + addressLine2;
                        }
                        else
                        {
                            address = sentence.Input1;
                        }
                        
                        //splitting the address in strings of max 32 chrarcters
                        var result = Parser.SplitSentence(address, 32);

                        CollectionAssert.AreEqual(sentence.Expected[0], result[0]);
                        CollectionAssert.AreEqual(sentence.Expected[1], result[1]);
                    }
                    else
                    {
                        var address1 = sentence.Input1;
                        var address2 = addressLine2;
                        CollectionAssert.AreEqual(sentence.Expected[0], address1);
                        CollectionAssert.AreEqual(sentence.Expected[1], address2);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(sentence.Input1, ex);
                }
            });
        }

    }

}
