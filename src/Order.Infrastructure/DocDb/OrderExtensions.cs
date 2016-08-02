using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Order.Domain;
using Order.Domain.Coupon;
using Order.Domain.Tax;

namespace Order.Infrastructure.DocDb
{
    internal static class OrderExtensions
    {
        public static JObject ToDocument(this Domain.Order order)
        {
            JObject doc = new JObject();
            doc["id"] = order.Id; // Use Id as the document id.
            doc["Store"] = JsonConvert.SerializeObject(order.Store, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            doc["State"] = order.State.ToString();
            doc["RootTaxStep"] = JsonConvert.SerializeObject(order.RootTaxStep, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            doc["Items"] = ToDocument(order.Items);
            doc["Customer"] = ToDocument(order.Customer);
            doc["DeliveryAddress"] = ToDocument(order.DeliveryAddress);
            doc["PaymentDetails"] = ToDocument(order.PaymentDetails);
            doc["Coupon"] = ToDocument(order.Coupon);
            doc["Discount"] = order.Discount;
            doc["TaxLines"] = ToDocument(order.TaxLines);
            doc["TotalAmount"] = order.TotalAmount;
            return doc;
        }

        public static Domain.Order FromDocument(JObject doc)
        {
            ConstructorInfo ci = typeof(Domain.Order).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            Domain.Order order = (Domain.Order)ci.Invoke(null);

            SetProperty<string>(order, "Id", doc["id"].Value<string>());
            SetProperty<Store>(order, "Store", JsonConvert.DeserializeObject<Store>(doc["Store"].Value<string>(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
            SetProperty<OrderState>(order, "State", (OrderState)Enum.Parse(typeof(OrderState), doc["State"].Value<string>()));
            SetProperty<ITaxStep>(order, "RootTaxStep", JsonConvert.DeserializeObject<ITaxStep>(doc["RootTaxStep"].Value<string>(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
            SetProperty<List<LineItem>>(order, "Items", FromDocumentItems(doc["Items"] as JArray));
            SetProperty<Customer>(order, "Customer", FromDocumentCustomer(doc["Customer"] as JObject));
            SetProperty<DeliveryAddress>(order, "DeliveryAddress", FromDocumentAddress(doc["DeliveryAddress"] as JObject));
            SetProperty<PaymentDetails>(order, "PaymentDetails", FromDocumentPayment(doc["PaymentDetails"] as JObject));
            SetProperty<ICoupon>(order, "Coupon", FromDocumentCoupon(doc["Coupon"] as JObject));
            SetProperty<double>(order, "Discount", doc["Discount"].Value<double>());
            SetProperty<List<TaxLineItem>>(order, "TaxLines", FromDocumentTaxLines(doc["TaxLines"] as JArray));
            SetProperty<double>(order, "TotalAmount", doc["TotalAmount"].Value<double>());

            return order;
        }

        private static List<TaxLineItem> FromDocumentTaxLines(JArray lineItemsDoc)
        {
            if (lineItemsDoc == null)
            {
                return null;
            }
            List<TaxLineItem> lineItems = new List<TaxLineItem>();
            foreach (var doc in lineItemsDoc)
            {
                lineItems.Add(new TaxLineItem(doc["Name"].Value<string>(), doc["Amount"].Value<double>()));
            }
            return lineItems;
        }

        private static ICoupon FromDocumentCoupon(JObject jToken)
        {
            if (jToken == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ICoupon>(
                jToken["Serialized"].ToString(),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
        }

        private static PaymentDetails FromDocumentPayment(JObject jToken)
        {
            if (jToken == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<PaymentDetails>(jToken.ToString());
        }

        private static DeliveryAddress FromDocumentAddress(JObject jToken)
        {
            if (jToken == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<DeliveryAddress>(jToken.ToString());
        }

        private static Customer FromDocumentCustomer(JObject jToken)
        {
            if (jToken == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Customer>(jToken.ToString());
        }

        private static List<LineItem> FromDocumentItems(JArray lineItemsDoc)
        {
            if (lineItemsDoc == null)
            {
                return null;
            }
            List<LineItem> lineItems = new List<LineItem>();
            foreach (var doc in lineItemsDoc)
            {
                LineItem li = new LineItem(
                    doc["Id"].Value<string>(),
                    FromDocumentProduct(doc["Product"] as JObject),
                    doc["Quantity"].Value<int>());

                SetProperty<double>(li, "Discount", doc["Discount"].Value<double>());
                SetProperty<double>(li, "Amount", doc["Amount"].Value<double>());
                lineItems.Add(li);
            }

            return lineItems;
        }

        private static Product FromDocumentProduct(JObject jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Product>(jObject.ToString());
        }

        private static void SetProperty<T>(object obj, string propertyName, T value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            pi.SetValue(obj, value);
        }

        private static JArray ToDocument(List<TaxLineItem> taxLines)
        {
            if (taxLines == null)
            {
                return null;
            }

            JArray arrayDoc = new JArray();
            foreach (var taxLine in taxLines)
            {
                arrayDoc.Add(ToDocument(taxLine));
            }
            return arrayDoc;
        }

        private static JObject ToDocument(TaxLineItem taxLine)
        {
            if (taxLine == null)
            {
                return null;
            }

            JObject doc = new JObject();
            doc["Name"] = taxLine.Name;
            doc["Amount"] = taxLine.Amount;

            return doc;
        }

        private static JObject ToDocument(ICoupon coupon)
        {
            if (coupon == null)
            {
                return null;
            }

            JObject doc = new JObject();
            doc["Serialized"] = JsonConvert.SerializeObject(
                coupon,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            return doc;
        }

        private static JObject ToDocument(PaymentDetails paymentDetails)
        {
            if (paymentDetails == null)
            {
                return null;
            }

            JObject doc = new JObject();
            doc["AddressLine1"] = paymentDetails.BankName;
            doc["AddressLine2"] = paymentDetails.TransactionId;

            return doc;
        }

        private static JObject ToDocument(DeliveryAddress deliveryAddress)
        {
            if (deliveryAddress == null)
            {
                return null;
            }

            JObject doc = new JObject();
            doc["AddressLine1"] = deliveryAddress.AddressLine1;
            doc["AddressLine2"] = deliveryAddress.AddressLine2;
            doc["City"] = deliveryAddress.City;
            doc["State"] = deliveryAddress.State;
            doc["PinCode"] = deliveryAddress.PinCode;

            return doc;
        }

        private static JObject ToDocument(Customer customer)
        {
            if (customer == null)
            {
                return null;
            }

            JObject doc = new JObject();
            doc["FirstName"] = customer.FirstName;
            doc["LastName"] = customer.LastName;
            doc["Email"] = customer.Email;
            doc["Mobile"] = customer.Mobile;

            return doc;
        }

        private static JArray ToDocument(List<LineItem> items)
        {
            if (items == null)
            {
                return null;
            }

            JArray arrayDoc = new JArray();
            foreach (var item in items)
            {
                arrayDoc.Add(ToDocument(item));
            }
            return arrayDoc;
        }

        private static JObject ToDocument(LineItem item)
        {
            if (item == null)
            {
                return null;
            }

            JObject doc = new JObject();
            doc["Id"] = item.Id;
            doc["Product"] = ToDocument(item.Product);
            doc["Quantity"] = item.Quantity;
            doc["Discount"] = item.Discount;
            doc["Amount"] = item.Amount;

            return doc;
        }

        private static JToken ToDocument(Product product)
        {
            if (product == null)
            {
                return null;
            }

            JObject doc = new JObject();
            doc["Id"] = product.Id;
            doc["Name"] = product.Name;
            doc["Description"] = product.Description;
            doc["UnitPrice"] = product.UnitPrice;
            doc["ImageUri"] = product.ImageUri;

            return doc;
        }
    }
}
