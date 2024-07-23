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
            text: privilege.description,
            value: false,
            onValueChanged: function (e) {
                updateSelectedPrivileges();
            }
        });

        $(`#${checkboxContainer.id}`).data('value', privilege.value);
    });
}

function setPrivileges(visible, operate) {
    setCheckboxes('visible-privileges-container', visible);
    setCheckboxes('operate-privileges-container', operate);
}

function setCheckboxes(containerId, value) {
    const container = document.getElementById(containerId);
    const checkboxes = container.querySelectorAll('.dx-checkbox');
    checkboxes.forEach((checkbox) => {
        const checkboxInstance = $(checkbox).dxCheckBox('instance');
        const checkboxValue = parseInt($(checkbox).data('value'));
        checkboxInstance.option('value', (value & checkboxValue) !== 0);
    });
}
