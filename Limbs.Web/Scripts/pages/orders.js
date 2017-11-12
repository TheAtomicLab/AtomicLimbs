$(document).ready(function () {

    /*#####################
    var uploader = $('#formDropzone');
    var dropzoneOptions = {
        dictDefaultMessage: 'Drop Here!',
        paramName: "file",
        maxFilesize: 2, // MB
        maxFiles: 1,
        addRemoveLinks: true,
        init: function () {
            this.on("success", function (file) {
                console.log("success > " + file.name);
            });
        }
    };
    var newDropzone = new Dropzone(uploader, dropzoneOptions);

    #######################*/




    disabledRegister();
    $("#formDropzone").bind("change",
        function () {
            if (this.files[0].size > 1000000 * 5) {
                alertAtomic("Seleccione un archivo menor a 5 MB.","#alertAtomic");
                this.value = null;
            }
        });

    $("#formDropzone").on("change", function () {
        validateImg();
    });
});


function validateImg() {
    if ($("#formDropzone").val() == "") {
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