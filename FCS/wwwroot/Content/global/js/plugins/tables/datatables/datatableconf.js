$.extend($.fn.dataTable.defaults, {
    autoWidth: false,
    dom: '<"datatable-header justify-content-start p-0 m-0"f<"ms-sm-auto"l><"ms-sm-3"r>><"datatable-scroll"t><"datatable-footer"ip>',
  /*  dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',*/
/*    dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',*/
    language: {
        search: '<div class="form-group-feedback form-group-feedback-right form-control-feedback-sm">_INPUT_<div class="form-control-feedback-icon"><i class="ph-magnifying-glass opacity-50"></i></div></div>',
        searchPlaceholder: 'Type to filter...',
        zeroRecords: "No records found",
        info: "Showing <b>_START_ to _END_</b> (of _TOTAL_)",
        infoFiltered: "",
        infoEmpty: "No records found",
        processing: '<i class="icon-spinner4 spinner"></i> Processing...',
        loadingRecords: 'Loading data...',
        lengthMenu: '<span class="mr-3">Show:</span> _MENU_',
        paginate: { 'first': 'First', 'last': 'Last', 'next': document.dir == "rtl" ? '&larr;' : '&rarr;', 'previous': document.dir == "rtl" ? '&rarr;' : '&larr;' }
    }
});

//<div class="form-group-feedback form-group-feedback-right form-control-feedback-sm">
//    <input type="text" id="" />
//    @Html.TextBox("Search_data", searchValue, new {@class = "form-control border-dark form-control-sm", @placeholder = "search..."})
//    <div class="form-control-feedback">
//        <button class="btn btn-sm" type="submit">
//            <i class="icon-folder-search"></i>
//        </button>
//    </div>
//</div>

