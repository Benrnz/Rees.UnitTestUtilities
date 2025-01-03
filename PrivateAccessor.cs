﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Rees.UnitTestUtilities;

/// <summary>
///     A utility class to allow tests to access to private members. For example, accessing private status fields,
///     protected/internal constructors.
///     Use with caution, using this private accessor should be the last resort.  Frequent use of this, may indicate design
///     problems.
/// </summary>
public static class PrivateAccessor
{
    /// <summary>
    ///     Gets a constant value from a type.
    /// </summary>
    /// <param name="type">The type on which to look for the constant.</param>
    /// <param name="constName">Name of the constant. Use nameof(CONSTANT_NAME) to get the name of the constant.</param>
    /// <returns>The value of the constant.</returns>
    public static object GetConstant(Type type, string constName)
    {
        return GetStaticField(type, constName)!;
    }

    /// <summary>
    ///     Gets a private field.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="fieldName">Name of the field. Use nameof(_fieldName) to get the name of the field.</param>
    /// <returns>The value of the private field.</returns>
    public static object? GetField(object instance, string fieldName)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrEmpty(fieldName))
        {
            throw new ArgumentNullException(nameof(fieldName));
        }

        var fieldInfo = GetFieldInfo(instance.GetType(), fieldName);
        return fieldInfo.GetValue(instance);
    }

    /// <summary>
    ///     Gets a private property.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="propertyName">Name of the property. Use nameof(PropertyName) to get the name of the property.</param>
    /// <returns>The value of the private property.</returns>
    public static object? GetProperty(object instance, string propertyName)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        var propertyInfo = GetPropertyInfo(instance.GetType(), propertyName);
        return propertyInfo.GetValue(instance, []);
    }

    /// <summary>
    ///     Gets a private property.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="propertyName">Name of the property. Use nameof(PropertyName) to get the name of the property.</param>
    /// <typeparam name="T">The type of the instance object. Use this to specify a different type other than its concrete type</typeparam>
    /// <returns>The value of the private property.</returns>
    public static object? GetProperty<T>(object instance, string propertyName)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        var propertyInfo = GetPropertyInfo(typeof(T), propertyName);
        return propertyInfo.GetValue(instance, new object[] { });
    }

    /// <summary>
    ///     Gets a static private field.
    /// </summary>
    /// <param name="type">The type on which to look for the field.</param>
    /// <param name="fieldName">Name of the field. Use nameof(_fieldName) to get the name of the field.</param>
    /// <returns>The value of the private field.</returns>
    public static object? GetStaticField(Type type, string fieldName)
    {
        Guard.Against<ArgumentNullException>(type is null, "type cannot be null");
        Guard.Against<ArgumentNullException>(fieldName is null, "fieldName cannot be null");
        var fieldInfo = GetStaticFieldInfo(type!, fieldName!);
        return fieldInfo.GetValue(null);
    }

    /// <summary>
    ///     Invokes a non-public function.
    /// </summary>
    /// <typeparam name="T">The return type of the member to invoke</typeparam>
    /// <param name="instance">The instance.</param>
    /// <param name="name">Name of the function. Use nameof(MethodName) to get the name of the method.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The return value of the method</returns>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
        Justification = "Typed function saves time and code")]
    public static T? InvokeFunction<T>(object instance, string name, params object[] arguments)
    {
        Guard.Against<ArgumentNullException>(instance is null, "instance cannot be null");
        Guard.Against<ArgumentNullException>(name is null, "name cannot be null");

        var returnType = typeof(T);
        var result = GetMethod(instance!.GetType(), name!, arguments, instance);
        try
        {
            return (T?)result;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Error invoking function '{0}'. The return type was not the expected type. Expected '{1}'. But was '{2}'",
                    name,
                    returnType.Name,
                    result?.GetType().Name),
                ex);
        }
    }

    /// <summary>
    ///     Invokes a non-public method.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="name">Name of the function.  Use nameof(MethodName) to get the name of the method.</param>
    /// <param name="arguments">The arguments.</param>
    public static void InvokeMethod(object instance, string name, params object[] arguments)
    {
        Guard.Against<ArgumentNullException>(instance is null, "instance cannot be null");
        Guard.Against<ArgumentNullException>(name is null, "name cannot be null");

        try
        {
            GetMethod(instance!.GetType(), name!, arguments, instance);
        }
        catch (TargetInvocationException ex)
        {
            var inner = ex.InnerException;
            if (inner == null)
            {
                throw;
            }

            throw inner;
        }
    }

    /// <summary>
    ///     Invokes a non-public static function.
    /// </summary>
    /// <typeparam name="T">The return type of the member to invoke</typeparam>
    /// <param name="type">The Type on which to look for the static method.</param>
    /// <param name="name">Name of the method.  Use nameof(MethodName) to get the name of the method.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The typed return value of the method</returns>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
        Justification = "Typed function saves time and code")]
    public static T? InvokeStaticFunction<T>(Type type, string name, params object[] arguments)
    {
        Guard.Against<ArgumentNullException>(type is null, "type cannot be null");
        Guard.Against<ArgumentNullException>(name is null, "name cannot be null");

        var returnType = typeof(T);
        var result = GetMethod(type!, name!, arguments, null);
        try
        {
            return (T?)result;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Error invoking function '{0}'. The return type was not the expected type. Expected '{1}'. But was '{2}'",
                    name,
                    returnType.Name,
                    result?.GetType().Name),
                ex);
        }
    }

    /// <summary>
    ///     Invokes a non-public static method.
    /// </summary>
    /// <param name="type">The Type on which to look for the static method.</param>
    /// <param name="name">Name of the method. Use nameof(MethodName) to get the name of the method.</param>
    /// <param name="arguments">The arguments.</param>
    public static void InvokeStaticMethod(Type type, string name, params object[] arguments)
    {
        Guard.Against<ArgumentNullException>(type is null, "type cannot be null");
        Guard.Against<ArgumentNullException>(name is null, "name cannot be null");

        GetMethod(type!, name!, arguments, null!);
    }

    /// <summary>
    ///     Accesses a Private / Internal / Protected constructor and creates an object.
    /// </summary>
    /// <typeparam name="T">The type on which to access the constructor</typeparam>
    /// <param name="argumentTypes">The argument types.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A newly constructed object</returns>
    public static T PrivateConstructor<T>(Type[] argumentTypes, object[] parameters) where T : class
    {
        var type = typeof(T);
        var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public,
            null, argumentTypes, null);
        return (constructor?.Invoke(parameters) as T)!;
    }

    /// <summary>
    ///     Accesses a Private / Internal / Protected default constructor and creates an object.
    /// </summary>
    /// <typeparam name="T">The type on which to access the constructor</typeparam>
    /// <returns>A newly constructed object</returns>
    public static T PrivateConstructor<T>() where T : class
    {
        return PrivateConstructor<T>(new Type[] { }, new object[] { });
    }

    /// <summary>
    ///     Sets the private field.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="fieldName">Name of the field. Use nameof(_fieldName) to get the name of the field.</param>
    /// <param name="value">The value to set it to.</param>
    public static void SetField(object instance, string fieldName, object value)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrEmpty(fieldName))
        {
            throw new ArgumentNullException(nameof(fieldName));
        }

        GetFieldInfo(instance.GetType(), fieldName).SetValue(instance, value);
    }

    /// <summary>
    ///     Sets the private field.
    ///     Use this overload when you wish to set a field on a parent class of the instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="fieldName">Name of the field. Use nameof(_fieldName) to get the name of the field.</param>
    /// <param name="value">The value to set it to.</param>
    /// <typeparam name="T">The type to attempt to set the fieldName on.</typeparam>
    public static void SetField<T>(object instance, string fieldName, object value)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrEmpty(fieldName))
        {
            throw new ArgumentNullException(nameof(fieldName));
        }

        GetFieldInfo(typeof(T), fieldName).SetValue(instance, value);
    }

    /// <summary>
    ///     Sets the private property.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="propertyName">Name of the property. Use nameof(PropertyName) to get the name of the property.</param>
    /// <param name="value">The value to set it to.</param>
    public static void SetProperty(object instance, string propertyName, object value)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        try
        {
            GetPropertyInfo(instance.GetType(), propertyName).SetValue(instance, value, new object[] { });
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, "Property {0} not found, unable to set it to value {1}",
                    propertyName, value), ex);
        }
    }

    /// <summary>
    ///     Sets the static private field.
    /// </summary>
    /// <param name="type">The type on which to look for the field.</param>
    /// <param name="fieldName">Name of the field. Use nameof(_fieldName) to get the name of the field.</param>
    /// <param name="value">The value to set it to.</param>
    public static void SetStaticField(Type type, string fieldName, object value)
    {
        Guard.Against<ArgumentNullException>(type == null, "type cannot be null");
        Guard.Against<ArgumentNullException>(fieldName == null, "fieldName cannot be null");
        try
        {
            GetStaticFieldInfo(type!, fieldName!).SetValue(null, value);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, "Field {0} not found, unable to set it to value {1}",
                    fieldName, value), ex);
        }
    }

    private static FieldInfo GetFieldInfo(Type type, string fieldName)
    {
        var info = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new ArgumentException(
                string.Format("Field '{0}' does not exist (is the field a static field?)", fieldName), "fieldName");
        return info;
    }

    private static object? GetMethod(Type type, string name, object[] arguments, object? instance)
    {
        var isStatic = instance is null;
        var flags = isStatic
            ? BindingFlags.NonPublic | BindingFlags.Static
            : BindingFlags.Instance | BindingFlags.NonPublic;
        var method = type.GetMethod(name, flags) ?? throw new NotSupportedException("Type does not include a member by this name: " + name);
        return method.Invoke(instance, arguments);
    }

    private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
    {
        var info = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                   throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Property '{0}' does not exist", propertyName), nameof(propertyName));
        return info;
    }

    private static FieldInfo GetStaticFieldInfo(Type type, string fieldName)
    {
        var info = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic) ??
                   throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Static Field '{0}' does not exist (is the field an instance field?)", fieldName), nameof(fieldName));
        return info;
    }
}
