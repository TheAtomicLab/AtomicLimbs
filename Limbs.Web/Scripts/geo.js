var mapStyle = [
    { elementType: 'geometry', stylers: [{ color: '#242f3e' }] },
    { elementType: 'labels.text.stroke', stylers: [{ color: '#242f3e' }] },
    { elementType: 'labels.text.fill', stylers: [{ color: '#746855' }] },
    {
        featureType: 'administrative.locality',
        elementType: 'labels.text.fill',
        stylers: [{ color: '#d59563' }]
    },
    {
        featureType: 'poi',
        elementType: 'labels.text.fill',
        stylers: [{ color: '#d59563' }]
    },
    {
        featureType: 'poi.park',
        elementType: 'geometry',
        stylers: [{ color: '#263c3f' }]
    },
    {
        featureType: 'poi.park',
        elementType: 'labels.text.fill',
        stylers: [{ color: '#6b9a76' }]
    },
    {
        featureType: 'road',
        elementType: 'geometry',
        stylers: [{ color: '#38414e' }]
    },
    {
        featureType: 'road',
        elementType: 'geometry.stroke',
        stylers: [{ color: '#212a37' }]
    },
    {
        featureType: 'road',
        elementType: 'labels.text.fill',
        stylers: [{ color: '#9ca5b3' }]
    },
    {
        featureType: 'road.highway',
        elementType: 'geometry',
        stylers: [{ color: '#746855' }]
    },
    {
        featureType: 'road.highway',
        elementType: 'geometry.stroke',
        stylers: [{ color: '#1f2835' }]
    },
    {
        featureType: 'road.highway',
        elementType: 'labels.text.fill',
        stylers: [{ color: '#f3d19c' }]
    },
    {
        featureType: 'transit',
        elementType: 'geometry',
        stylers: [{ color: '#2f3948' }]
    },
    {
        featureType: 'transit.station',
        elementType: 'labels.text.fill',
        stylers: [{ color: '#d59563' }]
    },
    {
        featureType: 'water',
        elementType: 'geometry',
        stylers: [{ color: '#17263c' }]
    },
    {
        featureType: 'water',
        elementType: 'labels.text.fill',
        stylers: [{ color: '#515c6d' }]
    },
    {
        featureType: 'water',
        elementType: 'labels.text.stroke',
        stylers: [{ color: '#17263c' }]
    }
];

function initMap() {
        
    var latLng = { lat: -34.633803, lng: -58.410774 };
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 16,
        center: latLng,
        styles: mapStyle
    });

    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        title: 'Atomic Lab <3'
    });

}

$(document).ready(function () {
    $('#checkAddress').click(function () {
        var geocoder = new google.maps.Geocoder();
        var map2 = new google.maps.Map(document.getElementById('map'), {
            zoom: 18, 
            styles: mapStyle
        });
        geocodeAddress(geocoder, map2);
        return false;
    });
});

function geocodeAddress(geocoder, resultsMap) {
    var country = $("#Country").val();
    var city = $("#City").val();
    var address = country + ', ' + city + ', ' + $("#Address").val();
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === 'OK') {
            resultsMap.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: resultsMap,
                position: results[0].geometry.location
            });
            enableRegister();
        } else {
            if (status === 'ZERO_RESULTS') {
                alert('Por favor ingresé una dirección valida para poder registrarse.');
            }else{
                alert('Geocode was not successful for the following reason: ' + status);
            }
        }
    });
}

/*
function enableRegister() {
    $("[name=register]").addClass("blue_button");
    $("[name=register]").removeClass("disabled_button");
    $("[name=register]").prop('disabled', false);
};
*/