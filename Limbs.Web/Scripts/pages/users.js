$(document).ready(function () {

    //se cambia propiedades del input Email
    $('#Email').attr('readonly', true);
    $('#Email').css('background-color', '#000000');
    $('#Email').css('opacity', 0.7);
    var selectUs = $('[name="selectUser"]');
    selectUs.on("change", function () {
        selectUser(selectUs.val());
    });
});

function selectUser(value) {
    if (value === 1) {
        //mostrar todo
        $('#ResponsableName').hide();
        $('#ResponsableLastName').hide();
        $('[name="titleDateUser"]').hide();
    } else {
        //mostrar datos de usuario
        $('#ResponsableName').show();
        $('[name="titleDateUser"]').show();
        $('#ResponsableLastName').show();
    }
}