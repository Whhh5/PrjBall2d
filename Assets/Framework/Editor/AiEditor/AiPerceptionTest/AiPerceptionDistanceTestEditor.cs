
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AiPerceptionDistanceTestEditor : AiPerceptionDistanceTest, IAiPerceptionTestEditor
{
    [EditorField(nameof(_MinDistance))]
    private float _MinDistance
    {
        get => this.GetFieldValue<float>(nameof(_MinDistance));
        set => this.SetFieldValue<float>(nameof(_MinDistance), value);
    }
    [EditorField(nameof(_MaxDistance))]
    private float _MaxDistance
    {
        get => this.GetFieldValue<float>(nameof(_MaxDistance));
        set => this.SetFieldValue<float>(nameof(_MaxDistance), value);
    }

    private AiNodeRectInfo[] _NodeRects;
    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);
        var minDisRect = new Rect(0, 0, 30, 30);
        rectInfos.Add(new(minDisRect, Vector2.zero));
        var maxDisRect = new Rect(0, 0, 30, 30);
        rectInfos.Add(new(maxDisRect, new Vector2(minDisRect.width + 40, 0)));

        _NodeRects = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_NodeRects, EditorConfig.BoxPadding);
        return thisRect;
    }

    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");

        var rectInfoIndex = 0;
        var minDis = EditorGUI.FloatField(_NodeRects[rectInfoIndex++].GetRect(rect.position), _MinDistance);
        if (minDis != _MinDistance)
            _MinDistance = minDis;
        var maxDis = EditorGUI.FloatField(_NodeRects[rectInfoIndex++].GetRect(rect.position), _MaxDistance);
        if (maxDis != _MaxDistance)
            _MaxDistance = maxDis;
    }
}