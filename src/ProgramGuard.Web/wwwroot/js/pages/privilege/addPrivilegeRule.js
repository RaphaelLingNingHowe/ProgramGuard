async function showAddRulePopup() {
    $("#addRulePopup").dxPopup("instance").show();
}
$("#btnAdd").click(function () {
    const visible = calculatePrivileges('visible-privileges-container');
    const operate = calculatePrivileges('operate-privileges-container');
    if (visible == 0 && operate == 0) {
        $("#btnAdd").blur();
        DevExpress.ui.notify("應至少選擇一種權限", "warning", 3000);
        return;
    }
    showAddRulePopup();
});

async function createRuleSubmit() {
    const visible = calculatePrivileges('visible-privileges-container');
    const operate = calculatePrivileges('operate-privileges-container');

    const name = $("#createRuleName").dxTextBox("instance").option("value");

    const requestData = {
        Name: name,
        Visible: visible,
        Operate: operate
    };

    fetch('/PrivilegeManage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(requestData)
    }).then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
        .then(data => {
            if (data.success) {
                DevExpress.ui.notify(data.message, "success", 3000);
                $("#addRulePopup").dxPopup("instance").hide();
            } else {
                DevExpress.ui.notify(data.message, "error", 3000);
            }
        })
        .catch(error => {
            console.error('Error', error);
            DevExpress.ui.notify("失敗", "error", 3000);
        });
        
}
