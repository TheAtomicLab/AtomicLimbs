$(document).ready(function () {
    let frmQuantity = $('.form-quantity');
    let quantityResponseMsg = $('.quantityResponseMsg');

    let grid_pedidos = $(".grid_pedidos").masonry({
        itemSelector: '.card'
    });

    $(".grid_pedidos .card").bind("DOMSubtreeModified", function () { $(grid_pedidos).masonry(); });
    grid_pedidos.imagesLoaded().progress(function () {
        $(grid_pedidos).masonry();
    });

    let inputCant = $('#CantEntregable')

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


    $('.sendQuantity').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: frmQuantity.get(0).action,
            type: frmQuantity.get(0).method,
            data: frmQuantity.serializeArray(),
            beforeSend: function () {
                $('#loadingModal').fadeIn();
                $('.validation-summary-errors').remove();
                $(this).prop('disabled', true);
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
                    quantityResponseMsg.removeClass('error').addClass('success').html('Cantidad actualizada correctamente.');
                }
            },
            error: function (r) {
                console.log(r);
            },
            complete: function () {
                $("#loadingModal").fadeOut();
                $(this).prop('disabled', false);
            }
        });
    });
});
