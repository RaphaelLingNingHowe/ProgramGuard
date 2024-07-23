function showCreateUserPopup() {
    showPopup('popupCreateUser');
}

//function onDataGridCellClick(e) {
//    if (e.column.dataField === "IsEnabled") {
//        var userId = e.data.UserId;
//        var isEnabled = !e.data.IsEnabled;

//        updateUserStatus(userId, isEnabled)
//    }
//} 

function showResetPasswordPopup(userId) {
    var popup = $("#popupResetPassword").dxPopup("instance");
    popup.show();
    popup.option("userId", userId);
}