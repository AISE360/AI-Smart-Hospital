import { contextBridge, ipcRenderer } from 'electron';

contextBridge.exposeInMainWorld('hospital', {
  getAppVersion: () => ipcRenderer.invoke('get-app-version'),
  getApiUrl: () => ipcRenderer.invoke('get-api-url'),
});

declare global {
  interface Window {
    hospital: {
      getAppVersion: () => Promise<string>;
      getApiUrl: () => Promise<string>;
    };
  }
}
