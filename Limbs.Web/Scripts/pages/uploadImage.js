    $(document).ready(function () {
        disabledRegister();

        var formManoMedidas = $("#formManoMedidas").dropzone({
            url: null,
            maxFilesize: 5,
            maxFiles: 1,
            uploadMultiple: false,
            maxThumbnailFilesize: 1,
            acceptedFiles: "image/*",
            autoProcessQueue: false,
            autoQueue: true,
            addRemoveLinks: true,
            dictDefaultMessage: "<i class=\"fa fa-upload fontFileUpload\" aria-hidden=\"true\" style=\"font-size: 5em;color: #2a2a56;\"></i><br><span class=\"fontUploadFile\">Arrastre su imagen aquí o presione click para cargar una.<span>",
            dictFileTooBig: "La imagen es muy grande ({{filesize}}MB). Tamaño máximo: {{maxFilesize}}MB.",
            dictInvalidFileType: "El tipo de archivo es invalido.",
            dictRemoveFile: "Borrar imagen",
            dictCancelUpload: "Cancelar subida",
            dictCancelUploadConfirmation: "¿Estas seguro de cancelar la subida de la foto?",
            dictRemoveFileConfirmation: null,
            dictMaxFilesExceeded: "No puedes subir más de una imagen.",
            createImageThumbnails: false, 
            init: function () {
                dropZoneManoMedidas = this;

                //defino el submit button
                $("#enviar").click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    //procesa los archivos que estan en la cola (en este caso 1)
                    dropZoneManoMedidas.processQueue();
                });
                
                this.on("addedfile", function (file) {

                    if (dropZoneManoMedidas.files.length > 1) {
                        /*for (var i = 1; i < dropZoneManoMedidas.files.length; i++) {
                            dropZoneManoMedidas.removeFile(dropZoneManoMedidas.files[i]);
                        }
                        */
                        dropZoneManoMedidas.removeFile(dropZoneManoMedidas.files[0]);
                    }

                    enableRegister()
                });
                this.on("removedfile", function (file) {
                    disabledRegister()
                });

                this.on("error", function (file,msg) {
                    disabledRegister()
                });
                
                this.on("success", function (file, response) {
                    window.location = response.Action;
                });
               
            }
        });

        var filesDropManoMedidas = dropZoneManoMedidas.files;
});

    function validateImg() {
        if (filesDropManoMedidas.length < 1) {
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