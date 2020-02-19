
namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Reflection;

    internal static class ComponentCache
    {
        internal static bool TryCreateViaReflection<T>(GameObject gameObject, out T componentCache, Func<Exception> typeException)
        {
            var cacheType = typeof(T);
            var componentType = typeof(Component);

            componentCache = default;
            var success = true;

            if (cacheType.IsSubclassOf(componentType))
            {
                var component = gameObject.GetComponent(cacheType);
                if (component)
                {
                    componentCache = (T)Convert.ChangeType(component, cacheType);
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                var isStruct = cacheType.IsValueType && !cacheType.IsPrimitive && !cacheType.IsEnum;
                if (isStruct)
                {
                    var cacheStruct = default(T);

                    var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                    foreach (var field in cacheType.GetFields(bindingFlags))
                    {
                        if (field.FieldType.IsSubclassOf(componentType))
                        {
                            var value = gameObject.GetComponent(field.FieldType);
                            if (value != null)
                            {
                                field.SetValue(cacheStruct, value);
                            }
                            else
                            {
                                success = false;
                            }
                        }
                    }

                    if (success)
                    {
                        componentCache = cacheStruct;
                    }
                }
                else
                {
                    throw typeException();
                }
            }

            return success;
        }
    }
}