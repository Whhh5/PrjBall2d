
// 1. 自定义契约解析器：让序列化器默认包含私有字段
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

public class PrivateFieldContractResolver : DefaultContractResolver
{
    public override JsonContract ResolveContract(Type type)
    {
        var property = base.ResolveContract(type);
        // property.me
        // if (member is FieldInfo)
        // {
        //     property.ShouldSerialize = _ => true; // 强制序列化该字段
        //     property.Ignored = false; // 不忽略该字段
        // }
        return property;
    }

    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        var list = base.GetSerializableMembers(objectType);
        // list.Add(new memberinfo);

        //var fieldInfos = objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        //for (int i = 0; i < fieldInfos.Length; i++)
        //{
        //    var filedInfo = fieldInfos[i];
        //    if (list.Find(item => item.Name == filedInfo.Name) != null)
        //        continue;
        //    list.Add(filedInfo);
        //}
        return list;
    }
    protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
    {
        var props = new List<JsonProperty>();

        // 获取所有字段（包括 private 和 public）
        var baseType = type;

        while (baseType != null)
        {
            foreach (var field in baseType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (props.Find(item => item.PropertyName == field.Name) != null)
                    continue;
                var prop = base.CreateProperty(field, memberSerialization);
                prop.Readable = true;
                prop.Writable = true;
                props.Add(prop);
            }
            baseType = baseType.BaseType;
        }

        // 获取所有属性（可选，通常只需要字段）
        //foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        //{
        //    var prop = base.CreateProperty(property, memberSerialization);
        //    prop.Readable = true;
        //    prop.Writable = true;
        //    props.Add(prop);
        //}

        return props;
    }
    protected override JsonStringContract CreateStringContract(Type objectType)
    {
        return base.CreateStringContract(objectType);
    }
}