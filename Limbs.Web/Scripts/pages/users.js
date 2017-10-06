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

function validAdultCheck(check) {
    if (!check.is(":checked")) {
        alert("Por favor. Si usted no es el usuario de la mano es necesario que sea mayor de 18 años.");
        return false;
    }
    return true;
}