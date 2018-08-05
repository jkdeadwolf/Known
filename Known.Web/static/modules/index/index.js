﻿$(function () {
    //menu
    var menu = new Menu('#mainMenu', {
        itemclick: function (item) {
            if (!item.children) {
                MainTabs.active(item);
            }
        }
    });

    $('.sidebar').mCustomScrollbar({ autoHideScrollbar: true });

    new MenuTip(menu);

    Ajax.getJson('/api/user/getmenus', function (result) {
        menu.loadData(result.Data);
    });

    //toggle
    $('#toggle, .sidebar-toggle').click(function () {
        var body = $('body'), toggle = $('.sidebar-toggle i');
        body.toggleClass('compact');
        if (body.hasClass('compact')) {
            toggle.removeClass('fa-dedent').addClass('fa-indent');
        } else {
            toggle.removeClass('fa-indent').addClass('fa-dedent');
        }
        mini.layout();
    });

    //dropdown
    $('.dropdown-toggle').click(function (event) {
        $(this).parent().addClass('open');
        return false;
    });
    $(document).click(function (event) {
        $('.dropdown').removeClass('open');
    });

    //navbar
    $('#navDevTool').click(function () { Navbar.devTool(); });
    $('#navTodo').click(function () { Navbar.todo(); });

    new Toolbar('tbUser', UserMenu);
    new Toolbar('tabsButtons', MainTabs);

    mini.parse();
    MainTabs.index();

});