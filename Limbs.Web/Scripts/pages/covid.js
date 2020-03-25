$(document).ready(function () {
    let validationTemplate = `<div class="validation-summary-errors" data-valmsg-summary="true">
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
        $('.msg-success').hide();

        if (!$(this).valid()) {
            if (!$('.validation-summary-errors').length) {
                $(validationTemplate).insertAfter('h2.f-titulo');
            }

            e.preventDefault();
        } else {
            e.preventDefault();

            $.ajax({
                url: frm.get(0).action,
                type: frm.get(0).method,
                data: frm.serializeArray(),
                beforeSend: function () {
                    $('#loadingModal').fadeIn();
                    $('.validation-summary-errors').remove();
                    $('#btn-register').prop('disabled', true);
                },
                success: function (r) {
                    if (r.Error) {
                        if (r.Msg) {
                            $(`<div class="validation-summary-errors" data-valmsg-summary="true">
                                <span>` + r.Msg + `</span>
                            </div>`).insertAfter('h2.f-titulo');
                        } else {
                            $(validationTemplate).insertAfter('h2.f-titulo');
                        }
                    } else {
                        $('.msg-success').show();
                        frm[0].reset();
                        $(window).scrollTop(0);
                    }
                },
                error: function (r) {
                    console.log(r);
                },
                complete: function () {
                    $("#loadingModal").fadeOut();
                    $('#btn-register').prop('disabled', false);
                }
            });
        }
    });
});