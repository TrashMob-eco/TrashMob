const SessionStorageKey = 'local-url';

export const storeCurrentPath = () => {
    sessionStorage.setItem(SessionStorageKey, window.location.pathname);
};

export const getStoredPath = () => {
    return sessionStorage.getItem(SessionStorageKey);
};

export const clearStoredPath = () => {
    sessionStorage.removeItem(SessionStorageKey);
};