async function changePasswordSubmit() {
    const currentPassword = $("#CurrentPassword").dxTextBox("instance").option("value");
    const newPassword = $("#NewPassword").dxTextBox("instance").option("value");
    const confirmPassword = $("#ConfirmPassword").dxTextBox("instance").option("value");

    let key = $('#hiddenAccount').text();
    if (!key) {
        key = getUsernameFromSession();
    }

    const requestData = {
        CurrentPassword: currentPassword,
        NewPassword: newPassword,
        ConfirmPassword: confirmPassword
    };
    const url = '/ChangePassword?Handler=ChangePassword&key=' + key;

    const {status, data} = await fetchData(url, "PUT", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('密碼更換成功', "success", 3000);
        hidePopup('popupChangePassword');
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}