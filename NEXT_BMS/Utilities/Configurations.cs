using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace NEXT_BMS.Utilities;
public class Configurations
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Configurations( IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void changeMenuType(int menuTypeId)
    {
        _httpContextAccessor.HttpContext.Session.SetInt32("MenuTypeId", menuTypeId);
        _httpContextAccessor.HttpContext.Session.SetString("SelectedItem", "");
    }

    public  void changeMenuType(int menuTypeId, string objectId, string Name, string Photo, string Detail)
    {
        _httpContextAccessor.HttpContext.Session.SetInt32("MenuTypeId", menuTypeId);
        _httpContextAccessor.HttpContext.Session.SetString("SelectedItem", Name);
        _httpContextAccessor.HttpContext.Session.SetString("SelectedObjectId", objectId);
        _httpContextAccessor.HttpContext.Session.SetString("SelectedObjectName", Name);
        _httpContextAccessor.HttpContext.Session.SetString("SelectedObjectPhoto", Photo);
        _httpContextAccessor.HttpContext.Session.SetString("SelectedObjectDetail", Detail);
    }

   
}

public static class HtmlHelperExtensions
{
    public static async Task<string> RenderPartialViewToStringAsync(this IHtmlHelper htmlHelper, string viewName, object model)
    {
        var viewEngine = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        var tempDataProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        using (var writer = new StringWriter())
        {
            var viewResult = viewEngine.FindView(htmlHelper.ViewContext, viewName, false);
            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} does not match any available view");
            }

            var viewContext = new ViewContext(
                htmlHelper.ViewContext,
                viewResult.View,
                viewData,
                htmlHelper.ViewContext.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
    }
}

