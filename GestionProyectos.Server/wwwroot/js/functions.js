window.getValueById = function (id) {
    var input = document.getElementById(id);
    if (input) {
        return input.value;
    }
    return null;
};

window.setInputValue = function (id, value) {
    var input = document.getElementById(id);
    if (input) {
        input.value = value;
    }
};

window.descargarArchivo = function (nombreArchivo, contenidoBase64) {
    var blob = base64toBlob(contenidoBase64);

    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = nombreArchivo;

    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);
};

function base64toBlob(base64String) {
    var byteCharacters = atob(base64String);
    var byteNumbers = new Array(byteCharacters.length);

    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    var byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray], { type: 'application/octet-stream' });
}