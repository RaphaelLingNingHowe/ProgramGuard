function showPopup(popupId) {
    $(`#${popupId}`).dxPopup("instance").show();
}

function hidePopup(popupId) {
    $(`#${popupId}`).dxPopup("instance").hide();
}

function reloadGrid() {
    $('#dataGrid').dxDataGrid("instance").refresh();
}

function changePasswordMode(name) {
    let editor = $(`#${name}`).dxTextBox("instance");
    editor.option('mode', editor.option('mode') === 'text' ? 'password' : 'text');
}

function getTextBoxValue(textbox) {
    return $("#" + textbox).dxTextBox("instance").option("text");
}
function getDateBoxValue(dateBoxId) {
    let date = $("#" + dateBoxId).dxDateBox("instance").option("value");

    return date == null ? null :
        (date.getFullYear() + "-" +
            (date.getMonth() + 1).toString().padStart(2, "0") + "-" +
            date.getDate().toString().padStart(2, "0") + "T" +
            date.getHours().toString().padStart(2, "0") + ":" +
            date.getMinutes().toString().padStart(2, "0"));
}

function getCheckBoxValue(checkBox) {
    return $("#" + checkBox).dxCheckBox("instance").option("value");
}

function validateDateTime(startTime, endTime) {
    if (!startTime || !endTime) {
        DevExpress.ui.notify("請提供查詢時間", "error", 3000);
        return false;
    }

    startTime = new Date(startTime);
    endTime = new Date(endTime);

    if (endTime < startTime) {
        DevExpress.ui.notify("結束時間不能早於起始時間", "error", 3000);
        return false;
    }

    const timeDiff = endTime - startTime;
    const daysDiff = Math.ceil(timeDiff / (1000 * 60 * 60 * 24));

    if (daysDiff > 7) {
        DevExpress.ui.notify("時間範圍不能超過7天", "error", 3000);
        return false;
    }
    return true;
}