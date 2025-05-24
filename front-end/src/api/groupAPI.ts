import { API_GROUPS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";


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