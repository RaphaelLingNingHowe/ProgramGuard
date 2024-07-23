function onEnterKeyLogin(e) {
    if (e.event.key === "Enter") {
        submitLogin();
    }
}

/*需要更換密碼時會用到的*/
function saveUsernameToSession(loginUserName) {
    sessionStorage.setItem('username', loginUserName);
}
function getUsernameFromSession() {
    return sessionStorage.getItem('username');
}

function setChangePasswordMessage(message) {
    $("#changePasswordMessage").text(message);
}