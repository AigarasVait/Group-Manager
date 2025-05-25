import { API_GROUPS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";
import type { GroupPost, Group } from "../types/Group";


export async function fetchGroups(userId:number | null): Promise<Group[]> {
    try {
        const url = `${API_GROUPS_ENDPOINT}?userId=${userId}`;
        const response = await apiFetch(url);
        if (!response.ok) {
            const message = await response.text();  
            throw new Error(`Fetch error at ${API_GROUPS_ENDPOINT}: ${message}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Failed to fetch groups:", error);
        throw error;
    }
}

export async function postGroup(groupData: GroupPost): Promise<Group> {
    try {
        const response = await apiFetch(API_GROUPS_ENDPOINT, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(groupData)
        });
        if (!response.ok) {
            throw new Error(`Post error at ${API_GROUPS_ENDPOINT}: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Failed to fetch groups:", error);
        throw error;
    }
}