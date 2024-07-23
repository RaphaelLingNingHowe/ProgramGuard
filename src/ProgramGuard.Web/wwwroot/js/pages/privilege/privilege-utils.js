function updateSelectedPrivileges() {
    const visiblePrivileges = calculatePrivileges('visible-privileges-container');
    const operatePrivileges = calculatePrivileges('operate-privileges-container');

    return {
        visible: visiblePrivileges,
        operate: operatePrivileges
    };
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


