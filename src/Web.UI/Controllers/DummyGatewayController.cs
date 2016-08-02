using Microsoft.AspNetCore.Mvc;

namespace Web.UI.Controllers
{
    public class DummyGatewayController : Controller
    {
        public IActionResult Index(double amt, string cb)
        {
            ViewData["amt"] = amt;
            ViewData["cb"] = cb;
            return View();
        }

        public IActionResult MakePayment(string cb)
        {
            ViewData["cb"] = cb;
            return View();
        }
    }
}
