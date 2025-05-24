import { API_GROUPS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";
import type { GroupPost, Group } from "../types/Group";

export async function fetchGroups() {
    try {
        const response = await apiFetch(API_GROUPS_ENDPOINT);
        if (!response.ok) {
            throw new Error(`Fetch error at ${API_GROUPS_ENDPOINT}: ${response.status}`);
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

export async function postAddGroup(
    groupData: GroupPost,
    setGroups: React.Dispatch<React.SetStateAction<Group[]>>
): Promise<void> {
    try {
        const newGroup = await postGroup(groupData);
        setGroups((prevGroups) => [...prevGroups, newGroup])
    }
    catch (error) {
        console.error("Failed to add group:", error);
    }
}