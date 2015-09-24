
using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace behavior
{

    static public class Debug
    {

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Break(string msg)
        {
            LogError(msg);

            UnityEngine.Debug.Break();
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b)
        {
            if (!b)
            {
                Break("CheckFailed");
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b, string message)
        {
            if (!b)
            {
                Break(message);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

    }
    
    static public class StringUtils
    {

        public static string FindExtension(string path)
        {
            return Path.GetExtension(path);
        }

    }



}
