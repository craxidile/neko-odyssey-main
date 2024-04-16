using System.Collections.Generic;
using System.Reflection;
using System;
using SpatiumInteractive.Libraries.Unity.Platform.Helpers;
using UnityEngine;
using Object = System.Object;

// using Unity.Plastic.Newtonsoft.Json;

namespace SpatiumInteractive.Libraries.Unity.Platform.Extensions
{
    public static class ObjectExtensions
    {
        #region Private Fields

        /// <summary>
        /// MemberwiseClone method. 
        /// 3X faster than BinaryFormatter !!
        /// </summary>
        private static readonly MethodInfo CloneMethod =
            typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion

        #region Public Methods

        public static IEnumerable<T> DeepCopyCollection<T>(this IEnumerable<T> original)
        {
            var cloned = new List<T>();

            foreach (var obj in original)
            {
                var clone = obj.DeepCopy();
                cloned.Add(clone);
            }

            return cloned;
        }

        public static T DeepCopy<T>(this T original)
        {
            return (T)DeepCopy((Object)original);
        }

        public static Object DeepCopy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static object GetPropValue(this object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetPropValue<T>(this T @this, string propertyName, object value)
        {
            Type type = @this.GetType();
            PropertyInfo property = type.GetProperty
            (
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
            );
            property.SetValue(@this, value, null);
        }

        public static bool IsEqualTo<T>(this T obj1, T obj2, bool nullsAllowed = true)
        {
            if (!nullsAllowed)
            {
                if (obj1 == null || obj2 == null) return false;
            }
            if ((obj1 == null && obj2 != null) || (obj1 != null && obj2 == null)) return false;
            // var obj1Serialized = JsonConvert.SerializeObject(obj1);
            // var obj2Serialized = JsonConvert.SerializeObject(obj2);
            var obj1Serialized = JsonUtility.ToJson(obj1);
            var obj2Serialized = JsonUtility.ToJson(obj2);

            return obj1Serialized == obj2Serialized;
        }

        #endregion

        #region Private Methods

        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;

            var typeToReflect = originalObject.GetType();

            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;

            var cloneObject = CloneMethod.Invoke(originalObject, null);

            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) =>
                        array.SetValue
                        (
                            InternalCopy(clonedArray.GetValue(indices), visited), indices)
                        );
                }
            }

            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);

            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields
        (
            object originalObject,
            IDictionary<object, object> visited,
            object cloneObject,
            Type typeToReflect
        )
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields
                (
                    originalObject,
                    visited,
                    cloneObject,
                    typeToReflect.BaseType
                );

                CopyFields
                (
                    originalObject,
                    visited,
                    cloneObject,
                    typeToReflect.BaseType,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    info => info.IsPrivate
                );
            }
        }

        private static void CopyFields
        (
            object originalObject,
            IDictionary<object, object> visited,
            object cloneObject,
            Type typeToReflect,
            BindingFlags bindingFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy,
            Func<FieldInfo,
            bool> filter = null
        )
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        #endregion
    }
}