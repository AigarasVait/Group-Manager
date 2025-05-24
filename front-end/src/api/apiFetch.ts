const backendPort = import.meta.env.VITE_API_BASE_URL;

export function apiFetch(path: string, options?: RequestInit) {
  return fetch(`${backendPort}${path}`, options);
}