$(document).ready(function () {

    //$("[name='ambassadorPrinter']").hide();

    let organization = $('#Organization');
    let organizationName = $('#OrganizationName');
    let roleInOrganization = $('#RoleInOrganization');
    let organizationBox = $('.organization-box');
    let frm = $('.form');

    if (organization.val() !== 'CuentaPropia') {
        organizationBox.show();
    }

    //$('[name="havePrinter"]').on("change", function () {
    //    havePrinter($(this).val());
    //});
    let printingArea = $('#Printer_PrintingArea');
    let printerWidth = $('#Printer_Width');
    let printerLong = $('#Printer_Long');
    let printerHeight = $('#Printer_Height');

    printingArea.on('input', function () {
        printerWidth.val('').addClass('ignore');
        printerLong.val('').addClass('ignore');
        printerHeight.val('').addClass('ignore');

        $(this).removeClass('ignore');
    });

    $(printerHeight, printerWidth, printerLong).on('input', function () {
        printingArea.val('').addClass('ignore');
        $(this).removeClass('ignore');
    });

    $('#Organization').change(function () {
        if ($(this).val() === 'CuentaPropia') {
            organizationName.val('');
            roleInOrganization.val('');

            organizationBox.hide();
        } else {
            organizationBox.show();
        }
    });

    var settngs = $.data($('.form')[0], 'validator').settings;
    settngs.ignore = ".ignore";

    frm.submit(function (e) {
        if (!$(this).valid()) {
            e.preventDefault();
        } else {
            $('#btn-register').prop('disabled', true);
        }
    });
});


//function havePrinter(value) {
//    if (value != "true") {
//        hidePrinter();
//    } else {
//        showPrinter();
//    }
//}

//function hidePrinter() {
//    $("[name='ambassadorPrinter']").hide();

//    $("[name='Printer.Brand']").val('');
//    $("[name='Printer.Model']").val('');
//    $("[name='Printer.Width']").val('');
//    $("[name='Printer.Long']").val('');
//    $("[name='Printer.Height']").val('');
//    $("[name='Printer.PrintingArea']").val('');

//    $("[name='Printer.IsHotBed']").prop('checked', false);
//}

//function showPrinter() {
//    $("[name='ambassadorPrinter']").show();
//}