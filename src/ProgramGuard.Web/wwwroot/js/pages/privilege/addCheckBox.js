fetch('/PrivilegeManage?handler=Data')
    .then(response => response.json())
    .then(data => {
        console.log('Received data:', data);
        if (data.visiblePrivileges && data.operatePrivileges) {
            createCheckboxes('visible-privileges-container', data.visiblePrivileges, 'visible');
            createCheckboxes('operate-privileges-container', data.operatePrivileges, 'operate');
        } else {
            console.error('Unexpected data format');
        }
    })
    .catch(error => console.error('Error:', error));

function createCheckboxes(containerId, privileges, type) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error(`Container ${containerId} not found`);
        return;
    }
    privileges.forEach(privilege => {
        const checkboxContainer = document.createElement('div');
        checkboxContainer.id = `${type}-${privilege.name}-container`;
        container.appendChild(checkboxContainer);

        $(`#${checkboxContainer.id}`).dxCheckBox({
            text: privilege.description ,
            value: false,
            onValueChanged: function (e) {
                updateSelectedPrivileges();
            }
        });

        // 將 value 存儲為自定義屬性
        $(`#${checkboxContainer.id}`).data('value', privilege.value);
    });
}

function updateSelectedPrivileges() {
    const visiblePrivileges = calculatePrivileges('visible-privileges-container');
    const operatePrivileges = calculatePrivileges('operate-privileges-container');
    console.log('當前選中的可見權限總和：', visiblePrivileges);
    console.log('當前選中的操作權限總和：', operatePrivileges);

    const resultElement = document.getElementById('selected-privileges-result');
    if (resultElement) {
        resultElement.textContent = `可見權限總和: ${visiblePrivileges}, 操作權限總和: ${operatePrivileges}`;
    }
}

function calculatePrivileges(containerId) {
    const container = document.getElementById(containerId);
    return Array.from(container.children)
        .filter(child => child.id.endsWith('-container'))
        .map(child => {
            const checkbox = $(child).dxCheckBox('instance');
            return checkbox.option('value') ? parseInt($(child).data('value')) : 0;
        })
        .reduce((sum, value) => sum + value, 0);
}

$.ajaxSetup({
    data: {
        __RequestVerificationToken: document.getElementsByName("__RequestVerificationToken")[0].value
    }
});
