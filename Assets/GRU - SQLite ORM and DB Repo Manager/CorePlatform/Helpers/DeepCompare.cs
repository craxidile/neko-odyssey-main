using System;
using System.Collections.Generic;
using System.Reflection;
using SpatiumInteractive.Libraries.Unity.Platform.Debugging;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.Platform.Helpers
{
    /// <summary>Comparison class.</summary>
    public static class DeepCompare
    {
        /// <summary>Compare the public instance properties. Uses deep comparison. 
        /// <b>BIG HEADSUP </b> Use this method only for  objects that have simple type properties.
        /// If yoo're comparing objects that have complex properties (e.g. some ienumerables of complex classes, nested 
        /// classes, etc) then better use the <see cref="AreEquallyDumped"/></summary>
        /// <param name="self">The reference object.</param>
        /// <param name="to">The object to compare.</param>
        /// <param name="ignore">Ignore property with name.</param>
        /// <typeparam name="T">Type of objects.</typeparam>
        /// <returns><see cref="bool">True</see> if both objects are equal, else <see cref="bool">false</see>.</returns>
        public static bool AreEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                var type = self.GetType();
                var ignoreList = new List<string>(ignore);
                foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ignoreList.Contains(pi.Name))
                    {
                        continue;
                    }

                    var selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                    var toValue = type.GetProperty(pi.Name).GetValue(to, null);

                    if (selfValue == null)
                    {
                        throw new TypeAccessException("sefValue used for property: " + type.Name + "." + pi.Name + " is most likely a complex object as its value couldn't be fetched using simple PropertyInfo.GetValue method. For comparing complex objects rather use  AreEquallyDumped if you can");
                    }
                    else if (toValue == null)
                    {
                        throw new TypeAccessException("toValue used for property: " + type.Name + "." + pi.Name + " is most likely a complex object as its value couldn't be fetched using simple PropertyInfo.GetValue method. For comparing complex objects rather use  AreEquallyDumped if you can");
                    }

                    if (pi.PropertyType.IsClass && !pi.PropertyType.Module.ScopeName.Equals("CommonLanguageRuntimeLibrary"))
                    {
                        // Check of "CommonLanguageRuntimeLibrary" is needed because string is also a class
                        if (AreEqual(selfValue, toValue, ignore))
                        {
                            continue;
                        }
                        return false;
                    }

                    //dates are a special case as they must be compared differently, regular Equals() doesn't work
                    if (selfValue.GetType() == typeof(DateTime))
                    {
                        DateTime d1 = (DateTime)selfValue;
                        DateTime d2 = (DateTime)toValue;

                        if (!d1.IsEqualTo(d2))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            SpatiumDebuggerBase.LogWarning("usporedio selfvalue: " + selfValue.ToString() + " i to value : " + toValue.ToString() + " i nisu isti pa vracam false ! ");
                            return false;
                        }
                    }
                }

                return true;
            }

            return self == to;
        }

        public static bool AreEquallyDumped<T>(T self, T to)
        {
            string selfDUmped = ObjectDumper.Dump(self);
            string toDumped = ObjectDumper.Dump(to);
            return selfDUmped == toDumped;
        }

    }
}
