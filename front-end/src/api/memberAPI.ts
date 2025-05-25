import { API_USERS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";


export async function validateLogin(credentials: { username: string, password: string }): Promise<{ id: number }> {
    try {
        const response = await apiFetch((API_USERS_ENDPOINT + "/validate"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(credentials)
        });

        if (!response.ok) {
            throw new Error(`Login failed`);
        }
        return await response.json();
    } catch (error) {
        throw error;
    }
}

export async function createNewLogin(credentials: { username: string, password: string }): Promise<{ id: number }> {
    try {
        const response = await apiFetch((API_USERS_ENDPOINT + "/create"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(credentials)
        });

        if (!response.ok) {
            throw new Error(`Error creating new login: User already exists`);
        }
        return await response.json();
    } catch (error) {
        throw error;
    }

}