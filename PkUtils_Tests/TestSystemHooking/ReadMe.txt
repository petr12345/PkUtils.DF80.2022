The project TestSystemHooking tests creating a system-wide hooks in a managed application,
with the help of class PK.PkUtils.SystemEx.WindowsSystemHookBase.

The hooks that the application attempts to create are following types:
Win32.HookType.WH_MOUSE, 
Win32.HookType.WH_KEYBOARD
Win32.HookType.WH_KEYBOARD_LL
Win32.HookType.WH_CALLWNDPROCRET

If the hook installation succeeded, the hook method writes ( logs ) a text
to the textbox in a main form.

Note the difference of WH_KEYBOARD_LL hook, which MUST be installed as a global hook
( see the code of WindowsSystemHookKbLL for details ).

For more details, see the code of MainForm.cs creating the hooks, 
and the class WindowsSystemHookBase itself.