using System;
using System.Collections.Generic;
using FusionSDK;
using Boomi.Erp.Sage.Webapi.Models.Orders;

namespace Boomi.Erp.Sage.Webapi.Data.Mappers
{
    public class OrderMapper
    {
        public void Map(SalesOrder sageOrder, Order crmOrder)
        {
            if (crmOrder.OrderNo != string.Empty)
            {
                sageOrder.setOrderNumber(crmOrder.OrderNo);
            }

            sageOrder.setHeaderValueDate("date_received", crmOrder.DateReceived.Date.ToString("dd/MM/yyyy HH:mm:ss.mmm"));
            sageOrder.setHeaderValueDate("date_required", crmOrder.DateRequired.Date.ToString("dd/MM/yyyy HH:mm:ss.mmm"));

            sageOrder.setHeaderValueString("customer_order_no", crmOrder.CustomerOrderNo);
            sageOrder.setHeaderValueString("alpha", crmOrder.OrderAlphaCode);
            sageOrder.setHeaderValueString("customer", crmOrder.ErpAccountNo);
            sageOrder.setHeaderValueString("wo_order_flag", "Y");

            //CRM value correction
            String addressLine2 = crmOrder.Address.Line2;
            if (addressLine2.Equals(" "))
            {
                addressLine2 = null;
            }

            //Split address if address line 1 or 2 has more than 32 characters
            if ((!String.IsNullOrEmpty(crmOrder.Address.Line1) && crmOrder.Address.Line1.Length > 32) || (!String.IsNullOrEmpty(addressLine2)) && addressLine2.Length > 32)
            {
                //Making address line 1 and 2 into a single address
                string address;
                if (!String.IsNullOrEmpty(addressLine2))
                {
                    address = crmOrder.Address.Line1 + " " + addressLine2;
                }
                else
                {
                    address = crmOrder.Address.Line1;
                }
                
                //splitting the address in strings of max 32 chrarcters
                var address_final = Helpers.Parser.SplitSentence(address, 32);

                //setting address 1
                sageOrder.setHeaderValueString("address1", address_final[0]);

                //setting address 2
                sageOrder.setHeaderValueString("address2", address_final[1]);
            }
            else
            {
                sageOrder.setHeaderValueString("address1", crmOrder.Address.Line1);
                sageOrder.setHeaderValueString("address2", addressLine2);
            }

            //Split city and state if combined they are more than 32 characters
            int cityLength, stateLength;
            if (String.IsNullOrEmpty(crmOrder.Address.City)){
                cityLength = 0;
            }
            else
            {
                cityLength = crmOrder.Address.City.Length;
            }
            if (String.IsNullOrEmpty(crmOrder.Address.State))
            {
                stateLength = 0;
            }
            else
            {
                stateLength = crmOrder.Address.State.Length;
            }
            if ((stateLength + cityLength + 2)>32)
            {
                //Making city and state into a single string
                string City;
                if (!String.IsNullOrEmpty(crmOrder.Address.State) && stateLength < 30)
                {
                    var city_final = Helpers.Parser.SplitSentence(crmOrder.Address.City, (30 - stateLength));
                    City = city_final[0] + ", " + crmOrder.Address.State;
                }
                else
                {
                    var CityState = crmOrder.Address.City + ", " + crmOrder.Address.State;
                    var result = Helpers.Parser.SplitSentence(CityState, 32);
                    City = result[0];
                }

                //setting city and state to adress 3
                sageOrder.setHeaderValueString("address3", City);
            }
            else
            {
                sageOrder.setHeaderValueString("address3", $"{crmOrder.Address.City}, {crmOrder.Address.State}");
            }

            //Split address line 3 if more than 32 characters
            if (!String.IsNullOrEmpty(crmOrder.Address.Line3) && crmOrder.Address.Line3.Length > 32)
            {
                //splitting the address 3 in strings of max 32 chrarcters
                var address3_final = Helpers.Parser.SplitSentence(crmOrder.Address.Line3, 32);

                //setting address 4
                sageOrder.setHeaderValueString("address4", address3_final[0]);
            }
            else 
            {
                sageOrder.setHeaderValueString("address4", crmOrder.Address.Line3);
            }

            //Split country if more than 32 characters
            if (!String.IsNullOrEmpty(crmOrder.Address.Country) && crmOrder.Address.Country.Length > 32)
            {
                //splitting the country in strings of max 32 chrarcters
                var country = Helpers.Parser.SplitSentence(crmOrder.Address.Country, 32);

                //setting country in address5
                sageOrder.setHeaderValueString("address5", country[0]);
            }
            else
            {
                sageOrder.setHeaderValueString("address5", crmOrder.Address.Country);
            }

            sageOrder.setHeaderValueString("shipping_note_ind", "Y");
            sageOrder.setHeaderValueString("invoice_required", "Y");
            sageOrder.setHeaderValueString("quote_proforma", "");
            sageOrder.setHeaderValueString("ack_sent_indicator", "A");

            if (!String.IsNullOrEmpty(crmOrder.RevisionNo))
            {
                sageOrder.addCommentLine("di", $"REVISION {crmOrder.RevisionNo}");
                sageOrder.addCommentLine("di", "----------------------------------------");
            }
            else
            {
                sageOrder.addCommentLine("di", "REVISION 0");
            }

            if (!String.IsNullOrEmpty(crmOrder.OwnerName))
            {
                sageOrder.addCommentLine("di", crmOrder.OwnerName);
            }
            else
            {
                sageOrder.addCommentLine("di", "SALES PERSON: ");
            }

            if (!String.IsNullOrEmpty(crmOrder.Shipping?.Terms))
            {
                var shippingTerms = $"SHIPPING TERMS: {crmOrder.Shipping.Terms}";

                if (crmOrder.Shipping.TermsName == "EXW" || crmOrder.Shipping.TermsName == "FCA")
                {
                    shippingTerms = $"{shippingTerms}, {crmOrder.ExworksLocation}";
                }

                this.addSafeComment(sageOrder, shippingTerms, "di", 32);
                sageOrder.addCommentLine("di", "----------------------------------------");
            }
            else
            {
                sageOrder.addCommentLine("di", "SHIPPING TERMS: ");
            }

            if (!String.IsNullOrEmpty(crmOrder.Shipping?.Method))
            {
                this.addSafeComment(sageOrder, $"SHIPPING METHOD: {crmOrder.Shipping.Method}", "di", 32);
                sageOrder.addCommentLine("di", "----------------------------------------");
            }
            else
            {
                sageOrder.addCommentLine("di", "SHIPPING METHOD: ");
            }

            sageOrder.addCommentLine("d", "----------------------------------------");
            sageOrder.addCommentLine("d", "TERMS AND CONDITIONS: ");
            this.addSafeComment(sageOrder, crmOrder.TermsConditions);

            //CAPSDS-9302--START
           
            if (!String.IsNullOrWhiteSpace(crmOrder.TandCNotes))
            {
                sageOrder.addCommentLine("d", "ENTER NOTE IF APPLICABLE: ");
                this.addSafeComment(sageOrder, crmOrder.TandCNotes);
            }
           
            sageOrder.addCommentLine("d", "----------------------------------------");
            //if (crmOrder.TermsConditions.Trim().ToLowerInvariant().StartsWith("other"))
            //{
            //    this.addSafeComment(sageOrder, crmOrder.TandCNotes);
            //}
            //CAPSDS-9302--END

            if (!String.IsNullOrEmpty(crmOrder.Shipping?.Partial))
            {
                this.addSafeComment(sageOrder, $"PARTIAL SHIPMENT: {crmOrder.Shipping.Partial}");
                sageOrder.addCommentLine("d", "----------------------------------------");
            }
            else
            {
                sageOrder.addCommentLine("d", "PARTIAL SHIPMENT: ");
            }

            sageOrder.addCommentLine("d", "PAYMENT TERMS: ");

            if (crmOrder.Payment != null)
            {
                this.addSafeComment(sageOrder, crmOrder.Payment.TermsCode);

                if (crmOrder.Payment.TermsCode.Trim().ToLowerInvariant().StartsWith("other"))
                {
                    this.addSafeComment(sageOrder, crmOrder.Payment.AlternativeTermsCode);
                }
            }

            sageOrder.addCommentLine("d", "----------------------------------------");

            if (!String.IsNullOrEmpty(crmOrder.QuoteNo))
            {
                sageOrder.addCommentLine("d", $"REFERENCE QUOTE: {crmOrder.QuoteNo}");
            }
            else
            {
                sageOrder.addCommentLine("d", "REFERENCE QUOTE: ");
            }

            sageOrder.addCommentLine("d", "COUNTRY OF END DESTINATION: ");
            this.addSafeComment(sageOrder, crmOrder.LocationName);

            sageOrder.addCommentLine("d", "CLIENT ORDER CONTACT: ");
            if (!String.IsNullOrWhiteSpace(crmOrder.Contact?.Name))
            {
                this.addSafeComment(sageOrder, crmOrder.Contact.Name);
            }

            sageOrder.addCommentLine("d", "CONTACT EMAIL: ");
            if (!String.IsNullOrWhiteSpace(crmOrder.Contact?.Name))
            {
                this.addSafeComment(sageOrder, crmOrder.Contact.Email);
            }

            sageOrder.addCommentLine("d", "REQUESTED BY: ");
            if (!String.IsNullOrWhiteSpace(crmOrder.RequestContact?.Name))
            {
                this.addSafeComment(sageOrder, crmOrder.RequestContact.Name);
            }

            sageOrder.addCommentLine("d", "REQUEST CONTACT EMAIL: ");
            if (!String.IsNullOrWhiteSpace(crmOrder.RequestContact?.Email))
            {
                this.addSafeComment(sageOrder, crmOrder.RequestContact.Email);
            }

            sageOrder.addCommentLine("d", "----------------------------------------");

            this.setOrderProductInformation(sageOrder, crmOrder);
            this.setOrderCommentInformations(sageOrder, crmOrder);
            this.setOrderTotalsInformation(sageOrder, crmOrder);
        }

