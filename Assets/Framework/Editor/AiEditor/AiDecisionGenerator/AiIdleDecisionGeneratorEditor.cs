

using System.Collections.Generic;
using UnityEngine;

public class AiIdleDecisionGeneratorEditor : AiIdleDecisionGenerator, IAiDecisionGeneratorEditor
{

    private AiNodeRectIndexInfo _TitleRectIndex;
    private AiNodeRectInfo[] _RectInfos = null;
    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, GetType().ToString());
        var titleRect = _RectInfos[_TitleRectIndex.GetIndex()].GetRect(rect.position);
        if (GUI.Button(titleRect, "闲逛"))
        {
            
        }
    }

    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>();
        var titleRect = new Rect(0, 0, 300, 300);
        _TitleRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
        rectInfos.Add(new AiNodeRectInfo(titleRect, Vector2.zero));

        _RectInfos = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_RectInfos, EditorConfig.BoxPadding);
        return thisRect;
    }
}