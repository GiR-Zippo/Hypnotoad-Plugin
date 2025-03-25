using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using HypnotoadPlugin.GameFunctions;
using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.Utils;
using ImGuiNET;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace HypnotoadPlugin.Windows;

#if DEBUG
#region Enums
public enum HandleType
{
    Unknown = 0,
    Other = 1,
    Mutant = 15,
}

public enum OBJECT_INFORMATION_CLASS
{
    ObjectBasicInformation,
    ObjectNameInformation,
    ObjectTypeInformation,
    ObjectAllTypesInformation,
    ObjectHandleInformation,
}

public enum SYSTEM_INFORMATION_CLASS
{
    SystemBasicInformation = 0,
    SystemPerformanceInformation = 2,
    SystemTimeOfDayInformation = 3,
    SystemProcessInformation = 5,
    SystemProcessorPerformanceInformation = 8,
    SystemHandleInformation = 16, // 0x00000010
    SystemInterruptInformation = 23, // 0x00000017
    SystemExceptionInformation = 33, // 0x00000021
    SystemRegistryQuotaInformation = 37, // 0x00000025
    SystemLookasideInformation = 45, // 0x0000002D
}

public enum NT_STATUS
{
    STATUS_BUFFER_OVERFLOW = -2147483643, // 0x80000005
    STATUS_INFO_LENGTH_MISMATCH = -1073741820, // 0xC0000004
    STATUS_SUCCESS = 0,
}

public enum SpecialWindowHandles
{
    HWND_TOP = 0,
    HWND_BOTTOM = 1,
    HWND_TOPMOST = -1,
    HWND_NOTOPMOST = -2
}

[Flags]
public enum WindowStyles : uint
{
    WS_BORDER = 0x800000,
    WS_CAPTION = 0xc00000,
    WS_CHILD = 0x40000000,
    WS_CLIPCHILDREN = 0x2000000,
    WS_CLIPSIBLINGS = 0x4000000,
    WS_DISABLED = 0x8000000,
    WS_DLGFRAME = 0x400000,
    WS_GROUP = 0x20000,
    WS_HSCROLL = 0x100000,
    WS_MAXIMIZE = 0x1000000,
    WS_MAXIMIZEBOX = 0x10000,
    WS_MINIMIZE = 0x20000000,
    WS_MINIMIZEBOX = 0x20000,
    WS_OVERLAPPED = 0x0,
    WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
    WS_POPUP = 0x80000000u,
    WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
    WS_SIZEFRAME = 0x40000,
    WS_SYSMENU = 0x80000,
    WS_TABSTOP = 0x10000,
    WS_VISIBLE = 0x10000000,
    WS_VSCROLL = 0x200000
}

[Flags]
public enum SetWindowPosFlags : uint
{
    SWP_ASYNCWINDOWPOS = 0x4000,
    SWP_DEFERERASE = 0x2000,
    SWP_DRAWFRAME = 0x0020,
    SWP_FRAMECHANGED = 0x0020,
    SWP_HIDEWINDOW = 0x0080,
    SWP_NOACTIVATE = 0x0010,
    SWP_NOCOPYBITS = 0x0100,
    SWP_NOMOVE = 0x0002,
    SWP_NOOWNERZORDER = 0x0200,
    SWP_NOREDRAW = 0x0008,
    SWP_NOREPOSITION = 0x0200,
    SWP_NOSENDCHANGING = 0x0400,
    SWP_NOSIZE = 0x0001,
    SWP_NOZORDER = 0x0004,
    SWP_SHOWWINDOW = 0x0040
}

public enum GWL
{
    GWL_WNDPROC = (-4),
    GWL_HINSTANCE = (-6),
    GWL_HWNDPARENT = (-8),
    GWL_STYLE = (-16),
    GWL_EXSTYLE = (-20),
    GWL_USERDATA = (-21),
    GWL_ID = (-12)
}
#endregion

#region Structs
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left, Top, Right, Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

    public int X
    {
        get { return Left; }
        set { Right -= (Left - value); Left = value; }
    }

    public int Y
    {
        get { return Top; }
        set { Bottom -= (Top - value); Top = value; }
    }

    public int Height
    {
        get { return Bottom - Top; }
        set { Bottom = value + Top; }
    }

    public int Width
    {
        get { return Right - Left; }
        set { Right = value + Left; }
    }

    public System.Drawing.Point Location
    {
        get { return new System.Drawing.Point(Left, Top); }
        set { X = value.X; Y = value.Y; }
    }

    public System.Drawing.Size Size
    {
        get { return new System.Drawing.Size(Width, Height); }
        set { Width = value.Width; Height = value.Height; }
    }

    public static implicit operator System.Drawing.Rectangle(RECT r)
    {
        return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
    }

    public static implicit operator RECT(System.Drawing.Rectangle r)
    {
        return new RECT(r);
    }

    public static bool operator ==(RECT r1, RECT r2)
    {
        return r1.Equals(r2);
    }

    public static bool operator !=(RECT r1, RECT r2)
    {
        return !r1.Equals(r2);
    }

    public bool Equals(RECT r)
    {
        return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
    }

    public override bool Equals(object obj)
    {
        if (obj is RECT)
            return Equals((RECT)obj);
        else if (obj is System.Drawing.Rectangle)
            return Equals(new RECT((System.Drawing.Rectangle)obj));
        return false;
    }

    public override int GetHashCode()
    {
        return ((System.Drawing.Rectangle)this).GetHashCode();
    }

    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
    }
}
#endregion

