using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor.SearchService;

using UnityEngine;

namespace Waker.Injection
{
    public static class Injector
    {
        public enum InjectType
        {
            GameObject,
            Component,
            Other,
        }

        public enum CollectionType
        {
            None,
            Array,
            List,
        }

        public static void Inject(Component component)
        {
            var componentType = component.GetType();

            var fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
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

        private static void InjectTo(Component component, FieldInfo fieldInfo, InjectAttribute inject)
        {
            var fieldType = fieldInfo.FieldType;
            var collectionType = GetCollectionType(fieldType);

            var injectType = collectionType switch
            {
                CollectionType.None => GetInjectType(fieldType),
                CollectionType.Array => GetInjectType(fieldType.GetElementType()),
                CollectionType.List => GetInjectType(fieldType.GenericTypeArguments[0]),
            };

            if (injectType == InjectType.Other)
            {
                Debug.LogWarning($"Field '{fieldType.Name}' of component '{fieldType.FullName}' is not of valid type 'UnityEngine.Component' nor 'UnityEngine.GameObject'.", component.gameObject);

                return;
            }

            switch (injectType, collectionType)
            {
                case (InjectType.Component, CollectionType.None):
                    InjectComponentToField(component, fieldInfo, inject);
                    break;
                case (InjectType.GameObject, CollectionType.None):
                    InjectGameObjectToField(component, fieldInfo, inject);
                    break;
                case (InjectType.Component, CollectionType.Array):
                    InjectComponentToArrayField(component, fieldInfo, inject);
                    break;
                case (InjectType.GameObject, CollectionType.Array):
                    InjectGameObjectToArrayField(component, fieldInfo, inject);
                    break;
                case (InjectType.Component, CollectionType.List):
                    InjectComponentToListField(component, fieldInfo, inject);
                    break;
                case (InjectType.GameObject, CollectionType.List):
                    InjectGameObjectToListField(component, fieldInfo, inject);
                    break;
            }
        }

        #region Inject Component
        private static void InjectComponentToField(Component component, FieldInfo field, InjectAttribute inject)
        {
            if (!string.IsNullOrEmpty(inject.Selector))
            {
                InjectComponentToFieldBySelector(component, field, inject.Selector);
            }
            else
            {
                InjectComponentToFieldByFlags(component, field, inject.InjectFrom);
            }
        }

        private static void InjectComponentToFieldBySelector(Component component, FieldInfo field, string selector)
        {
            field.SetValue(component, GameObjectSelector.SelectOne(component.gameObject, field.FieldType, selector));
        }

        private static void InjectComponentToFieldByFlags(Component component, FieldInfo fieldInfo, InjectFrom injectFrom)
        {
            var fieldType = fieldInfo.FieldType;

            Component c = injectFrom switch
            {
                InjectFrom.Self => component.GetComponent(fieldType),
                InjectFrom.Children => component.GetComponentInChildren(fieldType, true),
                _ => null,
            };

            fieldInfo.SetValue(component, c);
        }
        #endregion

        #region Inject GameObject
        private static void InjectGameObjectToField(Component component, FieldInfo fieldInfo, InjectAttribute inject)
        {
            if (!string.IsNullOrEmpty(inject.Selector))
            {
                InjectGameObjectToFieldBySelector(component, fieldInfo, inject.Selector);
            }
            else
            {
                InjectGameObjectToFieldByFlags(component, fieldInfo, inject.InjectFrom);
            }
        }

        private static void InjectGameObjectToFieldBySelector(Component component, FieldInfo fieldInfo, string selector)
        {
            fieldInfo.SetValue(component, GameObjectSelector.SelectOne(component.gameObject, selector));
        }

        private static void InjectGameObjectToFieldByFlags(Component component, FieldInfo fieldInfo, InjectFrom injectFrom)
        {
            GameObject gameObject = injectFrom switch
            {
                InjectFrom.Self => component.gameObject,
                InjectFrom.Children => GameObjectSelector.SelectOne(component.gameObject),
                _ => null
            };

            fieldInfo.SetValue(component, gameObject);
        }
        #endregion

        #region Inject Component Array
        private static void InjectComponentToArrayField(Component component, FieldInfo fieldInfo, InjectAttribute inject)
        {
            if (!string.IsNullOrEmpty(inject.Selector))
            {
                InjectComponentToArrayFieldBySelector(component, fieldInfo, inject.Selector);
            }
            else
            {
                InjectComponentToArrayFieldByFlags(component, fieldInfo, inject.InjectFrom);
            }
        }

        private static void InjectComponentToArrayFieldBySelector(Component component, FieldInfo fieldInfo, string selector)
        {
            var itemType = fieldInfo.FieldType.GetElementType();

            Component[] components = GameObjectSelector.SelectAll(component.gameObject, itemType, selector).ToArray();
            fieldInfo.SetValue(component, ConvertArray(components, itemType));
        }

        private static void InjectComponentToArrayFieldByFlags(Component component, FieldInfo fieldInfo, InjectFrom injectFrom)
        {
            var itemType = fieldInfo.FieldType.GetElementType();

            Component[] components = injectFrom switch
            {
                InjectFrom.Self => component.GetComponents(itemType),
                InjectFrom.Children => component.GetComponentsInChildren(itemType, true),
                _ => null,
            };

            fieldInfo.SetValue(component, ConvertArray(components, itemType));
        }
        #endregion

        #region Inject GameObject Array
        private static void InjectGameObjectToArrayField(Component component, FieldInfo fieldInfo, InjectAttribute inject)
        {
            if (!string.IsNullOrEmpty(inject.Selector))
            {
                InjectGameObjectToArrayFieldBySelector(component, fieldInfo, inject.Selector);
            }
            else
            {
                InjectGameObjectToArrayFieldByFlags(component, fieldInfo, inject.InjectFrom);
            }
        }

        private static void InjectGameObjectToArrayFieldBySelector(Component component, FieldInfo fieldInfo, string selector)
        {
            GameObject[] gameObjects = GameObjectSelector.SelectAll(component.gameObject, selector).ToArray();
            fieldInfo.SetValue(component, gameObjects);
        }

        private static void InjectGameObjectToArrayFieldByFlags(Component component, FieldInfo fieldInfo, InjectFrom injectFrom)
        {
            GameObject[] gameObjects = injectFrom switch
            {
                InjectFrom.Self => new GameObject[] { component.gameObject },
                InjectFrom.Children => GameObjectSelector.SelectAll(component.gameObject).ToArray(),
                _ => null,
            };

            fieldInfo.SetValue(component, gameObjects);
        }
        
        #endregion

        #region Inject Component List
        private static void InjectComponentToListField(Component component, FieldInfo fieldInfo, InjectAttribute inject)
        {
            if (!string.IsNullOrEmpty(inject.Selector))
            {
                InjectComponentToListFieldBySelector(component, fieldInfo, inject.Selector);
            }
            else
            {
                InjectComponentToListFieldByFlags(component, fieldInfo, inject.InjectFrom);
            }
        }

        private static void InjectComponentToListFieldBySelector(Component component, FieldInfo fieldInfo, string selector)
        {
            var itemType = fieldInfo.FieldType.GetGenericArguments()[0];

            fieldInfo.SetValue(component, GameObjectSelector.SelectAll(component.gameObject, itemType, selector));
        }

        private static void InjectComponentToListFieldByFlags(Component component, FieldInfo fieldInfo, InjectFrom injectFrom)
        {
            var itemType = fieldInfo.FieldType.GetGenericArguments()[0];

            List<Component> components = injectFrom switch
            {
                InjectFrom.Self => component.GetComponents(itemType).ToList(),
                InjectFrom.Children => GameObjectSelector.SelectAll(component.gameObject, itemType),
                _ => null,
            };

            fieldInfo.SetValue(component, components);
        }
        #endregion

        #region Inject GameObject List
        private static void InjectGameObjectToListField(Component component, FieldInfo fieldInfo, InjectAttribute inject)
        {
            if (!string.IsNullOrEmpty(inject.Selector))
            {
                InjectGameObjectToListFieldBySelector(component, fieldInfo, inject.Selector);
            }
            else
            {
                InjectGameObjectToListFieldByFlags(component, fieldInfo, inject.InjectFrom);
            }
        }

        private static void InjectGameObjectToListFieldBySelector(Component component, FieldInfo fieldInfo, string selector)
        {
            List<GameObject> gameObjects = GameObjectSelector.SelectAll(component.gameObject, selector);
            fieldInfo.SetValue(component, gameObjects);
        }

        private static void InjectGameObjectToListFieldByFlags(Component component, FieldInfo fieldInfo, InjectFrom injectFrom)
        {
            List<GameObject> components = injectFrom switch
            {
                InjectFrom.Self => new List<GameObject> { component.gameObject },
                InjectFrom.Children => GameObjectSelector.SelectAll(component.gameObject),
                _ => null,
            };

            fieldInfo.SetValue(component, components);
        }

        #endregion

        static Array ConvertArray(Component[] components, Type elementType)
        {
            Array array = Array.CreateInstance(elementType, components.Length);

            for (int i = 0; i < components.Length; i++)
            {
                array.SetValue(components[i], i);
            }

            return array;
        }

        private static InjectType GetInjectType(Type fieldType)
        {
            bool isComponent = fieldType.IsSubclassOf(typeof(Component));
            bool isGameObject = fieldType.IsEquivalentTo(typeof(GameObject));

            return (isComponent, isGameObject) switch
            {
                (true, false) => InjectType.Component,
                (false, true) => InjectType.GameObject,
                (true, true) => throw new System.Exception($"Something wrong. {fieldType.Name} is a Component and is also a GameObject. What happened."),
                _ => throw new NotImplementedException(),
            };
        }

        private static CollectionType GetCollectionType(Type fieldType)
        {
            bool isCollection = fieldType.IsArray || (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>));
            bool isArray = fieldType.IsArray;

            return (isCollection, isArray) switch
            {
                (true, true) => CollectionType.Array,
                (true, false) => CollectionType.List,
                (false, false) => CollectionType.None,
                _ => throw new System.Exception($"Something wrong. {fieldType.Name} is a List and is also a Array. What happened."),
            };
        }
    }
}