function updateCheckBoxUI(checkBoxElement, confirmStatus) {
    checkBoxElement.option({
        value: confirmStatus,
        disabled: confirmStatus
    });
}

function DigitalSignatureTemplate(cellElement, cellInfo) {
    $("<div>").text(cellInfo.value ? "✔️" : "❌")
        .appendTo(cellElement);
}