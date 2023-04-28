using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Waker.Injection
{
    public enum InjectFrom
    {
        All,
        Self,
        Children,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : System.Attribute
    {
        private InjectFrom injectFrom;
        private string selector;

        public InjectAttribute(string selector = null, InjectFrom injectFrom = InjectFrom.All)
        {
            this.injectFrom = !string.IsNullOrEmpty(selector) ? InjectFrom.Children : injectFrom;
            this.selector = selector;
        }

        public InjectFrom InjectFrom => injectFrom;

        public string Selector => selector;
    }

    public static class Injector
    {
        public static void Inject(Component component)
        {
            var componentType = component.GetType();

            var fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];

                var inject = (InjectAttribute)field.GetCustomAttribute(typeof(InjectAttribute), true);
                if (inject == null)
                {
                    continue;
                }

                InjectTo(component, field, inject);
            }
        }

        private static void InjectTo(Component component, FieldInfo field, InjectAttribute inject)
        {
            var fieldType = field.FieldType;

            if (fieldType.IsArray)
            {
                InjectToArrayField(component, field, inject);
            }
            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                InjectToListField(component, field, inject);
            }
            else
            {
                InjectToField(component, field, inject);
            }
        }

        private static void InjectToField(Component component, FieldInfo field, InjectAttribute inject)
        {
            var fieldType = field.FieldType;

            bool isComponent = fieldType.IsSubclassOf(typeof(Component));
            bool isGameObject = fieldType.IsEquivalentTo(typeof(GameObject));

            if (!isComponent && !isGameObject)
            {
                Debug.LogWarning($"Dependency Injection: Field '{field.Name}' of component '{fieldType.FullName}' is not of valid type 'UnityEngine.Component' nor 'UnityEngine.GameObject'.");
                return;
            }

            if (isGameObject)
            {
                if (!string.IsNullOrEmpty(inject.Selector))
                {
                    var gameObject = QuerySelection.QuerySelector(component.gameObject, inject.Selector);
                    field.SetValue(component, gameObject);
                }
                else
                {
                    var obj = component.gameObject;
                    field.SetValue(component, obj);
                }
            }
            else
            {
                Component c;
                if (!string.IsNullOrEmpty(inject.Selector))
                {
                    c = QuerySelection.QuerySelector(component.gameObject, fieldType, inject.Selector);
                }
                else if (inject.InjectFrom == InjectFrom.All || inject.InjectFrom == InjectFrom.Children)
                {
                    c = component.GetComponentInChildren(fieldType);
                }
                else 
                {
                    c = component.GetComponent(fieldType);
                }

                if (c != null)
                {
                    field.SetValue(component, c);
                }
            }
        }

        private static void InjectToListField(Component component, FieldInfo field, InjectAttribute inject)
        {
            var elementType = field.FieldType.GetGenericArguments()[0];

            bool isComponent = elementType.IsSubclassOf(typeof(Component));
            bool isGameObject = elementType.IsEquivalentTo(typeof(GameObject));

            if (!isComponent && !isGameObject)
            {
                Debug.LogWarning($"Dependency Injection: Field '{field.Name}' of component '{elementType.FullName}' is not of valid type 'UnityEngine.Component' nor 'UnityEngine.GameObject'.");
                return;
            }

            if (isGameObject)
            {
                if (inject.InjectFrom == InjectFrom.All || inject.InjectFrom == InjectFrom.Children)
                {
                    var gameObjects = QuerySelection.QuerySelectorAll(component.gameObject, inject.Selector);
                    field.SetValue(component, gameObjects.ToList());
                }
                else
                {
                    var obj = component.gameObject;
                    field.SetValue(component, new List<GameObject>() { obj });
                }
            }
            else
            {
                Component[] components;
                if (!string.IsNullOrEmpty(inject.Selector))
                {
                    components = QuerySelection.QuerySelectorAll(component.gameObject, elementType, inject.Selector);
                }
                else if (inject.InjectFrom == InjectFrom.All || inject.InjectFrom == InjectFrom.Children)
                {
                    components = component.GetComponentsInChildren(elementType);
                }
                else
                {
                    components = component.GetComponents(elementType);
                }

                field.SetValue(component, ConvertList(components, elementType));
            }

            // 리스트 변환 함수
            static IList ConvertList(Component[] components, Type elementType)
            {
                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                foreach (var component in components)
                {
                    list.Add(component);
                }

                return list;
            }
        }

        private static void InjectToArrayField(Component component, FieldInfo field, InjectAttribute inject)
        {
            var elementType = field.FieldType.GetElementType();

            bool isComponent = elementType.IsSubclassOf(typeof(Component));
            bool isGameObject = elementType.IsEquivalentTo(typeof(GameObject));

            if (!isComponent && !isGameObject)
            {
                Debug.LogWarning($"Dependency Injection: Field '{field.Name}' of component '{elementType.FullName}' is not of valid type 'UnityEngine.Component' nor 'UnityEngine.GameObject'.");
                return;
            }

            if (isGameObject)
            {
                if (inject.InjectFrom == InjectFrom.All || inject.InjectFrom == InjectFrom.Children)
                {
                    var gameObjects = QuerySelection.QuerySelectorAll(component.gameObject, inject.Selector);
                    field.SetValue(component, gameObjects);
                }
                else
                {
                    var obj = component.gameObject;
                    field.SetValue(component, new GameObject[] { obj });
                }
            }
            else
            {
                Component[] components;
                if (!string.IsNullOrEmpty(inject.Selector))
                {
                    components = QuerySelection.QuerySelectorAll(component.gameObject, elementType, inject.Selector);
                }
                else if (inject.InjectFrom == InjectFrom.All || inject.InjectFrom == InjectFrom.Children)
                {
                    components = component.GetComponentsInChildren(elementType);
                }
                else
                {
                    components = component.GetComponents(elementType);
                }

                field.SetValue(component, ConvertArray(components, elementType));
            }

            // 배열 변환 함수
            static Array ConvertArray(Component[] components, Type elementType)
            {
                Array array = Array.CreateInstance(elementType, components.Length);

                for (int i = 0; i < components.Length; i++)
                {
                    array.SetValue(components[i], i);
                }

                return array;
            }
        }
    }
}