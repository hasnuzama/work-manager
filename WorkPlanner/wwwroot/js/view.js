$(document).ready(function () {
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

    const groupBy = (array, key) => {
        return array.reduce((result, currentValue) => {
           (result[currentValue[key]] = result[currentValue[key]] || []).push(
                currentValue
            );           
            return result;
        }, {}); 
    };



    $("#datepicker").datepicker({
        format: "yyyy-mm-dd",
        setDate: "now",
        autoClose: true
    });

    var date = new Date();
    var today = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
    $("#datepicker").val(today);

    $("#submit").click(function () {
        var userIds = $("#employee").val();
        var date = $("#datepicker").val();
        var all = 0;
        $.ajax({
            url: '?handler=WorkPlans&userIds=' + userIds + '&date=' + date,
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            headers: {
                "XSRF-TOKEN": $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        }).done(function (result) {
            result = JSON.parse(result);
            if (result.Status == 1) {
                $("table").empty();
                if (result.Results.length == 0) {
                    toastr.info("No data found.");
                }
                else {
                    $("tbody").empty();
                    $("thead").empty();
                    var html = "";
                    var html1 = "";
                    var final = "";
                    var UserIds = groupBy(result.Results, 'UserId');
                    console.log(UserIds);

                    for (var i = 0; i < UserIds.length; i++) {

                        html += "<thead class=" + "thead-dark work-plans" + "><tr>" +
                            "<th>Employee Name</th>" +
                            "<th>Project</th>" +
                            "<th>Task</th>" +
                            "<th>Estimated hours</th>" +
                            "<th>Pinestem task id</th>" +
                            "<th>CreatedOn</th></tr></thead>";

                        html1 += " <tbody><tr>" +
                            "<td>" + UserIds[i].EmployeeName + "</td>" +
                            "<td>" + UserIds[i].ProjectName + "</td>" +
                            "<td>" + UserIds[i].TaskDetails + "</td>" +
                            "<td>" + UserIds[i].EstimatedHours + "</td>" +
                            "<td>" + UserIds[i].PineStemTaskID + "</td>" +
                            "<td>" + formatDate(UserIds[i].CreatedOn) + "</td>" +
                            "</tr></tbody>";

                        final = html + html1;

                        $("table").append(final);
                        html = '';
                        html1 = '';
                        final = '';

                    }
                }

            }
            else {
                toastr.error(ErrorMessages[result.ErrorCode]);
            }
        });
    });

    $(function () {
        $('select[multiple]').multiselect({
            columns: 1,
            placeholder: 'Select Employee',
            selectAll: true
        });

    });
});



