TestCloning demonstrates deep cloning of object graphs, including graphs with
cyclic references, using CloneHelperBinary and CloneHelperByContract.

Despite its historical name, CloneHelperBinary now delegates to JSON-based
serialization (CloneHelperJson); it no longer uses BinaryFormatter.
CloneHelperByContract uses DataContractSerializer and requires suitable data
contracts. See TestCloningBinary.cs and TestCloningByContract.cs for examples.
