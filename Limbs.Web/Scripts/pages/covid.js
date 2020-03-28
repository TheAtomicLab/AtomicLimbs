$(document).ready(function () {
    let validationTemplate = `<div class="validation-summary-errors" data-valmsg-summary="true">
                                <span>Hay errores en el formulario, revis&#225; los campos marcados en rojo</span>
                            </div>`;

    let frm = $('.form');

    frm.submit(function (e) {
        $('.msg-success').hide();
        $('.validation-summary-errors').remove();

        if (!$(this).valid()) {
            if (!$('.validation-summary-errors').length) {
                $(validationTemplate).insertAfter('h2.f-titulo');
            }

            e.preventDefault();
        } else {
            e.preventDefault();

            if (!$('#isEdit').length && !$('#termsAndConditions').is(':checked')) {
                $(`<div class="validation-summary-errors" data-valmsg-summary="true">
                                <span>Debe aceptar los t&eacute;rminos y condiciones</span>
                            </div>`).insertAfter('h2.f-titulo');

                $(window).scrollTop(0);

                return;
            }

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
                        $(window).scrollTop(0);
                    } else {
                        if ($('#isEdit').length) {
                            $('.msg-success').show();
                            $(window).scrollTop(0);
                        } else {
                            $('.msg-success').show();
                            $(window).scrollTop(0);

                            window.setTimeout(function () {
                                window.location = r.UrlRedirect;
                            }, 8000);
                        }
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