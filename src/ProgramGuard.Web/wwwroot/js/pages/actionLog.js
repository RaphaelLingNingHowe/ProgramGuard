async function searchOnClick() {
    var startTime = $("#startTime").dxDateBox("instance").option("value");
    var endTime = $("#endTime").dxDateBox("instance").option("value");

    try {
        let apiUrl = '/ActionLogs?Handler=ActionLog';

        let params = new URLSearchParams();
        if (startTime) {
            params.append('startTime', startTime.toLocaleString('zh-TW', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false
            }));
        }
        if (endTime) {
            params.append('endTime', endTime.toLocaleString('zh-TW', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false
            }));
        }

        if (params.toString()) {
            apiUrl += '&' + params.toString();
        }

        $.ajax({
            url: apiUrl,
            type: 'GET',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            contentType: 'application/json',
            success: function (response) {
                console.log('Search successful:', response);
                $(document).ready(function () {
                    $('#dataGrid').dxDataGrid({
                        dataSource: response,
                        keyExpr : "Id"
                        
                    });
                });
            },
            error: function (xhr, status, error) {
                console.error('Search failed:', error);
            }
        });
    } catch (error) {
        console.error('An error occurred:', error);
    }
}
