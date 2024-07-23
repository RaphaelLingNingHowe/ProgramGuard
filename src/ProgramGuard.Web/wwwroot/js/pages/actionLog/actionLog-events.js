function validateQuery() {
    let startTime = getDateBoxValue('startTime');
    let endTime = getDateBoxValue('endTime');

    if (!validateDateTime(startTime, endTime)) {
        return false;
    }
    reloadGrid();
}

