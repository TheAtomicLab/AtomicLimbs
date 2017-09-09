$(document).ready(function () {
    /*$('#checkAddress').click(function () {
        geocodeAddress($('#Address').val());
        return false;
    });*/
    // $('#map').hide();
});

function geocodeAddress(geocoder, resultsMap) {
    var address = document.getElementById('Address').value;
    var geocoder = new google.maps.Geocoder;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === 'OK') {
            resultsMap.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: resultsMap,
                position: results[0].geometry.location
            });
        } else {
            alert('Geocode was not successful for the following reason: ' + status);
        }
    });

    function putPointInMap(address) {
        var parametros = {
            "Address": address
        };
        $.ajax({
            type: "POST",
            url: "/Ambassador/GetPointGoogle",
            data: parametros,
            dataType: "json",
            beforeSend: function () {
                //  poner ruedita cargando
            },
            success: function (response) {
                // var response = JSON.stringiFy(response.Predictions[0].Tag);
                //  alert(response);
                var map = new google.maps.Map(document.getElementById('map'), {
                    zoom: 20,
                    center: {
                        lat: -34.631915, lng: -58.410862
                    }
                });
                var geocoder = new google.maps.Geocoder;
                var infowindow = new google.maps.InfoWindow;

                geocodeLatLng(geocoder, map, infowindow, response);
                //  $('#map').show(3000);
                //  $('#map').show("slow"); -> Efecto de mierda

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
}