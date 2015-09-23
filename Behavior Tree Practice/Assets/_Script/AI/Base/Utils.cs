
using UnityEngine;
using System.Diagnostics;

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

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

    }
}
