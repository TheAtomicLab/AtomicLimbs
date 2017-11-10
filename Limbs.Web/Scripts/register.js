$(document).ready(function () {
    disabledRegister();
    $("#Country,#City,#Address").on('change', function () {
        disabledRegister();
    });
    $("form.form").submit(function () {
        if ($("#termsandconditions").is(":checked")) {
            return true;
        }
        alertAtomic("Debe aceptar los términos y condiciones.");
        return false;
    });
});

function disabledRegister() {
    $("[name=register]").removeClass("bn_blue");
    $("[name=register]").addClass("disabled_button");
    $("[name=register]").prop('disabled', true);
};

function enableRegister() {
    $("[name=register]").addClass("bn_blue");
    $("[name=register]").removeClass("disabled_button");
    $("[name=register]").prop('disabled', false);
};


function validBirth(minAge) {
    var age = getAge2($("#Birth").val());

    if (age < 0) {
        alertAtomic("Por favor ingresá una fecha válida.");
        return false;
    } else if (age < minAge) {
        alertAtomic("La edad tiene que ser mayor a " + minAge + " años.");
        return false;
    }
    return true;
}

function getAge(birth) {
    var birthday = +new Date(birth);
    return ~~((Date.now() - birthday) / (31557600000));
}