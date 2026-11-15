TestExceptionHelper demonstrates how TargetInvocationException wraps an exception
thrown by a method invoked through reflection. The sample compares directly
throwing the inner exception with using ExceptionHelper.PrepareRethrow().

PrepareRethrow() retains the original stack-trace information in an auxiliary
RethrownException attached to the new exception. This is different from simply
rethrowing the caught inner exception, which can lose its original call stack.

For details, see MainForm.cs, ExceptionHelper.cs, and RethrownException.
