//TODO (Lucas): Listar alerts en li
function alertAtomic(text, div = "#alertAtomic") {

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