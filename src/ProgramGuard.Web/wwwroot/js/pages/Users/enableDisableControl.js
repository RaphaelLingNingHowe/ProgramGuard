function switchTemplate(cellElement, cellInfo) {
    $("<div>").dxSwitch({
        value: cellInfo.value,
        readOnly: true,
        onValueChanged: function (e) {
            cellInfo.setValue(e.value);
        }
    }).appendTo(cellElement);
}
function switchEditorTemplate(cellInfo) {
    return $("<div>").dxSwitch({
        value: cellInfo.value,
        onValueChanged: function (e) {
            cellInfo.setValue(e.value);
        }
    });
}
$(document).ready(function () {
    $("#dataGrid").dxDataGrid({
        onCellClick: function (e) {
            if (e.column.dataField === "IsEnabled") {
                var userId = e.data.UserId;
                var isEnabled = !e.data.IsEnabled;
                console.log("UserId:", userId);
                console.log("New IsEnabled value:", isEnabled);
                var apiUrl = isEnabled ? '/Users?Handler=ActiveAccount' : '/Users?Handler=DisableAccount'
                $.ajax({
                    url: apiUrl + '&key=' + userId,
                    type: 'PUT',
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    contentType: 'application/json',
                    success: function (response) {
                        if (response.success) {
                            DevExpress.ui.notify(response.message, "success", 3000);
                            e.component.cellValue(e.row.rowIndex, "IsEnabled", isEnabled);
                            e.data.IsEnabled = isEnabled;
                        } else {
                            DevExpress.ui.notify(response.message, "error", 3000);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log('發生了一些錯誤', textStatus, errorThrown);
                        if (jqXHR.status === 400) {
                            var errorMessage = jqXHR.responseText;
                            console.log('HTTP 400 Bad Request:', errorMessage);
                            DevExpress.ui.notify(errorMessage, "error", 3000);
                        } else {
                            console.log('其他錯誤');
                        }
                    }
                });
            }
        }
    });
});

$.ajaxSetup({
    data: {
        __RequestVerificationToken: document.getElementsByName("__RequestVerificationToken")[0].value
    }
});