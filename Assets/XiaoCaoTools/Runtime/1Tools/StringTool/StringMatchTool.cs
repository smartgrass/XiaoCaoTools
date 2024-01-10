using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace XiaoCao
{
    /// <summary>
    /// 字符串正则匹配 操作
    /// </summary>
    public static class StringMatchTool
    {
        /// <summary>
        ///匹配头尾中的内容
        ///如"abcd[xxx]efg",匹配[],输出"xxx"
        ///withSide为True 输出"[xxx]" 
        /// </summary>
        public static string MatchSide(string input, string head, string end, bool withSide = false)
        {
            //Regex.Escape 用于转义字符串中的正则表达式特殊字符，以确保这些字符被当作普通字符而不是正则表达式元字符来处理。
            string pattern = $"{Regex.Escape(head)}(.*?){Regex.Escape(end)}";
            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                if (withSide)
                {
                    return match.Groups[0].Value;
                }
                return match.Groups[1].Value;
            }
            return null;
        }
        //移除开头
        public static string RemoveHead(this string str, string removeStr)
        {
            if (str.StartsWith(removeStr))
            {
                return str.Remove(0, removeStr.Length);
            }
            else
            {
                Debug.LogError(str + "no StartsWith" + removeStr);
                return str;
            }
        }
        //移除结尾
        public static string RemoveEnd(this string str, string removeStr)
        {
            if (str.EndsWith(removeStr))
            {
                int len = str.Length;
                return str.Remove(len - removeStr.Length, removeStr.Length);
            }
            else
            {
                Debug.LogError(str + "no EndsWith " + removeStr);
                return str;
            }
        }
    }


    /// <summary>
    /// 序列化数组
    /// </summary>
    public static class StringArrayConvert
    {
        public static string ListToString<T>(this List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in list)
            {
                sb.Append(t);
                sb.Append(",");
            }
            return sb.ToString();
        }

        public static string ArrayToString<T>(this T[] args)
        {
            if (args == null)
            {
                return "";
            }

            string argStr = " [";
            for (int arrIndex = 0; arrIndex < args.Length; arrIndex++)
            {
                argStr += args[arrIndex];
                if (arrIndex != args.Length - 1)
                {
                    argStr += ", ";
                }
            }

            argStr += "]";
            return argStr;
        }

        public static string ArrayToString<T>(this T[] args, int index, int count)
        {
            if (args == null)
            {
                return "";
            }

            string argStr = " [";
            for (int arrIndex = index; arrIndex < count + index; arrIndex++)
            {
                argStr += args[arrIndex];
                if (arrIndex != args.Length - 1)
                {
                    argStr += ", ";
                }
            }

            argStr += "]";
            return argStr;
        }
    }
}


