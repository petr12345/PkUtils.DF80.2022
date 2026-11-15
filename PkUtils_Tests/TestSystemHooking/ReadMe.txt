TestSystemHooking demonstrates installing and observing Windows system hooks
from a managed Windows Forms application using
PK.PkUtils.SystemEx.WindowsSystemHookBase.

The tested Win32 hook types include WH_MOUSE, WH_KEYBOARD, WH_KEYBOARD_LL,
and WH_CALLWNDPROCRET. When installation succeeds, the hook callback logs
observed events in the application's text box.

WH_KEYBOARD_LL is a low-level keyboard hook; see WindowsSystemHookKbLL for
implementation details. Global and thread-specific hook installation have
different Win32 requirements, so results depend on the operating environment.

For details, see MainForm.cs and WindowsSystemHookBase.cs.
