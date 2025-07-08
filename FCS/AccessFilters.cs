using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection.Metadata;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FCS
{
    public class AccessFilters : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                if (controllerActionDescriptor.MethodInfo?.GetCustomAttributes(inherit: true)?.Any(a => a.GetType().Equals(typeof(SkipThisTaskAttribute))) ?? false)
                    return;

                if (controllerActionDescriptor.ControllerTypeInfo?.GetCustomAttributes(typeof(SkipThisTaskAttribute), true)?.Any() ?? false)
                    return;
            }
            var actionName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;
            var controllerName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerName;

            var controller = context.Controller as Controller;

            if (!controller.ViewData.ContainsKey("Title"))
            {
                controller.ViewData.Add("Title", $"{controllerName} / {actionName}");
            }

            if (!controller.ViewData.ContainsKey("action"))
            {
                controller.ViewData.Add("action", actionName);
            }

            if (!controller.ViewData.ContainsKey("pageName"))
            {
                controller.ViewData.Add("pageName", controllerName);
            }

            //if (context.HttpContext.Session.GetInt32("UserId") == null)
            //{
            //    var areaName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.RouteValues["area"];
            //    if (areaName != "Administrator")
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        string[] exceptions = {
            //        "account_login",
            //        "account_forgotPassword",
            //        "users_login"};
            //        string actionControllerPer = controllerName.ToLower() + "_" + actionName.ToLower();
            //        if (exceptions.Any(s => s.ToLower() == actionControllerPer))
            //        {
            //            return;
            //        }
            //        else
            //        {
            //            context.Result = new RedirectResult("~/Account/LogIn");
            //            return;
            //        }
            //    }
            //}

            //if (context.Filters.Any(x => x.GetType() == typeof(Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter)))
            //    return;
            //string? _roles = context.HttpContext.Session.GetString("Roles");
            //var menus = JsonConvert.DeserializeObject<List<Priviledges>>(context.HttpContext.Session.GetString("Previleges"));
            //if (!menus.Any(s => s.action.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) && s.controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)))
            //{
            //    if (!controller.TempData.ContainsKey("Unauthorized"))
            //    {
            //        controller.TempData.Add("Unauthorized", "You are not authorized to access this feature!!!!");
            //        context.Result = new RedirectResult("~/Account/LogIn");
            //        return;
            //    }
            //}
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

            // our code after action executes
        }
    }

    public class SkipThisTaskAttribute : Attribute { }
}