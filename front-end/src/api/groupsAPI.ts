import { API_GROUPS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";
import type { GroupSimpleDto, GroupDto } from "../types/Group";
import type { UserPatchDto } from "../types/User";

async function getFetchLogic<T>(url: string): Promise<T> {
    const response = await apiFetch(url);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(`Fetch error at ${API_GROUPS_ENDPOINT}: ${message}`);
    }
    const data = await response.json();
    return data as T;
}

export async function fetchGroups(userId: number | null): Promise<GroupSimpleDto[]> {
    try {
        const url = `${API_GROUPS_ENDPOINT}?userId=${userId}`;
        return await getFetchLogic<GroupSimpleDto[]>(url);
    } catch (error) {
        console.error("Failed to fetch groups:", error);
        throw error;
    }
}

export async function fetchSingleGroup(userId: number | null, groupId: number | null): Promise<GroupDto> {
    try {
        const url = `${API_GROUPS_ENDPOINT}/single?userId=${userId}&groupId=${groupId}`;
        return await getFetchLogic<GroupDto>(url);
    } catch (error) {
        console.error("Failed to fetch group:", error);
        throw error;
    }
}

export async function postGroup(groupData: GroupSimpleDto): Promise<GroupSimpleDto> {
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

export async function patchGroupMember(groupId: number, user:UserPatchDto): Promise<GroupSimpleDto> {
    try {
        const response = await apiFetch(`${API_GROUPS_ENDPOINT}/${groupId}`, {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(user)
        });
        if (!response.ok) {
            throw new Error(`Patch error at ${API_GROUPS_ENDPOINT}: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Failed to fetch groups:", error);
        throw error;
    }
}