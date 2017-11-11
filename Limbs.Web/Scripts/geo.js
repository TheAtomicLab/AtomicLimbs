var form = $("form.form");
var errorContainer = form.find("[data-valmsg-summary=true]"),
    errorList = errorContainer.find("ul");
var country = $("#Country", form);
var city = $("#City", form);
var address = $("#Address", form);

function geocodeAddress() {
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ "address": country.val() + "," + city.val() + "," + address.val() },
        function (results, status) {
            $(".address-selector").hide();
            if (status === "OK") {
                if (results.length === 0) {
                    setNoResultError();
                }
                if (results.length === 1) {
                    form.submit();
                } else {
                    $("li[data-for]", errorContainer).remove();
                    console.log(results);

                    var addressSelector = $(".address-selector").show().find("ul").empty();
                    var template = $($.parseHTML('<li><a href="#" data-country="" data-city="" data-address="" onclick="selectAddress(this); return false;">Ecuador 1419, CABA, Argentina</a></li>'));
                    for (var i = 0; i < results.length; i++) {
                        var item = template.clone();
                        var itemLink = item.find("a");
                        var cityStr = "", addressStr = "", addressNumber = "";
                        var addressComponents = results[i].address_components;
                        for (var j = 0; j < addressComponents.length; ++j) {
                            var types = addressComponents[j].types;
                            for (var k = 0; k < types.length; ++k) {

                                if (types[k] === "country") {
                                    itemLink.attr("data-country", addressComponents[j].long_name);
                                }

                                if (types[k] === "locality") {
                                    if (cityStr === "") {
                                        cityStr = addressComponents[j].long_name;
                                    } else {
                                        cityStr += ", " + addressComponents[j].long_name;
                                    }
                                }
                                if (types[k] === "administrative_area_level_2") {
                                    if (cityStr === "") {
                                        cityStr = addressComponents[j].long_name;
                                    } else {
                                        cityStr += ", " + addressComponents[j].long_name;
                                    }
                                }
                                if (types[k] === "administrative_area_level_1") {
                                    if (cityStr === "") {
                                        cityStr = addressComponents[j].long_name;
                                    } else {
                                        cityStr += ", " + addressComponents[j].long_name;
                                    }
                                }

                                if (types[k] === "street_number") {
                                    addressNumber = addressComponents[j].long_name;
                                }
                                if (types[k] === "route") {
                                    addressStr = addressComponents[j].long_name;
                                }
                            }
                        }
                        itemLink.attr("data-city", cityStr);
                        itemLink.attr("data-address", addressStr + " " + addressNumber);
                        itemLink.text(results[i].formatted_address);

                        addressSelector.append(item);
                    }
                }
            } else {
                if (status === "ZERO_RESULTS") {
                    setNoResultError();
                } else {
                    alert("Geocode was not successful for the following reason: " + status);
                }
            }
        });
}

function setNoResultError() {
    var errorArray = {
        "Address": "Dirección invalida."
    };
    form.validate().showErrors(errorArray);

    if (errorList && errorList.length) {
        $("li[data-for]", errorContainer).remove();
        errorContainer.addClass("validation-summary-errors").removeClass("validation-summary-valid");

        $.each(errorArray, function (k, m) {
            $("<li />").attr("data-for", k).html(m).appendTo(errorList);
        });
    }
}

function selectAddress(e) {
    var item = $(e);
    console.log(e);
    country.val(item.attr("data-country"));
    city.val(item.attr("data-city"));
    address.val(item.attr("data-address"));

    $(".address-selector").hide();
}

$(document).ready(function () {
    $("button[type=submit]").click(function (e) {
        e.preventDefault();

        if (form.valid()) {
            geocodeAddress();
        }
    });
});