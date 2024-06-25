function showPopup() {
    $("#addUserPopup").dxPopup("instance").show();
}

$("#addUserPopup").dxPopup({
    onHidden: function () {
        // 重置或清空输入框内容
        $("#UserName").dxTextBox("instance").reset();
        $("#Email").dxTextBox("instance").reset();
        $("#Password").dxTextBox("instance").reset();
    }
});
function addUserOnHidden() {
    $("#UserName").dxTextBox("instance").reset();
    $("#Email").dxTextBox("instance").reset();
    $("#Password").dxTextBox("instance").reset();
}
async function addUser() {
    var userName = $("#UserName").dxTextBox("instance").option("value");
    var email = $("#Email").dxTextBox("instance").option("value");
    var password = $("#Password").dxTextBox("instance").option("value");

    var createUserDto = {
        UserName: userName,
        Email: email,
        Password: password
    };

    $.ajax({
        url: '/Account/Manager?Handler=Manager',
        type: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(createUserDto),
        success: function (response) {
            if (response.success) {
                DevExpress.ui.notify(response.message, "success", 3000);
                $("#addUserPopup").dxPopup("instance").hide();
            } else {
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function () {
            DevExpress.ui.notify("使用者創建失敗", "error", 3000);
        }
    });
}