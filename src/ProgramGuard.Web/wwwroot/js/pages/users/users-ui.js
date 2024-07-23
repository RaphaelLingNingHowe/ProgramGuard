//function switchTemplate(cellElement, cellInfo) {
//    $("<div>").dxSwitch({
//        value: cellInfo.value,

//    }).appendTo(cellElement);
//}
//function switchEditorTemplate(cellElement, cellInfo) {
//    return $("<div>").dxSwitch({
//        value: cellInfo.value,
//        onValueChanged: function (e) {
//            cellInfo.setValue(e.value);
//            updateUserStatus(cellInfo.data.UserId, e.value);
//        }
//    }).appendTo(cellElement);
//}

// 配置单元格模板
function switchTemplate(cellElement, cellInfo) {
    // 根据 cellInfo.value 显示不同的图标或内容
    $("<div>").text(cellInfo.value ? "✔️" : "❌")
        .appendTo(cellElement);
}

function switchEditorTemplate(cellElement, cellInfo) {
    return $("<div>").dxSwitch({
        value: cellInfo.value,
        onValueChanged: function (e) {
            updateUserStatus(cellInfo.data.UserId, e.value)
        }
    }).appendTo(cellElement);
}

function resetPasswordTemplate(cellElement, cellInfo) {
    $("<div>")
        .addClass("reset-password-button")
        .dxButton({
            text: "重設密碼",
            type: "danger",
            stylingMode: "contained",
            onClick: function () {
                var userId = cellInfo.data.UserId;
                showResetPasswordPopup(userId)
            }
        })
        .appendTo(cellElement);
}

function unlockAccountTemplate(cellElement, cellInfo) {
    if (cellInfo.data.IsLocked) {
        $("<div>")
            .dxButton({
                text: "帳號解鎖",
                type: "normal",
                stylingMode: "contained",
                onClick: function () {
                    var userId = cellInfo.data.UserId;
                    unlockAccount(userId);
                }
            })
            .appendTo(cellElement);
    }
}

/*TODO 根據權限禁用*/
async function privilegeTemplate(cellElement, cellInfo) {
    const privileges = await getPrivileges();

    $('<div>').dxSelectBox({
        value: cellInfo.data.Privilege,
        valueExpr: 'Id',
        displayExpr: 'Name',
        dataSource: privileges,
        onValueChanged: function (e) {
            updateUserPrivilege(cellInfo.data.UserId, e.value)
        }
    }).appendTo(cellElement);
}

async function privilegeEditorTemplate(cellElement, cellInfo) {
    const privileges = await getPrivileges();

    $('<div>').dxSelectBox({
        value: cellInfo.data.Privilege,
        valueExpr: 'Id',
        displayExpr: 'Name',
        dataSource: privileges
    }).appendTo(cellElement);
}
