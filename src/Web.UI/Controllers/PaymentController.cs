using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Web.UI.Controllers
{
    public class PaymentController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Complete(string oid, string bn, string tid)
        {
            string order;
            using (HttpClient client = new HttpClient())
            {
                JObject payload = new JObject();
                payload["BankName"] = bn;
                payload["TransactionId"] = tid;
                StringContent content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

                var baseUrl = new UriBuilder(this.Request.Scheme, this.Request.Host.Host, 9090);
                var response = await client.PostAsync(
                    baseUrl + "/order/" + oid + "/payment",
                    content);
                response.EnsureSuccessStatusCode();
                order = await response.Content.ReadAsStringAsync();
            }

            return View(JObject.Parse(order));
        }
    }
}