        private void setOrderProductInformation(SalesOrder sageOrder, Order crmOrder)
        {
            if (crmOrder.Products == null || crmOrder.Products.Count == 0)
            {
                return;
            }

            crmOrder.Products.ForEach((detail) =>
            {
                // Check if product detail information line contains a WH code and if so, then populate the correct fields 
                // PHYSICAL PART, NOT SERVICE CODE
                if (!detail.IsServie())
                {
                    sageOrder.addGoodsLineWithPrices(
                        detail.WareHouseName,
                        detail.PartNumber,
                        detail.Quantity,
                        detail.PricePerUnit,
                        0.0,
                        detail.ManualDiscountAmt / detail.Quantity * -1.0,
                        false
                    );
                    sageOrder.setDetailValueString("wo_make", "Y");
                    sageOrder.setDetailValueString("no_of_labels", "0");
                    sageOrder.setDetailValueString("back_to_back_ind", "N");


                    if (detail.LineNumber != 0.0)
                    {
                        sageOrder.setDetailValueString("transaction_anals1", Convert.ToString(detail.LineNumber));
                    }

                    if (!String.IsNullOrWhiteSpace(detail.Comment))
                    {
                        this.addSafeComment(sageOrder, detail.Comment, "di");
                    }

                    if (detail.ManualDiscountAmt != 0.0)
                    {
                        sageOrder.setDetailValueString("discount_value_fla", "Y");
                    }

                    return;
                }

                sageOrder.addServiceLineWithPrices(
                    detail.PartNumber,
                    detail.Quantity,
                    detail.PricePerUnit,
                    0.0,
                    detail.ManualDiscountAmt / detail.Quantity * -1.0,
                    false
                );

                if (detail.LineNumber != 0.0)
                {
                    sageOrder.setDetailValueString("transaction_anals1", Convert.ToString(detail.LineNumber));
                }


                if (!String.IsNullOrWhiteSpace(detail.Comment))
                {
                    this.addSafeComment(sageOrder, detail.Comment, "di");
                }


                if (detail.ManualDiscountAmt != 0.0)
                {
                    sageOrder.setDetailValueString("discount_value_fla", "Y");
                }
            });
        }

