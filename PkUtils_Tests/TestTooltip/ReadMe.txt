TestTooltip demonstrates TipHandler and its derived types:
ListBoxTipHandler, ComboBoxTipHandler, and WindowTitleTipHandler.
TipHandler inherits from WindowMessageHook and supplies message-hook-based
tooltip behavior for Windows Forms controls.

For details, see the derived handlers and TipHandler.cs in PkUtils/UI/TipHandlers.

The sample also includes a "Use visual styles" checkbox. It allows testing
tooltips with and without visual styles after restarting the application,
which was useful when diagnosing earlier ComboBoxTipHandler behavior.
