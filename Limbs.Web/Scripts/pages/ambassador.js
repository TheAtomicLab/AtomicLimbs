$(document).ready(function () {

    //$("[name='ambassadorPrinter']").hide();

    $('[name="havePrinter"]').on("change", function () {
        havePrinter($(this).val());
    });
});

function havePrinter(value) {
    if (value != "true") {
        hidePrinter();
    } else {
        showPrinter();
    }
}

function hidePrinter() {
    $("[name='ambassadorPrinter']").hide();

    $("[name='Printer.Brand']").val('');
    $("[name='Printer.Model']").val('');
    $("[name='Printer.Width']").val('');
    $("[name='Printer.Long']").val('');
    $("[name='Printer.Height']").val('');
    $("[name='Printer.PrintingArea']").val('');

    $("[name='Printer.IsHotBed']").prop('checked', false);
}

function showPrinter() {
    $("[name='ambassadorPrinter']").show();
}