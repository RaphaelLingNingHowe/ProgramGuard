import Fetcher from "./util/fetcher.js";
var _fetcher = new Fetcher();

import Helper from "./util/helper.js";
var _helper = new Helper();

pageInit();

function pageInit() {
    /* Devexpress 控件中文化 中文化 js 檔案為 js / devextreme / localization / dx.messages.zh - tw.js 已 bundle */
    DevExpress.localization.locale("zh-tw");

    /* 給 Grid 以外的控件取資料遇到問題時的錯誤處理 */
    $.ajaxSetup({
        statusCode: {
            400: function () { _helper.badRequest(); },
            401: function () { _helper.redirectToLogin(); }
        }
    });

    $("#popupModifyPassword").dxPopup({
        toolbarItems: [{
            location: "before"
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "before",
            options: {
                text: "確定",
                onClick: modifyPassword
            }
        }, {
            toolbar: "bottom",
            widget: "dxButton",
            location: "after",
            options: {
                text: "取消",
                onClick: onHideModifyPasswordPopup
            }
        }]
    });

    $("#btnModifyPassword").click(onShowModifyPasswordPopup);
}

function onShowModifyPasswordPopup() {
    _helper.showPopup("popupModifyPassword");
}

function onHideModifyPasswordPopup() {
    _helper.hidePopup("popupModifyPassword");

    _helper.cleanTextbox("popupModifyOldPassword");
    _helper.cleanTextbox("popupModifyNewPassword");
    _helper.cleanTextbox("popupModifyPasswordConfirm");
}

async function modifyPassword() {
    let originalValue = _helper.getTextBoxValue("popupModifyOldPassword");
    let newValue = _helper.getTextBoxValue("popupModifyNewPassword");
    let valueConfirm = _helper.getTextBoxValue("popupModifyPasswordConfirm");
    let account = $("#hiddenAccount").text();

    if (originalValue == "") {
        DevExpress.ui.notify("請輸入目前密碼", "warning", 3000);
        return;
    }
    else if (newValue == "") {
        DevExpress.ui.notify("請輸入新密碼", "warning", 3000);
        return;
    }
    else if (valueConfirm == "") {
        DevExpress.ui.notify("請輸入密碼確認欄位", "warning", 3000);
        return;
    }
    else if (newValue != valueConfirm) {
        DevExpress.ui.notify("兩次密碼輸入不同, 請重新輸入", "warning", 3000);
        return;
    }
    else if (newValue.length > 256) {
        DevExpress.ui.notify("密碼超過可輸入上限(256)", "warning", 3000);
        return;
    }
    else if (_helper.checkPwdComplexity(newValue) == false) {
        return;
    }
    else if (account == "") {
        DevExpress.ui.notify("遺失帳號資訊, 請重新登入", "warning", 3000);
        return;
    }

    const response = await _fetcher.fetchJson("/Account/Manager?Handler=ModifyPassword&key=" + account, {
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        method: "PUT",
        body: JSON.stringify({
            OriginalValue: originalValue,
            NewValue: newValue
        })
    });

    if (_helper.responseAnalysis(response) == false) {
        return;
    }

    DevExpress.ui.notify("[" + account + "]-密碼已更新", "success", 3000);
    onHideModifyPasswordPopup();
}
