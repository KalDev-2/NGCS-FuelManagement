$('.etDate').each(function () {
   var gdate= convToEtDate($(this).text());
    //var gcdt = jQuery.format.date(new Date($(this).text()), "dd/MM/yyyy");
    //var date = gcdt.split('/');
    //var year = parseInt(date[2], 10);
    //var month = parseInt(date[1], 10);
    //var day = parseInt(date[0], 10);
    //var dt = $.calendars.instance('gregorian').newDate(year, month, day).toJD();
    //$(this).text(jQuery.format.date(new Date($.calendars.instance('ethiopian').fromJD(dt)), "dd/MM/yyyy"));
    $(this).text(gdate);
});

function convToEtDate(zethDate) {
    try {
        var gcdt = jQuery.format.date(new Date(zethDate), "dd/MM/yyyy");
        var date = gcdt.split('/');
        var year = parseInt(date[2], 10);
        var month = parseInt(date[1], 10);
        var day = parseInt(date[0], 10);
        var dt = $.calendars.instance('gregorian').newDate(year, month, day).toJD();

        return jQuery.format.date(new Date($.calendars.instance('ethiopian').fromJD(dt)), "dd/MM/yyyy");
    }
    catch (err) {
        console.log(err)
    }
    
}

function convToGCDate(zethDate) {
    try {
        var gcdt = jQuery.format.date(new Date(zethDate), "dd/MM/yyyy");
        var date = gcdt.split('/');
        var year = parseInt(date[2], 10);
        var month = parseInt(date[1], 10);
        var day = parseInt(date[0], 10);
        var dt = $.calendars.instance('ethiopian').newDate(year, month, day).toJD();

        return jQuery.format.date(new Date($.calendars.instance('gregorian').fromJD(dt)), "MM/dd/yyyy");
    }
    catch (err) {
        console.log(err)
    }
   
}

$('.timeAgo').each(function () {
    var timeAgodisp = jQuery.format.prettyDate(new Date($(this).text()).getTime())
   
    $(this).text(timeAgodisp);

});

function dateDiff(startingDate, endingDate) {
    let startDate = new Date(new Date(startingDate).toISOString().substr(0, 10));
    if (!endingDate) {
        endingDate = new Date().toISOString().substr(0, 10); // need date in YYYY-MM-DD format
    }
    let endDate = new Date(endingDate);
    if (startDate > endDate) {
        const swap = startDate;
        startDate = endDate;
        endDate = swap;
    }
    const startYear = startDate.getFullYear();
    const february = (startYear % 4 === 0 && startYear % 100 !== 0) || startYear % 400 === 0 ? 29 : 28;
    const daysInMonth = [31, february, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    let yearDiff = endDate.getFullYear() - startYear;
    let monthDiff = endDate.getMonth() - startDate.getMonth();
    if (monthDiff < 0) {
        yearDiff--;
        monthDiff += 12;
    }
    let dayDiff = endDate.getDate() - startDate.getDate();
    if (dayDiff < 0) {
        if (monthDiff > 0) {
            monthDiff--;
        } else {
            yearDiff--;
            monthDiff = 11;
        }
        dayDiff += daysInMonth[startDate.getMonth()];
    }
   
    return yearDiff + '/' + monthDiff + '/' + dayDiff;
}