$(document).ready(function () {
    disabledRegister();
    $("#fileUpload").bind("change",
        function () {
            if (this.files[0].size > 1000000 * 5) {
                alertAtomic("Seleccione un archivo menor a 5 MB.","#alertAtomic");
                this.value = null;
            }
        });

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