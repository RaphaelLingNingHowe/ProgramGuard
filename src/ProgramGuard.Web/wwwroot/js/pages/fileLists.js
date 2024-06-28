async function addFile() {
    var filePath = $("#FilePath").dxTextBox("instance").option("value");

    $.ajax({
        url: '/FileLists?Handler=FilePath',
        type: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(filePath),
        success: function (response) {
            if (response.success) {
                DevExpress.ui.notify(response.message, "success", 3000);
                location.reload();
            } else {
                DevExpress.ui.notify(response.message, "error", 3000);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 400) {
                var errorMessage = jqXHR.responseText;
                console.log('HTTP 400 Bad Request:', errorMessage);
                DevExpress.ui.notify(errorMessage, "error", 3000);
            } else if (textStatus === 'timeout') {
                $.ajax(this);
            } else {
                console.log('其他錯誤', textStatus, errorThrown);
            }
        }
    });
}

$.ajaxSetup({
    data: {
        __RequestVerificationToken: document.getElementsByName("__RequestVerificationToken")[0].value
    }
});