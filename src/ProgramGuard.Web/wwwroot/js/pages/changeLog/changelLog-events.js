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

function validateQuery() {
    let startTime = getDateBoxValue('startTime');
    let endTime = getDateBoxValue('endTime');
    const unConfirmed = getCheckBoxValue('unConfirmed');
    const fileName = getTextBoxValue('fileName');

    if (!startTime && !endTime && !fileName && !unConfirmed) {
        DevExpress.ui.notify("請至少提供一個查詢條件", "error", 3000);
        return false;
    }
    if (startTime || endTime) {
        if (!validateDateTime(startTime, endTime)) {
            return false;
        }
    }
    
    reloadGrid();
}

