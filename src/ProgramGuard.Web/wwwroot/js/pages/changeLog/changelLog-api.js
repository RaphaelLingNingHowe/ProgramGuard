async function onCheckBoxChanged(cellInfo, checkBoxElement) {
    var rowId = cellInfo.key;

    updateCheckBoxUI(checkBoxElement, true);
    const url = '/ChangeLogs?Handler=Confirm&key=' + rowId;
    const { status, data } = await fetchData(url, "PUT");
    if (status >= 200 && status < 300) {
        if (cellInfo.data) {
            cellInfo.data.ConfirmStatus = true;
        }
        DevExpress.ui.notify('審核成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}