using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Linq;

[CustomEditor(typeof(UISwitchController))]
public class UISwitchControllerEditor : Editor
{
    private UISwitchController m_Taarget = null;
    private int m_MaxSwitchCount = 0;
    private void OnEnable()
    {
        m_Taarget = target as UISwitchController;
        UpdateChildSwitch();
        EditorUtility.SetDirty(m_Taarget);
    }
    public override void OnInspectorGUI()
    {
        var arrlist = m_Taarget.GetArrChildSwitch();
        EditorGUILayout.BeginHorizontal();
        {
            for(var i = 0; i < m_MaxSwitchCount; i++)
            {
                if(GUILayout.Button($"{i}"))
                {
                    SwitchChild(i);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        for (var i = 0; i < arrlist.Count; i++)
        {
            var item = arrlist[i];
            if (GUILayout.Button($"{item}"))
            {
                Selection.activeObject = item;
            }
            m_MaxSwitchCount = Mathf.Max(m_MaxSwitchCount, item.GetSwitchCount());
        }
    }
    private void UpdateChildSwitch()
    {
        var arrChildCom = m_Taarget.GetComponentsInChildren<UISwitch>();
        m_Taarget.SetArrChildSwitch(arrChildCom.ToList());
    }
    private void SwitchChild(int index)
    {
        foreach(var item in m_Taarget.GetArrChildSwitch())
        {
            item.Switch(index);
        }
        EditorUtility.SetDirty(m_Taarget);
    }
}
