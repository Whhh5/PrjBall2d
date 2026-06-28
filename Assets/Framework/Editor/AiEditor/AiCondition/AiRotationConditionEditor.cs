

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AiRotationConditionEditor : AiRotationCondition, IAiConditionEditor
{
    
    [EditorField(nameof(_ConditionType))]
    private EnAiThanConditionType _ConditionType
    {
        get => this.GetFieldValue<EnAiThanConditionType>(nameof(_ConditionType));
        set => this.SetFieldValue<EnAiThanConditionType>(nameof(_ConditionType), value);
    }
    [EditorField(nameof(_Angle))]
    private float _Angle
    {
        get => this.GetFieldValue<float>(nameof(_Angle));
        set => this.SetFieldValue<float>(nameof(_Angle), value);
    }


    private AiNodeRectIndexInfo _ConditionTypeRectIndex;
    private AiNodeRectIndexInfo _AngleRectIndex;
    private AiNodeRectInfo[] _RectInfos = null;


    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");
        
        var conditionRect = _RectInfos[_ConditionTypeRectIndex.GetIndex()].GetRect(rect.position);
        var angleRect = _RectInfos[_AngleRectIndex.GetIndex()].GetRect(rect.position);
        EditorGuiUtil.DrawEnum(conditionRect, _ConditionType, value => _ConditionType = value);
        var value = EditorGUI.FloatField(angleRect, _Angle);
        if (!Mathf.Approximately(value, _Angle))
        {
            _Angle = value;
        }
    }

    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);

        var conditionRect = new Rect(0, 0, 100, 20);
        var AngleRect = new Rect(0, 0, 100, 30);

        _ConditionTypeRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
        rectInfos.Add(new AiNodeRectInfo(conditionRect, Vector2.zero));
        _AngleRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
        rectInfos.Add(new AiNodeRectInfo(AngleRect, conditionRect.position + new Vector2(0, conditionRect.height + 2)));

        _RectInfos = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_RectInfos, EditorConfig.BoxPadding);
        return thisRect;
    }
}