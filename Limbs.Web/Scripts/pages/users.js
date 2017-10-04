//TODO: Sacar los alert y poner un mensaje lindo
$(document).ready(function () {
    var birth = $("#Birth");
    var birthDatepicker = birth.datepicker(
        {
            minDate: new Date(1900, 1, 1), maxDate: '-3Y',
            //ISO 8601
            dateFormat: 'yy-mm-dd',
            changeYear: true,
            changeMonth: true,
            yearRange: '-110:-3'
        }
    );

    var selectUs = $('[name="selectUser"]');
    selectUs.on("change", function () {
        selectUser(selectUs.val());
    });

    $('[name="register"]').click(function () {
        isUser(selectUs.val());
    });
});

var checkBoxAdult = $('[name="isAdultCheck"]').children("input");

function selectUser(value) {
    if (value === '1') {
        //es usuario
        $('#ResponsableName').hide();
        $('#ResponsableLastName').hide();
        $('[name="titleDateUser"]').hide();
        $('[name="isAdultCheck"]').hide();
    } else {
        //no es usuario
        $('#ResponsableName').show();
        $('[name="titleDateUser"]').show();
        $('#ResponsableLastName').show();
        $('[name="isAdultCheck"]').show();
    }
}

function isUser(val) {
    if (val === '1')
    { validBirth(18); }
    else {
        validBirth(4);
        validAdultCheck(checkBoxAdult);
    }
}

function validBirth(minAge) {
    //age = getAge(birth);
    age = getAge($("#Birth"));

    if (age < 0) {
        alert("Por favor ingresa una fecha valida. Todavia no naciste :) .");
        return false;
    } else if (age < minAge) {
        
        alert("La edad tiene que ser mayor a " + minAge + " años.");
        //reset datepicker
        //$("#Birth").datepicker('setDate', null);
        return false;
    }
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
    if (!check.is(':checked'))
        alert("Por favor. Si usted no es el usuario de la protesis es necesario que sea mayor de 18 años.");
}