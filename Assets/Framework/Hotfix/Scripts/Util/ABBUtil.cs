using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using Input = UnityEngine.Input;
using Random = UnityEngine.Random;


public static class ABBUtil
{
    private static string GetLogStr(params object[] messageList)
    {
        var str = new StringBuilder();
        foreach (var item in messageList)
        {
            str.Append(item);
        }
        var result = str.ToString();
        return result;
    }
    public static void Log(params object[] messageList)
    {
        var str = GetLogStr(messageList);
        Debug.Log($"ABB: {str}");
    }
    public static void LogWarring(params object[] messageList)
    {
        var str = GetLogStr(messageList);
        Debug.LogWarning($"ABB: {str}");
    }
    public static void LogError(params object[] messageList)
    {
        var str = GetLogStr(messageList);
        Debug.LogError($"ABB: {str}");
    }


    public static float GetRange(float from, float to)
    {
        var value = Random.Range(from, to);
        return value;
    }
    public static int GetRange(int from, int to)
    {
        var value = Random.Range(from, to);
        return value;
    }

    private static CancellationTokenSource m_SceneChangeToken = new();
    public static CancellationTokenSource GetSceneTokenSource()
    {
        return m_SceneChangeToken;
    }

    private static int m_TempKey = 0;
    public static int GetTempKey()
    {
        return ++m_TempKey;
    }


    public static RuntimePlatform GetCurRuntimePlatform()
    {
        return Application.platform;
    }
    public static string GetDataPath()
    {
        return Application.dataPath;
    }
    public static string GetUnityRootPath()
    {
        var unityPath = Path.GetDirectoryName(GetDataPath());
        return unityPath;
    }
    public static string GetUnityCustomCachePath(string name)
    {
        var unityPath = Path.Combine(GetUnityMiscPath(), "Cache", name);
        return unityPath;
    }
    public static string GetUnityMiscPath()
    {
        var unityPath = Path.Combine(GetUnityRootPath(), "Misc");
        return unityPath;
    }
    public static string GetFullPathByUnityPath(string unityPath)
    {
        var path = Path.Combine(GetUnityRootPath(), unityPath);
        return path;
    }
    public static string GetUnityPathByFullPath(string fullPath)
    {
        var path = Path.GetRelativePath(GetUnityRootPath(), fullPath);
        return path;
    }


    public static float GetTimeDelta()
    {
        return Time.deltaTime;
    }
    public static float GetGameTimeSeconds()
    {
        return Time.time;
    }
    public static float GetSystemTimeSeconds()
    {
        var dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        var time = (DateTime.Now - dtFrom).TotalSeconds;
        return (float)time;
    }
    public static Quaternion Dir2Quaternion(Vector3 dir)
    {
        // 将方向向量转换为角度
        Vector3 eulerAngles = dir.normalized * Mathf.Rad2Deg;

        // 转换为欧拉角
        eulerAngles.x = 0; // Mathf.Clamp(eulerAngles.x, -90f, 90f);
        eulerAngles.y = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        eulerAngles.z = 0f;

        return Quaternion.Euler(eulerAngles);
    }

    public static Vector2 GetMousePoint()
    {
        return Input.mousePosition;
    }
    public static Vector2 GetMousePositionDelta()
    {
        //return Input.mousePositionDelta;
        return Input.mouseScrollDelta;
    }
    public static float GetMouseScrollDelta()
    {
        return Input.mouseScrollDelta.y;
    }

    public static long CalculateLength(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return stream.Length;
    }


    public static bool TryGetClassesWithBaseClass<T>(out Type outType)
    {
        var inType = typeof(T);
        // 获取所有加载的程序集
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            // 获取程序集中的所有类型
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                // 检查类型是否带有GameStartAttribute特性
                if (!type.IsInterface && inType.IsAssignableFrom(type))
                {
                    outType = type;
                    return true;
                }
            }
        }
        outType = null;
        return false;
    }

    public static bool ArrayElementEquals<T>(T[] arr1, T[] arr2)
    {
        if (arr1 == null && arr2 == null) return true;
        if (arr1 == null || arr2 == null) return false;
        if (arr1.Length != arr2.Length) return false;
        for (int i = 0; i < arr1.Length; i++)
        {
            if (!arr1[i].Equals(arr2[i])) return false;
        }
        return true;
    }

    public static string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMMdd_HHmmss");
    }
    public static DateTime StringToDateTime(string dateTimeStr)
    {
        var dt = DateTime.ParseExact(dateTimeStr, "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture);
        return dt;
    }
}
