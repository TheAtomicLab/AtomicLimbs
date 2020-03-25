$(document).ready(function () {

    let validationTemplate = `<div class="validation-summary-errors " data-valmsg-summary="true">
                                <span>Hay errores en el formulario, revis&#225; los campos marcados en rojo</span>
                            </div>`;

    let organization = $('#CovidOrganizationEnum');
    let organizationName = $('#OrganizationName');
    let organizationBox = $('.organization-box');
    let frm = $('.form');

    organization.change(function () {
        if ($(this).val() !== 'Otro') {
            organizationName.val('');
            organizationBox.addClass('hide');
        } else {
            organizationBox.removeClass('hide');
        }
    });

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