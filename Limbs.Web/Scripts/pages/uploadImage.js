    $(document).ready(function () {

        var formDropzone = $("#formDropzone").dropzone({
            url: null,
            maxFilesize: 5,
            filesizeBase: 1000,
            maxFiles: 1,
            clickable: true,
            ignoreHiddenFiles: true,
            acceptedFiles: null,
            acceptedMimeTypes: null,
            autoProcessQueue: false,
            autoQueue: true,
            addRemoveLinks: false,
            previewsContainer: null,
            hiddenInputContainer: "body",
            capture: null,
            dictDefaultMessage: "Arrastre su imagen aqui o presione click para cargar una.",
            dictFallbackMessage: "Your browser does not support drag'n'drop file uploads.",
            dictFallbackText: "Please use the fallback form below to upload your files like in the olden days.",
            dictFileTooBig: "File is too big ({{filesize}}MiB). Max filesize: {{maxFilesize}}MiB.",
            dictInvalidFileType: "You can't upload files of this type.",
            dictResponseError: "Server responded with {{statusCode}} code.",
            dictCancelUpload: "Cancel upload",
            dictCancelUploadConfirmation: "Are you sure you want to cancel this upload?",
            dictRemoveFile: "Remove file",
            dictRemoveFileConfirmation: null,
            dictMaxFilesExceeded: "You can not upload any more files.",
            createImageThumbnails: false, 
            init: function () {
                myDropZone = this;
                $("#enviar").click(function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    myDropZone.processQueue();
                    //myDropZone.disable();
                });
                this.on("addedfile", function (file) {

                    // Create the remove button
                    var removeButton = Dropzone.createElement("<button> Remove file</button>");
                    var _this = this;

                    removeButton.addEventListener("click", function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        _this.removeFile(file);
                    });

                    // Add the button to the file preview element.
                    file.previewElement.appendChild(removeButton);
                    enableRegister()
                });
                
                this.on("complete", function (file) {
                    window.location = "/Orders/ManoMedidas/";
                });
            }
        });
    });
