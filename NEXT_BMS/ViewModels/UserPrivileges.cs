using NEXT_BMS.Models;

namespace NEXT_BMS.ViewModels
{
    public class UserPrivileges
    {
        public int Id { set; get; }
        public string Controller { set; get; }
        public string Action { set; get; }
        public string Title { set; get; }

        public int CategoryId { set; get; }

        public bool IsMenu { set; get; }

        public int? ApplicationId { set; get; }

        public int MenuTypeId { set; get; }

        public string PrimaryController { set; get; }

        public MenuCategory MenuCategory { set; get; }
    }
}
