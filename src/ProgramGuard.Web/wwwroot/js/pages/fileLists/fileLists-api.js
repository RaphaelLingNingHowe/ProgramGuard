async function submitCreateFile() {
    const filePath = $("#FilePath").dxTextBox("instance").option("value");

    const requestData = {
        FilePath: filePath
    };
    const url = '/FileLists?Handler=CreateFile';

    const {status, data} = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('檔案添加成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}