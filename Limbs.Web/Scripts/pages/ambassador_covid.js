﻿$(document).ready(function () {
    let frmQuantity = $('.form-quantity');
    let quantityResponseMsg = $('.quantityResponseMsg');

    let grid_pedidos = $(".grid_pedidos").masonry({
        itemSelector: '.card'
    });

    $(".grid_pedidos .card").bind("DOMSubtreeModified", function () {
        $(grid_pedidos).masonry();
    });
    grid_pedidos.imagesLoaded().progress(function () {
        $(grid_pedidos).masonry();
    });

    let inputCant = $('#CantEntregable');
    let savedTmpCant = parseInt(inputCant.val());

    $('.addCovidQuantity').click(function () {
        quantityResponseMsg.html('');
        inputCant.val(parseInt(inputCant.val()) + 1);
    });

    $('.removeCovidQuantity').click(function () {
        quantityResponseMsg.html('');

        let cant = parseInt(inputCant.val());
        if (cant === 0) {
            return;
        }

        inputCant.val(cant - 1);
    });

    $('.addCovidOrderQuantity').click(function () {
        let inputCantToUpdate = $(this).parent().prev().children();
        let inputCantToUpdateVal = parseInt(inputCantToUpdate.val());

        let maxCant = parseInt($(this).attr('data-max'));

        if (inputCantToUpdateVal === maxCant) {
            return;
        }

        inputCantToUpdate.val(inputCantToUpdateVal + 1);
    });

    $('.removeCovidOrderQuantity').click(function () {
        let inputCantToUpdate = $(this).parent().next().children();
        let inputCantToUpdateVal = parseInt(inputCantToUpdate.val());

        if (inputCantToUpdateVal === 0) {
            return;
        }

        inputCantToUpdate.val(inputCantToUpdateVal - 1);
    });

    $('.saveQuantityOrder').click(function (e) {
        e.preventDefault();

        $('.error').parent().parent().remove();
        let frmQuantityOrder = $(this).parents().eq(8);
        let saveBtn = $(this);

        let inputTmpSaved = saveBtn.parent().prev().prev().children();
        let cantSaved = parseInt(inputTmpSaved.val());

        if (cantSaved === 0 && saveBtn.attr('data-isEdit') === 'False') {
            return;
        }

        let execute = confirm('¿Está seguro?');
        if (!execute) {
            return;
        }

        $.ajax({
            url: frmQuantityOrder.get(0).action,
            type: frmQuantityOrder.get(0).method,
            data: frmQuantityOrder.serializeArray(),
            beforeSend: function () {
                $('#loadingModal').fadeIn();
                saveBtn.prop('disabled', true);
                quantityResponseMsg.html('');
            },
            success: function (r) {
                if (r.Error) {
                    if (r.Msg) {
                        $('<tr><td colspan="2"><span class="error">' + r.Msg + '</span></td></tr>').insertAfter(saveBtn.parents().eq(5));
                    }
                } else {
                    inputTmpSaved.val('0');
                    inputCant.val(parseInt(inputCant.val()) - cantSaved);
                    savedTmpCant = parseInt(inputCant.val());

                    if (saveBtn.attr('data-isFeatured') === 'True') {
                        alert('Se van a estar contactando desde delivery entre hoy y dos días, por favor espera el llamado de ellos a tu teléfono.');
                    }
                    window.location = window.location;
                }
            },
            error: function (r) {
                console.log(r);
            },
            complete: function () {
                $("#loadingModal").fadeOut();
                saveBtn.prop('disabled', false);
            }
        });
    });

    $('.sendQuantity').click(function (e) {
        e.preventDefault();
        let saveFirstBtn = $(this);

        $.ajax({
            url: frmQuantity.get(0).action,
            type: frmQuantity.get(0).method,
            data: frmQuantity.serializeArray(),
            beforeSend: function () {
                $('#loadingModal').fadeIn();
                $('.validation-summary-errors').remove();
                saveFirstBtn.prop('disabled', true);
                quantityResponseMsg.html('');
            },
            success: function (r) {
                if (r.Error) {
                    if (r.Msg) {
                        quantityResponseMsg.removeClass('success').addClass('error').html(r.Msg);
                    } else {
                        quantityResponseMsg.removeClass('success').addClass('error').html('Ha ocurrido un error, vuelva a intentarlo.');
                    }
                } else {
                    savedTmpCant = parseInt(inputCant.val());
                    quantityResponseMsg.removeClass('error').addClass('success').html('Cantidad actualizada correctamente.');
                    window.location = window.location;
                }
            },
            error: function (r) {
                console.log(r);
            },
            complete: function () {
                $("#loadingModal").fadeOut();
                saveFirstBtn.prop('disabled', false);
            }
        });
    });

    let pag = $('#pagination-demo');
    let totalPages = parseInt(pag.attr('data-totalPages'));
    let actualPage = parseInt(pag.attr('data-actualPage'));

    pag.twbsPagination({
        totalPages: totalPages,
        visiblePages: 10,
        startPage: actualPage,
        first: '<<',
        prev: 'Anterior',
        last: '>>',
        next: 'Siguiente',
        onPageClick: function (event, page) {
            if (page !== actualPage) {
                window.location = window.location.protocol + "//" + window.location.host + window.location.pathname + '?pag=' + page
            }
        }
    });
});
