//TODO: Sacar los alert y poner un mensaje lindo
$(document).ready(function() {
    setDatePicker(4);

    $('[name="selectUser"]').on("change", function () {
        selectUser($(this).val());
    });

    $("form.form").submit(function() {
        return isUser($('[name="selectUser"]').val());
    });
});

var checkBoxAdult = $('[name="isAdultCheck"]');

function setDatePicker(maxYear) {
    $("#Birth").datepicker("destroy");
    $("#Birth").datepicker(
        {
            minDate: new Date(1900, 1, 1),
            maxDate: "-" + maxYear + "Y",
            dateFormat: "yy-mm-dd",//ISO 8601
            changeYear: true,
            changeMonth: true,
            yearRange: "-110:-" + maxYear
        }
    );
}

function selectUser(value) {
    if (value === "true") {
        //es usuario
        setDatePicker(18);
        $("#UserName").hide();
        $("#UserLastName").hide();
        $("[name='titleDateUser']").hide();
        $("#isAdultCheckContainer").hide();
    } else {
        //no es usuario
        setDatePicker(4);
        $("#UserName").show();
        $("#UserLastName").show();
        $("[name='titleDateUser']").show();
        $("#isAdultCheckContainer").show();
    }
}

function isUser(val) {
    if (val === "true") {
        return validBirth(18);
    } else {
        return validBirth(4) && validAdultCheck(checkBoxAdult);
    }
}

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

    var birthday = new Date(now.getFullYear(), born.getMonth(), born.getDate());
    if (now >= birthday)
        return now.getFullYear() - born.getFullYear();
    else
        return -1;
}

function validAdultCheck(check) {
    if (!check.is(":checked")) {
        alert("Por favor. Si usted no es el usuario de la mano es necesario que sea mayor de 18 años.");
        return false;
    }
    return true;
}