const backendPort = import.meta.env.VITE_FRONTEND_PORT;

export function apiFetch(path: string, options?: RequestInit) {
  return fetch(`${backendPort}${path}`, options);
}