using System.Collections.Generic;
using UnityEngine;

public class AiPerceptionInfosEditor : AiPerceptionInfos, IAiNodeEditor
{
    public AiPerceptionInfosEditor(int aiCfgId)
    {
        _AiCfgId = aiCfgId;
    }
    [EditorField(nameof(_Perceptions))]
    private IAiPerceptionEditor[] _Perceptions
    {
        get => this.GetFieldValue<IAiPerceptionEditor[]>(nameof(_Perceptions));
        set => this.SetFieldValue(nameof(_Perceptions), value);
    }

    private AiNodeRectIndexInfo _PerceptionsRectIndex;
    private AiNodeRectIndexInfo _AddBtnRectIndex;
    private AiNodeRectInfo[] _RectInfos = null;
    private int _AiCfgId;
    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");
        
        var count = _Perceptions?.Length ?? 0;
        for (var i = 0; i < count; i++)
        {
            var itemRect = _RectInfos[_PerceptionsRectIndex.GetIndex(i)].GetRect(rect.position);
            _Perceptions[i].DrawNodeArea(itemRect);
        }

        var addBtnRect = _RectInfos[_AddBtnRectIndex.GetIndex()].GetRect(rect.position);
        EditorGuiUtil.DrawEnum<EnAiPerceptionId>(addBtnRect, "监视器", perceptionId =>
        {
            var addPerceptionEditor = SkillEditorDefine.CreateAiPerceptionEditor(perceptionId);
            _Perceptions = EditorUtil.AddArrayElement(_Perceptions, addPerceptionEditor);
        });
        
        // lines
        for (var i = 0; i < count; i++)
        {
            var itemRect = _RectInfos[_PerceptionsRectIndex.GetIndex(i)].GetRect(rect.position);
            EditorGuiUtil.LinkRectA2b(addBtnRect, itemRect, 0);
        }
    }

    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);
        var count = _Perceptions?.Length ?? 0;
        _PerceptionsRectIndex = new AiNodeRectIndexInfo(rectInfos.Count, count);
        var offset = new Vector2();
        for (var i = 0; i < count; i++)
        {
            var rect = _Perceptions[i].CalculateLocalNodeArea();
            rectInfos.Add(new AiNodeRectInfo(rect, offset));
            offset.x += rect.width + 5;
        }

        var perceptionArea = IAiNodeEditor.CalculateRect(rectInfos.ToArray(), _PerceptionsRectIndex);

        var addRect = new Rect(0, 0, 100, 30);
        var btnOffset = new Vector2()
        {
            x = perceptionArea.center.x - addRect.width / 2,
            y = perceptionArea.yMin - addRect.height,
        };
        _AddBtnRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
        rectInfos.Add(new AiNodeRectInfo(addRect, btnOffset - EditorConfig.LayerInterval));

        _RectInfos = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_RectInfos, EditorConfig.BoxPadding);
        return thisRect;
    }
}