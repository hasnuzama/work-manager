$(function () {

    var ErrorMessages = {
        1000: 'Invalid request.',
        1001: 'Invalid credentials.',
        1002: 'Invalid data identified.',
        1003: 'Session has expired. Please login again.',
        1004: 'Unauthorized.',
        0: 'Something went wrong. Please contact administrator.'
    }
    var viewPageUrl = "https://localhost:44386/View";


    $("#login").click(function () {
        var email = $("#email").val();
        var pwd = $("#password").val();
        login(email, pwd);
    });

    function login(email, pwd) {
        var jsonData = {
            Email: email,
            Password: pwd
        };
        $.ajax({
            url: '?handler=Login',
            type: 'post',
            contentType: 'application/json; charset=utf-8',
            headers: {
                "XSRF-TOKEN": $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            data: JSON.stringify(jsonData)
        }).done(function (result) {
            result = JSON.parse(result);
            if (result.Status == 1) {
                window.location.replace(viewPageUrl);
            }
            else {
                toastr.error(ErrorMessages[result.ErrorCode]);
            }
        });
    }
});