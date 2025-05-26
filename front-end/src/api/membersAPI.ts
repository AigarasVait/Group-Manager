import { API_USERS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";

async function postUserFetch<T>(path: string, credentials: { username: string; password: string }): Promise<T> {
  const response = await apiFetch(`${API_USERS_ENDPOINT}${path}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    // Customize error message based on endpoint
    const errorMsg = path === "/validate" ? "Login failed" : "Error creating new login: User already exists";
    throw new Error(errorMsg);
  }

  return response.json() as Promise<T>;
}

export async function validateLogin(credentials: { username: string; password: string }): Promise<{ id: number }> {
  return postUserFetch<{ id: number }>("/validate", credentials);
}

export async function createNewLogin(credentials: { username: string; password: string }): Promise<{ id: number }> {
  return postUserFetch<{ id: number }>("/create", credentials);
}