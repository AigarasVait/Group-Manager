import { API_GROUPS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";
import type { GroupSimpleDto, GroupDto, GroupPatchDto } from "../types/Group";

/**
 * Core fetch helper.
 */
async function fetchWithErrorHandling<T>(url: string, options?: RequestInit): Promise<T> {
    const response = await apiFetch(url, options);

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`API error at ${url}: ${errorText}`);
    }

    return response.json() as Promise<T>;
}

/**
 * Fetches all groups for a given user.
 * Throws if userId is null to prevent invalid request.
 */
export async function fetchGroups(userId: number | null): Promise<GroupSimpleDto[]> {
    if (userId === null) {
        throw new Error("User ID must be provided to fetch groups.");
    }

    const url = `${API_GROUPS_ENDPOINT}?userId=${userId}`;
    try {
        return await fetchWithErrorHandling<GroupSimpleDto[]>(url);
    } catch (error) {
        console.error("Failed to fetch groups:", error);
        throw error;
    }
}

/**
 * Fetches information for a single group by ID.
 * Throws if userId or groupId is null to prevent invalid request.
 */
export async function fetchSingleGroup(userId: number | null, groupId: number | null): Promise<GroupDto> {
    if (userId === null || groupId === null) {
        throw new Error("User ID and Group ID must be provided to fetch a single group.");
    }

    const url = `${API_GROUPS_ENDPOINT}/single?userId=${userId}&groupId=${groupId}`;
    try {
        return await fetchWithErrorHandling<GroupDto>(url);
    } catch (error) {
        console.error("Failed to fetch group:", error);
        throw error;
    }
}

/**
 * Creates a new group on the server.
 */
export async function postGroup(groupData: GroupSimpleDto): Promise<GroupSimpleDto> {
    try {
        return await fetchWithErrorHandling<GroupSimpleDto>(API_GROUPS_ENDPOINT, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(groupData),
        });
    } catch (error) {
        console.error("Failed to create group:", error);
        throw error;
    }
}

/**
 * Patches group members by adding or removing user IDs.
 * `patchData` contains the partial update object.
 */
export async function patchGroupMember(groupId: number, patchData: GroupPatchDto): Promise<GroupSimpleDto> {
    try {
        return await fetchWithErrorHandling<GroupSimpleDto>(`${API_GROUPS_ENDPOINT}/${groupId}`, {
            method: "PATCH",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(patchData),
        });
    } catch (error) {
        console.error("Failed to patch group members:", error);
        throw error;
    }
}
