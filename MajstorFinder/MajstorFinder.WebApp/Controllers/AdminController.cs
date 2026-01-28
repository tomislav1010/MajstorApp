using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public abstract class AdminController : Controller
    {
        protected bool IsAdmin => HttpContext.Session.GetString("isAdmin") == "1";
        protected int CurrentUserId => HttpContext.Session.GetInt32("userId") ?? 0;

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            // ako nije logiran -> login
            if (CurrentUserId == 0)
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }

            // ako nije admin -> zabrani
            if (!IsAdmin)
            {
                context.Result = Forbid();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
