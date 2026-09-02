import { app, BrowserWindow, ipcMain } from 'electron';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { spawn, ChildProcess } from 'node:child_process';
import { existsSync } from 'node:fs';
import http from 'node:http';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

let win: BrowserWindow | null = null;
let apiProc: ChildProcess | null = null;

function isApiReachable(url: string): Promise<boolean> {
  return new Promise((resolve) => {
    const req = http.get(url + '/health', { timeout: 1500 }, (res) => {
      resolve(res.statusCode === 200);
      res.resume();
    });
    req.on('error', () => resolve(false));
    req.setTimeout(1500, () => { req.destroy(); resolve(false); });
  });
}

async function ensureApiRunning() {
  const apiUrl = process.env.API_URL || 'http://localhost:5115';
  if (await isApiReachable(apiUrl)) {
    console.log(`[main] API already reachable at ${apiUrl}`);
    return;
  }
  // Try to spawn bundled API (extraResources/api/SmartHospital.Api.exe or .dll)
  const isPackaged = app.isPackaged;
  const resourcesPath = isPackaged ? process.resourcesPath : path.join(__dirname, '../../publish/api');
  const exeCandidates = [
    path.join(resourcesPath, 'api', 'SmartHospital.Api.exe'),
    path.join(resourcesPath, 'SmartHospital.Api.exe'),
    path.join(resourcesPath, 'api', 'SmartHospital.Api.dll'),
    path.join(__dirname, '../../publish/api/SmartHospital.Api.dll'),
  ];
  let target: string | null = null;
  for (const p of exeCandidates) {
    if (existsSync(p)) { target = p; break; }
  }
  if (!target) {
    console.log('[main] No bundled API found, expecting external API at', apiUrl);
    return;
  }
  const isDll = target.endsWith('.dll');
  const cmd = isDll ? 'dotnet' : target;
  const args = isDll ? [target, '--urls', apiUrl] : ['--urls', apiUrl];
  console.log(`[main] Spawning API: ${cmd} ${args.join(' ')} from ${path.dirname(target)}`);
  try {
    apiProc = spawn(cmd, args, {
      cwd: path.dirname(target),
      env: { ...process.env, ASPNETCORE_ENVIRONMENT: 'Production', DOTNET_ROLL_FORWARD: 'Major', UseInMemoryDatabase: 'true' },
      stdio: 'ignore',
      detached: false,
    });
    apiProc.on('error', (e) => console.error('[api] spawn error', e));
    apiProc.on('exit', (code) => console.log('[api] exited', code));
    // wait a bit for API to boot
    for (let i = 0; i < 15; i++) {
      await new Promise(r => setTimeout(r, 1000));
      if (await isApiReachable(apiUrl)) {
        console.log('[main] Bundled API started');
        return;
      }
    }
    console.log('[main] Bundled API did not become reachable in time');
  } catch (e) {
    console.error('[main] Failed to spawn API', e);
  }
}

function createWindow() {
  win = new BrowserWindow({
    width: 1400,
    height: 900,
    minWidth: 1200,
    minHeight: 700,
    backgroundColor: '#f8fafc',
    titleBarStyle: 'hiddenInset',
    autoHideMenuBar: true,
    webPreferences: {
      preload: path.join(__dirname, 'preload.mjs'),
      contextIsolation: true,
      nodeIntegration: false,
    },
    show: false,
    title: 'AI Smart Hospital — Command Center',
  });

  const isDev = !app.isPackaged;

  if (isDev) {
    win.loadURL('http://localhost:5173');
    // keep devtools closed for professional look; open with Ctrl+Shift+I
  } else {
    win.loadFile(path.join(__dirname, '../dist/index.html'));
  }

  win.once('ready-to-show', () => win?.show());
  win.on('closed', () => (win = null));
}

app.whenReady().then(async () => {
  await ensureApiRunning();
  createWindow();
});

app.on('window-all-closed', () => {
  if (apiProc) try { apiProc.kill(); } catch {}
  if (process.platform !== 'darwin') app.quit();
});
app.on('before-quit', () => {
  if (apiProc) try { apiProc.kill(); } catch {}
});
app.on('activate', () => {
  if (BrowserWindow.getAllWindows().length === 0) createWindow();
});

ipcMain.handle('get-app-version', () => app.getVersion());
ipcMain.handle('get-api-url', () => process.env.API_URL || 'http://localhost:5115');
