$(document).ready(function () {
    $("form.form").submit(function () {
        if ($("#termsAndConditions").val()) {
            return true;
        }
        alertAtomic("Debe aceptar los términos y condiciones.");
        return false;
    });
});