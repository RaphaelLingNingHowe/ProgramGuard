export default class Helper {
    badRequest() {
        DevExpress.ui.notify("輸入參數異常", "warning", 3000);
    }

    redirectToLogin() {
        this.redirectToLoginWithMsg("登入授權已過期、請重新登入");
    }

    redirectToLoginWithMsg(msg) {
        DevExpress.ui.notify(msg, "warning", 3000);
        setTimeout(() => { window.location.href = "/Account/Login?returnUrl=" + DOMPurify.sanitize(window.location.href); }, 3000);
    }

    accessDenied() {
        DevExpress.ui.notify("不具有操作此功能權限", "warning", 3000);
    }

    notFound() {
        DevExpress.ui.notify("找不到相關資源", "warning", 3000);
    }

    /* Grid Function */
    selectAllGrid(grid) {
        $("#" + grid).dxDataGrid("instance").selectAll();
    }

    unselectAllGrid(grid) {
        $("#" + grid).dxDataGrid("instance").clearSelection();
    }

    resetFocusedRow(grid) {
        $("#" + grid).dxDataGrid("instance").option("focusedRowIndex", -1);
    }

    refreshGrid(grid, data) {
        $("#" + grid).dxDataGrid("instance").option("dataSource", data);
    }

    reloadGrid(grid) {
        $("#" + grid).dxDataGrid("getDataSource").reload();
    }

    showGridLoadingPanel(grid, message) {
        $("#" + grid).dxDataGrid("instance").beginCustomLoading(message);
    }

    hideGridLoadingPanel(grid) {
        $("#" + grid).dxDataGrid("instance").endCustomLoading();
    }

    setGridSelectRows(grid, keys) {
        $("#" + grid).dxDataGrid("instance").selectRows(keys);
    }

    getGridSelectRowKeys(grid) {
        return $("#" + grid).dxDataGrid("getSelectedRowKeys");
    }

    getGridCellValue(grid, index, columnName) {
        return $("#" + grid).dxDataGrid("instance").cellValue(index, columnName);
    }

    getGridTotalCount(grid) {
        return $("#" + grid).dxDataGrid("totalCount");
    }

    getGridSelectedRowsData(grid) {
        return $("#" + grid).dxDataGrid("instance").getSelectedRowsData()
    }

    getGridTotalItems(grid) {
        return $("#" + grid).dxDataGrid("instance").getDataSource().items() 
    }

    getGridInstance(grid) {
        return $("#" + grid).dxDataGrid("instance");
    }

    cleanGridData(grid) {
        $("#" + grid).dxDataGrid("instance").option('dataSource', []);;
    }
    /* Grid Function */

    getCheckBoxValue(checkBox) {
        return $("#" + checkBox).dxCheckBox("instance").option("value");
    }

    setCheckBoxValue(checkBox, value) {
        $("#" + checkBox).dxCheckBox("instance").option("value", value);
    }

    setCheckBoxEnabled(checkBox, value) {
        $("#" + checkBox).dxCheckBox("instance").option("disabled", !value);
    }

    // getMonth 是 zero-base 實際使用要 + 1
    getDateBoxValue(datebox) {
        let date = $("#" + datebox).dxDateBox("instance").option("value");

        return date == null ? null :
            (date.getFullYear() + "-" +
            (date.getMonth() + 1).toString().padStart(2, "0") + "-" +
            date.getDate().toString().padStart(2, "0") + "T" +
            date.getHours().toString().padStart(2, "0") + ":" +
            date.getMinutes().toString().padStart(2, "0"));
    }

    getRadioGroupValue(radioGroup) {
        return $("#" + radioGroup).dxRadioGroup("instance").option("value");
    }

    getNumberBoxValue(numberBox) {
        return $("#" + numberBox).dxNumberBox("instance").option("value");
    }

    setNumberBoxValue(numberBox, format, value) {
        $("#" + numberBox).dxNumberBox({ format: format, value: value });
    }

    resetNumberBox(numberBox) {
        $("#" + numberBox).dxNumberBox("instance").reset();
    }

    getTextBoxValue(textbox) {
        return $("#" + textbox).dxTextBox("instance").option("text");
    }

    setTextBoxValue(textbox, value) {
        $("#" + textbox).dxTextBox({ value: value });
    }

    setTextBoxVisible(textbox, value) {
        $("#" + textbox).dxTextBox("option", "visible", value);
    }

    cleanTextbox(textbox) {
        let textboxInstance = $("#" + textbox).dxTextBox("instance");
        textboxInstance.option("value", null);
        textboxInstance.option("isValid", true);
    }

    getTextAreaValue(textArea) {
        return $("#" + textArea).dxTextArea("instance").option("value");
    }

    cleanTextArea(textArea) {
        $("#" + textArea).dxTextArea("instance").option("value", null);
    }

    getSelectBoxValue(selectBox) {
        let value = $("#" + selectBox).dxSelectBox("instance").option("value");
        return value != null ? value : "";
    }

    getSelectBoxText(selectBox) {
        let value = $("#" + selectBox).dxSelectBox("instance").option("text");
        return value != null ? value : "";
    }

    setSelectBoxValue(selectBox, value) {
        $("#" + selectBox).dxSelectBox("instance").option("value", value);
    }

    reloadSelectBox(selectBox) {
        let ds = $("#" + selectBox).dxSelectBox("getDataSource");
        ds.pageIndex(0);
        ds.reload();
    }

    reloadList(list) {
        $("#" + list).dxList("instance").reload();
    }

    updateSelectBoxByFilter(selectBoxId, key, value) {
        let selectBox = $("#" + selectBoxId).dxSelectBox("instance");

        let dataSource = selectBox.getDataSource();
        dataSource.filter(key, "=", value);
        dataSource.load();
    }

    resetSelectBox(selectBox) {
        $("#" + selectBox).dxSelectBox("instance").reset();
    }

    showPopup(popup) {
        $("#" + popup).dxPopup("instance").show();
    }

    hidePopup(popup) {
        $("#" + popup).dxPopup("instance").hide();
    }

    convertTimestamp(timestamp) {
        return timestamp.replace(/T/g, " ").replace(/Z/g, " ").replace(/-/g, "/");
    }

    verifyDeviceModel(cbxBrand, cbxDeviceModel) {
        let brand = $("#" + cbxBrand).dxSelectBox("instance").option("value");
        let deviceModel = $("#" + cbxDeviceModel).dxSelectBox("instance").option("value");

        if (brand != null && deviceModel == null) {
            DevExpress.ui.notify("未選擇設備型號", "warning", 3000);
            return false;
        }

        return true
    }

    verifyPart(cbxBrand, cbxPart) {
        let brand = $("#" + cbxBrand).dxSelectBox("instance").option("value");
        let part = $("#" + cbxPart).dxSelectBox("instance").option("value");

        if (brand != null && part == null) {
            DevExpress.ui.notify("未選擇料件", "warning", 3000);
            return false;
        }

        return true
    }

    verifyOrderProcessStatus(id) {
        let status = $("#" + id).dxSelectBox("instance").option("value");

        if (status == null) {
            DevExpress.ui.notify("請選擇處理狀態", "warning", 3000);
            return false;
        }

        return true
    }

    verifyTimeRange(dateBoxBegin, dateBoxEnd, queryRangeMaxDays, fieldName1 = "起始時間", fieldName2 = "結束時間") {
        let beginInstance = $("#" + dateBoxBegin).dxDateBox('instance');
        let endInstance = $("#" + dateBoxEnd).dxDateBox('instance');

        if (beginInstance.option('isValid') == false) {
            DevExpress.ui.notify(fieldName1 + "內容異常", "warning", 3000);
            return false;
        }
        else if (endInstance.option('isValid') == false) {
            DevExpress.ui.notify(fieldName2 + "內容異常", "warning", 3000);
            return false;
        }

        let begin = beginInstance.option('value');
        let end = endInstance.option('value');
        if (begin == null) {
            DevExpress.ui.notify("請輸入" + fieldName1, "warning", 3000);
            return false;
        }
        else if (end == null) {
            DevExpress.ui.notify("請輸入" + fieldName2, "warning", 3000);
            return false;
        }
        else if (begin == end) {
            DevExpress.ui.notify(fieldName1 + "不可等於" + fieldName2, "warning", 3000);
            return false;
        }

        let beginDate = new Date(begin);
        let endDate = new Date(end);

        if (beginDate > endDate) {
            DevExpress.ui.notify(fieldName1 + "不可大於" + fieldName2, "warning", 3000);
            return false;
        }

        if (queryRangeMaxDays != null || queryRangeMaxDays != undefined) {
            let diff = (endDate.getTime() - beginDate.getTime()) / (86400 * 1000);
            if (diff > queryRangeMaxDays) {
                DevExpress.ui.notify("查詢時間之區間不可大於" + queryRangeMaxDays + "日", "warning", 3000);
                return false;
            }
        }

        return true;
    }

    verifyTime(time, field = "填寫時間") {
        let timeInstance = $("#" + time).dxDateBox('instance');

        if (timeInstance.option('isValid') == false) {
            DevExpress.ui.notify(field + "內容異常", "warning", 3000);
            return false;
        }

        let timeValue = timeInstance.option('value');
        if (timeValue == null) {
            DevExpress.ui.notify("請輸入" + field, "warning", 3000);
            return false;
        }

        return true;
    }

    responseAnalysis(response) {
        if (response == undefined) {
            DevExpress.ui.notify("資料取得異常", "error", 3000);
            return false;
        }

        if (response.StatusCode == 401 || response.IsResponsePageResult) {
            if (response.Message != '' && response.Message != undefined) {
                this.redirectToLoginWithMsg(response.Message + "[" + response.StatusCode + "]");
            }
            else {
                this.redirectToLogin();
            }

            return false;
        }
        else if (response.StatusCode == 200 || response.StatusCode == 201 || response.StatusCode == 204) {
            return true;
        }
        else {
            let message = response.Message + "[" + response.StatusCode + "]";
            DevExpress.ui.notify(message, "error", 3000);

            return false;
        }
    }

    checkPwdComplexity(value) {
        const regExp = new RegExp(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/);

        if (regExp.test(value) == false) {
            DevExpress.ui.notify("密碼需為八碼以上並具備大小寫、數字、特殊符號", "warning", 3000);
            return false
        }

        return true;
    }

    getCurrentTimeStr() {
        let now = new Date();

        let year = now.getFullYear();
        let month = ('0' + (now.getMonth() + 1)).slice(-2);
        let day = ('0' + now.getDate()).slice(-2);
        let hour = ('0' + now.getHours()).slice(-2);
        let minute = ('0' + now.getMinutes()).slice(-2);

        return year + '-' + month + '-' + day + '-' + hour + '-' + minute;
    }
}
