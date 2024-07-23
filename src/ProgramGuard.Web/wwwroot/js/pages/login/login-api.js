async function submitLogin() {
    const loginUserName = $("#loginUserName").dxTextBox("instance").option("value");
    const loginPassword = $("#loginPassword").dxTextBox("instance").option("value");

    const requestData = {
        LoginUserName: loginUserName,
        LoginPassword: loginPassword
    };
    const url = '/Login?Handler=Login';

    const { status, data } = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        if (data.requirePasswordChange) {
            saveUsernameToSession(loginUserName);
            showPopup('popupChangePassword');
            setChangePasswordMessage(data.message);
            DevExpress.ui.notify(data.message, "error", 3000);
        } else {
            window.location.href = '/FileLists';
        }
    } else {
        
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}