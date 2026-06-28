using System.Diagnostics;
using System.IO;
//using OfficeOpenXml;
using UnityEngine;

public static class ExtendUtil
{
    public static void OpenFile(this FileInfo filePath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = filePath.FullName,
            UseShellExecute = true
        };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();
    }

    public static void CopyTo(this int[] orignal, int index, int[] source, int count)
    {
        for (int i = 0; i < count; i++)
        {
            source[i] = orignal[i + index];
        }
    }
    public static T[] Copy<T>(this T[] orignal, int index, int count)
        where T: struct
    {
        var result = new T[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = orignal[i + index];
        }
        return result;
    }
    public static T[] Copy<T>(this T[] orignal)
        where T : struct
    {
        return orignal.Copy(0, orignal.Length);
    }
}
