using System.Reflection;
using UnityEngine;


public static class EditorExtend
{
    public static T GetFieldValue<T>(this IEditor obj, string fieldName)
    {
        var result = GetFieldValue(obj, fieldName, default(T));
        return result;
    }
    public static T GetFieldValue<T>(this IEditor obj, string fieldName, T defaultValue)
    {
        var propertyInfo = obj.GetType()
            .GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        var att = propertyInfo.GetCustomAttribute<EditorFieldAttribute>();
        if (att == null)
        {
            Debug.LogError($"获取变量失败， 请使用 {typeof(EditorFieldAttribute)}");
            return defaultValue;
        }

        var baseFieldInfo = EditorUtil.GetBaseFieldInfo(obj, att.fieldName);
        var value = baseFieldInfo.GetValue(obj);

        if (value == null || value.GetType() == typeof(T) || typeof(T).IsAssignableFrom(value.GetType()))
            return (T)value;

        Debug.LogError($"type error   {value.GetType()} != {typeof(T)}");
        return defaultValue;
    }

    public static void SetFieldValue<T>(this IEditor obj, string fieldName, T value)
    {
        var field = EditorUtil.GetBaseFieldInfo(obj, fieldName);
        field.SetValue(obj, value);
    }
}