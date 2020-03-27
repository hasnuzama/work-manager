$(document).ready(function () {

    var ErrorMessages = {
        1000: 'Invalid request.',
        1001: 'Invalid credentials.',
        1002: 'Invalid data identified.',
        1003: 'Session has expired. Please login again.',
        1004: 'Unauthorized.',
        0: 'Something went wrong. Please contact administrator.'
    }
    const $tableID = $('#table');
    const $BTN = $('#export-btn');
    const $EXPORT = $('#export');

    const newTr = `
<tr class="hide">
  <td class="pt-3-half" contenteditable="true"></td>
  <td class="pt-3-half" contenteditable="true"></td>
  <td class="pt-3-half" contenteditable="true"></td>
  <td class="pt-3-half" contenteditable="true"></td>
  <td>
    <span class="table-remove"><button type="button" class="btn btn-danger btn-rounded btn-sm my-0 waves-effect waves-light">Remove</button></span>
  </td>
</tr>`;

    $('.table-add').on('click', 'button', () => {

        const $clone = $tableID.find('tbody tr').last().clone(true).removeClass('hide table-line');

        if ($tableID.find('tbody tr').length === 0) {

            $('tbody').append(newTr);
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

    // A few jQuery helpers for exporting only
    jQuery.fn.pop = [].pop;
    jQuery.fn.shift = [].shift;

    $BTN.on('click', () => {

        const $rows = $tableID.find('tr:not(:hidden)');
        const headers = [];
        const data = [];

        // Get the headers (add special header logic here)
        $($rows.shift()).find('th:not(:empty)').each(function () {

            headers.push($(this).text().toLowerCase());
        });

        // Turn all existing rows into a loopable array
        $rows.each(function () {
            const $td = $(this).find('td');
            const h = {};

            // Use the headers from earlier to name our hash keys
            headers.forEach((header, i) => {

                h[header] = $td.eq(i).text();
            });

            data.push(h);
        });

        // Output the result
        $EXPORT.text(JSON.stringify(data));
    });

    $("#submit").click(function () {
        var jsonData = [];
        $("table > tbody > tr").each(function () {
            jsonData.push({
                ProjectName: "" + $(this).find('td').eq(0).text(),
                TaskDetails: "" + $(this).find('td').eq(1).text(),
                EstimatedHours: "" + $(this).find('td').eq(2).text(),
                PineStemTaskID: "" + $(this).find('td').eq(3).text()
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
    });

    function clearTable() {
        $("table > tbody > tr").each(function () {
            $(this).find('td').eq(0).text("");
            $(this).find('td').eq(1).text("");
            $(this).find('td').eq(2).text("");
            $(this).find('td').eq(3).text("");
        });
    }
});

