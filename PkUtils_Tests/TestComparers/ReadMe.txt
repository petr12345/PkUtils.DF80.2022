TestComparers demonstrates two generic equality comparer implementations:
FunctionalEqualityComparer and KeyEqualityComparer. It compares their use with
LINQ Distinct for case-insensitive string comparisons and illustrates why a
custom equality comparer needs a compatible hash-code implementation.

For details, see ComparersTest.TestCaseInsensitiveDistinct() and DoTest().
