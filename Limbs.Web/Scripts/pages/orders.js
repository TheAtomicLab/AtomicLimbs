$(document).ready(function () {
    let frm = $('#formManoMedidas');
    let btnEnviar = $('#enviar');

    disabledRegister();
    $("#fileUpload").bind("change",
        function () {
            if (this.files[0].size > 1000000 * 5) {
                alertAtomic("Seleccione un archivo menor a 5 MB.", "#alertAtomic");
                this.value = null;
            }
        });

    $("#fileUpload").on("change", function () {
        validateImg();
    });

    btnEnviar.click(function () {
        frm.submit();
        $(this).attr('disabled', true);
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
    $("[name=enviar]").prop('disabled', true);
};

function enableRegister() {
    $("[name=enviar]").prop('disabled', false);
};