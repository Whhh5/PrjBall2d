using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public static class AssemblyUtil
{
	public static List<(Type type, T attribute)> GetTypesByAttribute<T>(Assembly assenbly)
		where T: Attribute
    {
		var result = new List<(Type, T)> ();
		var types = assenbly.GetTypes();
		foreach (var type in types)
		{
			var atts = type.GetCustomAttribute(typeof(T));
			if (atts == null)
				continue;
			result.Add((type, atts as T));
        }
		return result;
    }
}

