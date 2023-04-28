using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Waker.Injection
{
    public static class QuerySelection
    {
        private static (string GameObjectName, string GameObjectId, string GameObjectClass) ParseSelector(string selector)
        {
            string gameObjectName = string.Empty;
            string gameObjectId = string.Empty;
            string gameObjectClass = string.Empty;

            string[] selectors = selector.Split(' ');

            foreach (string sel in selectors)
            {
                if (sel.StartsWith("#"))
                {
                    gameObjectId = sel;
                }
                else if (sel.StartsWith("."))
                {
                    gameObjectClass = sel;
                }
                else
                {
                    gameObjectName = sel;
                }
            }

            return (gameObjectName, gameObjectId, gameObjectClass);
        }


        public static TComponent QuerySelector<TComponent>(this GameObject root, string selector) where TComponent : Component
        {
            (var gameObjectName, var gameObjectId, var gameObjectClass) = ParseSelector(selector);

            TComponent[] componentsInChildren = root.GetComponentsInChildren<TComponent>(true);
            foreach (TComponent component in componentsInChildren)
            {
                GameObject gameObject = component.gameObject;
                string[] gameObjectTags = gameObject.name.Split(' ');

                bool hasName = string.IsNullOrEmpty(gameObjectName) || gameObjectTags.Contains(gameObjectName);
                bool hasId = string.IsNullOrEmpty(gameObjectId) || gameObjectTags.Contains(gameObjectId);
                bool hasClass = string.IsNullOrEmpty(gameObjectClass) || gameObjectTags.Contains(gameObjectClass);

                if (hasName && hasId && hasClass)
                {
                    return component;
                }
            }

            return null;
        }
        public static TComponent[] QuerySelectorAll<TComponent>(this GameObject root, string className) where TComponent : Component
        {
            if (!string.IsNullOrEmpty(className) && !className.StartsWith('.')) className = '.' + className;

            List<TComponent> foundComponents = new List<TComponent>();

            TComponent[] componentsInChildren = root.GetComponentsInChildren<TComponent>(true);
            foreach (TComponent component in componentsInChildren)
            {
                GameObject gameObject = component.gameObject;
                string[] gameObjectTags = gameObject.name.Split(' ');

                bool hasClass = string.IsNullOrEmpty(className) || gameObjectTags.Contains(className);

                if (hasClass)
                {
                    foundComponents.Add(component);
                }
            }

            return foundComponents.ToArray();
        }

        public static Component QuerySelector(this GameObject root, System.Type type, string selector)
        {
            (var gameObjectName, var gameObjectId, var gameObjectClass) = ParseSelector(selector);

            Component[] componentsInChildren = root.GetComponentsInChildren(type, true);
            foreach (Component component in componentsInChildren)
            {
                GameObject gameObject = component.gameObject;
                string[] gameObjectTags = gameObject.name.Split(' ');

                bool hasName = string.IsNullOrEmpty(gameObjectName) || gameObjectTags.Contains(gameObjectName);
                bool hasId = string.IsNullOrEmpty(gameObjectId) || gameObjectTags.Contains(gameObjectId);
                bool hasClass = string.IsNullOrEmpty(gameObjectClass) || gameObjectTags.Contains(gameObjectClass);

                if (hasName && hasId && hasClass)
                {
                    return component;
                }
            }

            return null;
        }
        public static Component[] QuerySelectorAll(this GameObject root, Type type, string className)
        {
            if (!string.IsNullOrEmpty(className) && !className.StartsWith('.')) className = '.' + className;

            List<Component> foundComponents = new List<Component>();

            Component[] componentsInChildren = root.GetComponentsInChildren(type, true);
            foreach (Component component in componentsInChildren)
            {
                GameObject gameObject = component.gameObject;
                string[] gameObjectTags = gameObject.name.Split(' ');

                bool hasClass = string.IsNullOrEmpty(className) || gameObjectTags.Contains(className);

                if (hasClass)
                {
                    foundComponents.Add(component);
                }
            }

            return foundComponents.ToArray();
        }

        public static GameObject QuerySelector(this GameObject root, string selector)
        {
            (var gameObjectName, var gameObjectId, var gameObjectClass) = ParseSelector(selector);

            Transform[] transformsInChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform childTransform in transformsInChildren)
            {
                GameObject gameObject = childTransform.gameObject;
                string[] gameObjectTags = gameObject.name.Split(' ');

                bool hasName = string.IsNullOrEmpty(gameObjectName) || gameObjectTags.Contains(gameObjectName);
                bool hasId = string.IsNullOrEmpty(gameObjectId) || gameObjectTags.Contains(gameObjectId);
                bool hasClass = string.IsNullOrEmpty(gameObjectClass) || gameObjectTags.Contains(gameObjectClass);

                if (hasName && hasId && hasClass)
                {
                    return gameObject;
                }
            }

            return null;
        }

        public static GameObject[] QuerySelectorAll(this GameObject root, string className)
        {
            if (!string.IsNullOrEmpty(className) && !className.StartsWith('.')) className = '.' + className;

            List<GameObject> foundGameObjects = new List<GameObject>();

            Transform[] transformsInChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform childTransform in transformsInChildren)
            {
                GameObject gameObject = childTransform.gameObject;
                string[] gameObjectTags = gameObject.name.Split(' ');

                bool hasClass = string.IsNullOrEmpty(className) || gameObjectTags.Contains(className);

                if (hasClass)
                {
                    foundGameObjects.Add(gameObject);
                }
            }

            return foundGameObjects.ToArray();
        }
    }

}