
//example alertAtomic("Esto es un alert","#alertAtomic")
function alertAtomic(text, div) {
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
};