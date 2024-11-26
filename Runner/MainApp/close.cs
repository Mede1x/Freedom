using System.Runtime.InteropServices;
using System;

public class KeyboardInterceptor
{
    // Constants for hook types and key codes
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int VK_LWIN = 0x5B;  // Left Windows key
    private const int VK_RWIN = 0x5C;  // Right Windows key

    // Change visibility of the delegate to public
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    // Set the hook
    private static IntPtr hookId = IntPtr.Zero;
    private static LowLevelKeyboardProc proc = HookCallback;

    // Import the necessary WinAPI functions
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    // Hook callback method
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        // If a key is pressed (WM_KEYDOWN)
        if (nCode >= 0 && (int)wParam == WM_KEYDOWN)
        {
            // Check if the key pressed is the Windows key (either left or right)
            int vkCode = Marshal.ReadInt32(lParam);

            // Disable both the left and right Windows keys
            if (vkCode == VK_LWIN || vkCode == VK_RWIN)
            {
                return (IntPtr)1;  // Suppress the key press
            }
        }

        return CallNextHookEx(hookId, nCode, wParam, lParam);
    }

    // Function to start listening for key presses
    public static void StartHook()
    {
        hookId = SetHook(proc);
    }

    // Function to set the hook
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    // Function to stop listening for key presses
    public static void StopHook()
    {
        UnhookWindowsHookEx(hookId);
    }
}
