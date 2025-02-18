//IMPORT ALL JS FILES TO THIS

function GetCookie() {
    return document.cookie
}

function SetCookie(key, value) {
    document.cookie = `${key}=${value}; path=/;`;
}