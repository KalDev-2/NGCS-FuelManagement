$.fn.extend({
    treed: function (o) {
        var openedClass = 'icon-minus2';
        var closedClass = 'icon-plus2';

        if (typeof o != 'undefined') {
            if (typeof o.openedClass != 'undefined') {
                openedClass = o.openedClass;
            }
            if (typeof o.closedClass != 'undefined') {
                closedClass = o.closedClass;
            }
        };

        var tree = $(this);
        tree.addClass("tree");
        tree.find('li').has("ul").each(function () {
            var branch = $(this); //li with children ul
            branch.prepend("<i class='indicator  " + closedClass + "'></i>");
            branch.addClass('branch');
            branch.on('click', function (e) {
                if (this == e.target) {
                    var icon = $(this).children('i:first');
                    icon.toggleClass(openedClass + " " + closedClass);
                    $(this).children().children().toggle();
                }
            })
            branch.children().children().toggle();
        });
        //fire event from the dynamically added icon
        tree.find('.branch .indicator').each(function () {
            $(this).on('click', function () {
                $(this).closest('li').click();
            });
        });
        //fire event to open branch if the li contains an anchor instead of text
        tree.find('.branch>a').each(function () {
            $(this).on('click', function (e) {
                $(this).closest('li').click();
                e.preventDefault();
            });
        });
        //fire event to open branch if the li contains a button instead of text
        tree.find('.branch>button').each(function () {
            $(this).on('click', function (e) {
                $(this).closest('li').click();
                e.preventDefault();
            });
        });
    }
});

$('.mainnode').treed({ openedClass: 'icon-folder-open', closedClass: 'icon-folder' });

function SetEditCategory(Id, Name) {
    $("#DocumentCategoryId").val(Id);
    $("#CategoryName").val(Name);
}

function SetAddCategory(Id, BranchDepartmentId) {
    $("#BranchDepartmentId").val(BranchDepartmentId);
    $("#ParentCategoryId").val(Id);
}

function SetDocument(Id, Name, Description) {
    $("#DocumentId").val(Id);
    $("#DocumentName").val(Name);
    $("#Description").val(Description);
}

function DeleteCategory(Id) {
    alertify.set({ labels: { ok: "Yes Continue", cancel: "No Cancel" } });
    alertify.confirm("Are you sure you want to delete delected category?", function (e) {
        if (e) {
            $.ajax({
                cache: false,
                type: 'POST',
                url: '../../Documents/DeleteCategory',
                data: { 'Id': Id },
                async: false,
                success: function (data) {
                    location.reload(true);
                }
            });
        }
    });
}

$(".branch").each(function () {
    $(this).css("display", "");
   
});

$(".selected").each(function () {
    $(this).addClass("text-info-800");
});