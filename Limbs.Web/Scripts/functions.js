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

function enableButtonbyAge(value) {
    if (value) {
        $(".no-design").hide();
    }
    else
    {
        $(".no-design").show();
        $('.bn_blue').removeAttr("href");
        $("[name='btn_pedir']").css('pointer-events', 'none');
        $("[name='btn_pedir']").css('background', '#cad4e2');
        $("[name='btn_pedir']").css('border-color', '#cad4e2');
        $("[name='manoPedir']").css('pointer-events', 'none');
    }
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