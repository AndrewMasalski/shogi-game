using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Yasc.Utils.Automation
{
  public class SendInputInterop
  {
    public static void SpinMouseWheel(int distance, WheelType wheelType)
    {
      var input = new Input { Type = InputType.Mouse };
      input.InputUnion.Mouse.Flags = wheelType == WheelType.Vertical ? 
                                                                       MouseFlags.Wheel : MouseFlags.HorizontalWheel;
      input.InputUnion.Mouse.MouseData = (uint)distance*WheelDelta;
      SendInput(1, ref input, Marshal.SizeOf(input));
    }
    public static void MoveMouseCursor(Point point)
    {
      var input = new Input { Type = InputType.Mouse };
      var screen = Screen.PrimaryScreen.Bounds;
      input.InputUnion.Mouse.Flags = MouseFlags.Absolute | MouseFlags.Move;
      input.InputUnion.Mouse.Dx = (int)Math.Ceiling(point.X * 65536 / screen.Width);
      input.InputUnion.Mouse.Dy = (int)Math.Ceiling(point.Y * 65536 / screen.Height);
      SendInput(1, ref input, Marshal.SizeOf(input));
    }
    public static void ToggleMouseButton(int times, MouseButton button, ToggleMode mode)
    {
      var input = new Input { Type = InputType.Mouse };
      input.InputUnion.Mouse.MouseData = GetMouseData(button);
      input.InputUnion.Mouse.Flags = GetMouseFlags(button, mode) | MouseFlags.Absolute;
      SendInput((uint)times, ref input, Marshal.SizeOf(input));
    }
    public static void ToggleKeyboardButton(int times, Key key, ToggleMode mode)
    {
      var input = new Input { Type = InputType.Keyboard };
      input.InputUnion.Keyboard.VirtualKeyCode = (ushort)KeyInterop.VirtualKeyFromKey(key);
      if (mode == ToggleMode.Release)
        input.InputUnion.Keyboard.Flags |= KeyboardFlags.KeyUp;
      SendInput((uint)times, ref input, Marshal.SizeOf(input));
    }
    public static void ToggleKeyboardButton(int times, char c, ToggleMode mode)
    {
      var input = new Input { Type = InputType.Keyboard };
      input.InputUnion.Keyboard.Flags = KeyboardFlags.Unicode;
      if (mode == ToggleMode.Release)
        input.InputUnion.Keyboard.Flags |= KeyboardFlags.KeyUp;
      input.InputUnion.Keyboard.Scan = c;
      SendInput((uint)times, ref input, Marshal.SizeOf(input));
    }

    private static uint GetMouseData(MouseButton button)
    {
      switch (button)
      {
        case MouseButton.Left:
        case MouseButton.Middle:
        case MouseButton.Right:
          return 0;
        case MouseButton.XButton1:
          return (uint)XButtons.XButton1;
        case MouseButton.XButton2:
          return (uint)XButtons.XButton2;
        default: throw new ArgumentOutOfRangeException("button");
      }
    }
    private static MouseFlags GetMouseFlags(MouseButton button, ToggleMode mode)
    {
      switch (button)
      {
        case MouseButton.Left:
          switch (mode)
          {
            case ToggleMode.Press: return MouseFlags.LeftDown;
            case ToggleMode.Release: return MouseFlags.LeftUp;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case MouseButton.Middle:
          switch (mode)
          {
            case ToggleMode.Press: return MouseFlags.MiddleDown;
            case ToggleMode.Release: return MouseFlags.MiddleUp;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case MouseButton.Right:
          switch (mode)
          {
            case ToggleMode.Press: return MouseFlags.RightDown;
            case ToggleMode.Release: return MouseFlags.RightUp;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case MouseButton.XButton1:
          switch (mode)
          {
            case ToggleMode.Press: return MouseFlags.XDown;
            case ToggleMode.Release: return MouseFlags.XUp;
            default: throw new ArgumentOutOfRangeException("mode");
          }
        case MouseButton.XButton2:
          switch (mode)
          {
            case ToggleMode.Press: return MouseFlags.XDown;
            case ToggleMode.Release: return MouseFlags.XUp;
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
    private static extern uint SendInput(uint nInputs, ref Input pInputs, int cbSize);

    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local

    /// <summary>The MOUSEINPUT structure contains information about a simulated mouse event.</summary>
    /// <remarks>
    /// <para>If the mouse has moved, indicated by <see cref="MouseFlags.Move"/>, 
    /// dxand dy specify information about that movement. 
    /// The information is specified as absolute or relative integer values.</para>
    /// 
    /// <para>If <see cref="MouseFlags.Absolute"/> value is specified, dx and dy contain 
    /// normalized absolute coordinates between 0 and 65,535. The event procedure 
    /// maps these coordinates onto the display surface. Coordinate (0,0) maps 
    /// onto the upper-left corner of the display surface; coordinate 
    /// (65535,65535) maps onto the lower-right corner. In a multimonitor
    /// system, the coordinates map to the primary monitor.</para>
    ///
    /// <para>Windows 2000/XP: If <see cref="MouseFlags.Virtualdesk"/> is specified, 
    /// the coordinates map to the entire virtual desktop.</para>
    ///
    /// <para>If the <see cref="MouseFlags.Absolute"/> value is not specified, dxand dy 
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
    /// but dx and dy are not pixel values when using <see cref="MouseFlags.Absolute"/>. 
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
    [StructLayout(LayoutKind.Sequential)]
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
      /// <summary><para>If Flags contains <see cref="MouseFlags.Wheel"/>, then mouseData 
      /// specifies the amount of wheel movement. 
      /// A positive value indicates that the wheel was rotated forward, 
      /// away from the user; a negative value indicates that the wheel 
      /// was rotated backward, toward the user. One wheel click is defined 
      /// as <see cref="SendInputInterop.WheelDelta"/>, which is 120.</para>
      /// </summary>
      /// <remarks>
      /// <para><b>Windows Vista:</b> If Flags contains <see cref="MouseFlags.HorizontalWheel"/>, 
      /// then dwData specifies the amount of wheel movement. 
      /// A positive value indicates that the wheel was rotated to the right; 
      /// a negative value indicates that the wheel was rotated to the left. 
      /// One wheel click is defined as <see cref="SendInputInterop.WheelDelta"/>, which is 120.
      /// </para>
      /// <para><b>Windows 2000/XP:</b> IfdwFlags does not contain <see cref="MouseFlags.Wheel"/>,
      ///  <see cref="MouseFlags.XDown"/>, or <see cref="MouseFlags.XUp"/>, then mouseData should be zero.
      /// </para>
      /// <para>If Flags contains <see cref="MouseFlags.XDown"/> or <see cref="MouseFlags.XUp"/>, 
      /// then mouseData specifies which X buttons were pressed or released.
      ///  This value may be any combination of the following flags.
      /// </para>
      /// </remarks>
      public uint MouseData;
      /// <summary><para>A set of bit flags that specify various aspects of mouse 
      /// motion and button clicks. The bits in this member can be any reasonable 
      /// combination of the following values.</para>
      /// 
      /// <para>The bit flags that specify mouse button status are set to indicate
      /// changes in status, not ongoing conditions. For example, if the left
      /// mouse button is pressed and held down, <see cref="MouseFlags.LeftDown"/> is set
      /// when the left button is first pressed, but not for subsequent motions.
      /// Similarly, <see cref="MouseFlags.LeftUp"/> is set only when the button is first
      /// released.</para>
      /// 
      /// <para>You cannot specify both the <see cref="MouseFlags.Wheel"/> flag and either
      /// <see cref="MouseFlags.XDown"/> or <see cref="MouseFlags.XUp"/> flags simultaneously in the
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

    /// <summary>
    /// Goes to the <see cref="MouseInput.MouseData"/> field
    /// </summary>
    [Flags]
    private enum XButtons
    {
      /// <summary>Set if the first X button is pressed or released. </summary>
      XButton1 = 0x0001,
      /// <summary>Set if the second X button is pressed or released. </summary>
      XButton2 = 0x0002,
    }

    [Flags]
    private enum MouseFlags
    {
      /// <summary>Specifies that movement occurred.</summary>
      Move = 0x0001,
      /// <summary>Specifies that the left button was pressed.</summary>
      LeftDown = 0x0002,
      /// <summary>Specifies that the left button was released.</summary>
      LeftUp = 0x0004,
      /// <summary>Specifies that the right button was pressed. </summary>
      RightDown = 0x0008,
      /// <summary>Specifies that the right button was released.</summary>
      RightUp = 0x0010,
      /// <summary> </summary>
      MiddleDown = 0x0020,
      /// <summary>Specifies that the middle button was pressed.</summary>
      MiddleUp = 0x0040,
      /// <summary>Windows 2000/XP: Specifies that an X button was pressed.</summary>
      XDown = 0x0080,
      /// <summary>Windows 2000/XP: Specifies that an X button was released.</summary>
      XUp = 0x0100,
      /// <summary>Windows NT/2000/XP: Specifies that the wheel was moved, 
      /// if the mouse has a wheel. The amount of movement is specified in 
      /// mouseData.</summary>
      Wheel = 0x0800,
      /// <summary>Windows 2000/XP: Maps coordinates to the entire desktop. 
      /// Must be used with <see cref="Absolute"/>.</summary>
      Virtualdesk = 0x4000,
      /// <summary> Specifies that the dx and dy members contain normalized 
      /// absolute coordinates. If the flag is not set, dxand dy contain 
      /// relative data (the change in position since the last reported position).
      /// This flag can be set, or not set, regardless of what kind of
      /// mouse or other pointing device, if any, is connected to the system.
      /// For further information about relative mouse motion, see the following
      /// Remarks section.
      /// </summary>
      Absolute = 0x8000,
      /// <summary>Windows Vista: Specifies that WM_MOUSEMOVE messages will not 
      /// be coalesced. The default behavior is to coalesce WM_MOUSEMOVE
      /// messages. </summary>
      MoveNoCoalesce = 0x2000,
      /// <summary>
      /// Windows Vista: Specifies that the wheel was moved horizontally, 
      /// if the mouse has a wheel. The amount of movement is specified in 
      /// mouseData.
      /// </summary>
      HorizontalWheel = 0x01000,
    }

    /// <summary>The KEYBDINPUT structure contains information about a 
    /// simulated keyboard event.</summary>
    /// <remarks>
    /// <para>Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input 
    /// methods—such as handwriting recognition or voice recognition—as 
    /// if it were text input by using the <see cref="KeyboardFlags.Unicode"/> flag. 
    /// If <see cref="KeyboardFlags.Unicode"/> is specified, SendInput sends a WM_KEYDOWN or 
    /// WM_KEYUP message to the foreground thread's message queue with wParam 
    /// equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, 
    /// passing the message to TranslateMessage posts a WM_CHAR message with 
    /// the Unicode character originally specified by Scan. This Unicode 
    /// character will automatically be converted to the appropriate ANSI 
    /// value if it is posted to an ANSI window.</para>
    /// 
    /// <para>Windows 2000/XP: Set the <see cref="KeyboardFlags.ScanCode"/> flag to define keyboard 
    /// input in terms of the scan code. This is useful to simulate a physical
    /// keystroke regardless of which keyboard is currently being used. 
    /// The virtual key value of a key may alter depending on the current 
    /// keyboard layout or what other keys were pressed, but the scan code
    /// will always be the same.</para> 
    /// </remarks>
    /// <see cref="http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx"/>
    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardInput
    {
      /// <summary>Specifies a virtual-key code. 
      /// The code must be a value in the range 1 to 254. 
      /// If the Flags member specifies 
      /// <see cref="KeyboardFlags.Unicode"/>, <see cref="VirtualKeyCode"/> must be 0.
      /// </summary>
      public ushort VirtualKeyCode;
      /// <summary>Specifies a hardware scan code for the key. 
      /// If Flags specifies <see cref="KeyboardFlags.Unicode"/>, Scan specifies
      /// a Unicode character which is to be sent to the foreground 
      /// application.
      /// </summary>
      public ushort Scan;
      /// <summary>Specifies various aspects of a keystroke. 
      /// This member can be certain combinations of the following values.
      /// </summary>
      public KeyboardFlags Flags;
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
    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
      public InputType Type;
      public MouseKeyboardHardwareInput InputUnion;
    }
    private enum InputType
    {
      Mouse,
      Keyboard,
    }
    [Flags]
    private enum KeyboardFlags
    {
      /// <summary>If specified, the scan code was preceded by a
      /// prefix byte that has the value 0xE0 (224).</summary>
      ExtendedKey = 0x0001,
      /// <summary>If specified, the key is being released. 
      /// If not specified, the key is being pressed.</summary>
      KeyUp = 0x0002,
      /// <summary>Windows 2000/XP: If specified, the system synthesizes a 
      /// VK_PACKET keystroke. The VirtualKeyCode parameter must be zero. 
      /// This flag can only be combined with the <see cref="KeyUp"/> flag. 
      /// For more information, see the Remarks section.
      /// </summary>
      Unicode = 0x0004,
      /// <summary>If specified, Scan identifies the key and <see cref="KeyboardInput.VirtualKeyCode"/> is ignored.</summary>
      ScanCode = 0x0008,
    }
    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore FieldCanBeMadeReadOnly.Local
    // ReSharper restore UnusedMember.Local

    private const int WheelDelta = 120;

  }
}