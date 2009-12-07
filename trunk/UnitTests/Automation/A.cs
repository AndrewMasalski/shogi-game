using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace UnitTests.Automation
{
  public class A
  {

    public void Key(Key k, MouseButton b, ClickMode m)
    {
      var v = KeyInterop.VirtualKeyFromKey(k);

    }
    public void MouseButton(int counts, Point p, MouseButton b, ClickMode m)
    {
      var input = new Input { Type = InputType.Mouse };
      var screen = Screen.PrimaryScreen.Bounds;
      input.InputUnion.Mouse.Flags = GetMouseFlags(b, m) | MouseFlags.MOUSEEVENTF_ABSOLUTE;
      input.InputUnion.Mouse.Dx = (int)Math.Ceiling(p.X * 65536 / screen.Width);
      input.InputUnion.Mouse.Dy = (int)Math.Ceiling(p.Y * 65536 / screen.Height);
      SendInput((uint)counts, ref input, Marshal.SizeOf(input));
    }

    private MouseFlags GetMouseFlags(MouseButton button, ClickMode mode)
    {
      switch (button)
      {
        case System.Windows.Input.MouseButton.Left:
          switch (mode)
          {
            case ClickMode.Press: return MouseFlags.MOUSEEVENTF_LEFTDOWN;
            case ClickMode.Release: return MouseFlags.MOUSEEVENTF_LEFTUP;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case System.Windows.Input.MouseButton.Middle:
          switch (mode)
          {
            case ClickMode.Press: return MouseFlags.MOUSEEVENTF_MIDDLEDOWN;
            case ClickMode.Release: return MouseFlags.MOUSEEVENTF_MIDDLEUP;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case System.Windows.Input.MouseButton.Right:
          switch (mode)
          {
            case ClickMode.Press: return MouseFlags.MOUSEEVENTF_RIGHTDOWN;
            case ClickMode.Release: return MouseFlags.MOUSEEVENTF_RIGHTUP;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case System.Windows.Input.MouseButton.XButton1:
          switch (mode)
          {
            case ClickMode.Press: return MouseFlags.MOUSEEVENTF_XDOWN | MouseFlags.XBUTTON1;
            case ClickMode.Release: return MouseFlags.MOUSEEVENTF_XUP | MouseFlags.XBUTTON1;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case System.Windows.Input.MouseButton.XButton2:
          switch (mode)
          {
            case ClickMode.Press: return MouseFlags.MOUSEEVENTF_XDOWN | MouseFlags.XBUTTON2;
            case ClickMode.Release: return MouseFlags.MOUSEEVENTF_XUP | MouseFlags.XBUTTON2;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        default: throw new ArgumentOutOfRangeException("button");
      }
    }

    /// <summary>The SendInput function synthesizes keystrokes, 
    /// mouse motions, and button clicks.</summary>
    /// <param name="nInputs">Number of structures in the pInputs array.</param>
    /// <param name="pInputs">Pointer to an array of INPUT structures. 
    /// Each structure represents an event to be inserted into the keyboard or 
    /// mouse input stream.</param>
    /// <param name="cbSize">Specifies the size, in bytes, of an INPUT structure. 
    /// If cbSize is not the size of an INPUT structure, the function fails.</param>
    /// <returns>
    /// <para>The function returns the number of events that it 
    /// successfully inserted into the keyboard or mouse input stream. 
    /// If the function returns zero, the input was already blocked by another
    /// thread. To get extended error information, call GetLastError.</para>
    /// <para><b>Microsoft Windows Vista.</b> This function fails when it is blocked 
    /// by User Interface Privilege Isolation (UIPI). Note that neither
    /// GetLastError nor the return value will indicate the failure was caused 
    /// by UIPI blocking.</para></returns>
    /// <remarks><para><b>Microsoft Windows Vista.</b>
    /// This function is subject to UIPI. Applications are permitted to 
    /// inject input only into applications that are at an equal or lesser 
    /// integrity level.</para>
    /// 
    /// <para>The SendInput function inserts the events in the INPUT 
    /// structures serially into the keyboard or mouse input stream.
    /// These events are not interspersed with other keyboard or mouse 
    /// input events inserted either by the user (with the keyboard or mouse)
    /// or by calls to keybd_event, mouse_event, or other calls to SendInput.
    /// </para>
    /// 
    /// <para> This function does not reset the keyboard's current state.
    /// Any keys that are already pressed when the function is called might 
    /// interfere with the events that this function generates. 
    /// To avoid this problem, check the keyboard's state with the 
    /// GetAsyncKeyState function and correct as necessary.</para>
    /// </remarks>
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms646310(VS.85).aspx"/>
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, ref Input pInputs, int cbSize);

    /// <summary>The MOUSEINPUT structure contains information about a simulated mouse event.</summary>
    /// <remarks>
    /// <para>If the mouse has moved, indicated by MOUSEEVENTF_MOVE, 
    /// dxand dy specify information about that movement. 
    /// The information is specified as absolute or relative integer values.</para>
    /// 
    /// <para>If MOUSEEVENTF_ABSOLUTE value is specified, dx and dy contain 
    /// normalized absolute coordinates between 0 and 65,535. The event procedure 
    /// maps these coordinates onto the display surface. Coordinate (0,0) maps 
    /// onto the upper-left corner of the display surface; coordinate 
    /// (65535,65535) maps onto the lower-right corner. In a multimonitor
    /// system, the coordinates map to the primary monitor.</para>
    ///
    /// <para>Windows 2000/XP: If MOUSEEVENTF_VIRTUALDESK is specified, 
    /// the coordinates map to the entire virtual desktop.</para>
    ///
    /// <para>If the MOUSEEVENTF_ABSOLUTE value is not specified, dxand dy 
    /// specify movement relative to the previous mouse event (the last 
    /// reported position). Positive values mean the mouse moved right 
    /// (or down); negative values mean the mouse moved left (or up).</para>
    ///
    /// <para>Relative mouse motion is subject to the effects of the mouse
    /// speed and the two-mouse threshold values. A user sets these three 
    /// values with the Pointer Speed slider of the Control Panel's Mouse 
    /// Properties sheet. You can obtain and set these values using the 
    /// SystemParametersInfo function.</para>
    ///
    /// <para>The system applies two tests to the specified relative mouse 
    /// movement. If the specified distance along either the x or y axis 
    /// is greater than the first mouse threshold value, and the mouse speed 
    /// is not zero, the system doubles the distance. If the specified 
    /// distance along either the x or y axis is greater than the second 
    /// mouse threshold value, and the mouse speed is equal to two, the 
    /// system doubles the distance that resulted from applying the first 
    /// threshold test. It is thus possible for the system to multiply 
    /// specified relative mouse movement along the x or y axis by up to 
    /// four times.</para>
    /// 
    /// <para>It's not obvious unless you read this entire page, 
    /// but dx and dy are not pixel values when using MOUSEEVENTF_ABSOLUTE. 
    /// To convert from pixels, do this:</para>
    /// 
    /// <para>dx = ceiling(x*65536/ScreenWidth)</para>
    /// <para>dy = ceiling(y*65536/ScreenHeight)</para>
    /// 
    /// <para>In a multi-monitor setup, ScreenWidth and ScreenHeight are the
    /// width and height of the primary monitor.</para>
    ///
    /// <para>It has only been tested for positive values of x and y, 
    /// and only on Windows XP SP3.</para> 
    /// </remarks>
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx"/>
    [StructLayout(LayoutKind.Explicit)]
    private struct MouseInput
    {
      /// <summary><para>Specifies the absolute position of the mouse, or 
      ///  the amount of motion since the last mouse event was generated, 
      ///  depending on the value of the Flags member.</para>
      /// <para>Absolute data is specified as the x coordinate of the mouse;
      ///  relative data is specified as the number of pixels moved.</para>
      /// </summary>
      public int Dx;
      /// <summary><para>Specifies the absolute position of the mouse, or 
      ///  the amount of motion since the last mouse event was generated, 
      ///  depending on the value of the Flags member.</para>
      /// <para>Absolute data is specified as the x coordinate of the mouse;
      ///  relative data is specified as the number of pixels moved.</para>
      /// </summary>
      public int Dy;
      /// <summary><para>If Flags contains MOUSEEVENTF_WHEEL, then mouseData 
      /// specifies the amount of wheel movement. 
      /// A positive value indicates that the wheel was rotated forward, 
      /// away from the user; a negative value indicates that the wheel 
      /// was rotated backward, toward the user. One wheel click is defined 
      /// as WHEEL_DELTA, which is 120.</para>
      /// </summary>
      /// <remarks>
      /// <para><b>Windows Vista:</b> If Flags contains MOUSEEVENTF_HWHEEL, 
      /// then dwData specifies the amount of wheel movement. 
      /// A positive value indicates that the wheel was rotated to the right; 
      /// a negative value indicates that the wheel was rotated to the left. 
      /// One wheel click is defined as WHEEL_DELTA, which is 120.
      /// </para>
      /// <para><b>Windows 2000/XP:</b> IfdwFlags does not contain MOUSEEVENTF_WHEEL,
      ///  MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero.
      /// </para>
      /// <para>If Flags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, 
      /// then mouseData specifies which X buttons were pressed or released.
      ///  This value may be any combination of the following flags.
      /// </para>
      /// <para><b>XBUTTON1</b>
      /// Set if the first X button is pressed or released.</para>
      /// <para><b>XBUTTON2</b>
      /// Set if the second X button is pressed or released.</para>
      /// </remarks>
      public uint MouseData;
      /// <summary><para>A set of bit flags that specify various aspects of mouse 
      /// motion and button clicks. The bits in this member can be any reasonable 
      /// combination of the following values.</para>
      /// 
      /// <para>The bit flags that specify mouse button status are set to indicate
      /// changes in status, not ongoing conditions. For example, if the left
      /// mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set
      /// when the left button is first pressed, but not for subsequent motions.
      /// Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first
      /// released.</para>
      /// 
      /// <para>You cannot specify both the MOUSEEVENTF_WHEEL flag and either
      /// MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the
      /// Flags parameter, because they both require use of the mouseData 
      /// field.</para>  
      /// </summary>
      public MouseFlags Flags;
      /// <summary>Time stamp for the event, in milliseconds. 
      /// If this parameter is 0, the system will provide its own Time stamp.
      /// </summary>
      public uint Time;
      /// <summary>Specifies an additional value associated with the mouse event.
      /// An application calls GetMessageExtraInfo to obtain this extra 
      /// information.  
      /// </summary>
      public IntPtr ExtraInfo;
    }

    [Flags]
    private enum MouseFlags
    {
      XBUTTON1 = 0x0001,
      XBUTTON2 = 0x0002,
      /// <summary>Specifies that movement occurred.</summary>
      MOUSEEVENTF_MOVE = 0x0001,
      /// <summary>Specifies that the left button was pressed.</summary>
      MOUSEEVENTF_LEFTDOWN = 0x0002,
      /// <summary>Specifies that the left button was released.</summary>
      MOUSEEVENTF_LEFTUP = 0x0004,
      /// <summary>Specifies that the right button was pressed. </summary>
      MOUSEEVENTF_RIGHTDOWN = 0x0008,
      /// <summary>Specifies that the right button was released.</summary>
      MOUSEEVENTF_RIGHTUP = 0x0010,
      /// <summary> </summary>
      MOUSEEVENTF_MIDDLEDOWN = 0x0020,
      /// <summary>Specifies that the middle button was pressed.</summary>
      MOUSEEVENTF_MIDDLEUP = 0x0040,
      /// <summary>Windows 2000/XP: Specifies that an X button was pressed.</summary>
      MOUSEEVENTF_XDOWN = 0x0080,
      /// <summary>Windows 2000/XP: Specifies that an X button was released.</summary>
      MOUSEEVENTF_XUP = 0x0100,
      /// <summary>Windows NT/2000/XP: Specifies that the wheel was moved, 
      /// if the mouse has a wheel. The amount of movement is specified in 
      /// mouseData.</summary>
      MOUSEEVENTF_WHEEL = 0x0800,
      /// <summary>Windows 2000/XP: Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.</summary>
      MOUSEEVENTF_VIRTUALDESK = 0x4000,
      /// <summary> Specifies that the dx and dy members contain normalized 
      /// absolute coordinates. If the flag is not set, dxand dy contain 
      /// relative data (the change in position since the last reported position).
      /// This flag can be set, or not set, regardless of what kind of
      /// mouse or other pointing device, if any, is connected to the system.
      /// For further information about relative mouse motion, see the following
      /// Remarks section.
      /// </summary>
      MOUSEEVENTF_ABSOLUTE = 0x8000,
      /// <summary>Windows Vista: Specifies that WM_MOUSEMOVE messages will not 
      /// be coalesced. The default behavior is to coalesce WM_MOUSEMOVE
      /// messages. </summary>
      MOUSEEVENTF_MOVE_NOCOALESCE,
      /// <summary>
      /// Windows Vista: Specifies that the wheel was moved horizontally, 
      /// if the mouse has a wheel. The amount of movement is specified in 
      /// mouseData.
      /// </summary>
      MOUSEEVENTF_HWHEEL,
    }

    /// <summary>The KEYBDINPUT structure contains information about a 
    /// simulated keyboard event.</summary>
    /// <remarks>
    /// <para>Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input 
    /// methods—such as handwriting recognition or voice recognition—as 
    /// if it were text input by using the KEYEVENTF_UNICODE flag. 
    /// If KEYEVENTF_UNICODE is specified, SendInput sends a WM_KEYDOWN or 
    /// WM_KEYUP message to the foreground thread's message queue with wParam 
    /// equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, 
    /// passing the message to TranslateMessage posts a WM_CHAR message with 
    /// the Unicode character originally specified by Scan. This Unicode 
    /// character will automatically be converted to the appropriate ANSI 
    /// value if it is posted to an ANSI window.</para>
    /// 
    /// <para>Windows 2000/XP: Set the KEYEVENTF_SCANCODE flag to define keyboard 
    /// input in terms of the scan code. This is useful to simulate a physical
    /// keystroke regardless of which keyboard is currently being used. 
    /// The virtual key value of a key may alter depending on the current 
    /// keyboard layout or what other keys were pressed, but the scan code
    /// will always be the same.</para> 
    /// </remarks>
    /// <see cref="http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx"/>
    [StructLayout(LayoutKind.Explicit)]
    private struct KeyboardInput
    {
      /// <summary>Specifies a virtual-key code. 
      /// The code must be a value in the range 1 to 254. 
      /// The Winuser.h header file provides macro definitions (VK_*) 
      /// for each value. If the Flags member specifies 
      /// KEYEVENTF_UNICODE, VirtualKeyCode must be 0.
      /// </summary>
      public ushort VirtualKeyCode;
      /// <summary>Specifies a hardware scan code for the key. 
      /// If Flags specifies KEYEVENTF_UNICODE, Scan specifies
      /// a Unicode character which is to be sent to the foreground 
      /// application.
      /// </summary>
      public ushort Scan;
      /// <summary>Specifies various aspects of a keystroke. 
      /// This member can be certain combinations of the following values.
      /// </summary>
      public uint Flags;
      /// <summary>Time stamp for the event, in milliseconds. 
      /// If this parameter is 0, the system will provide its own Time stamp.
      /// </summary>
      public uint Time;
      /// <summary>Specifies an additional value associated with the mouse event.
      /// An application calls GetMessageExtraInfo to obtain this extra 
      /// information.  
      /// </summary>
      public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct MouseKeyboardHardwareInput
    {
      [FieldOffset(0)]
      public MouseInput Mouse;

      [FieldOffset(0)]
      public KeyboardInput Keyboard;
    }

    /// <summary>For use with the Input struct, see SendInput for an example</summary>
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx"/>
    [StructLayout(LayoutKind.Explicit)]
    private struct Input
    {
      public InputType Type;
      public MouseKeyboardHardwareInput InputUnion;
    }
    public enum InputType
    {
      Mouse,
      Keyboard,
    }
    /*
    private enum VK : ushort
    {
      //
      // Virtual Keys, Standard Set
      //
      VK_LBUTTON = 0x01,
      VK_RBUTTON = 0x02,
      VK_CANCEL = 0x03,
      VK_MBUTTON = 0x04,    // NOT contiguous with L & RBUTTON

      VK_XBUTTON1 = 0x05,    // NOT contiguous with L & RBUTTON
      VK_XBUTTON2 = 0x06,    // NOT contiguous with L & RBUTTON

      // 0x07 : unassigned

      VK_BACK = 0x08,
      VK_TAB = 0x09,

      // 0x0A - 0x0B : reserved

      VK_CLEAR = 0x0C,
      VK_RETURN = 0x0D,

      VK_SHIFT = 0x10,
      VK_CONTROL = 0x11,
      VK_MENU = 0x12,
      VK_PAUSE = 0x13,
      VK_CAPITAL = 0x14,

      VK_KANA = 0x15,
      VK_HANGEUL = 0x15,  // old name - should be here for compatibility
      VK_HANGUL = 0x15,
      VK_JUNJA = 0x17,
      VK_FINAL = 0x18,
      VK_HANJA = 0x19,
      VK_KANJI = 0x19,

      VK_ESCAPE = 0x1B,

      VK_CONVERT = 0x1C,
      VK_NONCONVERT = 0x1D,
      VK_ACCEPT = 0x1E,
      VK_MODECHANGE = 0x1F,

      VK_SPACE = 0x20,
      VK_PRIOR = 0x21,
      VK_NEXT = 0x22,
      VK_END = 0x23,
      VK_HOME = 0x24,
      VK_LEFT = 0x25,
      VK_UP = 0x26,
      VK_RIGHT = 0x27,
      VK_DOWN = 0x28,
      VK_SELECT = 0x29,
      VK_PRINT = 0x2A,
      VK_EXECUTE = 0x2B,
      VK_SNAPSHOT = 0x2C,
      VK_INSERT = 0x2D,
      VK_DELETE = 0x2E,
      VK_HELP = 0x2F,

      //
      // VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
      // 0x40 : unassigned
      // VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
      //

      VK_LWIN = 0x5B,
      VK_RWIN = 0x5C,
      VK_APPS = 0x5D,

      //
      // 0x5E : reserved
      //

      VK_SLEEP = 0x5F,

      VK_NUMPAD0 = 0x60,
      VK_NUMPAD1 = 0x61,
      VK_NUMPAD2 = 0x62,
      VK_NUMPAD3 = 0x63,
      VK_NUMPAD4 = 0x64,
      VK_NUMPAD5 = 0x65,
      VK_NUMPAD6 = 0x66,
      VK_NUMPAD7 = 0x67,
      VK_NUMPAD8 = 0x68,
      VK_NUMPAD9 = 0x69,
      VK_MULTIPLY = 0x6A,
      VK_ADD = 0x6B,
      VK_SEPARATOR = 0x6C,
      VK_SUBTRACT = 0x6D,
      VK_DECIMAL = 0x6E,
      VK_DIVIDE = 0x6F,
      VK_F1 = 0x70,
      VK_F2 = 0x71,
      VK_F3 = 0x72,
      VK_F4 = 0x73,
      VK_F5 = 0x74,
      VK_F6 = 0x75,
      VK_F7 = 0x76,
      VK_F8 = 0x77,
      VK_F9 = 0x78,
      VK_F10 = 0x79,
      VK_F11 = 0x7A,
      VK_F12 = 0x7B,
      VK_F13 = 0x7C,
      VK_F14 = 0x7D,
      VK_F15 = 0x7E,
      VK_F16 = 0x7F,
      VK_F17 = 0x80,
      VK_F18 = 0x81,
      VK_F19 = 0x82,
      VK_F20 = 0x83,
      VK_F21 = 0x84,
      VK_F22 = 0x85,
      VK_F23 = 0x86,
      VK_F24 = 0x87,

      //
      // 0x88 - 0x8F : unassigned
      //

      VK_NUMLOCK = 0x90,
      VK_SCROLL = 0x91,

      //
      // VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
      // Used only as parameters to GetAsyncKeyState() and GetKeyState().
      // No other API or message will distinguish left and right keys in this way.
      //
      VK_LSHIFT = 0xA0,
      VK_RSHIFT = 0xA1,
      VK_LCONTROL = 0xA2,
      VK_RCONTROL = 0xA3,
      VK_LMENU = 0xA4,
      VK_RMENU = 0xA5,

      VK_BROWSER_BACK = 0xA6,
      VK_BROWSER_FORWARD = 0xA7,
      VK_BROWSER_REFRESH = 0xA8,
      VK_BROWSER_STOP = 0xA9,
      VK_BROWSER_SEARCH = 0xAA,
      VK_BROWSER_FAVORITES = 0xAB,
      VK_BROWSER_HOME = 0xAC,

      VK_VOLUME_MUTE = 0xAD,
      VK_VOLUME_DOWN = 0xAE,
      VK_VOLUME_UP = 0xAF,
      VK_MEDIA_NEXT_TRACK = 0xB0,
      VK_MEDIA_PREV_TRACK = 0xB1,
      VK_MEDIA_STOP = 0xB2,
      VK_MEDIA_PLAY_PAUSE = 0xB3,
      VK_LAUNCH_MAIL = 0xB4,
      VK_LAUNCH_MEDIA_SELECT = 0xB5,
      VK_LAUNCH_APP1 = 0xB6,
      VK_LAUNCH_APP2 = 0xB7,

      //
      // 0xB8 - 0xB9 : reserved
      //

      VK_OEM_1 = 0xBA,   // ';:' for US
      VK_OEM_PLUS = 0xBB,   // '+' any country
      VK_OEM_COMMA = 0xBC,   // ',' any country
      VK_OEM_MINUS = 0xBD,   // '-' any country
      VK_OEM_PERIOD = 0xBE,   // '.' any country
      VK_OEM_2 = 0xBF,   // '/?' for US
      VK_OEM_3 = 0xC0,   // '`~' for US

      //
      // 0xC1 - 0xD7 : reserved
      //

      //
      // 0xD8 - 0xDA : unassigned
      //

      VK_OEM_4 = 0xDB,  //  '[{' for US
      VK_OEM_5 = 0xDC,  //  '\|' for US
      VK_OEM_6 = 0xDD,  //  ']}' for US
      VK_OEM_7 = 0xDE,  //  ''"' for US
      VK_OEM_8 = 0xDF

      //
      // 0xE0 : reserved
      //
    }
    */
    private enum KeyboardFlags
    {
      /// <summary>If specified, the scan code was preceded by a
      /// prefix byte that has the value 0xE0 (224).</summary>
      KEYEVENTF_EXTENDEDKEY = 0x0001,
      /// <summary>If specified, the key is being released. 
      /// If not specified, the key is being pressed.</summary>
      KEYEVENTF_KEYUP = 0x0002,
      /// <summary>Windows 2000/XP: If specified, the system synthesizes a 
      /// VK_PACKET keystroke. The VirtualKeyCode parameter must be zero. 
      /// This flag can only be combined with the KEYEVENTF_KEYUP flag. 
      /// For more information, see the Remarks section.
      /// </summary>
      KEYEVENTF_UNICODE = 0x0004,
      /// <summary>If specified, Scan identifies the key and VirtualKeyCode is ignored.</summary>
      KEYEVENTF_SCANCODE = 0x0008,
    }

  }
}