/*
$(window).load(function () {
    $(".loader").fadeOut();
    $("#loadingModal").delay(500).fadeOut("slow")
})
*/

//TODO (Lucas): Listar alerts en li
function alertAtomic(text, div = "#alertAtomic") {
/*
    if (call) {
        $(div).find('li').remove();
        call = false;
    }
*/

    $(div).find('p').remove();
    var p = "<p>" + text + "</p>";
    $(div).append(p);
    $(div).dialog({
        modal: true,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
            }
        }
    })
    return false;
};

/*
function alertAtomic(text) {
    $("#alertAtomic").find('p').remove();
    var dialog = "<div id=alertAtomic ><p>" + text + "</p></div>";
    $('body').append(dialog);
    $("#alertAtomic").dialog({
        modal: true,
        //title: "Advertencia",
        buttons: {
            Ok: function () {
                $(this).dialog("close");
            }
        }
    })
    return false;
};
*/