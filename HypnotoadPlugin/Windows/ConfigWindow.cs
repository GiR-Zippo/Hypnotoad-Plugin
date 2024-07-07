using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using HypnotoadPlugin.Offsets;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace HypnotoadPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow(Hypnotoad plugin) : base(
        "A Wonderful Configuration Window",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
    }

    public void Dispose() 
    { 

    }

    public override void Draw()
    {
        if (ImGui.Button("Connect"))
        {
            var player = Api.ClientState?.LocalPlayer;
            if (player != null)
            {
                TestCommand();
                Api.PluginLog.Debug("config");
            }
        }
    }

    static bool running = false;
    public unsafe static void TestCommand()
    {



        //Control.Instance()->CameraManager.Camera->FoV = (float)0.0; //100.0 for liath
        //Control.Instance()->CameraManager.Camera-> = 100000;
        //WindowSizer.SetWindowSize((nint)Device.Instance()->hWnd, 400, 200);
        //Device.Instance()->NewHeight = 200;
        //Device.Instance()->NewWidth = 200;

    }




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
            SystemHelper.SendMessageA(hWND, 0x0232, 0, 0); //WM_EXITSIZEMOVE
        }
    }
#endif
}