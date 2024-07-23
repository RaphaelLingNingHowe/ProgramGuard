function onPrivilegeSelected(e) {
    selectedPrivilege = e.value;
    if (selectedPrivilege) {
        setPrivileges(selectedPrivilege.Visible, selectedPrivilege.Operate);
    }
}

function clearPrivilegeSelected() {
    var selectBox = $("#privilege").dxSelectBox("instance");
    selectBox.reset();
    selectBox.getDataSource().reload();
    clearPrivilegeCheckBox();
}

function clearPrivilegeCheckBox() {
    setPrivileges(0, 0);
}

$("#btnCreate").click(function () {
    const visible = calculatePrivileges('visible-privileges-container');
    const operate = calculatePrivileges('operate-privileges-container');
    if (visible == 0 && operate == 0) {
        DevExpress.ui.notify("應至少選擇一種權限", "warning", 3000);
        return;
    }
    showPopup('popupCreateRule');
});

$("#btnUpdate").click(function () {
    const privilege = $("#privilege").dxSelectBox("instance").option("value");
    if (!privilege) {
        DevExpress.ui.notify("請選擇欲編輯之權限規則", "warning", 3000);
        return
    }  
    const visible = calculatePrivileges('visible-privileges-container');
    const operate = calculatePrivileges('operate-privileges-container');
    if (visible == 0 && operate == 0) {
        DevExpress.ui.notify("應至少選擇一種權限", "warning", 3000);
        return;
    }
    showPopup('popupUpdateRule');
    $('#updateConfirm').text(`確定要編輯 ${selectedPrivilege.Name} 嗎?`);
});

$("#btnDelete").click(function () {
    const privilege = $("#privilege").dxSelectBox("instance").option("value");
    if (!privilege) {
        DevExpress.ui.notify("請選擇欲刪除之權限規則", "warning", 3000);
        return
    }
    showPopup('popupDeleteRule');
    $('#deleteConfirm').text(`確定要刪除 ${selectedPrivilege.Name} 嗎?`);

});

