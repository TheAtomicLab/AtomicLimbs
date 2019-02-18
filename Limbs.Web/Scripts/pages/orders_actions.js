$(document).ready(function () {
    let frm = $('.form-modal');
    let settngs = $.data($('.form-modal')[0], 'validator').settings;
    settngs.ignore = ".ignore";

    let checkIsWrongImage = $('#IsWrongImages');
    let comments = $('#Comments');

    let modal = $('#myModal');

    let btn = $("#open_modal_btn");

    let span = $("#close_modal");

    btn.click(function () {
        modal.show();
    });

    span.click(function () {
        modal.hide();
    });

    $(window).click(function (event) {
        if (event.target == modal[0]) {
            modal.hide();
        }
    });

    checkIsWrongImage.change(function () {
        if (checkIsWrongImage.is(":checked")) {
            comments.addClass('ignore');
            comments.val('');
            return;
        }

        comments.removeClass('ignore');
    });

    frm.submit(function (e) {
        if (!$(this).valid()) {
            e.preventDefault();
        } else {
            $('#btn-send').prop('disabled', true);
        }
    });
});