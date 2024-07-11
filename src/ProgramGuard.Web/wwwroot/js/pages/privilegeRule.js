import Fetcher from "../../util/fetcher.js";
var _fetcher = new Fetcher();

import Helper from "../../util/helper.js";
var _helper = new Helper();

pageInit();

function pageInit() {
    $("#cbxRule").dxSelectBox({
        onValueChanged: function (e) {
            updatePrivilegeForm(e);
        }
    });

    $("#btnAdd").click(function () {
        let visible = calcVisiblePrivilege();
        let operate = calcOperatePrivilege();
        if (visible == 0 && operate == 0) {
            $("#btnAdd").blur();
            DevExpress.ui.notify("應至少選擇一種權限", "warning", 3000);
            return;
        }

        onShowAddRulePopup();
    });

    $("#btnModify").click(function () {
        let rule = _helper.getSelectBoxValue("cbxRule");
        if (rule == null || rule == '') {
            $("#btnModify").blur();
            DevExpress.ui.notify("請選擇欲編輯之權限規則", "warning", 3000);
            return;
        }

        let visible = calcVisiblePrivilege();
        let operate = calcOperatePrivilege();
        if (visible == 0 && operate == 0) {
            $("#btnModify").blur();
            DevExpress.ui.notify("應至少選擇一種權限", "warning", 3000);
            return;
        }

        onShowModifyRulePopup();
    });

    $("#btnDelete").click(function () {
        let rule = _helper.getSelectBoxValue("cbxRule");
        if (rule == null || rule == '') {
            $("#btnDelete").blur();
            DevExpress.ui.notify("請選擇欲刪除之權限規則", "warning", 3000);
            return;
        }

        onShowDeleteRulePopup();
    });


    $("#popupAddRule").dxPopup({
        toolbarItems: [{
            location: "before"
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "before",
            options: {
                text: "確定",
                onClick: async function () { await addPrivilegeRule(); }
            }
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "after",
            options: {
                text: "取消",
                onClick: function () { onHideAddRulePopup(); }
            }
        }]
    });


    $("#popupModifyRule").dxPopup({
        toolbarItems: [{
            location: "before"
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "before",
            options: {
                text: "確定",
                onClick: async function () { await modifyPrivilegeRule(); }
            }
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "after",
            options: {
                text: "取消",
                onClick: function () { onHideModifyRulePopup(); }
            }
        }]
    });


    $("#popupDeleteRule").dxPopup({
        toolbarItems: [{
            location: "before"
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "before",
            options: {
                text: "確定",
                onClick: async function () { await deletePrivilegeRule(); }
            }
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "after",
            options: {
                text: "取消",
                onClick: function () { onHideDeleteRulePopup(); }
            }
        }]
    });
}

async function addPrivilegeRule() {
    let visible = calcVisiblePrivilege();

    let operate = calcOperatePrivilege();

    let name = _helper.getTextBoxValue("PopupAddRuleName");
    if (name == null || name == '') {
        DevExpress.ui.notify("請輸入規則名稱", "warning", 3000);
        return;
    }

    const response = await _fetcher.fetchJson("/Account/PrivilegeRule", {
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        method: "POST",
        body: JSON.stringify({
            Name: name,
            Visible: visible,
            Operate: operate
        })
    });

    if (_helper.responseAnalysis(response) == false) {
        return;
    }

    /* TODO 理論上成功儲存至資料庫後, 只要直接改 selectbox 目前的值就好, 但目前有問題, 所以選擇直接全部刷新 */
    onHideAddRulePopup();
    _helper.reloadSelectBox("cbxRule");
    DevExpress.ui.notify("已儲存", "success", 3000);
}

async function modifyPrivilegeRule() {
    let rule = _helper.getSelectBoxValue("cbxRule");
    if (rule == null) {
        DevExpress.ui.notify("選擇異常, 請重新嘗試", "warning", 3000);
        return;
    }

    let visible = calcVisiblePrivilege();
    let operate = calcOperatePrivilege();

    const response = await _fetcher.fetchJson("/Account/PrivilegeRule", {
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        method: "PUT",
        body: JSON.stringify({
            Id: rule.Id,
            Visible: visible,
            Operate: operate
        })
    });

    if (_helper.responseAnalysis(response) == false) {
        return;
    }

    /* TODO 理論上成功儲存至資料庫後, 只要直接改 selectbox 目前的值就好, 但目前有問題, 所以選擇直接全部刷新 */
    onHideModifyRulePopup();
    _helper.reloadSelectBox("cbxRule");
    DevExpress.ui.notify("已編輯", "success", 3000);
}

