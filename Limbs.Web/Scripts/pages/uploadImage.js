﻿$(document).ready(function () {
    disabledRegister();

    var typeFileAccepted = "image/*";
    var maxFileSizeImage = 5; //MB
    var maxUploadFiles = 6;
    var fileTooBigMsg = "El archivo es muy grande. Tamaño máximo permitido: " + maxFileSizeImage + " MB.";
    var invalidFileTypeMsg = "Tipo de archivo inválido";
    var msgWhileUploadFile = "Subiendo imagen, por favor espere"


    //Documentation: http://www.dropzonejs.com/
    var formManoMedidas = $("#formManoMedidas").dropzone({
        url: null,
        previewTemplate: "<div class=\"dz-preview dz-file-preview\">\n  <div class=\"dz-imageMano\"><img data-dz-thumbnail /></div>\n  <div class=\"dz-details\">\n    <div class=\"dz-size\"><span data-dz-size></span></div>\n    <div class=\"dz-filename\"><span data-dz-name></span></div>\n  </div>\n </div>",
        maxFilesize: maxFileSizeImage,
        maxFiles: maxUploadFiles,
        uploadMultiple: true,
        maxThumbnailFilesize: 1,
        acceptedFiles: "image/*",
        autoProcessQueue: false,
        autoQueue: true,
        addRemoveLinks: true,
        dictDefaultMessage: "<i class=\"fa fa-upload fontFileUpload\" aria-hidden=\"true\" style=\"font-size: 5em;color: #2a2a56;\"></i><br><span class=\"fontUploadFile\">Arrastre su imagen aquí o presione click para cargar una.<span>",
        //dictFileTooBig: fileTooBig,
        //dictInvalidFileType: "El tipo de archivo es invalido.",
        //dictFileTooBig: null,
        dictInvalidFileType: null,
        dictRemoveFile: "Borrar imagen",
        dictCancelUpload: null,
        //dictCancelUploadConfirmation: "¿Estas seguro de cancelar la subida de la foto?",
        dictRemoveFileConfirmation: null,
        //dictMaxFilesExceeded: "No puedes subir más de una imagen.",
        createImageThumbnails: false,
        init: function () {
            dropZoneManoMedidas = this;

            //defino el submit button
            $("#enviar").click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                //procesa los archivos que estan en la cola 
                dropZoneManoMedidas.processQueue();
            });

            this.on("processing", function (file) {

                $('#msgManoImagen').empty();
                $('#msgManoImagen').html(msgWhileUploadFile);

            });

            this.on("addedfile", function (file) {

                if (dropZoneManoMedidas.files.length > maxUploadFiles) {
                    /*for (var i = 1; i < dropZoneManoMedidas.files.length; i++) {
                        dropZoneManoMedidas.removeFile(dropZoneManoMedidas.files[i]);
                    }
                    */
                    dropZoneManoMedidas.removeFile(dropZoneManoMedidas.files[0]);
                }

                $('#msgErrorManoImagen').empty();

                //Recorrer TODOS los archivos

                updateRegisterFiles();


            });
            this.on("removedfile", function (file) {
                $('#msgErrorManoImagen').empty();

                updateRegisterFiles();

                dropZoneManoMedidas.files.length == 0 ? disabledRegister() : "";

            });

            this.on("error", function (file, errormessage, xhr) {
                if (xhr == undefined)
                    return false;

                var t = xhr.response.responseText;

                $('#msgManoImagen').empty();
                $('#msgErrorManoImagen').empty();
                showError("Ocurrió un error. Por favor envíenos la foto para solucionarle el problema.");
                console.log($(errormessage)[1].textContent);

                disabledRegister();
            });

            //TODO: Barra cargando
            this.on("uploadprogress", function (file, progress) {
                //Progress -> Progress file upload
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
        Dropzone.sizeFile
        var ConditionSizeImage = isValidSizeImage(file, maxFileSizeImage);

        if (!ConditionSizeImage) {
            showError(fileTooBigMsg);
        }

        if (!ConditionTypeImage) {
            showError(invalidFileTypeMsg);
        }
    }

    function updateRegisterFiles() {
        for (var i = 0; i < dropZoneManoMedidas.files.length; i++) {

            var file = dropZoneManoMedidas.files[i];

            if (!isConditionsValid(file)) {
                disabledRegister();
                showErrors(file);
                break;
            } else {
                enableRegister();
            }
        }
    }
});



function isValidSizeImage(file, nro) {
    //var sizeFile = file.size / 1024 / 1024;
    sizeFile = file.size / 1000000 // -> Dropzone calculate size
    return sizeFile <= nro;
}

function validateImg() {
    if (filesDropManoMedidas.length < 1) {
        disabledRegister()
    } else {
        enableRegister()
    }
}

function showError(msg) {
    var ErrorMsg = "<li>" + msg + "</li>";

    $('#msgErrorManoImagen').append(ErrorMsg);
}

function disabledRegister() {
    $("[name=enviar]").prop('disabled', true);
};

function enableRegister() {
    $("[name=enviar]").prop('disabled', false);
};