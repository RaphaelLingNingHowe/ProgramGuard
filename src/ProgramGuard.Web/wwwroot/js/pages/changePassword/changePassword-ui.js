function onPopupHidden() {
    $("#CurrentPassword").dxTextBox("instance").reset();
    $("#NewPassword").dxTextBox("instance").reset();
    $("#ConfirmPassword").dxTextBox("instance").reset();
}

