function abrirmenu() {
	var h = document.getElementsByTagName('nav')[0];
	var b = document.getElementsByTagName('body')[0];
	if ($(h).hasClass("nav_mobile_act")) {
		$(h).removeClass('nav_mobile_act');
		b.style.overflow = "auto";
	} else {
		$(h).addClass('nav_mobile_act');
		b.style.overflow = "hidden";
	}
}

function cerrarmenu() {
	var h = document.getElementsByTagName('nav')[0];
	var b = document.getElementsByTagName('body')[0];
	if ($(h).hasClass("nav_mobile_act")) {
		$(h).removeClass('nav_mobile_act');
		b.style.overflow = "auto";
	}
}

function menu_mobil_activo() {
    $('#menu_perfil').toggleClass('menu_mobil_activo');
}