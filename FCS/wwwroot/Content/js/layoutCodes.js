window.CKEDITOR_BASEPATH = '/Content/global/js/plugins/editors/ckeditor/';

function changeLayout(name) {
    $.ajax({
        cache: false,
        type: 'GET',
        url: '/Home/ChangeLayout',
        data: { 'Name': name },
        success: function (data) {
            window.location.reload(true);
        }
    });
}
$(document).ready(function () {
    $('#npag').find('.select2-selection').attr('style', 'max-height: 34px; width:60px;');
   
    if (document.body.clientWidth<576) {
        $('#Search_data').attr('style', 'width: 100px');
    } else {
        $('#Search_data').attr('style', 'width: 300px');
    }
    var url = window.location.pathname;
    //$('li.nav-item a[href="' + url + '"]').addClass('active').parent().parent().attr("style", "display: block;").parent().addClass('nav-item-open');
    //$('li.nav-item a[href="' + url.substr(0, url.lastIndexOf('/')) + '"]').addClass('active').parent().parent().attr("style", "display: block;").parent().addClass('nav-item-open');
    //$('li.nav-item a[href="/' + url.split('/')[1]+ '"]').addClass('active').parent().parent().attr("style", "display: block;").parent().addClass('nav-item-open');

    $('.oursidemenu a').filter(function () {
        //alert(url.substr(0, url.lastIndexOf('/')) + " " + this.href);
        if (this.href.split('/')[4] == null && url.split('/')[2] == null ) {
            if (url.split('/')[1] == this.href.split('/')[3]) {
                return this.href;
            }
        } else if (this.href.split('/')[4] != null && url.split('/')[2] != null) {
            if (url.split('/')[1] + '/' + url.split('/')[2] == this.href.split('/')[3] + '/' + this.href.split('/')[4]) {
                return this.href;
            }
        }
        else if (this.href.split('/')[4] == null && url.split('/')[2] != null) {
            if (url.split('/')[1] == this.href.split('/')[3]) {
                return this.href;
            }
        }
        else {
            if (url.split('/')[1] + '/' + url.split('/')[2] == this.href.split('/')[3] + '/' + this.href.split('/')[4]) {
                return this.href;
            } 
        }
    }).addClass('active bg-dark').children().addClass('icon-check').parent().parent().parent().attr("style", "display: block;").parent().addClass('nav-item-open');
   
});

