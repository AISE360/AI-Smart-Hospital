import axios from 'axios';

const API_URL = (import.meta as any).env?.VITE_API_URL || 'http://localhost:5000';

export const api = axios.create({
  baseURL: API_URL + '/api',
  timeout: 15000,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (r) => r,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('token');
      // redirect to login if not already there
      if (window.location.pathname !== '/login') window.location.href = '/login';
    }
    // offline tolerance: show friendly message
    if (!err.response) {
      err.message = 'API unreachable — running in offline view mode. Data shown may be cached.';
    }
    return Promise.reject(err);
  }
);

// queue writes when offline (very light MVP - store in localStorage)
const OFFLINE_QUEUE_KEY = 'offline_queue';
export function queueOfflineWrite(url: string, data: any) {
  const q = JSON.parse(localStorage.getItem(OFFLINE_QUEUE_KEY) || '[]');
  q.push({ url, data, at: new Date().toISOString() });
  localStorage.setItem(OFFLINE_QUEUE_KEY, JSON.stringify(q));
}
export async function flushOfflineQueue() {
  const q: any[] = JSON.parse(localStorage.getItem(OFFLINE_QUEUE_KEY) || '[]');
  if (!q.length) return;
  for (const item of q) {
    try { await api.post(item.url, item.data); } catch {}
  }
  localStorage.removeItem(OFFLINE_QUEUE_KEY);
}
