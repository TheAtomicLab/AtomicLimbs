$(document).ready(function () {
    $('#checkAddress').click(function () { putPointInMap($('#Address').val()); });
});

function putPointInMap(address) {
    var parametros = {
        "Address": address
    };
    $.ajax({
        type: "POST",
        url:"/Ambassador/GetPointGoogle",
        data: parametros,
        dataType: "json",
        beforeSend: function () {
          //  alert('Me viene por aqui');
        },
        success: function (response) {
            alert('Me viene por aca');
           // var response = JSON.stringiFy(response.Predictions[0].Tag);
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
            return false;

        },
        failure: function () {
            alert("fallo");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            // Display error message.
            alert("Error");
            var errorString = (errorThrown === "") ? "Error. " : errorThrown + " (" + jqXHR.status + "): ";
            errorString += (jqXHR.responseText === "") ? "" : jQuery.parseJSON(jqXHR.responseText).message;
            alert(errorString);
        }
    });
}

function geocodeLatLng(geocoder, map, infowindow, latlong) {
    //var input = latlong;
    //var input = document.getElementById('latlng').value;
    var latlngStr = latlong.result.split(',', 2);
    var latlng = { lat: parseFloat(latlngStr[0]), lng: parseFloat(latlngStr[1]) };
    //var latlng = latlong;
    geocoder.geocode({ 'location': latlng }, function (results, status) {
        if (status === 'OK') {
            if (results[1]) {
                map.setZoom(11);
                var marker = new google.maps.Marker({
                    position: latlng,
                    map: map
                });
                infowindow.setContent(results[1].formatted_address);
                infowindow.open(map, marker);
            } else {
                window.alert('No results found');
            }
        } else {
            window.alert('Geocoder failed due to: ' + status);
        }
    });
}
