using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public  static class AotUtility
{

    // 获取所有带有GameStartAttribute特性的类
    public static Type GetClassesWithGameStartAttribute(Type attribute)
    {
        // 获取所有加载的程序集
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            // 获取程序集中的所有类型
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                // 检查类型是否带有GameStartAttribute特性
                if (type.IsDefined(attribute, false))
                {
                    Debug.Log($"找到带有GameStartAttribute的类: {type.Name}");
                    return type;
                }
            }
        }
        Debug.LogError($"没有找到带有GameStartAttribute的类，length: {assemblies.Length}");
        foreach (Assembly assembly in assemblies)
        {
            Debug.LogError(assembly.FullName);
        }
        return null;
    }
    public static string GetPersistentDataPath()
    {
        return Application.persistentDataPath;
    }
    public static string CalculateFileMD5(string filePath)
    {
        var data = File.ReadAllBytes(filePath);
        return CalculateFileMD5(data);
    }
    public static string CalculateFileMD5(byte[] data)
    {
        using var md5 = MD5.Create();

        byte[] hashBytes = md5.ComputeHash(data);
        var result = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return result;
    }
    public static string CalculateStringMD5(string data)
    {
        var bytes = Encoding.ASCII.GetBytes(data);
        var result = CalculateFileMD5(bytes);
        return result;
    }
}
