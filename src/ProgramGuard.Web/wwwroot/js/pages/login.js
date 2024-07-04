
function loginPasswordOnClick() {
    var passwordEditor = $("#loginPassword").dxTextBox("instance");
    passwordEditor.option("mode", passwordEditor.option("mode") === "text" ? "password" : "text");
}

function loginOnEnterKey(e) {
    if (e.event.key === "Enter") {
        e.event.preventDefault();
        loginOnClick();
    }
}

async function loginOnClick() {
    var loginUserName = $("#loginUserName").dxTextBox("instance").option("value");
    var loginPassword = $("#loginPassword").dxTextBox("instance").option("value");

    if (!loginUserName || !loginPassword) {
        DevExpress.ui.notify("請輸入用戶名和密碼", "error", 3000);
        return;
    }

    var loginDto = {
        LoginUserName: loginUserName,
        LoginPassword: loginPassword
    };

    try {
        const response = await $.ajax({
            url: '/Login?Handler=Login',
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            contentType: 'application/json',
            data: JSON.stringify(loginDto)
        });

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
    } catch (error) {
        DevExpress.ui.notify("登錄失敗", "error", 3000);
    }
}


/*需要更換密碼時會用到的*/
function saveUsernameToSession(loginUserName) {
    sessionStorage.setItem('username', loginUserName);
}
function getUsernameFromSession() {
    return sessionStorage.getItem('username');
}