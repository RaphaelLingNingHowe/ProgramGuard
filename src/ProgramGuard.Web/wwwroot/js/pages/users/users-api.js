async function submitCreateUser() {
    const userName = $("#UserName").dxTextBox("instance").option("value");
    const password = $("#Password").dxTextBox("instance").option("value");

    const requestData = {
        UserName: userName,
        Password: password
    };
    const url = '/Users?Handler=CreateUser';

    const { status, data } = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('帳號創建成功', "success", 3000);
        hidePopup('popupCreateUser');
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function updateUserStatus(userId, isEnabled) {
    const apiUrl = isEnabled ? '/Users?Handler=ActiveAccount' : '/Users?Handler=DisableAccount';
    const url = apiUrl + '&key=' + userId;

    const { status, data } = await fetchData(url, "PUT");

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify(data.message , "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}


//async function getPrivileges() {
//    const url = '/Users?Handler=Privilege';

//    const { status, data } = await fetchData(url, "GET");

//    if (status >= 200 && status < 300) {
//        return data; // 返回取得的權限資料
//    } else {
//        throw new Error(data.message || '獲取權限失敗');
//    }
//}
const getPrivileges = createCachedFunction(async () => {
    const url = '/Users?Handler=Privilege';
    const { status, data } = await fetchData(url, "GET");
    if (status >= 200 && status < 300) {
        return data;
    } else {
        throw new Error(data.message || '獲取權限失敗');
    }
});
async function unlockAccount(userId) {
    const url = '/Users?handler=UnlockAccount&userId=' + userId;

    const { status, data } = await fetchData(url, "PUT");

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('帳號已解鎖', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}
async function updateUserPrivilege(userId, privilegeId) {
    const url = '/Users?handler=UpdatePrivilege&userId=' + userId + "&privilegeId=" + privilegeId;

    const { status, data } = await fetchData(url, "PUT");

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('權限更新成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function submitResetPassword() {
    const userId = $("#popupResetPassword").dxPopup("instance").option("userId");
    const resetPassword = $("#resetPassword").dxTextBox("instance").option("value");

    const requestData = {
        ResetPassword : resetPassword
    };
    const url = '/Users?Handler=ResetPassword&key=' + userId;

    const { status, data } = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('密碼重置成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}
