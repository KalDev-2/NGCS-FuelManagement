using System.Data;
using System.Data.Entity;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NEXT_BMS.Models;
using Document = NEXT_BMS.Models.Document;
namespace NEXT_BMS.Utilities;

public class DocumentManagement
{
    private NEXT_BMSContext _context = new NEXT_BMSContext();
   
    private readonly IWebHostEnvironment _hostEnvironment;

    private readonly CurrentUser _currentUser;
    public DocumentManagement(NEXT_BMSContext context, IWebHostEnvironment hostingEnvironment, CurrentUser  currentUser)
    {
        _context = context;
        _hostEnvironment = hostingEnvironment;
        _currentUser = currentUser;
    }
    public int saveDocument(int categoryId, string name, string description)
    {
        var document = _context.Documents.FirstOrDefault(x => x.CategoryId == categoryId && x.Name.Trim().ToLower() == name.Trim().ToLower());
        if (document!=null)
        {
            document.IsActive = true;
            document.IsDeleted=false;
            _context.Update(document);
        }
        else {
            document = new Document
            {
                Name = name,
                Description = description,
                CategoryId = categoryId,
                ActionBy = _currentUser.GetActionBy(),
                UploadedDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };
            _context.Documents.Add(document);
        }
        _context.SaveChanges();
        return document.Id;
    }

    public int SaveDocument(int CategoryId, int? OwnerEntityId, string Name, string Description, IFormFile[] files)
    {
        Document document = new Document
        {
            Name = Name,
            Description = Description,
            CategoryId = CategoryId,
            ActionBy = 1,
            UploadedDate = DateTime.Now,
            IsActive = true,
            IsDeleted = false
        };
        _context.Documents.Add(document);
        _context.SaveChanges();
        SaveFiles(document.Id, OwnerEntityId, files);
        return document.Id;
    }

