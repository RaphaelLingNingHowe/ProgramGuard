function assignPrivilegeTemplate(cellElement, cellInfo) {
    $("<div>")
        .dxButton({
            text: "分配權限",
            type: "default",
            stylingMode: "contained",
            onClick: function () {
                var userId = cellInfo.data.UserId;
                var privilegeId = cellInfo.data.Privilege;
                showAssignPrivilegePopup(userId, privilegeId);
            }
        })
        .appendTo(cellElement);
}
function showAssignPrivilegePopup(userId, privilegeId) {
    var popup = $("#assignPrivilegePopup").dxPopup("instance");
    popup.show();
    popup.option({
        userId: userId,
        privilegeId: privilegeId
    });

    $.ajax({
        url: '/Users?handler=Privilege',
        type: 'GET',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        success: function (response) {
            var $privilegeList = $('#privilegeList');
            $privilegeList.empty();

            var switches = [];

            $.ajax({
                url: '/Users?handler=Privilege', 
                type: 'GET',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                contentType: 'application/json',
                success: function () {
                    response.forEach(function (privilege) {
                        var $field = $('<div class="dx-field"></div>');
                        var $label = $('<div class="dx-field-label"></div>').text(privilege.Name);
                        var $value = $('<div class="dx-field-value"></div>');
                        var $switch = $('<div></div>').attr('id', 'switch-' + privilege.Id);
                        $value.append($switch);
                        $field.append($label).append($value);
                        $privilegeList.append($field);

                        var isUserPrivilege = privilege.Id === privilegeId;

                        var switchInstance = $switch.dxSwitch({
                            value: isUserPrivilege,
                            onValueChanged: function (e) {
                                if (e.value) {
                                    switches.forEach(function (s) {
                                        if (s !== switchInstance) {
                                            s.option('value', false);
                                        }
                                    });

                                    updateUserPrivilege(userId, privilege.Id);
                                } else {
                                    if (!switches.some(s => s.option('value'))) {
                                        e.component.option('value', true);
                                    }
                                }
                            }
                        }).dxSwitch('instance');

                        switches.push(switchInstance);
                    });

                    if (!switches.some(s => s.option('value'))) {
                        switches[0].option('value', true);
                        updateUserPrivilege(userId, response[0].Id, true);
                    }
                },
                error: function () {
                    DevExpress.ui.notify("获取用户权限失败", "error", 3000);
                }
            });
        },
        error: function () {
            DevExpress.ui.notify("数据获取失败", "error", 3000);
        }
    });
}
function updateUserPrivilege(userId, privilegeId) {
    $.ajax({
        url: '/Users?handler=UpdatePrivilege&userId='+userId+"&privilegeId="+privilegeId,
        type: 'PUT',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        success: function (response) {
            DevExpress.ui.notify("權限更新成功", "success", 3000);
        },
        error: function (xhr, status, error) {
            console.error('Error updating privilege:', status, error);
            DevExpress.ui.notify("權限更新失敗", "error", 3000);
        }
    });
}
