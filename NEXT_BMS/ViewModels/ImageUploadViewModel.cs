namespace NEXT_BMS.ViewModels
{
    public class ImageUploadViewModel
    {
        public int AdministrationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }

    //    [AdministrationId][int] NOT NULL,

    //[Title] [nvarchar] (250) NOT NULL,

    //[Description] [nvarchar]
    //    (max) NULL,

    //[URL] [nvarchar] (50) NOT NULL,

    //[IsLogoImage] [bit]
    //    NOT NULL,

    //[IsCoverImage] [bit]
    //    NOT NULL,

    //[IsSlideImage] [bit]
    //    NOT NULL,
    }
}
