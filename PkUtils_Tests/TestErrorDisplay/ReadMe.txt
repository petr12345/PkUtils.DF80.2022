The project TestErrorDisplay demonstrates the usage of an implicit argument values in .NET 4.0
in the interface definition, and consequently in the implementing classes definitions.

The feature is tested with public interface IErrorDisplay.

Conclusion:
* One can define implicit arguments values in the interface methods

* Class implementing the interface can define implicit arguments with exactly the same values, 
  or may define different implicit values, or might not define implicit values at all.

* With the call of method of the interface, it all depends on compiler evaluation 
   "who" is considered to be an implementing entity:
 - If the compiler has type information about the class implementing the interface, 
   it will use implicit values from the definition of that class.
 - If the only information the compiler has about the implementing object is the interface, 
   it will use implicit values from the interface definition.
