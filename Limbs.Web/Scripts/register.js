$(document).ready(function () {
    $("#Country,#City,#Address").on('change', function () {
        disabledRegister();
    });
    disabledRegister();
});

function disabledRegister() {
    $("[name=register]").removeClass("blue_button");
    $("[name=register]").addClass("disabled_button");
    $("[name=register]").prop('disabled', true);
};

function enableRegister() {
    $("[name=register]").addClass("blue_button");
    $("[name=register]").removeClass("disabled_button");
    $("[name=register]").prop('disabled', false);
};