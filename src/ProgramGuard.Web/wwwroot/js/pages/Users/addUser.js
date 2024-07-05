function showPopup() {
    $("#addUserPopup").dxPopup("instance").show();
    }

$("#addUserPopup").dxPopup({
    onHidden: function () {
        $("#UserName").dxTextBox("instance").reset();
        $("#Password").dxTextBox("instance").reset();
    }
});
function addUserOnHidden() {
    $("#UserName").dxTextBox("instance").reset();
    $("#Password").dxTextBox("instance").reset();
}
async function addUser() {
    var userName = $("#UserName").dxTextBox("instance").option("value");
    var password = $("#Password").dxTextBox("instance").option("value");

    var createUserDto = {
        UserName: userName,
        Password: password
    };

    $.ajax({
        url: '/Users?Handler=AddUser',
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
                location.reload();
            } else {
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('發生了一些錯誤', textStatus, errorThrown);
            if (jqXHR.status === 400) {
                var errorMessage = jqXHR.responseText;
                console.log('HTTP 400 Bad Request:', errorMessage);
                DevExpress.ui.notify(errorMessage, "error", 3000);
            } else {
                console.log('其他錯誤');
            }
        }
    });
}
