using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Text;
using NEXT_BMS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Configuration;
using Hangfire;

namespace NEXT_BMS.Utilities
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IMailService _mailService;

        public CustomExceptionFilter(IModelMetadataProvider modelMetadataProvider, IMailService mailService)
        {
            _modelMetadataProvider = modelMetadataProvider;
            _mailService = mailService;
        }

        public void OnException(ExceptionContext context)
        {

           // var request = context.HttpContext.Request;
           // var stringBuilder = new StringBuilder();
           // string userInfo = "User Not logged in";
           // if (context.HttpContext.Session.GetInt32("UserId") != null) { userInfo = context.HttpContext.Session.GetInt32("UserId").ToString(); }
           // stringBuilder.Append("User Id: ").Append(userInfo).Append("<br/>")
           //     .Append("Address: ").Append($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}").Append("<br/>")
           //     .Append("Local IP address: ").Append(context.HttpContext.Connection.LocalIpAddress).Append("<br/>")
           //     .Append("Remote IP address: ").Append(context.HttpContext.Connection.RemoteIpAddress).Append("<br/>");

           // BuildExceptionText(stringBuilder, context.Exception);
           // MailRequest mailRequest = new MailRequest();
           // mailRequest.Subject = "System Error Report";
           // mailRequest.Body = stringBuilder.ToString();
           // mailRequest.FullName = "Developers";
           //// BackgroundJob.Enqueue<IMailService>(x => x.SendAsync(mailRequest, true));

           // var result = new ViewResult { ViewName = "CustomError" };
           // result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
           // result.ViewData.Add("Exception", context.Exception.InnerException);
           // // Here we can pass additional detailed data via ViewData
           // context.ExceptionHandled = true; // mark exception as handled
           // context.Result = result;
        }


        private StringBuilder BuildExceptionText(StringBuilder stringBuilder, Exception exception)
        {
            stringBuilder.Append("<h2>").Append(exception.Message).Append("</h2>")
               .Append(exception.Source ?? "").Append("<hr/>");
            if (exception.StackTrace != null)
            {
                stringBuilder.Append("<h3>Stack trace: </h3>").Append(exception.StackTrace.Replace(Environment.NewLine, "<br/>"));
            }

            if (exception.InnerException != null)
            {
                stringBuilder.Append("<h3>Inner exception: </h3>").Append(exception.InnerException);
            }

            return stringBuilder;
        }
    }
}