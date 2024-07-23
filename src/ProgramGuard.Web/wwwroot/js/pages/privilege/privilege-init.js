async function init() {
    const url = '/PrivilegeManage?handler=PrivilegeRule';
    const { status, data } = await fetchData(url, "GET");
    if (status >= 200 && status < 300) {
        if (data.visiblePrivileges && data.operatePrivileges) {
            createCheckboxes('visible-privileges-container', data.visiblePrivileges, 'visible');
            createCheckboxes('operate-privileges-container', data.operatePrivileges, 'operate');
        } else {
            console.error('Unexpected data format');
        }
    } else {
        throw new Error('Network response was not ok');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    init();
});
