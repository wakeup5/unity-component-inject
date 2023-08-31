using System.Collections.Generic;

using UnityEngine;

namespace Waker.Injection
{
    public class Query
    {
        private List<QueryElement> _queryElements = new List<QueryElement>();

        public Query(string query)
        {
            string[] queryElements = query.Split('>');
            foreach (string queryElement in queryElements)
            {
                _queryElements.Add(new QueryElement(queryElement.Trim()));
            }
        }

        public int Depth => _queryElements.Count - 1;

        public bool IsMatchElement(string name, int depth)
        {
            if (depth < 0 || depth >= _queryElements.Count)
                return false;

            var element = _queryElements[depth];
            return element.IsMatch(name);
        }

        public bool IsMatch(Transform root, GameObject current)
        {
            if (current == null)
                return false;

            return IsMatch(root, current.transform);
        }        
        // 오브젝트가 쿼리에 맞는지 확인
        public bool IsMatch(Transform root, Transform current)
        {
            if (current == null)
                return false;

            // 현재 오브젝트는 마지막 아이템과 일치해야 한다.
            if (!IsMatchElement(current.name, Depth))
            {
                return false;
            }
            
            // 순차적으로 부모를 타고 올라가면서 쿼리에 맞는지 확인
            Transform c = current.parent;
            int depth = Depth - 1;

            while (c != null)
            {
                if (depth < 0)
                    break;

                if (root != null && c == root.parent)
                    break;

                if (IsMatchElement(c.name, depth))
                {
                    depth--;
                }

                c = c.parent;
            }

            return depth < 0;
        }

        private class QueryElement
        {
            public QueryElement(string fullName, string name, string id, string @class)
            {
                FullName = fullName;
                Name = name;
                Id = id;
                Class = @class;
            }

            public QueryElement(string fullName)
            {
                FullName = fullName;
                (Name, Id, Class) = QueryExtensions.Parse(fullName);
            }

            public string FullName { get; set; }
            public string Name { get; set; }
            public string Id { get; set; }
            public string Class { get; set; }

            public bool IsMatch(string str)
            {
                return QueryExtensions.IsMatch(str, Name, Id, Class);
            }
        }
    }
}