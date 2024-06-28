// 登录成功后保存用户名到 Session Storage 或者全局变量中
function saveUsernameToSession(loginUserName) {
    sessionStorage.setItem('username', loginUserName);
}

// 在需要更改密码时获取保存的用户名
function getUsernameFromSession() {
    return sessionStorage.getItem('username');
}
function loginPasswordOnClick() {
    var passwordEditor = $("#loginPassword").dxTextBox("instance");
    passwordEditor.option("mode", passwordEditor.option("mode") === "text" ? "password" : "text");
}

async function loginOnClick() {
    var loginUserName = $("#loginUserName").dxTextBox("instance").option("value");
    var loginPassword = $("#loginPassword").dxTextBox("instance").option("value");

    var loginDto = {
        LoginUserName: loginUserName,
        LoginPassword: loginPassword
    };

    $.ajax({
        url: '/Login?Handler=Login',
        type: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(loginDto),
        success: function (response) {
            if (response.success) {
                if (response.requirePasswordChange) {
                    console.log('超過80天未更換密碼，請更換密碼', response.message);
                    DevExpress.ui.notify(response.message, "error", 3000);
                    saveUsernameToSession(loginUserName);
                    showChangePasswordPopup();
                } else {
                    window.location.href = '/FileLists';
                }
            } else {
                console.error('Error changing password:', response.message);
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function () {
            DevExpress.ui.notify("登錄失敗", "error", 3000);
        }
    });
}
