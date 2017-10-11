$(document).ready(function () {
    disabledRegister();
    $("#fileUpload").on("change", function () {
        validateImg();
    });
});


function validateImg() {
    if ($("#fileUpload").val() == "") {
        disabledRegister()      
    } else {
        enableRegister()
    }
}

function disabledRegister() {
    $("[name=enviar]").removeClass("blue_button");
    $("[name=enviar]").addClass("disabled_button");
    $("[name=enviar]").prop('disabled', true);
};

function enableRegister() {
    $("[name=enviar]").addClass("blue_button");
    $("[name=enviar]").removeClass("disabled_button");
    $("[name=enviar]").prop('disabled', false);
};