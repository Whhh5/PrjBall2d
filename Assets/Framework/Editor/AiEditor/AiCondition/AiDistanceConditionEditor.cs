
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AiDistanceConditionEditor : AiDistanceCondition, IAiConditionEditor
{
    [EditorField(nameof(_ConditionType))]
    private EnAiThanConditionType _ConditionType
    {
        get => this.GetFieldValue<EnAiThanConditionType>(nameof(_ConditionType));
        set => this.SetFieldValue<EnAiThanConditionType>(nameof(_ConditionType), value);
    }
    [EditorField(nameof(_Distance))]
    private float _Distance
    {
        get => this.GetFieldValue<float>(nameof(_Distance));
        set => this.SetFieldValue<float>(nameof(_Distance), value);
    }

    private AiNodeRectIndexInfo _ConditionTypeRectIndex;
    private AiNodeRectIndexInfo _DistanceRectIndex;
    private AiNodeRectInfo[] _RectInfos = null;


    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");

        var conditionRect = _RectInfos[_ConditionTypeRectIndex.GetIndex()].GetRect(rect.position);
        var distanceRect = _RectInfos[_DistanceRectIndex.GetIndex()].GetRect(rect.position);
        EditorGuiUtil.DrawEnum(conditionRect, _ConditionType, value => _ConditionType = value);
        var value = EditorGUI.FloatField(distanceRect, _Distance);
        if (!Mathf.Approximately(value, _Distance))
        {
            _Distance = value;
        }
    }

    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);

        var conditionRect = new Rect(0, 0, 100, 20);
        var disRect = new Rect(0, 0, 100, 30);

        _ConditionTypeRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
        rectInfos.Add(new AiNodeRectInfo(conditionRect, Vector2.zero));
        _DistanceRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
        rectInfos.Add(new AiNodeRectInfo(disRect, conditionRect.position + new Vector2(0, conditionRect.height + 2)));

        _RectInfos = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_RectInfos, EditorConfig.BoxPadding);
        return thisRect;
    }
}