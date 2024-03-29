﻿$(document).ready(function () {
    var ErrorMessages = {
        1000: 'Invalid request.',
        1001: 'Invalid credentials.',
        1002: 'Invalid data identified.',
        1003: 'Session has expired. Please login again.',
        1004: 'Unauthorized.',
        0: 'Something went wrong. Please contact administrator.'
    }

    function formatDate(dataAsString) {
        var date = new Date(dataAsString);
        var year = date.getFullYear();
        var day = date.getDate();
        var mm = (date.getMonth() + 1);
        if (day < 10)
            day = "0" + day;
        if (mm < 10)
            mm = "0" + mm;
        var curDate = year + "-" + mm + "-" + day;
        var hours = date.getHours()
        var minutes = date.getMinutes()
        var seconds = date.getSeconds();
        if (hours < 10)
            hours = "0" + hours;
        if (minutes < 10)
            minutes = "0" + minutes;
        if (seconds < 10)
            seconds = "0" + seconds;
        return curDate + " " + hours + ":" + minutes + ":" + seconds;
    }

    $("#datepicker").datepicker({
        format: "yyyy-mm-dd",
        setDate: "now",
        autoClose: true
    });

    $("#datepicker").val(new Date().toISOString().substring(0, 10));

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
                            "<td>" + result.Results[i].ProjectName + "</td>" +
                            "<td>" + result.Results[i].TaskDetails + "</td>" +
                            "<td>" + result.Results[i].EstimatedHours + "</td>" +
                            "<td>" + result.Results[i].PineStemTaskID + "</td>" +
                            "<td>" + formatDate(result.Results[i].CreatedOn) + "</td>" +
                            "</tr>";
                    }
                    $("tbody.work-plans").append(html);
                }

            }
            else {
                toastr.error(ErrorMessages[result.ErrorCode]);
            }
        });
    });

});