async function deletePrivilegeRule() {
    let rule = _helper.getSelectBoxValue("cbxRule");
    if (rule == null) {
        DevExpress.ui.notify("選擇異常, 請重新嘗試", "warning", 3000);
        return;
    }

    const response = await _fetcher.fetchJson("/Account/PrivilegeRule?Handler=Delete&id=" + rule.Id, {
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        method: "DELETE"
    });

    if (_helper.responseAnalysis(response) == false) {
        return;
    }

    /* TODO 理論上成功儲存至資料庫後, 只要直接改 selectbox 目前的值就好, 但目前有問題, 所以選擇直接全部刷新 */
    onHideDeleteRulePopup();
    _helper.reloadSelectBox("cbxRule");
    DevExpress.ui.notify("已刪除", "success", 3000);
    _helper.setSelectBoxValue("cbxRule", null);
}

function calcVisiblePrivilege() {
    let privilege = 0;

    if (_helper.getCheckBoxValue("ckbShowOrganization") == true) {
        privilege += 1;
    }

    if (_helper.getCheckBoxValue("ckbShowAccountManager") == true) {
        privilege += 2;
    }

    if (_helper.getCheckBoxValue("ckbShowPrivilegeRule") == true) {
        privilege += 4;
    }

    if (_helper.getCheckBoxValue("ckbShowOperateLog") == true) {
        privilege += 8;
    }

    if (_helper.getCheckBoxValue("ckbShowUID") == true) {
        privilege += 16;
    }

    return privilege;
}

function calcOperatePrivilege() {
    let privilege = 0;

    if (_helper.getCheckBoxValue("ckbAddAccount") == true) {
        privilege += 1;
    }

    if (_helper.getCheckBoxValue("ckbDeleteAccount") == true) {
        privilege += 2;
    }

    if (_helper.getCheckBoxValue("ckbEnableAccount") == true) {
        privilege += 4;
    }

    if (_helper.getCheckBoxValue("ckbDisableAccount") == true) {
        privilege += 8;
    }

    if (_helper.getCheckBoxValue("ckbModifyAccountOrganization") == true) {
        privilege += 16;
    }

    if (_helper.getCheckBoxValue("ckbModifyAccountPrivilegeRule") == true) {
        privilege += 32;
    }

    if (_helper.getCheckBoxValue("ckbUnlockAccount") == true) {
        privilege += 64;
    }

    if (_helper.getCheckBoxValue("ckbResetOtherPeoplePwd") == true) {
        privilege += 128;
    }

    if (_helper.getCheckBoxValue("ckbModifyOrganization") == true) {
        privilege += 256;
    }

    if (_helper.getCheckBoxValue("ckbModifyUID") == true) {
        privilege += 512;
    }

    if (_helper.getCheckBoxValue("ckbModifyDisplayName") == true) {
        privilege += 1024;
    }

    if (_helper.getCheckBoxValue("ckbModifyPrivilegeRule") == true) {
        privilege += 2048;
    }

    return privilege;
}

function updatePrivilegeForm(data) {
    if (data.value == null) {
        return;
    }

    // visible privilege
    diffPrivilegeAndSetCheckBox(data.value.Visible, 1, "ckbShowOrganization");
    diffPrivilegeAndSetCheckBox(data.value.Visible, 2, "ckbShowAccountManager");
    diffPrivilegeAndSetCheckBox(data.value.Visible, 4, "ckbShowPrivilegeRule");
    diffPrivilegeAndSetCheckBox(data.value.Visible, 8, "ckbShowOperateLog");
    diffPrivilegeAndSetCheckBox(data.value.Visible, 16, "ckbShowUID");

    // operate privilege
    diffPrivilegeAndSetCheckBox(data.value.Operate, 1, "ckbAddAccount");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 2, "ckbDeleteAccount");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 4, "ckbEnableAccount");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 8, "ckbDisableAccount");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 16, "ckbModifyAccountOrganization");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 32, "ckbModifyAccountPrivilegeRule");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 64, "ckbUnlockAccount");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 128, "ckbResetOtherPeoplePwd");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 256, "ckbModifyOrganization");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 512, "ckbModifyUID");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 1024, "ckbModifyDisplayName");
    diffPrivilegeAndSetCheckBox(data.value.Operate, 2048, "ckbModifyPrivilegeRule");
}

function diffPrivilegeAndSetCheckBox(hasPrivilege, needPrivilege, controlName) {
    (hasPrivilege & needPrivilege) == needPrivilege ?
        _helper.setCheckBoxValue(controlName, true) :
        _helper.setCheckBoxValue(controlName, false);
}

function onShowAddRulePopup() {
    _helper.showPopup("popupAddRule");
}

function onShowModifyRulePopup() {
    _helper.showPopup("popupModifyRule");
}

function onShowDeleteRulePopup() {
    _helper.showPopup("popupDeleteRule");
}

function onHideAddRulePopup() {
    _helper.cleanTextbox("PopupAddRuleName");
    _helper.hidePopup("popupAddRule");
}

function onHideModifyRulePopup() {
    _helper.hidePopup("popupModifyRule");
}

function onHideDeleteRulePopup() {
    _helper.hidePopup("popupDeleteRule");
}
