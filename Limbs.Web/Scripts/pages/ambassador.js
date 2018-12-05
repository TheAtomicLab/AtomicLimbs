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

    $('[name="havePrinter"]').on("change", function () {
        havePrinter($(this).val());
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

    frm.submit(function (e) {
        if (!$(this).valid()) {
            e.preventDefault();
        } else {
            $('#btn-register').prop('disabled', true);
        }
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