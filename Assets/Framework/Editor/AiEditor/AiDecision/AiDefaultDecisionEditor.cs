using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public partial class AiDefaultDecisionEditor : AiDefaultDecision, IAiDecisionEditor
{
    [EditorField(nameof(_DecisionInfos))]
    private AiDecisionGeneratorInfoEditor[] _DecisionInfos
    {
        get => this.GetFieldValue<AiDecisionGeneratorInfoEditor[]>(nameof(_DecisionInfos));
        set => this.SetFieldValue(nameof(_DecisionInfos), value);
    }


    private AiNodeRectIndexInfo _DecisionInfosRectIndex;
    private AiNodeRectIndexInfo _AddBtnRectIndex;
    private AiNodeRectInfo[] _Rects = null;

    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);


        var localOffset = new Vector2() + EditorConfig.LayerInterval;
        var decInfoCount = _DecisionInfos?.Length ?? 0;
        _DecisionInfosRectIndex = new AiNodeRectIndexInfo(rectInfos.Count, decInfoCount);
        for (var i = 0; i < decInfoCount; i++)
        {
            var rect = _DecisionInfos[i].CalculateLocalNodeArea();
            rectInfos.Add(new AiNodeRectInfo(rect, localOffset));
            localOffset.x += rect.width + 5;
        }

        if (decInfoCount == 0)
        {
            var addRect = new Rect(0, 0, 50, 50);
            _AddBtnRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            rectInfos.Add(new AiNodeRectInfo(addRect, localOffset));
        }

        _Rects = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_Rects, EditorConfig.BoxPadding);
        return thisRect;
    }
    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");
        var decInfoCount =  _DecisionInfos?.Length ?? 0;
        for (var i = 0; i < decInfoCount; i++)
        {
            ref readonly var rectInfo = ref _Rects[_DecisionInfosRectIndex.GetIndex(i)];
            _DecisionInfos[i].DrawNodeArea(rectInfo.GetRect(rect.position));
        }


        DrawOther(in rect);


        if (decInfoCount == 0)
        {
            var addBtnRect = _Rects[_AddBtnRectIndex.GetIndex()].GetRect(rect.position);
            if (EditorGuiUtil.DrawCreateBtn(addBtnRect))
            {
                _DecisionInfos = EditorUtil.AddArrayElement(_DecisionInfos, new AiDecisionGeneratorInfoEditor());
            }
        }
    }

    private void DrawOther(in Rect rect)
    {
        var decInfoCount =  _DecisionInfos?.Length ?? 0;
        EditorGuiUtil.DrawAreaDelAndAddBtn(in rect, decInfoCount, _Rects, _DecisionInfosRectIndex, delIndex =>
        {
            _DecisionInfos = EditorUtil.RemoveArrayElement(_DecisionInfos, delIndex);
        }, () =>
        {
            _DecisionInfos = EditorUtil.AddArrayElement(_DecisionInfos, new AiDecisionGeneratorInfoEditor());
        });
    }
}