# bddsharp
Implementation of a BDD Library in .NET

From Abstract written in 2006

This Master Thesis involved a series of implementation techniques in order
to achieve a usable, yet performance-oriented BDD library in .NET, called
BddSharp. Several implementations of BDD libraries already exist, where the
most well-known and widespread library is the C-implementation, BuDDy, but
as yet none in C# or .NET framework. The main purposes were therefore to
1) research and implement the necessary data structures that supported the
libraries kernel in achieving the best possible performance and 2) reroute the
.NET garbage collecting, that together contribute in making the library able to
be seen upon as a worthy alternative to the implementations used today.
Through comparative analysis, involving the BddSharp library and BuDDy,
the results have indicated that C# is applicable for these types of implemen-
tations. Also, comparative analysis was used on different types of .NET data
structures.
Building a BDD library that could outperform a C-based library, did not
prove possible in the given time-frame. Test results showed a difference in the
two libraries, where BuDDy was 3-4 times faster than BddSharp. Our transition
test examples often used up to 40 percent of the time in the .NET garbage
collector. Performance aside, constructing a BDD library in managed code, did
offer some great advantages, such as memory leakage prevention.
