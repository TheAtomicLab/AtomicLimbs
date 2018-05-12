    $(document).ready(function () {
        disabledRegister();

        var typeFileAccepted = "image/*";
        var maxFileSizeImage = 5; //MB
        var fileTooBigMsg = "El archivo es muy grande. Tamaño máximo permitido: " + maxFileSizeImage + " MB.";
        var invalidFileTypeMsg = "Tipo de archivo inválido";


        //Documentation: http://www.dropzonejs.com/
        var formManoMedidas = $("#formManoMedidas").dropzone({
            url: null,
            previewTemplate: "<div class=\"dz-preview dz-file-preview\">\n  <div class=\"dz-imageMano\"><img data-dz-thumbnail /></div>\n  <div class=\"dz-details\">\n    <div class=\"dz-size\"><span data-dz-size></span></div>\n    <div class=\"dz-filename\"><span data-dz-name></span></div>\n  </div>\n  <div class=\"dz-error-message\"><span data-dz-errormessage></span></div>\n</div>",
            maxFilesize: 5,
            maxFiles: 1,
            uploadMultiple: false,
            maxThumbnailFilesize: 1,
            acceptedFiles: "image/*",
            autoProcessQueue: false,
            autoQueue: true,
            addRemoveLinks: true,
            dictDefaultMessage: "<i class=\"fa fa-upload fontFileUpload\" aria-hidden=\"true\" style=\"font-size: 5em;color: #2a2a56;\"></i><br><span class=\"fontUploadFile\">Arrastre su imagen aquí o presione click para cargar una.<span>",
            //dictFileTooBig: fileTooBig,
            //dictInvalidFileType: "El tipo de archivo es invalido.",
            dictFileTooBig: null,
            dictInvalidFileType: null,
            dictRemoveFile: "Borrar imagen",
            dictCancelUpload: null,
            //dictCancelUploadConfirmation: "¿Estas seguro de cancelar la subida de la foto?",
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

                    $('#msgErrorManoImagen').empty();

                    if (!isConditionsValid(file)) {
                        showErrors(file);
                    } else {
                        enableRegister();
                    }
                });
                this.on("removedfile", function (file) {
                    $('#msgErrorManoImagen').empty();
                    disabledRegister()
                });

                this.on("error", function (file) {
                    disabledRegister();
                });

                this.on("processing", function (file) {
                    console.log("Subiendo imagen");
                    $('#msgManoImagen').empty();
                    $('#errorMessages').html("Subiendo imágen");
                });

                this.on("success", function (file, response) {
                    window.location = response.Action;
                });
               
            }
        });

        var filesDropManoMedidas = dropZoneManoMedidas.files;

        function isConditionsValid(file) {
            var ConditionTypeImage = Dropzone.isValidFile(file, typeFileAccepted);
            var ConditionSizeImage = isValidSizeImage(file, maxFileSizeImage);

            return ConditionTypeImage && ConditionSizeImage;
        }

        function showErrors(file) {
            var ConditionTypeImage = Dropzone.isValidFile(file, typeFileAccepted);
            var ConditionSizeImage = isValidSizeImage(file, maxFileSizeImage);

            if (!ConditionSizeImage) {
                $('#msgErrorManoImagen').append("<li>"+fileTooBigMsg+"</li>");
            }

            if (!ConditionTypeImage) {
                $('#msgErrorManoImagen').append("<li>"+invalidFileTypeMsg + "</li>");
            }
        }
});

    

    function isValidSizeImage(file, nro) {
        var sizeFile = file.size / 1024 / 1024;
        return sizeFile <= nro;
    }

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