        private void setOrderCommentInformations(SalesOrder sageOrder, Order crmOrder)
        {
            if (crmOrder.Comments == null || crmOrder.Comments.Count == 0)
            {
                return;
            }

            crmOrder.Comments.ForEach((detail) =>
            {
                if (!String.IsNullOrWhiteSpace(detail.GroupDescription))
                {
                    sageOrder.addCommentLine("d", "");
                    this.addSafeComment(sageOrder, detail.GroupDescription);
                }

                if (!String.IsNullOrWhiteSpace(detail.Description))
                {
                    this.addSafeComment(
                        sageOrder,
                        detail.Description,
                        detail.Type ?? String.Empty
                    );
                }
            });
        }

        private void setOrderTotalsInformation(SalesOrder sageOrder, Order crmOrder) {
            sageOrder.setHeaderValueDouble("gross_value", crmOrder.Totals.DetailAmount);
            sageOrder.setHeaderValueDouble("nett_value", crmOrder.Totals.Amount);

            sageOrder.setDetailValueDouble("list_price", crmOrder.Totals.DetailAmount);
            sageOrder.setDetailValueDouble("val", crmOrder.Totals.Amount);

        }

        private void addSafeComment(SalesOrder sageOrder, string comment, string commentLine = "d", int limit = 40)
        {
            Helpers.Parser.SplitSentence(comment, limit).ForEach((c) => sageOrder.addCommentLine(commentLine, c));
        }
    }
}
