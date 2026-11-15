TestErrorDisplay demonstrates how C# optional parameter default values are
selected when an interface method is called through either a concrete class
reference or an interface reference.

The experiment uses IErrorDisplay and its three implementations:
  DisplayA - repeats the same optional defaults as the interface.
  DisplayB - specifies different defaults and makes one parameter required.
  DisplayC - specifies no optional defaults on its implementation methods.

Conclusion:
Optional argument default values are substituted by the compiler at the call
site, using the method declaration selected from the compile-time type of the
reference. When the reference is typed as IErrorDisplay, the defaults from the
interface declaration apply; when it is typed as a concrete class, the class's
own declaration determines which arguments may be omitted and which defaults
apply. This does not depend on the object's runtime type.

See IErrorDisplay.cs, DisplayA.cs, DisplayB.cs, DisplayC.cs, and MainForm.cs.
