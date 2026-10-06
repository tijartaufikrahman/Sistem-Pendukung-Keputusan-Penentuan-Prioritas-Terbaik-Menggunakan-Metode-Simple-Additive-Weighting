// =======================================================
// Konfigurasi API Backend & Sesi Autentikasi JWT Bearer
// SPK Penentuan Prioritas Alternatif Terbaik
// =======================================================

window.API_BASE_URL = window.API_BASE_URL || 'http://localhost:10010/api';
var API_BASE_URL = window.API_BASE_URL;

// Helper Session Pengguna & JWT Token
window.AuthSession = window.AuthSession || {};
var AuthSession = window.AuthSession;

// 1. Cookie Helper yang Aman
AuthSession.getCookie = function (name) {
    var prefix = name + "=";
    var cookies = document.cookie ? document.cookie.split(';') : [];
    for (var i = 0; i < cookies.length; i++) {
        var c = cookies[i].trim();
        if (c.indexOf(prefix) === 0) {
            try {
                return decodeURIComponent(c.substring(prefix.length));
            } catch (e) {
                return c.substring(prefix.length);
            }
        }
    }
    return null;
};

AuthSession.setCookie = function (name, value, hours) {
    var expires = "";
    var h = (hours !== undefined && hours !== null) ? hours : 24;
    var date = new Date();
    date.setTime(date.getTime() + (h * 60 * 60 * 1000));
    expires = "; expires=" + date.toUTCString();
    document.cookie = name + "=" + encodeURIComponent(value || "") + expires + "; path=/; SameSite=Lax";
};

AuthSession.removeCookie = function (name) {
    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Lax";
};

// 2. Token Management
AuthSession.getToken = function () {
    var token = localStorage.getItem('jwtToken');
    if (!token) {
        token = this.getCookie('jwtToken');
        if (token) {
            try { localStorage.setItem('jwtToken', token); } catch (e) { }
        }
    }
    return token;
};

AuthSession.setToken = function (token) {
    if (!token) return;
    try {
        localStorage.setItem('jwtToken', token);
    } catch (e) { }
    this.setCookie('jwtToken', token, 24);
};

AuthSession.removeToken = function () {
    try {
        localStorage.removeItem('jwtToken');
    } catch (e) { }
    this.removeCookie('jwtToken');
};

// 3. Validasi Masa Berlaku Token JWT
AuthSession.isTokenExpired = function (token) {
    if (!token || typeof token !== 'string') return true;
    var parts = token.split('.');
    if (parts.length !== 3) return true;

    try {
        var base64Url = parts[1];
        var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        while (base64.length % 4 !== 0) {
            base64 += '=';
        }
        var jsonPayload = decodeURIComponent(escape(window.atob(base64)));
        var payload = JSON.parse(jsonPayload);
        if (payload && payload.exp) {
            return (payload.exp + 15) < (Date.now() / 1000);
        }
        return false;
    } catch (e) {
        return false;
    }
};

// 4. User Profile Management
AuthSession.getUser = function () {
    try {
        var data = localStorage.getItem('currentUser');
        if (data) return JSON.parse(data);
    } catch (e) { }

    var name = this.getCookie('userName');
    var role = this.getCookie('userRole');
    if (name) {
        return {
            namaLengkap: name,
            roleName: role || 'Administrator SPK',
            roleId: (role === 'Tim Pengambil Keputusan' || role === 'Manajer Pengadaan' ? 2 : 1)
        };
    }
    return null;
};

AuthSession.setUser = function (user) {
    if (!user) return;
    try {
        localStorage.setItem('currentUser', JSON.stringify(user));
    } catch (e) { }

    if (user.namaLengkap) {
        this.setCookie('userName', user.namaLengkap, 24);
    }
    if (user.roleName) {
        this.setCookie('userRole', user.roleName, 24);
    }
};

AuthSession.removeUser = function () {
    try {
        localStorage.removeItem('currentUser');
    } catch (e) { }
    this.removeToken();
    this.removeCookie('userName');
    this.removeCookie('userRole');
};

AuthSession.isAuthenticated = function () {
    var token = this.getToken();
    var user = this.getUser();
    if (!token || !user) return false;
    return !this.isTokenExpired(token);
};

AuthSession.requireAuth = function () {
    if (!this.isAuthenticated()) {
        this.removeUser();
        window.location.href = '/Account/Login';
    }
};

// 5. Global jQuery Interceptors untuk Otentikasi JWT Bearer
function shouldAttachToken(url) {
    if (!url) return false;
    var u = url.toLowerCase();
    if (u.indexOf('/api/auth/login') !== -1 || u.indexOf('/api/users/login') !== -1) {
        return false;
    }
    return u.indexOf('/api') !== -1 || (window.API_BASE_URL && url.indexOf(window.API_BASE_URL) !== -1);
}

function attachBearerHeader(xhr, headersObj) {
    var token = AuthSession.getToken();
    if (token) {
        if (xhr && typeof xhr.setRequestHeader === 'function') {
            try {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            } catch (e) { }
        }
        if (headersObj) {
            headersObj['Authorization'] = 'Bearer ' + token;
        }
    }
}

if (window.jQuery && $.ajaxPrefilter) {
    $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        if (shouldAttachToken(options.url)) {
            options.headers = options.headers || {};
            attachBearerHeader(jqXHR, options.headers);
        }
    });
}

if (window.jQuery) {
    $(document).ajaxSend(function (event, jqXHR, settings) {
        if (shouldAttachToken(settings.url)) {
            attachBearerHeader(jqXHR, null);
        }
    });
}

function initJwtAjaxInterceptor() {
    if (window.jQuery && $.ajaxSetup) {
        $.ajaxSetup({
            beforeSend: function (xhr, settings) {
                if (shouldAttachToken(settings.url)) {
                    attachBearerHeader(xhr, null);
                }
            },
            statusCode: {
                401: function () {
                    var curPath = (window.location.pathname || '').toLowerCase();
                    var isAuthPage = curPath.indexOf('/account/login') !== -1 ||
                                     curPath.indexOf('/account/forgotpassword') !== -1 ||
                                     curPath.indexOf('/account/verifyotp') !== -1 ||
                                     curPath.indexOf('/account/resetpassword') !== -1;

                    if (isAuthPage) return;

                    var token = AuthSession.getToken();
                    var isExpired = !token || AuthSession.isTokenExpired(token);

                    if (isExpired) {
                        console.warn('Sesi otentikasi JWT telah kedaluwarsa (401). Mengalihkan ke login...');
                        AuthSession.removeUser();
                        window.location.href = '/Account/Login?sessionExpired=1';
                    }
                }
            }
        });
    }
}

initJwtAjaxInterceptor();
if (window.jQuery) {
    $(function () {
        initJwtAjaxInterceptor();
    });
}
