using System;

namespace Waker.Injection
{
    public class QueryExtensions
    {
        public static (string name, string id, string @class) Parse(string fullName)
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
            (var n, var i, var c) = Parse(str);

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

    }
}