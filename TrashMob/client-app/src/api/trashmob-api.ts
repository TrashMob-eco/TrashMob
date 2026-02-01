import { Client } from "./trashmob-api.v2.generated";

const baseUrl = import.meta.env.VITE_API_URL;

export const trashmobApiClient = new Client(baseUrl);
