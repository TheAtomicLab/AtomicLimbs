$(document).ready(function () {
    $('input[data-type = number]').change(function () { ValidateNumberControl($(this)); });

    //*********************
    $('#BtnModificar').click(function () { OnClickModificar($(this), '../../api/combinacion/Put'); });
    //**********************

    $('#btnDemodulador').click(function () { OnClickSeleccionarDemodulador($(this)); });
    $('.gridRowHeader').data('ascending', true).click(function () { OrdenarGrilla($(this)); }); 
});

function putPointInMap(address) {
    var parametros = {
        "direccion": address
    };
    $.ajax({
        data: parametros,
        url: 'procesamiento.php',
        type: 'post',
        datatype: "string",
        beforeSend: function () {
            alert('Me viene por aqui');
        },
        success: function (response) {
            alert('Me viene por aca');
            //  alert(response);
            var map = new google.maps.Map(document.getElementById('map'), {
                zoom: 20,
                center: {
                    lat: -34.686387, lng: -58.338313
                }
            });
            var geocoder = new google.maps.Geocoder;
            var infowindow = new google.maps.InfoWindow;

            geocodeLatLng(geocoder, map, infowindow, response);

        }
    });
}