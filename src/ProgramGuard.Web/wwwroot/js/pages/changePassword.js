async function changePasswordMode(name) {
    let editor = $(`#${name}`).dxTextBox("instance");
    editor.option('mode', editor.option('mode') === 'text' ? 'password' : 'text');
}
async function showChangePasswordPopup() {
    $("#changePasswordPopup").dxPopup("instance").show();
}
$(document).ready(function () {
    $("#navChangePassword").on("click", function (event) {
        event.preventDefault();
        $("#changePasswordPopup").dxPopup("instance").show();
    });

    $("#changePasswordPopup").dxPopup({
        onHidden: function () {
            $("#CurrentPassword").dxTextBox("instance").reset();
            $("#NewPassword").dxTextBox("instance").reset();
            $("#ConfirmPassword").dxTextBox("instance").reset();
        }
    });
});

async function changePassword() {
    var currentPassword = $("#CurrentPassword").dxTextBox("instance").option("value");
    var newPassword = $("#NewPassword").dxTextBox("instance").option("value");
    var confirmPassword = $("#ConfirmPassword").dxTextBox("instance").option("value");

    var changePasswordDto = {
        CurrentPassword: currentPassword,
        NewPassword: newPassword,
        ConfirmPassword: confirmPassword
    };
    var key = $('#hiddenAccount').text()
    if (!key) {
        key = getUsernameFromSession();
    }

    $.ajax({
        url: '/ChangePassword?Handler=ChangePassword&key=' + key,
        type: 'PUT',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(changePasswordDto),
        success: function (response) {
            if (response.success) {
                console.log('Password changed successfully:', response.message);
                DevExpress.ui.notify(response.message, "success", 3000);
                $("#changePasswordPopup").dxPopup("instance").hide();
            } else {
                console.error('Error changing password:', response.message);
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function () {
            DevExpress.ui.notify("密碼更換失敗", "error", 3000);
        }
    });
}