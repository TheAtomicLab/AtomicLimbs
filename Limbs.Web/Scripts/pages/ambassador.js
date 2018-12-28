$(document).ready(function () {

    let validationTemplate = `<div class="validation-summary-errors " data-valmsg-summary="true">
                                <span>Hay errores en el formulario, revis&#225; los campos marcados en rojo</span>
                            </div>`;

    let organization = $('#Organization');
    let organizationName = $('#OrganizationName');
    let roleInOrganization = $('#RoleInOrganization');
    let organizationBox = $('.organization-box');
    let frm = $('.form');

    if (organization.val() !== 'CuentaPropia') {
        organizationBox.show();
    }

    $('#Organization').change(function () {
        if ($(this).val() === 'CuentaPropia') {
            organizationName.val('');
            roleInOrganization.val('');

            organizationBox.hide();
        } else {
            organizationBox.show();
        }
    });

    //var settngs = $.data($('.form')[0], 'validator').settings;
    //settngs.ignore = ".ignore";

    frm.submit(function (e) {
        if (!$(this).valid()) {
            if (!$('.validation-summary-errors').length) {
                $(validationTemplate).insertAfter('h2.f-titulo');
            }

            e.preventDefault();
        } else {
            $('#btn-register').prop('disabled', true);
        }
    });
});