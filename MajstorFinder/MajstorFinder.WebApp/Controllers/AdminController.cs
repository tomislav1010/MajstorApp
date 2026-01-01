using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MajstorFinder.WebApp.Controllers
{
    public class AdminController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("jwt");
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
