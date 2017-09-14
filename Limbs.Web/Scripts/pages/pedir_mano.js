$(document).ready(function () {
    $('#button_encarando_proyecto').click(function () {
        iniciar_pedido();
    });
    $('#cat_mano_div').click(function () {
        ajax_form('mano');
    });
    $('#cat_brazo_div').click(function () {
        ajax_form('brazo');
    });
});

function iniciar_pedido() {
    ini = document.getElementById('realiza_pedido');
    cat = document.getElementById('categoria_mano');
    ini.style.display = 'none';
    ini.style.opacity = '0';
    cat.style.display = 'block';
    setTimeout(function () { cat.style.opacity = '1'; }, 100);
}

function ajax_form(x) {
    var file = '';
    if (x === 'mano') {
        file = 'Orders/PedirManoMedidas';
    } else {
        file = 'Orders/PedirBrazoMedidas';
    }
    file = 'include/' + file;
    $.ajax({
        data: { success: true },
        url: file,
        type: 'post',
        beforeSend: function () {
            $('#cont').html("<div id='cargando' class='full'><p>Cargando</p></div>").show();
            window.scrollTo(0, 0);
        },
        success: function (datos) {
            $('#cont').html(datos).show();
        }
    });
}