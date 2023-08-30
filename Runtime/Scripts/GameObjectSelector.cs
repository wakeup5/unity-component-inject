using System;
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
            (var name, var id, var @class) = ParseSelector(selector);

            Transform[] transforms = Traversal(root);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform transform = transforms[i];
                if (IsMatch(transform.name, name, id, @class))
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
            (var name, var id, var @class) = ParseSelector(selector);

            List<GameObject> list = new();

            Transform[] transforms = Traversal(root);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform transform = transforms[i];
                if (IsMatch(transform.name, name, id, @class))
                {
                    list.Add(transform.gameObject);
                }
            }

            return list;
        }
        public static TComponent SelectOne<TComponent>(this GameObject root) where TComponent : Component
        {
            return root.GetComponentInChildren<TComponent>(true);
        }

        public static TComponent SelectOne<TComponent>(this GameObject root, string selector) where TComponent : Component
        {
            (var name, var id, var @class) = ParseSelector(selector);

            TComponent[] componentsInChildren = root.GetComponentsInChildren<TComponent>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                TComponent component = componentsInChildren[i];
                if (IsMatch(component.name, name, id, @class))
                {
                    return component;
                }
            }

            return null;
        }

        public static List<TComponent> SelectAll<TComponent>(this GameObject root) where TComponent : Component
        {
            return root.GetComponentsInChildren<TComponent>(true).ToList();
        }

        public static List<TComponent> SelectAll<TComponent>(this GameObject root, string selector) where TComponent : Component
        {
            (var name, var id, var @class) = ParseSelector(selector);

            var foundComponents = new List<TComponent>();

            TComponent[] componentsInChildren = root.GetComponentsInChildren<TComponent>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                TComponent component = componentsInChildren[i];
                if (IsMatch(component.name, name, id, @class))
                {
                    foundComponents.Add(component);
                }
            }

            return foundComponents;
        }
        
        public static Component SelectOne(this GameObject root, System.Type type)
        {
            return root.GetComponentInChildren(type, true);
        }

        public static Component SelectOne(this GameObject root, System.Type type, string selector)
        {
            (var name, var id, var @class) = ParseSelector(selector);

            Component[] componentsInChildren = root.GetComponentsInChildren(type, true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Component component = componentsInChildren[i];
                if (IsMatch(component.name, name, id, @class))
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
            (var name, var id, var @class) = ParseSelector(selector);

            var foundComponents = new List<Component>();

            Component[] componentsInChildren = root.GetComponentsInChildren(type, true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Component component = componentsInChildren[i];
                if (IsMatch(component.name, name, id, @class))
                {
                    foundComponents.Add(component);
                }
            }

            return foundComponents;
        }

        static bool WildcardMatch(string str, string pattern)
        {
            int s = 0, p = 0, match = 0, starIdx = -1;
            while (s < str.Length)
            {
                // 일치하는 경우 또는 '?' 와일드카드인 경우
                if (p < pattern.Length && (pattern[p] == '?' || str[s] == pattern[p]))
                {
                    s++;
                    p++;
                }
                // '*' 와일드카드인 경우
                else if (p < pattern.Length && pattern[p] == '*')
                {
                    starIdx = p;
                    match = s;
                    p++;
                }
                // '*' 와일드카드가 이전에 나왔던 경우
                else if (starIdx != -1)
                {
                    p = starIdx + 1;
                    match++;
                    s = match;
                }
                // 일치하지 않는 경우
                else
                    return false;
            }

            // 남은 패턴이 모두 '*'인지 확인
            while (p < pattern.Length && pattern[p] == '*')
            {
                p++;
            }

            return p == pattern.Length;
        }

        public static bool IsMatch(string str, string name, string id, string @class)
        {
            (var n, var i, var c) = ParseSelector(str);

            if (!string.IsNullOrEmpty(@class) && @class != c)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(id) && id != i)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(name) && !WildcardMatch(n, name))
            {
                return false;
            }

            return true;
        }

        public static (string name, string id, string @class) ParseSelector(string fullName)
        {
            string name;
            string id = string.Empty;
            string @class = string.Empty;

            int idIndex = fullName.IndexOf('#');
            if (idIndex >= 0)
            {
                int idSpaceIndex = fullName.IndexOf(' ', idIndex);
                if (idSpaceIndex >= 0)
                {
                    id = fullName[idIndex..idSpaceIndex];
                }
                else
                {
                    id = fullName[idIndex..];
                }
            }

            int classIndex = fullName.IndexOf('.');
            if (classIndex >= 0)
            {
                int classSpaceIndex = fullName.IndexOf(' ', classIndex);
                if (classSpaceIndex >= 0)
                {
                    @class = fullName[classIndex..classSpaceIndex];
                }
                else
                {
                    @class = fullName[classIndex..];
                }
            }

            if (idIndex >= 0 || classIndex >= 0)
            {
                if (classIndex < 0)
                {
                    name = fullName[..idIndex];
                }
                else if (idIndex < 0)
                {
                    name = fullName[..classIndex];
                }
                else
                {
                    name = fullName[..Math.Min(idIndex, classIndex)];
                }
            }
            else
            {
                name = fullName;
            }

            name = name.Trim();

            return (name, id, @class);
        }
    }

}