$(document).ready(function() {
    setDatePicker(0);

    $('[name="IsProductUser"]').on("change", function () {
        isProductUser($(this).val());
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

function isProductUser(value) {
    if (value === "true") {
        //es usuario
        setDatePicker(18);
        $("#ResponsableName").hide();
        $("#ResponsableLastName").hide();
        $("#ResponsableDni").hide();
        $("[name='titleDateUser']").hide();
        $("#isAdultCheckContainer").hide();
    } else {
        //no es usuario
        setDatePicker(0);
        $("#ResponsableName").show();
        $("#ResponsableLastName").show();
        $("#ResponsableDni").show();
        $("[name='titleDateUser']").show();
        $("#isAdultCheckContainer").show();
    }
}

function validAdultCheck(check) {
    if (!check.is(":checked")) {
        alertAtomic("Por favor. Si usted no es el usuario de la prótesis es necesario que sea mayor de 18 años.");
        return false;
    }
    return true;
}