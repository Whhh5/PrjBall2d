using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UISpriteSwitch))]
public class UISpriteSwitchEditor : Editor 
{
    private UISpriteSwitch m_Target;
    private List<UISpriteSwitchItem> m_ArrSprite => m_Target.GetArrSprite();
    private void OnEnable()
    {
        m_Target = target as UISpriteSwitch;
    }
    public override void OnInspectorGUI()
    {
        DrawTopBtn();
        DrawArrSpriteItem();

    }
    private void DrawTopBtn()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add", GUILayout.Width(100)))
            {
                m_ArrSprite.Add(new() { });
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    private void DrawArrSpriteItem()
    {
        for (int i = 0; i < m_ArrSprite.Count; i++)
        {
            DrawSpriteItem(i);
        }
    }
    private void DrawSpriteItem(int index)
    {
        var item = m_ArrSprite[index];

        EditorGUILayout.BeginHorizontal();
        {
            if(GUILayout.Button($"{index}", GUILayout.Width(20)))
            {
                m_Target.Switch(index);
                // SceneView.RepaintAll();
                // Canvas.ForceUpdateCanvases();
                // AssetDatabase.Refresh();
                EditorUtility.SetDirty(m_Target);
            }
            var sprite = EditorGUILayout.ObjectField(item.GetSprite(), typeof(Sprite), false, GUILayout.Width(150)) as Sprite;
            item.SetSprite(sprite);
            var color = EditorGUILayout.ColorField(item.GetColor(), GUILayout.Width(50));
            item.SetColor(color);
            if(GUILayout.Button("✖️", GUILayout.Width(20)))
            {
                m_ArrSprite.RemoveAt(index);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
