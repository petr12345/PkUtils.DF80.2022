The project TestTooltip demonstrates the functionality of the class TipHandler,
and the specialized derived classes ListBoxTipHandler, ComboBoxTipHandler, WindowTitleTipHandler.
The TipHandler itself is derived from and uses the functionality of MessageHookOnHwnd.

For more details see the code of ListBoxTipHandler, ComboBoxTipHandler 
and the base class TipHandler.

Remark: 
In previous version of ComboBoxTipHandler, the tooltips for combo boxes did not work quite well 
if EnableVisualStyles() was called in the program initialization.
To test this behaviors, the UI now has a CheckBox "Use visual styles",
and you can restart the application to see the program that applies changed settings.