    public void SaveFiles(int DocumentId, int? OwnerEntityId, IFormFile[] files)
    {
        string[] extentions = { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
        var subdirectory = GetDirectory(DocumentId, OwnerEntityId);
        var directory = "~/Uploads/" + subdirectory + "/" + DocumentId;
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                if (extentions.Contains(fileExtension.ToLower()))
                {
                    string name = Path.GetFileName(file.FileName).Replace('_', ' '); ;
                    string FileName = DocumentId + "__" + name;
                    if (!Directory.Exists(Path.Combine(_hostEnvironment.WebRootPath, directory)))
                    {
                        Directory.CreateDirectory(Path.Combine(_hostEnvironment.WebRootPath, directory));
                    }
                    string fileLocation = Path.Combine(_hostEnvironment.WebRootPath, directory, FileName);
                    if (File.Exists(fileLocation))
                    {
                        try
                        {
                            File.Delete(fileLocation);
                        }
                        catch { }
                    }
                   //file.CopyTo(new FileStream(fileLocation,file));
                    DocumentFile gdf = new DocumentFile();
                    gdf.DocumentId = DocumentId;
                    gdf.Url = FileName;
                    gdf.IsActive = true;
                    gdf.IsDeleted = false;
                    _context.DocumentFiles.Add(gdf);
                    _context.SaveChanges();
                }
            }
        }
    }

    public void RemoveFile(int Id, int? OwnerEntityId)
    {
        var file = _context.DocumentFiles.Find(Id);
        int DocumentId = file.DocumentId;
        var Subdirectory = GetDirectory(DocumentId, OwnerEntityId);
        var directory = Path.Combine("Uploads",Subdirectory, DocumentId.ToString());
        string _path = Path.Combine(_hostEnvironment.WebRootPath,directory, file.Url);
        if (System.IO.File.Exists(_path))
        {
            try
            {
                System.IO.File.Delete(_path);
                file.IsActive = false;
                file.IsDeleted = true;
                _context.Update(file);
                _context.SaveChanges();
                //TempData["Success"] = @Html.Raw(Localization.Get("The file is deleted successfully!");
            }
            catch { }
        }
        else
        {
           // TempData["info"] = @Html.Raw(Localization.Get("The file is not deleted successfully!");
        }
    }

    public void MoveFile(int Id, int NewDocumentId, int? OwnerEntityId)
    {
        var file = _context.DocumentFiles.FirstOrDefault(x => x.Id == Id);
        int documentId = file.DocumentId;
        var subdirectory = GetDirectory(documentId, OwnerEntityId);
        var directory = Path.Combine("Uploads",subdirectory ,documentId.ToString());
        string _path = Path.Combine(_hostEnvironment.WebRootPath, directory, file.Url);
        string newFileName = NewDocumentId + "__" + file.Url.Replace('_', ' ');
        string _newpath = Path.Combine(_hostEnvironment.ContentRootPath, directory, newFileName);

        System.IO.File.Move(_path, _newpath);
        file.DocumentId = NewDocumentId;
        file.Url = newFileName;
        _context.Update(file);
        _context.SaveChanges();


        // string newpath = null;
        //if (directory != null && file != null && file.FileURL != null)
        //{
        //    oldpath = Path.Combine(Server.MapPath(directory + "/"), file.FileURL);
        //}

        //string NewFileName = NewDocumentId + "__" + file.FileURL.Replace('_', ' ');
        //if (directory != null  && NewFileName != null)
        //{
        //    newpath = Path.Combine(Server.MapPath(directory + "/"), NewFileName);
        //}

        //if (!System.IO.File.Exists(newpath))
        //{
        //    if (System.IO.File.Exists(oldpath))
        //    {
        //        System.IO.File.Move(oldpath, newpath);
        //        file.DocumentId = NewDocumentId;
        //        file.FileURL = NewFileName;
        //        _context.Entry(file).State = EntityState.Modified;
        //        _context.SaveChanges();
        //        TempData["Success"] = @Html.Raw(Localization.Get("The file is moved successfully!");
        //    }
        //    else
        //    {
        //        TempData["info"] = @Html.Raw(Localization.Get("The file is not exists in this location!");
        //    }
        //}
        //else
        //{
        //    TempData["info"] = @Html.Raw(Localization.Get("The file is already exists in the destination location!");
        //}
    }

    public void RenameFile(int FileId, string FileName, int? OwnerEntityId)
    {
        var file = _context.DocumentFiles.Find(FileId);

        var directory = "~/Uploads/" + GetDirectory(file.DocumentId, OwnerEntityId) + "/" + file.DocumentId;

        string oldpath = Path.Combine(_hostEnvironment.EnvironmentName, directory, file.Url);

        string NewFileName = file.DocumentId + "__" + FileName.Replace('_', ' ');

        string newpath = Path.Combine(_hostEnvironment.EnvironmentName, directory, NewFileName);

        if (!System.IO.File.Exists(newpath))
        {
            if (System.IO.File.Exists(oldpath))
            {
                System.IO.File.Move(oldpath, newpath);

                file.Url = NewFileName;
                _context.Update(file);
                _context.SaveChanges();
               // TempData["Success"] = @Html.Raw(Localization.Get("The file is renamed successfully!");
            }
            else
            {
               // TempData["info"] = @Html.Raw(Localization.Get("The file is not exists in this location!");
            }
        }
        else
        {
            //TempData["Error"] = @Html.Raw(Localization.Get("The file is already exists in the destination location!");
        }
    }

    public void RenameDocument(int DocumentId, string DocumentName, string DocumentDescription)
    {
        var document = _context.Documents.Find(DocumentId);
        document.Name = DocumentName;
        document.Description = DocumentDescription;
        _context.Update(document);
        _context.SaveChanges();
    }

    public string GetDirectory(int DocumentId)
    {
        var Document = _context.Documents.Find(DocumentId);

        int DocumentTypeId = Document.Category.DocumentTypeId;

        var BranchDepartmentId = Document.Category.OrganizationId;

        int? ItemId = null;

        //if (Document.ProjectDocuments.Any())
        //{
        //    ItemId = Document.ProjectDocuments.FirstOrDefault().ProjectId;
        //}

        //if (Document.FirmDocuments.Any())
        //{
        //    ItemId = Document.FirmDocuments.FirstOrDefault().FirmId;
        //}

        //if (Document.EmployeeDocuments.Any())
        //{
        //    ItemId = Document.EmployeeDocuments.FirstOrDefault().EmployeeId;
        //}
        //if (Document.ProfessionalDocuments.Any())
        //{
        //    ItemId = Document.ProfessionalDocuments.FirstOrDefault().ProfessionalId;
        //}

        //if (Document.OutgoingLetters.Any())
        //{
        //    ItemId = Document.OutgoingLetters.FirstOrDefault().BranchDepartmentId;
        //}

        //if (Document.IncomingLetters.Any())
        //{
        //    ItemId = Document.IncomingLetters.FirstOrDefault().OrganizationId;
        //}

        return ItemId != null ? string.Format("{0}/{1}", DocumentTypeId, ItemId) : string.Format("{0}/{1}", DocumentTypeId, BranchDepartmentId);

    }

    public string GetDirectory(int DocumentId, int? OwnerEntityId)
    {
        var document = _context.Documents.Where(x => x.Id == DocumentId).Include(x => x.Category).FirstOrDefault();

        int DocumentTypeId = document.Category.DocumentTypeId;

        var BranchDepartmentId = document.Category.OrganizationId;

        var OwnerId = OwnerEntityId != null && OwnerEntityId != 0 ? OwnerEntityId : BranchDepartmentId;

        return String.Format("{0}/{1}", DocumentTypeId, OwnerId);
    }

    public string GetFileclass(string filename)
    {
        string fn = filename.ToLower();

        if (fn.EndsWith(".jpg") || fn.EndsWith(".png") || fn.EndsWith(".jpeg") | fn.EndsWith(".bmp") || fn.EndsWith(".tif") || fn.EndsWith(".tiff"))
        {
            return "icon-file-picture text-info-400";
        }

        if (fn.EndsWith(".xls") || fn.EndsWith(".xlsx"))
        {
            return "icon-file-excel text-success-400";
        }
        if (fn.EndsWith(".doc") || fn.EndsWith(".docx"))
        {
            return "icon-file-word text-primary-400";
        }

        if (fn.EndsWith(".pdf"))
        {
            return "icon-file-pdf text-orange-400";
        }
        return "";
    }

    //public int AddEmployeeDocument(int DocumentId, int EmployeeId)
    //{
    //    var employeeDocuments = _context.EmployeeDocuments.Where(x => x.EmployeeId == EmployeeId && x.DocumentId == DocumentId);
    //    if (employeeDocuments.Any())
    //    {
    //        var employeeDocument = employeeDocuments.FirstOrDefault();
    //        employeeDocument.IsActive = true;
    //        employeeDocument.IsDeleted = false;
    //        _context.Entry(employeeDocument).State = EntityState.Modified;
    //        _context.SaveChanges();
    //        return employeeDocument.Id;
    //    }
    //    else
    //    {
    //        EmployeeDocument employeeDocument = new EmployeeDocument();
    //        employeeDocument.EmployeeId = EmployeeId;
    //        employeeDocument.DocumentId = DocumentId;
    //        employeeDocument.IsActive = true;
    //        employeeDocument.IsDeleted = false;
    //        _context.EmployeeDocuments.Add(employeeDocument);
    //        _context.SaveChanges();
    //        return employeeDocument.Id;
    //    }

    //}

    //public int AddFirmDocument(int DocumentId, int FirmId)
    //{

    //    var firmDocuments = _context.FirmDocuments.Where(x => x.FirmId == FirmId && x.DocumentId == DocumentId);
    //    if (firmDocuments.Any())
    //    {
    //        var firmDocument = firmDocuments.FirstOrDefault();
    //        firmDocument.IsActive = true;
    //        firmDocument.IsDeleted = false;
    //        _context.Entry(firmDocument).State = EntityState.Modified;
    //        _context.SaveChanges();
    //        return firmDocument.Id;
    //    }
    //    else
    //    {
    //        FirmDocument firmDocument = new FirmDocument();
    //        firmDocument.FirmId = FirmId;
    //        firmDocument.DocumentId = DocumentId;
    //        firmDocument.IsActive = true;
    //        firmDocument.IsDeleted = false;
    //        _context.FirmDocuments.Add(firmDocument);
    //        _context.SaveChanges();
    //        return firmDocument.Id;
    //    }
    //}

    //public int AddProjectDocument(int DocumentId, int ProjectId)
    //{
    //    var documents = _context.ProjectDocuments.Where(x => x.ProjectId == ProjectId && x.DocumentId == DocumentId);
    //    if (documents.Any())
    //    {
    //        var document = documents.FirstOrDefault();
    //        document.IsActive = true;
    //        document.IsDeleted = false;
    //        _context.Entry(document).State = EntityState.Modified;
    //        _context.SaveChanges();
    //        return document.Id;
    //    }
    //    else
    //    {
    //        ProjectDocument document = new ProjectDocument();
    //        document.ProjectId = ProjectId;
    //        document.DocumentId = DocumentId;
    //        document.IsActive = true;
    //        document.IsDeleted = false;
    //        _context.ProjectDocuments.Add(document);
    //        _context.SaveChanges();
    //        return document.Id;
    //    }
    //}

    //public int AddProfessionalDocument(int DocumentId, int ProfessionalId)
    //{

    //    var documents = _context.ProfessionalDocuments.Where(x => x.ProfessionalId == ProfessionalId && x.DocumentId == DocumentId);
    //    if (documents.Any())
    //    {
    //        var document = documents.FirstOrDefault();
    //        document.IsActive = true;
    //        document.IsDeleted = false;
    //        _context.Entry(document).State = EntityState.Modified;
    //        _context.SaveChanges();
    //        return document.Id;
    //    }
    //    else
    //    {
    //        ProfessionalDocument document = new ProfessionalDocument();
    //        document.ProfessionalId = ProfessionalId;
    //        document.DocumentId = DocumentId;
    //        document.IsActive = true;
    //        document.IsDeleted = false;
    //        _context.ProfessionalDocuments.Add(document);
    //        _context.SaveChanges();
    //        return document.Id;
    //    }
    //}



    public void Upload(IFormFile[] files)
    {
        foreach (var file in files)
        {
            var fileName = System.IO.Path.GetFileName(file.FileName);
            var webRootPath = _hostEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, "upload", fileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (var localFile = System.IO.File.OpenWrite(filePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }
            }
            catch (Exception ex)
            {
              
                // Handle the exception (log, display an error message, etc.)
                // For example:
                // Console.WriteLine($"Error uploading file: {ex.Message}");
            }
        }
    }

    #region ------------Document Category----------------
    public void addDocumentCategory(int? parentId, int organizationId, int documentTypeId, string name)
    {
        var documentCategory = _context.DocumentCategories
            .FirstOrDefault(x => x.OrganizationId == organizationId && x.DocumentTypeId == documentTypeId && x.Name.Trim().ToLower() == name.Trim().ToLower());
        if (documentCategory != null)
        {
            documentCategory.IsActive = true;
            documentCategory.IsDeleted = false;
            documentCategory.ParentId = parentId;
            _context.Update(documentCategory);
        }
        else
        {
            documentCategory = new DocumentCategory()
            {
                OrganizationId = organizationId,
                DocumentTypeId = documentTypeId,
                Name = name,
                ParentId=parentId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };
            _context.Add(documentCategory);
        }
        _context.SaveChanges();
    }

    public void editDocumentCategory(int id, int organizationId, int documentTypeId, string name)
    {
        var documentCategory = _context.DocumentCategories
            .FirstOrDefault(x => x.Id == id);
        documentCategory.Name = name;
        documentCategory.OrganizationId = organizationId;
        documentCategory.DocumentTypeId = documentTypeId;
        documentCategory.CreatedDate = DateTime.Now;
        documentCategory.IsActive = true;
        documentCategory.IsDeleted = false;
        _context.Update(documentCategory);
        _context.SaveChanges();
    }

    public void removeDocumentCategory(int id)
    {
        var documentCategory = _context.DocumentCategories
            .FirstOrDefault(x => x.Id == id);
        documentCategory.IsActive = false;
        documentCategory.IsDeleted = true;
        _context.Update(documentCategory);
        _context.SaveChanges();
    }
    #endregion


}

