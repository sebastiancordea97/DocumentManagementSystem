/*
 * This code is applied to the modals from SysAdmin departments view.
 * 
 */
$(document).on("click", "#editOpenModal", function () {
    var id = $(this).data('id');
    var depName = $(this).data('depname');
    var supervisor = $(this).data('super');
    
    $("#departmentNameEdit").val(depName);
    $("#departmentEdit").val(id);
    $("#departmentSupervisorEdit").val(supervisor);

   
});

$(document).on("click", "#deleteOpenModal", function () {
    var id = $(this).data('id');
    var value = $(this).data('value');
    $("#departmentNameDelete").text(value);
    $("#departmentIdDelete").attr("href", "/SysAdmin/DeleteDepartment?id=" + id);
});

$(document).ready(function () {
    $("#showDepartments").click(function () {
        $(".depshow").css('display', 'block');
        $(".usershow").css('display', 'none');
        $(".documentshow").css('display', 'none');
    });
});


$(document).ready(function () {
    $("#showUsers").click(function () {
        $(".usershow").css('display', 'block');
        $(".depshow").css('display', 'none');
        $(".documentshow").css('display', 'none');
    });
});

$(document).ready(function () {
    $("#showDocs").click(function () {
        $(".depshow").css('display', 'none');
        $(".usershow").css('display', 'none');
        $(".documentshow").css('display', 'block');
        

    });
});

$(document).on('click', '.show_hide_password a', function (event) {
    event.preventDefault();
    if ($('.show_hide_password input').attr("type") == "text") {
        $('.show_hide_password input').attr('type', 'password');
        $('.show_hide_password i').addClass("fa-eye-slash");
        $('.show_hide_password i').removeClass("fa-eye");
    } else if ($('.show_hide_password input').attr("type") == "password") {
        $('.show_hide_password input').attr('type', 'text');
        $('.show_hide_password i').removeClass("fa-eye-slash");
        $('.show_hide_password i').addClass("fa-eye");
    }
});

$(document).on("click", "#editOpenModal2", function () {
    var id = $(this).data('id');
    var username = $(this).data('username');
    var firstname = $(this).data('firstname');
    var rank = $(this).data('rank');
    var func = $(this).data('function');
    var email = $(this).data('email');
    var depId = $(this).data('depid');
    var userTypeId = $(this).data('usertypeid');
  
    $("#userIdEdit").val(id);
    $("#usernameEdit").val(username);
    $("#rank").val(rank);
    $("#func").val(func);
    $("#firstName").val(firstname);

    $("#userEmailEdit").val(email);
    $("#departmentIdEdit option").each(function () {
        if ($(this).val() == depId)
            $(this).attr("selected", "selected");
    });
    $("#userRoleIdEdit option").each(function () {
        if ($(this).val() == userTypeId)
            $(this).attr("selected", "selected");
    });
    
});

$(document).on("click", "#deleteOpenModal2", function () {
    var id = $(this).data('id');
    var username = $(this).data('username');
    var email = $(this).data('email');
    $("#usernameDelete").text(username);
    $("#userEmailDelete").text(email);
    $("#userIdDelete").attr("href", "/SysAdmin/DeleteUser?id=" + id);
});





