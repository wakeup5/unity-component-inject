using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Waker.Injection
{
    public static class GameObjectSelector
    {
        private static Transform[] Traversal(GameObject root)
        {
            return root.GetComponentsInChildren<Transform>(true);
        }

        private static T[] Traversal<T>(GameObject root) where T : Component
        {
            return root.GetComponentsInChildren<T>(true);
        }

        private static void AddChildrenToList(Transform current, List<GameObject> children)
        {
            if (current == null)
                return;

            foreach (Transform child in current)
            {
                children.Add(child.gameObject);
                AddChildrenToList(child, children);
            }
        }

        public static GameObject SelectOne(this GameObject root)
        {
            if (root.transform.childCount > 0)
            {
                return root.transform.GetChild(0).gameObject;
            }

            return null;
        }

        public static GameObject SelectOne(this GameObject root, string selector)
        {
            var query = new Query(selector);

            Transform[] transforms = Traversal(root);
            foreach (Transform transform in transforms)
            {
                if (query.IsMatch(root.transform, transform))
                {
                    return transform.gameObject;
                }
            }

            return null;
        }

        public static List<GameObject> SelectAll(this GameObject root)
        {
            // 하위 모든 오브젝트를 반환
            var children = new List<GameObject>();

            AddChildrenToList(root.transform, children);

            return children;
        }

        public static List<GameObject> SelectAll(this GameObject root, string selector)
        {
            var query = new Query(selector);

            List<GameObject> founds = new();

            Transform[] transforms = Traversal(root);
            foreach (Transform transform in transforms)
            {
                if (query.IsMatch(root.transform, transform))
                {
                    founds.Add(transform.gameObject);
                }
            }

            return founds;
        }

        public static T SelectOne<T>(this GameObject root) where T : Component
        {
            return root.GetComponentInChildren<T>(true);
        }

        public static T SelectOne<T>(this GameObject root, string selector) where T : Component
        {
            var query = new Query(selector);

            T[] components = Traversal<T>(root);
            foreach (T component in components)
            {
                if (query.IsMatch(root.transform, component.transform))
                {
                    return component;
                }
            }

            return null;
        }

        public static List<T> SelectAll<T>(this GameObject root) where T : Component
        {
            return root.GetComponentsInChildren<T>(true).ToList();
        }

        public static List<T> SelectAll<T>(this GameObject root, string selector) where T : Component
        {
            var query = new Query(selector);

            var founds = new List<T>();

            T[] components = Traversal<T>(root);
            foreach (T component in components)
            {
                if (query.IsMatch(root.transform, component.transform))
                {
                    founds.Add(component);
                }
            }

            return founds;
        }
        
        public static Component SelectOne(this GameObject root, System.Type type)
        {
            return root.GetComponentInChildren(type, true);
        }

        public static Component SelectOne(this GameObject root, System.Type type, string selector)
        {
            var query = new Query(selector);

            Component[] components = root.GetComponentsInChildren(type, true);
            foreach (Component component in components)
            {
                if (query.IsMatch(root.transform, component.transform))
                {
                    return component;
                }
            }

            return null;
        }

        public static List<Component> SelectAll(this GameObject root, Type type)
        {
            return root.GetComponentsInChildren(type, true).ToList();
        }

        public static List<Component> SelectAll(this GameObject root, Type type, string selector)
        {
            var query = new Query(selector);

            var founds = new List<Component>();

            Component[] components = root.GetComponentsInChildren(type, true);
            foreach (Component component in components)
            {
                if (query.IsMatch(root.transform, component.transform))
                {
                    founds.Add(component);
                }
            }

            return founds;
        }
    }
}