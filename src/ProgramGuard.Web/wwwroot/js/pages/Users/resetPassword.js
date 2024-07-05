function resetPasswordTemplate(cellElement, cellInfo) {
    $("<div>")
        .addClass("reset-password-button")
        .dxButton({
            text: "重設密碼",
            type: "danger",
            stylingMode: "contained",
            onClick: function () {
                var userId = cellInfo.data.UserId; // 获取用户ID
                /*console.log("Selected User ID:", userId);*/
                showResetPasswordPopup(userId)
            }
        })
        .appendTo(cellElement);
}

function showResetPasswordPopup(userId) {
    /*console.log("Selected User ID:", userId);*/
    var popup = $("#resetPasswordPopup").dxPopup("instance");
    popup.show();
    popup.option("userId", userId);
}

$("#resetPasswordPopup").dxPopup({
    onHidden: function () {
        resetPasswordOnHidden();
    }
});
function resetPasswordOnHidden() {
    $("#resetPassword").dxTextBox("instance").reset();
}
function resetPassword() {
    var userId = $("#resetPasswordPopup").dxPopup("instance").option("userId");
    console.log("Selected User ID:", userId);
    var resetPassword = $("#resetPassword").dxTextBox("instance").option("value");
    $.ajax({
        url: '/Users?Handler=ResetPassword&key='+userId,
        type: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify({ ResetPassword: resetPassword }),
        success: function (response) {
            if (response.success) {
                DevExpress.ui.notify(response.message, "success", 3000);
            } else {
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function () {
            DevExpress.ui.notify("密碼重置失敗", "error", 3000);
        }
    });
}