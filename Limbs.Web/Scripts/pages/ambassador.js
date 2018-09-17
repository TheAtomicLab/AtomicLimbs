$(document).ready(function () {

    $("[name='ambassadorPrinter']").hide();

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
}

function showPrinter() {
    $("[name='ambassadorPrinter']").show();
}