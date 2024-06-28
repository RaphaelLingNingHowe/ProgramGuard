async function searchOnClick() {
    var fileName = $("#fileName").dxTextBox("instance").option("value");
    var startTime = $("#startTime").dxDateBox("instance").option("value");
    var endTime = $("#endTime").dxDateBox("instance").option("value");
    var checkUnconfirm = $("#checkUnconfirm").dxCheckBox("instance").option("value");

    var queryDto = {
        FileName: fileName,
        StartTime: startTime,
        EndTime: endTime,
        CheckUnconfirm: checkUnconfirm
    };

    $.ajax({
        url: '/ChangeLogs?Handler=Search',
        type: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(queryDto),
        success: function (response) {
            if (response.success) {
                console.log('Password changed successfully:', response.message);
                DevExpress.ui.notify(response.message, "success", 3000);
            } else {
                console.error('Error changing password:', response.message);
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function () {
            DevExpress.ui.notify("查詢失敗", "error", 3000);
        }
    });
}

function updateConfirm(e) {
    return e.row.data.ConfirmStatus !== true;
}