public static class SystemHelper
{
    [DllImport("ntdll.dll")]
    public static extern NT_STATUS NtQueryObject(
      [In] IntPtr Handle,
      [In] OBJECT_INFORMATION_CLASS ObjectInformationClass,
      [In] IntPtr ObjectInformation,
      [In] int ObjectInformationLength,
      out int ReturnLength);

    [DllImport("ntdll.dll")]
    public static extern NT_STATUS NtQuerySystemInformation(
      [In] SYSTEM_INFORMATION_CLASS SystemInformationClass,
      [In] IntPtr SystemInformation,
      [In] int SystemInformationLength,
      out int ReturnLength);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle([In] IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(
      [In] IntPtr hSourceProcessHandle,
      [In] IntPtr hSourceHandle,
      [In] IntPtr hTargetProcessHandle,
      out IntPtr lpTargetHandle,
      [In] int dwDesiredAccess,
      [MarshalAs(UnmanagedType.Bool), In] bool bInheritHandle,
      [In] int dwOptions);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(
      [In] IntPtr hSourceProcessHandle,
      [In] IntPtr hSourceHandle,
      [In] IntPtr hTargetProcessHandle,
      [Out] IntPtr lpTargetHandle,
      [In] int dwDesiredAccess,
      [MarshalAs(UnmanagedType.Bool), In] bool bInheritHandle,
      [In] int dwOptions);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(
      [In] int dwDesiredAccess,
      [MarshalAs(UnmanagedType.Bool), In] bool bInheritHandle,
      [In] int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint QueryDosDevice(
      string lpDeviceName,
      StringBuilder lpTargetPath,
      int ucchMax);

    [DllImport("user32.dll")]
    public static extern int SetWindowText(IntPtr hWnd, string text);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    [DllImport("user32.dll")]
    public static extern bool AdjustWindowRect(ref RECT lpRect, uint dwStyle, bool bMenu);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SendMessageA(IntPtr hWnd, int wMsg, int wParam, int lParam);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

}

public static class WindowSizer
{
    public static void SetWindowSize(IntPtr hWND, int width, int height, int pos_X = 0, int pos_Y = 0, bool move = false)
    {
        //Resize window
        RECT clientRect = new RECT(0, 0, width, height);

        int cx = clientRect.Right - clientRect.Left;
        int cy = clientRect.Bottom - clientRect.Top;

        SystemHelper.AdjustWindowRect(ref clientRect, (uint)SystemHelper.GetWindowLongPtr(hWND, (int)GWL.GWL_STYLE), false);
        if (move)
            SystemHelper.SetWindowPos(hWND, new IntPtr((int)SpecialWindowHandles.HWND_TOP), pos_X, pos_Y, cx, cy, (SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSENDCHANGING | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_FRAMECHANGED));
        else
            SystemHelper.SetWindowPos(hWND, new IntPtr((int)SpecialWindowHandles.HWND_TOP), 0, 0, cx, cy, (SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSENDCHANGING | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_FRAMECHANGED));
        //SystemHelper.SendMessageA(hWND, 0x0232, 0, 0); //WM_EXITSIZEMOVE
    }
}
#endif

public sealed class WndHandler : IDisposable
{
    private static WinProc newWndProc = null;
    private static IntPtr oldWndProc = IntPtr.Zero;
    private delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("User32.dll")]
    internal static extern int GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    public static int MinWindowWidth { get; set; } = 100;
    public static int MaxWindowWidth { get; set; } = 1800;
    public static int MinWindowHeight { get; set; } = 100;
    public static int MaxWindowHeight { get; set; } = 1600;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public WndHandler()
    {
        RegisterWindowMinMax();
    }

    public void Dispose()
    {
    }

    private static void RegisterWindowMinMax()
    {
        var hwnd = Api.PluginInterface.UiBuilder.WindowHandlePtr;

        //newWndProc = new WinProc(WndProc);
        //oldWndProc = SetWindowLongPtr(hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
    }


    public static int sw = 300;
    public static int sh = 300;
    /*private unsafe static IntPtr WndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        if (!((int)Msg == 127 || (int)Msg == 132 || (int)Msg == 512 || (int)Msg == 32
|| (int)Msg == 160 || (int)Msg == 641 || (int)Msg == 642 || (int)Msg == 776 || (int)Msg == 32))
            Api.PluginLog.Debug(Msg.ToString());
        switch (Msg)
        {
            case WindowMessage.WM_MOVE:
                break;
            case WindowMessage.WM_MOVING:
            case WindowMessage.WM_PAINT:
            case WindowMessage.WM_SIZE:
            case WindowMessage.WM_WINDOWPOSCHANGED:
            case WindowMessage.WM_WINDOWPOSCHANGING:
                break;
            case WindowMessage.WM_SIZING:
                Api.PluginLog.Debug("size");
                var sizing = Marshal.PtrToStructure<RECT>(lParam);

                //SystemHelper.SetWindowPos(Api.PluginInterface.UiBuilder.WindowHandlePtr, IntPtr.Zero, 0, 0, (sizing.Right - sizing.Left), (sizing.Bottom - sizing.Top), 19U);
                WindowSizer.SetWindowSize(Api.PluginInterface.UiBuilder.WindowHandlePtr, (sizing.Right - sizing.Left), (sizing.Bottom - sizing.Top));
                sw = (sizing.Right - sizing.Left);
                sh = (sizing.Bottom - sizing.Top);
                //Marshal.StructureToPtr(sizing, lParam, true);
                break;
            case WindowMessage.WM_ENTERSIZEMOVE:
            case WindowMessage.WM_EXITSIZEMOVE:
                WindowSizer.SetWindowSize(Api.PluginInterface.UiBuilder.WindowHandlePtr, sw, sh);

                Device.Instance()->Width = (uint)sw;
                Device.Instance()->Height = (uint)sh;
                Device.Instance()->RequestResolutionChange = (byte)1;
                break;
            case WindowMessage.WM_GETMINMAXINFO:
                Api.PluginLog.Debug("MAX");
                var dpi = GetDpiForWindow(hWnd);
                var scalingFactor = (float)dpi / 96;

                var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.x = (int)(MinWindowWidth);
                minMaxInfo.ptMaxTrackSize.x = (int)(MaxWindowWidth * scalingFactor);
                minMaxInfo.ptMinTrackSize.y = (int)(MinWindowHeight);
                minMaxInfo.ptMaxTrackSize.y = (int)(MaxWindowHeight * scalingFactor);

                Marshal.StructureToPtr(minMaxInfo, lParam, true);
                break;

        }
        return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
    }*/

    private static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
    {
        if (IntPtr.Size == 8)
            return SetWindowLongPtr64(hWnd, nIndex, newProc);
        else
            return new IntPtr(SetWindowLong32(hWnd, nIndex, newProc));
    }

    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [Flags]
    private enum WindowLongIndexFlags : int
    {
        GWL_WNDPROC = -4,
    }

    private enum WindowMessage : int
    {
        WM_MOVE= 0x0003,
        WM_SIZE = 0x0005,
        WM_PAINT = 0x000F,
        WM_WINDOWPOSCHANGED = 0x47,
        WM_WINDOWPOSCHANGING = 70,

        WM_GETMINMAXINFO = 0x0024,
        WM_SIZING = 0x0214,
        WM_MOVING = 0x0216,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
    }
}

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow(Hypnotoad plugin) : base(
        "A Wonderful Configuration Window",
        ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
    }

    public void Dispose() 
    {

    }

    WndHandler move;
    public override void Draw()
    {
        if (ImGui.Button("test"))
        {
            move = new WndHandler();
        }
        if (ImGui.Button("st"))
        {
            move.Dispose();
            /*foreach (var f in Api.PartyList)
            {
                if (f == null)
                    continue;
                if (f.Name.TextValue == Api.ClientState.LocalPlayer.Name.TextValue)
                    continue;
                Api.PluginLog.Debug("gotcha");
                Party.Instance.Kick(f.Name.TextValue, (ulong)f.ContentId);
            }`*/


        }
    }

    public unsafe static void TestCommand()
    {
        

        //ActionManager.Instance()->UseAction(ActionType.None, 0);
        //Control.Instance()->CameraManager.Camera->FoV = (float)0.0; //100.0 for liath
        //Control.Instance()->CameraManager.Camera-> = 100000;
        //WindowSizer.SetWindowSize((nint)Device.Instance()->hWnd, 400, 200);
        //Device.Instance()->NewHeight = 200;
        //Device.Instance()->NewWidth = 200;

    }




}