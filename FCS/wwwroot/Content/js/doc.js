
function SetImgsrc(src) {
    $("#imgpreview").attr("src", src);
}

function downladfile(id) {
    $.ajax(
        {
            url: $("#DownloadAttachment").attr("data-url"),
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            data: {
                id: id
            },
            type: "GET",
            success: function () {
                var url = "../../Documents/DownloadAttachment/" + id;
                window.location.replace(url);
            },
            error: function () {
                alert("Something is wrong!");
            }
        });
}

function RemoveFile(Id, ItemId) {
    alertify.set({ labels: { ok: "Yes Continue", cancel: "No Cancel" } });
    alertify.confirm("Are you sure you want to delete selected file?", function (e) {
        if (e) {
            $.ajax({
                cache: false,
                type: 'POST',
                url: $("#RemoveFile").attr("data-url"),
                data: { 'Id': Id, 'ItemId': ItemId },
                async: false,
                success: function (data) {
                    alertify.success("File deleted successfully!")
                }
            });
        }
    });
}

function RemoveDocument(Id) {
    alertify.set({ labels: { ok: "Yes Continue", cancel: "No Cancel" } });
    alertify.confirm("Are you sure you want to delete this document?", function (e) {
        if (e) {
            $.ajax({
                cache: false,
                type: 'POST',
                url: $("#DeleteDocument").attr("data-url"),
                data: { 'DocumentId': Id },
                async: false,
                success: function (data) {
                    alertify.success("Document deleted successfully!")
                }
            });
        }
    });
}

function SetFileId(Id) {
    $("#OldFileId").val(Id);
}

function SetRenameFile(Id, name) {
    $("#FileId").val(Id);
    var newname = name.split('_')[2]
    $("#FileName").val(newname);
}

function SetRenameFolder(Id, name, CategoryId, Description) {
    $("#FolderId").val(Id);
    $("#DocumentName").val(name);
    $("#CategoryId").val(CategoryId);
    $('#CategoryId').select2();
    $("#DocumentDescription").val(Description);
}

$('#StoreId').change(function () {
    $("#ShelfId").empty();
    var Id = $('#StoreId').val();
    $.ajax({
        cache: false,
        type: 'GET',
        url: $("#GetShelfsByStore").attr("data-url"),
        data: { 'Id': Id },
        async: false,
        success: function (data) {
            $("#ShelfId").append("<option></option>");
            $.each(data, function (id, fos) {
                $("#ShelfId").append("<option value='" + fos.Id + "'>" + fos.Name + "</option>");
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertify.error('Failed to retrieve data.' + thrownError);
        }
    });
    $("#ShelfId").trigger('select:updated');
});

$('#ShelfId').change(function () {
    $("#ShelfSlotId").empty();
    var Id = $('#ShelfId').val();
    $.ajax({
        cache: false,
        type: 'GET',
        url: $("#GetSlotByShelf").attr("data-url"),
        data: { 'Id': Id },
        async: false,
        success: function (data) {
            $.each(data, function (id, fos) {
                $("#ShelfSlotId").append("<option value='" + fos.Id + "'>" + fos.Name + "</option>");
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertify.error('Failed to retrieve data.' + thrownError);
        }
    });
    $("#ShelfSlotId").trigger('select:updated');
});

$("#uploadDocs").on("click", function (e) {
    e.preventDefault();
    var DocumentId = $('#SelectedDocumentId').val();;
    var OwnerId = $('#DocumentOwnerId').val();;
    $('.file-uploader').pluploadQueue({
        runtimes: 'html5, html4, Flash, Silverlight',
        url: $("#UploadBigFiles").attr("data-url"),
        unique_names: true,
        header: true,
        filters: {
            max_file_size: '200mb',
            mime_types: [{
                title: 'Image files',
                extensions: 'jpg,gif,png,pdf'
            }]
        },
        multipart_params: {
            "DocumentId": DocumentId,
            "OwnerId": OwnerId
        }
    });
    $("#uploadDocument").modal('show')
});
//---------------------------------------//---------------------------------------
function GetDocuments() {
    $("#docaccordion").html("");
    var OwnerEntityId = $("#OwnerEntityId").val();
    var DocumentTypeId = $("#DocumentTypeId").val();
    $.ajax({
        url: $("#GetEntityDocuments").attr("data-url"),
        type: 'POST',
        data: {
            'OwnerEntityId': OwnerEntityId,
            'DocumentTypeId': DocumentTypeId
        },
        cache: false,
        success: function (data) {
            $(data.Categories).each(function (index, item) {
                var cat = RendarCategories(item.Id, item.Name, data.Documents);
                $("#docaccordion").prepend(cat);
            });
        }
    });
}

$(document).ready(function () {
    GetDocuments();
});

function RendarCategories(Id, Name, Documents) {
    var link = '<a href="#" class="text-success-300 float-right" onclick="SetAddCategory('+Id+')" data-toggle="modal" data-target="#model_addDocument"><i class="icon-plus2"></i> Add New Document</a> ';
    var cat = "<div class='card m-0 mb-0'> <div class='card-header bg-light'> <a class='text-info-800' data-toggle='collapse' href='#collapse" + Id + "'>" + Name + link +
        "</a> </div> <div id='collapse" + Id + "' class = 'collapse' data-parent = '#docaccordion'><div class='card-body m-0 p-0'> " + RendareDocument(Id, Documents)+" </div> </div> </div>";
    return cat;
}

function RendareDocument(Id, Documents) {
    var count = 1;
    var docs = "<table class='table table-togglable table-hover table-xs'> <thead><tr><th>No.</th><th>Docu</th><th></th><thead><tbody>";
    var trs = "";
    $(Documents).each(function (index,item) {
        if (item.CategoryId == Id) {
            var select = `SelectDocument(${item.Id},${item.DocumentId}, '${item.Name}')`;
            trs = trs + '<tr><td>' + count + '</td>' + '<td><a href="#" class="text-info-800" onclick="'+ select+'" data-dismiss="modal">' + item.Name + '</a></td><td></td></tr>';
            count++
        }
    });
    docs = docs + trs +'</tbody></table>';
    return docs;
}

function SelectDocument(EntityDocumentId, DocumentId, Name) {
   
    $("#SelectedDocumentName").text(Name);
    var InputName = $("#InputName").val();
    var InputType = $("#InputType").val();

    if (InputType == 'EntityDocument') {

        $("#" + InputName).val(EntityDocumentId);
    }
    else if (InputType == 'GeneralDocument') {

        $("#" + InputName).val(DocumentId);
    }
}

function SetAddCategory(Id) {
    $("#DocumentCategoryId").val(Id);
}

$('#btnUpload').click(function () {

    if (window.FormData !== undefined) {

        var fileUpload = $("#FileUpload1").get(0);
        var files = fileUpload.files;

        var EntityId = $("#EntityId").val();
        var DocumentTypeId = $("#DocumentTypeId").val();
        var DocumentCategoryId = $("#DocumentCategoryId").val();
        var DocumentName = $("#DocumentName").val();
        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        // Adding one more key to FormData object  
        fileData.append('EntityId', EntityId);
        fileData.append('DocumentTypeId', DocumentTypeId);
        fileData.append('DocumentName', DocumentName);
        fileData.append('DocumentCategoryId', DocumentCategoryId);

        $.ajax({
            url: $("#JsonUploadFiles").attr("data-url"),
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,
            success: function (result) {
                GetDocuments();
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    } else {
        alert("FormData is not supported.");
    }
});