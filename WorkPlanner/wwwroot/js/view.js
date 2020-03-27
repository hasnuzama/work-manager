$(document).ready(function () {
    var ErrorMessages = {
        1000: 'Invalid request.',
        1001: 'Invalid credentials.',
        1002: 'Invalid data identified.',
        1003: 'Session has expired. Please login again.',
        1004: 'Unauthorized.',
        0: 'Something went wrong. Please contact administrator.'
    }

    $("#datepicker").datepicker({
        format: "yyyy-mm-dd",
        setDate: "now",
        autoClose: true
    });

    var date = new Date();
    var today = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
    $("#datepicker").val(today);

    $("#submit").click(function () {
        var userId = $("#employee").val();
        var date = $("#datepicker").val();
        $.ajax({
            url: '?handler=WorkPlans&userId=' + userId + '&date=' + date,
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            headers: {
                "XSRF-TOKEN": $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        }).done(function (result) {
            result = JSON.parse(result);
            if (result.Status == 1) {
                $("tbody").empty();
                if (result.Results.length == 0) {
                    toastr.info("No data found.");
                }
                else {
                    var html = "";
                    for (var i = 0; i < result.Results.length; i++) {
                        html += "<tr>" +
                            "<th scope=\"row\">" + (i + 1) + "</th>" +
                            "<td>" + result.Results[i].ProjectName + "</td>" +
                            "<td>" + result.Results[i].TaskDetails + "</td>" +
                            "<td>" + result.Results[i].EstimatedHours + "</td>" +
                            "<td>" + result.Results[i].PineStemTaskID + "</td>" +
                            "</tr>";
                    }
                    $("tbody").append(html);
                }

            }
            else {
                toastr.error(ErrorMessages[result.ErrorCode]);
            }
        });
    });

});