using OfficeOpenXml.FormulaParsing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializeToIntArrayUserData : IClassPoolUserData
{
    public int startIndex { get; set; }
    public int count { get; set; }
    public int[] arrParams { get; set; }
}
public interface ISerializeToIntArray<T> : ISerializeToIntArray, IClassPoolInit<ISerializeToIntArrayUserData>
    where T : ISerializeToIntArrayUserData
{
    void IClassPool<ISerializeToIntArrayUserData>.OnPoolInit(ISerializeToIntArrayUserData userData) 
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
        OnPoolInit((T)userData);
    }
    public void OnPoolInit(T userData);
}

public interface ISerializeToIntArray : IClassPoolInit
{
    public EnTypeId GetTypeDefineId();
    public List<int> SerializeToIntArray();
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);

    public static IClassPool GetTypeInstance(in int typeId, in IClassPoolUserData userData)
    {
        var ins = TypeDefine.CreateInstance(in typeId, in userData);
        return ins;
    }
    private static readonly int ClsNull = -2;
    private static readonly int ClsValid = -1;
    public static void ParseToValue(in int[] datas, in int start, in int count, ref int index, out bool result)
    {
        var value = index < count ? datas[start + index++] : default;
        result = value > 0;
    }
    public static void ParseToValue(in int[] datas, in int start, in int count, ref int index, out float result)
    {
        var value = index < count ? datas[start + index++] : default;
        result = value / 100f;
    }
    public static int ParseToValue(in int[] datas, in int start, in int count, in int index)
    {
        var value = index < count ? datas[start + index] : default;
        return value;
    }
    public static void ParseToValue(in int[] datas, in int start, in int count, ref int index, out int result)
    {
        var value = index < count ? datas[start + index++] : default;
        result = value;
    }
    public static void ParseToValue(in int[] datas, in int start, in int count, ref int index, out Vector3 result)
    {
        ParseToValue(in datas, in start, in count, ref index, out result.x);
        ParseToValue(in datas, in start, in count, ref index, out result.y);
        ParseToValue(in datas, in start, in count, ref index, out result.z);
    }
    public static void ParseToValue(in int[] datas, in int start, in int count, ref int index, out int[] result)
    {
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out int arrCount);
        result = new int[arrCount];
        for (int i = 0; i < arrCount; i++)
        {
            ParseToValue(datas, start, count, ref index, out result[i]);
        }
    }
    public static void ParseToValue<T>(in int[] datas, in int start, in int count, ref int index, out T[] result, in ISerializeToIntArrayUserData userData)
        where T : ISerializeToIntArray
    {
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out int arrCount);
        var arr = new T[arrCount];

        for (int i = 0; i < arrCount; i++)
        {
            var localIndex = 0;
            ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out int itemCount);
            ISerializeToIntArray.ParseToValue<T>(in datas, start + index, in itemCount, ref localIndex, out arr[i], in userData);
            index += itemCount;
        }
        result = arr;
    }
    public static void ParseToValue<T>(in int[] datas, in int start, in int count, ref int index, out T result)
        where T : Enum
    {
        if (typeof(T).IsEnum)
        {
            var value = index < count ? datas[start + index++] : default;
            result = (T)Enum.ToObject(typeof(T), value);
        }
        else
        {
            throw new Exception($"ParseToValue not support type:{typeof(T)}");
        }
    }

    public static void ParseToValue<T>(in int[] datas, in int start, in int count, ref int index, out T result, in ISerializeToIntArrayUserData userData)
        where T : ISerializeToIntArray
    {
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out int valid);
        if (valid == ClsNull)
        {
            result = default;
            return;
        }
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out int typeId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out int dataCount);
        userData.arrParams = datas;
        userData.count = dataCount;
        userData.startIndex = start + index;
        var ins = GetTypeInstance(typeId, userData);
        index += dataCount;

        if (ins is T insT)
            result = insT;
        else
            result = default;

    }
    public static void SerializeToInt(ref List<int> data, in int value)
    {
        data.Add(value);
    }
    public static void SerializeToInt(ref List<int> data, in Vector3 value)
    {
        SerializeToInt(ref data, in value.x);
        SerializeToInt(ref data, in value.y);
        SerializeToInt(ref data, in value.z);
    }
    public static void SerializeToInt(ref List<int> data, in bool value)
    {
        data.Add(value ? 1 : 0);
    }
    public static void SerializeToInt(ref List<int> data, in float value)
    {
        var valuef = Mathf.RoundToInt(value * 100);
        data.Add(valuef);
    }
    public static void SerializeToInt(ref List<int> data, in Enum value)
    {
        var valueInt = Convert.ToInt32(value);
        SerializeToInt(ref data, in valueInt);
    }
    public static void SerializeToInt<T>(ref List<int> data, in T value)
        where T : ISerializeToIntArray
    {
        data.Add(value == null ? ClsNull : ClsValid);
        if (value == null)
            return;

        var timelineNodeSer = value?.SerializeToIntArray() ?? new();
        var typeId = value.GetTypeDefineId();
        data.Add((int)typeId);
        data.Add(timelineNodeSer.Count);
        data.AddRange(timelineNodeSer);
    }
    public static void SerializeToInt<T>(ref List<int> data, in T[] value)
        where T : ISerializeToIntArray
    {
        var length = value?.Length ?? 0;
        SerializeToInt(ref data, length);
        for (int i = 0; i < length; i++)
        {
            var list = new List<int>();
            SerializeToInt(ref list, in value[i]);
            data.Add(list.Count);
            data.AddRange(list);
        }
    }
    public static void SerializeToInt(ref List<int> data, in int[] value)
    {
        var length = value?.Length ?? 0;
        SerializeToInt(ref data, length);
        for (int i = 0; i < length; i++)
        {
            SerializeToInt(ref data, value[i]);
        }
    }

    public static void Release<T>(ref T[] result)
    {
        if (typeof(ISerializeToIntArray).IsAssignableFrom(typeof(T)))
        {
            foreach (var item in result)
            {
                if (item is ISerializeToIntArray ser)
                    TypeDefine.DestroyInstance(ser);
            }
        }
        result = null;
    }
    public static void Release<T>(ref T result)
    {
        if (result is ISerializeToIntArray ser)
        {
            TypeDefine.DestroyInstance(ser);
        }
        result = default;
    }
}
public interface IVirtualTimelineNode<T> : IClassPoolInit<T>, IVirtualTimelineNode
    where T : VirtualTimelineUserData
{
}
public interface IVirtualTimelineNode : IClassPoolInit, ISerializeToIntArray
{
    public float GetVirtualTimelineNodeDurationTime();
    public void OnVirtualTimelineNodeUpdate(in float nodeLocalTime, in float deltaTime, in float nodeProgress01);
    public void OnVirtualTimelineNodeStart();
    public bool IsVirtualTimelineNodeEnd(in float nodeLocalTime, in float deltaTime, in float nodeProgress01);
    public void OnVirtualTimelineNodeEnd();
}
public interface IVirtualTimelineEvent : IClassPoolInit, ISerializeToIntArray
{
    public void OnVirtualTimelineEvent();
}
public interface IVirtualTimelineEvent<T> : IVirtualTimelineEvent, IClassPoolInit<T>
    where T : class, IClassPoolUserData
{
}



public interface IVirtualTimeline<T> : IVirtualTimeline, IClassPoolInit<T>
    where T : class, IClassPoolUserData
{
}
public interface IVirtualTimeline : IClassPoolInit, ISerializeToIntArray
{
    public ref readonly float GetVirtualTimelineTotalTime();
    public void OnVirtualTimelineStart(in float localTime);
    public void OnVirtualTimelineUpdtae(in float localTime, in float deltaTime, in float progress01);
    public void OnVirtualTimelineEnd(in float localTime, in float deltaTime);
}
