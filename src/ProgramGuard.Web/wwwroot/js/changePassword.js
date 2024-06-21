function changePasswordMode(name) {
    let editor = $(`#${name}`).dxTextBox("instance");
    editor.option('mode', editor.option('mode') === 'text' ? 'password' : 'text');
}

$(document).ready(function () {
    $("#navChangePassword").on("click", function (event) {
        event.preventDefault();
        $("#changePasswordPopup").dxPopup("instance").show();
    });

    // 配合 AntiForgeryToken 使用
    $.ajaxSetup({
        headers: {
            'RequestVerificationToken': document.getElementsByName("__RequestVerificationToken")[0].value
        }
    });

    var baseUrl = '@Configuration["AppSettings:BaseUrl"]';

    function submitChangePasswordForm() {
        var formData = {
            UserName: $('#UserName').dxTextBox('instance').option('value'),
            CurrentPassword: $('#CurrentPassword').dxTextBox('instance').option('value'),
            NewPassword: $('#NewPassword').dxTextBox('instance').option('value'),
            ConfirmPassword: $('#ConfirmPassword').dxTextBox('instance').option('value')
        };

        $.ajax({
            url: baseUrl + '/User/ChangePassword', // 构建完整的 API 路径
            type: 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            headers: {
                'RequestVerificationToken': document.getElementsByName("__RequestVerificationToken")[0].value
            },
            success: function (response) {
                // 处理成功情况，例如关闭弹出窗口
                $('#changePasswordPopup').dxPopup('instance').hide();
                // 可以根据需要执行其他操作
            },
            error: function (xhr, status, error) {
                // 处理失败情况，例如显示错误消息
                console.error(error);
                alert('表单提交失败，请重试！');
            }
        });
    }

    // 绑定提交按钮的点击事件
    $('#submitChangePasswordButton').on('click', function () {
        submitChangePasswordForm();
    });
});
