$(function () {

    //defaults
    //$.fn.editable.defaults.url = '/settings/save';
    $.fn.editable.defaults.mode = 'inline';
    $.fn.editable.defaults.inputclass = 'form-control';
    $.fn.editableform.buttons =
        '<button type="submit" class="btn btn-success editable-submit btn-sm"><i class="icon-check"></i></button>' +
        '<button type="button" class="btn editable-cancel btn-sm"><i class="icon-cross3"></i></button>';


    //enable / disable
    $('#enable').click(function () {
        $('#user .editable').editable('toggleDisabled');
    });

    //editables 
    $('.trans').editable({
        url: '/settings/update',
        type: 'text'
    });

    $('#username').editable({
        url: '/settings/update',
        type: 'text',
        pk: 1,
        title: 'Enter username'
    });

    $('#firstname').editable({
        validate: function (value) {
            if ($.trim(value) == '') return 'This field is required';
        }
    });

    $('.sex').editable({
        source: [
            { value: "Male", text: 'Male' },
            { value: "Female", text: 'Female' }
        ],
        display: function (value, sourceData) {
            var colors = { "Male": "green", "Female": "blue" },
                elem = $.grep(sourceData, function (o) { return o.value == value; });

            if (elem.length) {
                $(this).text(elem[0].text).css("color", colors[value]);
            } else {
                $(this).empty();
            }
        },
        url: '/student/editStudent',
        name: 'sex',
        
    });

    $('#status').editable();

    $('#group').editable({
        showbuttons: false
    });

    $('#vacation').editable({
        datepicker: {
            todayBtn: 'linked'
        }
    });

    $('.dob').editable({
        url: '/student/editStudent',
        pk: 1,
        name: 'dob',
        title: 'Enter Date of Birth'
    });
    $('#event').editable({
        placement: 'right',
        combodate: {
            firstItem: 'name'
        }
    });

    $('#meeting_start').editable({
        format: 'yyyy-mm-dd hh:ii',
        viewformat: 'dd/mm/yyyy hh:ii',
        validate: function (v) {
            if (v && v.getDate() == 10) return 'Day cant be 10!';
        },
        datetimepicker: {
            todayBtn: 'linked',
            weekStart: 1
        }
    });

    $('#comments').editable({
        showbuttons: 'bottom'
    });

    $('#note').editable();
    $('#pencil').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $('#note').editable('toggle');
    });

    $('#fruits').editable({
        pk: 1,
        limit: 3,
        source: [
         { value: 1, text: 'banana' },
         { value: 2, text: 'peach' },
         { value: 3, text: 'apple' },
         { value: 4, text: 'watermelon' },
         { value: 5, text: 'orange' }
        ]
    });

    $('#tags').editable({
        inputclass: 'input-large',
        select2: {
            tags: ['html', 'javascript', 'css', 'ajax'],
            tokenSeparators: [",", " "]
        }
    });

   
    $('#address').editable({
        url: '/post',
        value: {
            city: "Moscow",
            street: "Lenina",
            building: "12"
        },
        validate: function (value) {
            if (value.city == '') return 'city is required!';
        },
        display: function (value) {
            if (!value) {
                $(this).empty();
                return;
            }
            var html = '<b>' + $('<div>').text(value.city).html() + '</b>, ' + $('<div>').text(value.street).html() + ' st., bld. ' + $('<div>').text(value.building).html();
            $(this).html(html);
        }
    });

    $('#user .editable').on('hidden', function (e, reason) {
        if (reason === 'save' || reason === 'nochange') {
            var $next = $(this).closest('tr').next().find('.editable');
            if ($('#autoopen').is(':checked')) {
                setTimeout(function () {
                    $next.editable('show');
                }, 300);
            } else {
                $next.focus();
            }
        }
    });

});