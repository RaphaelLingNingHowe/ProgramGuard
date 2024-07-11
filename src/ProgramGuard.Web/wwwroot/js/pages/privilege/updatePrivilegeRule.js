async function showUpdateRulePopup() {
    $("#updateRulePopup").dxPopup("instance").show();
    const id = selectedPrivilege.Id;
    console.log(id);
}

$("#btnUpdate").click(function () {
    const privilege = $("#privilege").dxSelectBox("instance").option("value");
    if (!privilege) {
        DevExpress.ui.notify("請選擇欲編輯之權限規則", "warning", 3000);
    }
    showUpdateRulePopup();
});

async function updateRuleSubmit() {
    
    const visible = calculatePrivileges('visible-privileges-container');
    const operate = calculatePrivileges('operate-privileges-container');

    const name = $("#updateRuleName").dxTextBox("instance").option("value");

    const requestData = {
        Name: name,
        Visible: visible,
        Operate: operate
    };

    fetch('/PrivilegeManage&key=' + id, {
        method: 'PUT',
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
                $("#updateRulePopup").dxPopup("instance").hide();
            } else {
                DevExpress.ui.notify(data.message, "error", 3000);
            }
        })
        .catch(error => {
            console.error('Error', error);
            DevExpress.ui.notify("失敗", "error", 3000);
        });

}
