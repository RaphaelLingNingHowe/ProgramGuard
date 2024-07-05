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
    var dataGrid = $("#dataGrid").dxDataGrid("instance");

    dataGrid.option("onCellClick", function (e) {
        if (e.column.dataField === "IsAdmin") {
            handleSwitchClick(e, "IsAdmin", '/Users?Handler=SetAdmin', '/Users?Handler=RemoveAdmin');
        } else if (e.column.dataField === "IsEnabled") {
            handleSwitchClick(e, "IsEnabled", '/Users?Handler=ActiveAccount', '/Users?Handler=DisableAccount');
        }
    });
});
function handleSwitchClick(e, fieldName, activeUrl, inactiveUrl) {
    var userId = e.data.UserId;
    var newValue = !e.data[fieldName];
    var apiUrl = newValue ? activeUrl : inactiveUrl;

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
                e.component.cellValue(e.rowIndex, fieldName, newValue);
                e.data[fieldName] = newValue;
            } else {
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('發生了一些錯誤', textStatus, errorThrown);
            var errorMessage = '發生了未知錯誤，請稍後再試。';
            if (jqXHR.status === 400) {
                errorMessage = jqXHR.responseText;
            }
            console.log('錯誤信息:', errorMessage);
            DevExpress.ui.notify(errorMessage, "error", 3000);
        }
    });
}

$.ajaxSetup({
    data: {
        __RequestVerificationToken: document.getElementsByName("__RequestVerificationToken")[0].value
    }
});