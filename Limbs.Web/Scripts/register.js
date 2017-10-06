$(document).ready(function () {
    $("#Country,#City,#Address").on('change', function () {
        disabledRegister();
    });
    disabledRegister();
});

function disabledRegister() {
    $("[name=register]").removeClass("blue_button");
    $("[name=register]").addClass("disabled_button");
    $("[name=register]").prop('disabled', true);
};

function enableRegister() {
    $("[name=register]").addClass("blue_button");
    $("[name=register]").removeClass("disabled_button");
    $("[name=register]").prop('disabled', false);
};

function validBirth(minAge) {
    var age = getAge($("#Birth"));

    if (age < 0) {
        alert("Por favor ingresá una fecha válida. Todavía no naciste :) .");
        return false;
    } else if (age < minAge) {
        alert("La edad tiene que ser mayor a " + minAge + " años.");
        return false;
    }
    return true;
}

function getAge(birth) {

    var dob = birth.val();
    var now = new Date();
    var birthdate = dob.split("-");
    var born = new Date(birthdate[0], birthdate[1] - 1, birthdate[2]);

    var birthday = new Date(born.getFullYear(), born.getMonth(), born.getDate());
    if (now >= birthday)
        return now.getFullYear() - born.getFullYear();
    else
        return -1;
}
