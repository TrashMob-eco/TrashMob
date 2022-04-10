import UserData from "../components/Models/UserData";

const SessionStorageKey = 'user';

export const storeUser = (user: UserData) => {
    sessionStorage.setItem(SessionStorageKey, JSON.stringify(user));
};

export const getStoredUser = () => {
    return sessionStorage.getItem(SessionStorageKey);
};

export const clearStoredUser = () => {
    sessionStorage.removeItem(SessionStorageKey);
};