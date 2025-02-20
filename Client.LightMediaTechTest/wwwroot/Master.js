//IMPORT ALL JS FILES TO THIS

function GetCookie() {
    return document.cookie
}

function SetCookie(key, value) {
    document.cookie = `${key}=${value}; path=/;`;
}

async function DownloadFileFromStreamAsync(filename, streamReference) {
    const arrayBuffer = await streamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElem = document.createElement('a');

    anchorElem.href = url;
    anchorElem.download = filename ?? '';

    anchorElem.click();
    anchorElem.remove;
    URL.revokeObjectURL(url);
}