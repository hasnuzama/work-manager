$(document).ready(function () {

    var ErrorMessages = {
        1000: 'Invalid request.',
        1001: 'Invalid credentials.',
        1002: 'Invalid data identified.',
        1003: 'Session has expired. Please login again.',
        1004: 'Unauthorized.',
        0: 'Something went wrong. Please contact administrator.'
    }

    function postWorkPlan() {
        var jsonData = [];
        var date = $("#datepicker").val();
        $($('tbody')[0]).find('tr').each(function () {
            jsonData.push({
                ProjectName: "" + $(this).find('td').eq(0).text(),
                TaskDetails: "" + $(this).find('td').eq(1).text(),
                EstimatedHours: "" + $(this).find('td').eq(2).text(),
                PineStemTaskID: "" + $(this).find('td').eq(3).text(),
                WorkDate: date
            });
        });
        //
        if (jsonData.length == 0) {
            toastr.error("Please enter workplan details.");
            return;
        }
        //
        for (var i = 0; i < jsonData.length; i++) {
            if (!jsonData[i].ProjectName ||
                !jsonData[i].TaskDetails ||
                !jsonData[i].EstimatedHours) {
                toastr.error("ProjectName/TaskName/EstimatedHours are mandatory in all rows.");
                return;
            }
            if (!jsonData[i].WorkDate) {
                toastr.error("Work Date is mandatory.");
                return;
            }
        }
        //
        $.ajax({
            url: '?handler=WorkPlan',
            type: 'post',
            contentType: 'application/json; charset=utf-8',
            headers: {
                "XSRF-TOKEN": $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            data: JSON.stringify(jsonData)
        }).done(function (result) {
            result = JSON.parse(result);
            if (result.Status == 1) {
                clearTable();
                toastr.success("Workplan created successfully.");
            }
            else {
                toastr.error(ErrorMessages[result.ErrorCode]);
            }
        });
    }

    function clearTable() {
        $("table > tbody > tr").each(function () {
            $(this).find('td').eq(0).text("");
            $(this).find('td').eq(1).text("");
            $(this).find('td').eq(2).text("");
            $(this).find('td').eq(3).text("");
        });
    }

    function bindDatePicker() {
        $("#datepicker").datepicker({
            format: "yyyy-mm-dd",
            setDate: "now",
            autoClose: true
        });
        $("#datepicker").val(new Date().toISOString().substring(0, 10));
    }

    function bindTable() {
        const $tableID = $('#table');

        $('.table-add').on('click', 'button', () => {
            const $clone = $tableID.find('tbody tr').last().clone(true).removeClass('hide table-line');
            if ($tableID.find('tbody tr').length === 0) {
                return;
            }
            $tableID.find('table').append($clone);
        });

        $tableID.on('click', '.table-remove', function () {
            $(this).parents('tr').detach();
        });

        $tableID.on('click', '.table-up', function () {
            const $row = $(this).parents('tr');
            if ($row.index() === 1) {
                return;
            }
            $row.prev().before($row.get(0));
        });

        $tableID.on('click', '.table-down', function () {

            const $row = $(this).parents('tr');
            $row.next().after($row.get(0));
        });
    }

    (function onLoad() {
        bindDatePicker();
        bindTable();
        $("#submit").click(postWorkPlan);
    })();
});

