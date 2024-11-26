const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const axios = require('axios');  // Import axios

function createWindow() {
    const win = new BrowserWindow({
        frame: false,
        fullscreen: true,
        skipTaskbar: true,
        webPreferences: {
            nodeIntegration: false,
            contextIsolation: true,
            preload: path.join(__dirname, 'preload.js')
        }
    });

    win.loadFile('index.html');
}

ipcMain.on('close-app', () => {
    app.quit();
});

app.whenReady().then(() => {
    GET("http://127.0.0.1:18080/data/disable/tm") // C++ Function
    createWindow();

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

function GET(url) {
    axios.get(url)
        .then(response => {
            console.log('Data received:', response.data);
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}
