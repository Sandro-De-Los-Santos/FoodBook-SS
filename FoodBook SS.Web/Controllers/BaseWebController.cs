using FoodBook_SS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace FoodBook_SS.Web.Controllers
{
 
    public abstract class BaseWebController : Controller
    {
        
        protected IActionResult HandleResult(OperationResult result, string successView, object? model = null)
        {
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(successView, model);
            }
            return View(successView, result.Data);
        }

        protected IActionResult RedirectOnSuccess(OperationResult result, string action, string? failView = null)
        {
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return failView is not null ? View(failView) : RedirectToAction(action);
            }
            TempData["Success"] = result.Message;
            return RedirectToAction(action);
        }
    }
}