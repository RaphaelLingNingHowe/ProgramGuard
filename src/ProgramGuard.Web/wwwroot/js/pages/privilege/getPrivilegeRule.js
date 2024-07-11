let selectedPrivilege

function onPrivilegeSelected(e) {
    selectedPrivilege = e.value;    
    if (selectedPrivilege) {
        console.log(selectedPrivilege);
        setPrivileges(selectedPrivilege.Visible, selectedPrivilege.Operate);
    }
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