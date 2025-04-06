The project TestMsgBoxObserver demonstrates a usage of a class MsgBoxObserver 
from the namespace PK.PkUtils.UI.Utils.

Th class utilizes its base class WindowsSystemHook, 
"watches" the system for any system MessageBox creation, activation and consequent
destroying of it. 
Raison d'être: unfortunately, there is no such functionality in WinForms.

For more details, see the code of MainForm.cs and class MsgBoxObserver itself.