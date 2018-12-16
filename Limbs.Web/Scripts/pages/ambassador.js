$(document).ready(function () {
    let organization = $('#Organization');
    let organizationName = $('#OrganizationName');
    let roleInOrganization = $('#RoleInOrganization');
    let organizationBox = $('.organization-box');
    let frm = $('.form');

    if (organization.val() !== 'CuentaPropia') {
        organizationBox.show();
    }

    $('#Organization').change(function () {
        if ($(this).val() === 'CuentaPropia') {
            organizationName.val('');
            roleInOrganization.val('');

            organizationBox.hide();
        } else {
            organizationBox.show();
        }
    });

    //var settngs = $.data($('.form')[0], 'validator').settings;
    //settngs.ignore = ".ignore";

    frm.submit(function (e) {
        if (!$(this).valid()) {
            e.preventDefault();
        } else {
            $('#btn-register').prop('disabled', true);
        }
    });
});