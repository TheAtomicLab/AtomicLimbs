$(document).ready(function () {

    //se cambia propiedades del input Email
    $('#Email').attr('readonly', true);
    $('#Email').css('background-color', '#000000');
    $('#Email').css('opacity', 0.7);
    $("#Birth").datepicker(
        {
            minDate: new Date(1900, 1, 1),
            maxDate: "-18Y",
            dateFormat: "yy-mm-dd",//ISO 8601
            changeYear: true,
            changeMonth: true,
            yearRange: "-110:-18"
        }
    );
});
