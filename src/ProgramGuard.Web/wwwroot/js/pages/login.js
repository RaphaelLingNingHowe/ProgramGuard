function changePasswordModeLogin() {
    var passwordEditor = $("#loginPassword").dxTextBox("instance");
    passwordEditor.option("mode", passwordEditor.option("mode") === "text" ? "password" : "text");
}