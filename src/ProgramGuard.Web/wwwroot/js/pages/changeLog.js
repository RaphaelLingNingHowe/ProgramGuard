function checkBoxTemplate(cellElement, cellInfo) {
    var confirmStatus = isTrue(cellInfo.value);

    $("<div>").dxCheckBox({
        value: confirmStatus,
        disabled: confirmStatus,
        onValueChanged: function (e) {
            if (e.value === true && !confirmStatus) {
                onCheckBoxChanged(cellInfo, this);
            }
        }
    }).appendTo(cellElement);
}

function isTrue(value) {
    if (typeof value === 'string') {
        return value.toLowerCase() === 'true';
    }
    return value === true;
}

function onCheckBoxChanged(cellInfo, checkBoxElement) {
    var rowId = cellInfo.key;

    updateCheckBoxUI(checkBoxElement, true);

    $.ajax({
        url: '/ChangeLogs?Handler=Confirm&key=' + rowId,
        method: 'PUT',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (cellInfo.data) {
                cellInfo.data.ConfirmStatus = true;
            }
            $(document).ready(function () {
                $('#dataGrid').dxDataGrid({
                    dataSource: response
                });
            }); 
        },
        error: function (error) {
            updateCheckBoxUI(checkBoxElement, false);
            if (cellInfo.data) {
                cellInfo.data.ConfirmStatus = false;
            }
        }
    });
}

function updateCheckBoxUI(checkBoxElement, confirmStatus) {
    checkBoxElement.option({
        value: confirmStatus,
        disabled: confirmStatus
    });
}
async function searchOnClick() {
    var fileName = $("#fileName").dxTextBox("instance").option("value");
    var startTime = $("#startTime").dxDateBox("instance").option("value");
    var endTime = $("#endTime").dxDateBox("instance").option("value");
    var unConfirmed = $("#unConfirmed").dxCheckBox("instance").option("value");

    try {
        let apiUrl = '/ChangeLogs?Handler=ChangeLog';
        let params = new URLSearchParams();

        if (startTime) {
            params.append('startTime', startTime.toLocaleString('zh-TW', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false
            }));
        }
        if (endTime) {
            params.append('endTime', endTime.toLocaleString('zh-TW', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false
            }));
        }
        if (fileName) {
            params.append('fileName', fileName);
        }
        if (unConfirmed !== null) {
            params.append('unConfirmed', unConfirmed);
        }

        // 如果 params 不为空，附加到 URL 中
        if (params.toString()) {
            apiUrl += '&' + params.toString();
        }
        console.log(apiUrl);
        console.log('Start Time:', startTime);
        console.log('End Time:', endTime);

        $.ajax({
            url: apiUrl,
            type: 'GET',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            contentType: 'application/json',
            success: function (response) {
                console.log('Search successful:', response);
                $(document).ready(function () {
                    $('#dataGrid').dxDataGrid({
                        dataSource: response,
                        keyExpr: "Id"
                    });
                });               
            },
            error: function (xhr, status, error) {
                console.error('Search failed:', error);
            }
        });
    } catch (error) {
        console.error('An error occurred:', error);
    }
}
