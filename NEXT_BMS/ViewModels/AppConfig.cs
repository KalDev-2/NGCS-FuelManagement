using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Policy;

namespace NEXT_BMS.ViewModels
{
    public class AppConfig
    {
        public string OrganizationName { get; set; }
        public string LogoUrl { get; set; }
        public string LogoText { get; set; }
        public string Moto { get; set; }
        public string FooterNote { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Icon { get; set; }
        public string BrandColor  { get; set; }
    }
}