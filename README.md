# Rees.UnitTestUtilities
A set of unit testing utilities to make unit testing a little easier.
This library is intended only for use in unit testing code.

PrivateAccessor
===============
A helper class that provides methods that makes it easier to access private, protected and internal fields, methods, and constructors. Use with caution, frequent use of private accessors either means poor design or incorrect unit testing technique.

Examples:
Set a property with a private setter:
```csharp
PrivateAccessor.SetProperty(this.subject, nameof(SubjectType.Created), new DateTime(2012, 2, 12));
```
Invoke a private method:
```csharp
PrivateAccessor.InvokeMethod(this.subject, nameof(SubjectType.InitialiseTheRulesCollections), someVariable);
```

Guard
=====
A precondition helper class that provides an easier to read syntax for argument checking and some validation.

Examples:
```csharp
// ==========================================================================================
// Instead of 
if (type == null) {
  throw new ArgumentNullException("type cannot be null");
}
// Use
Guard.Against<ArgumentNullException>(type == null, "type cannot be null");

// ==========================================================================================
// Instead of
if (this.subject is IDisposable)
{
    throw new InvalidOperationException("The subject type must implement IDisposable.");
}
// Or
if (typeof(IDisposable).IsAssignableFrom(this.subject.GetType()))
{
    throw new InvalidOperationException("The subject type must implement IDisposable.");
}
// Or
if (typeof(IDisposable).IsInstanceOfType(this.subject))
{
    throw new InvalidOperationException("The subject type must implement IDisposable.");
}

// Use
Guard.Implements<IDisposable>(this.subject.GetType(), "The subject type must implement IDisposable.");

// ==========================================================================================
// Instead of
if (this.subject is MySuperClass)
{
    throw new InvalidOperationException("The subject type must inherit MySuperClass.");
}
// Or
if (typeof(MySuperClass).IsAssignableFrom(this.subject.GetType()))
{
    throw new InvalidOperationException("The subject type must inherit MySuperClass.");
}
// Or
if (typeof(MySuperClass).IsInstanceOfType(this.subject))
{
    throw new InvalidOperationException("The subject type must inherit MySuperClass.");
}

// Use
Guard.InheritsFrom<MySuperClass>(this.subject.GetType(), "The subject type must inherit from MySuperClass.");
```
