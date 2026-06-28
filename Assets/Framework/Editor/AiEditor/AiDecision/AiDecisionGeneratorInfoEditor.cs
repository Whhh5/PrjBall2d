using System.Collections.Generic;
using UnityEngine;

public partial class AiDecisionGeneratorInfoEditor : AiDecisionGeneratorInfo, IAiNodeEditor
{
    [EditorField(nameof(_DecisionLevel))]
    private EnDecisionLevel _DecisionLevel
    {
        get => this.GetFieldValue<EnDecisionLevel>(nameof(_DecisionLevel));
        set => this.SetFieldValue(nameof(_DecisionLevel), value);
    }

    [EditorField(nameof(_DecisionGenerator))]
    private IAiDecisionGeneratorEditor _DecisionGenerator
    {
        get => this.GetFieldValue<IAiDecisionGeneratorEditor>(nameof(_DecisionGenerator));
        set => this.SetFieldValue<IAiDecisionGeneratorEditor>(nameof(_DecisionGenerator), value);
    }

    private AiNodeRectIndexInfo _DecisionLevelRectIndex;
    private AiNodeRectIndexInfo _DecisionGeneratorRectIndex;
    private AiNodeRectIndexInfo _AddDecisionGenRectIndex;
    private AiNodeRectInfo[] _Rects = null;

    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");

        var levelRect = _Rects[_DecisionLevelRectIndex.GetIndex()].GetRect(rect.position);


        EditorGuiUtil.DrawEnum(levelRect, _DecisionLevel, value => _DecisionLevel = value);

        if (_DecisionGenerator != null)
        {
            var decisionRectInfo = _Rects[_DecisionGeneratorRectIndex.GetIndex()];
            var decisionRect = decisionRectInfo.GetRect(rect.position);
            _DecisionGenerator.DrawNodeArea(in decisionRect);

            EditorGuiUtil.LinkRectA2b(levelRect, decisionRect, 0);
        }
        else
        {
            var addRectInfo = _Rects[_AddDecisionGenRectIndex.GetIndex()];
            var addRect = addRectInfo.GetRect(rect.position);
            
            EditorGuiUtil.DrawCreateBtn<EnAiDecisionGeneratorType>(addRect, value =>
            {
                _DecisionGenerator = SkillEditorDefine.CreateAiDecisionGeneratorEditor(value);
            });
            EditorGuiUtil.LinkRectA2b(levelRect, addRect, 0);
        }
    }

    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);

        {
            var offset = new Vector2();
            if (_DecisionGenerator != null)
            {
                var decisionRect = _DecisionGenerator.CalculateLocalNodeArea();
                _DecisionGeneratorRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
                rectInfos.Add(new AiNodeRectInfo(decisionRect, offset));
            }
            else
            {
                var addRect = new Rect(0, 0, 200, 30);
                _AddDecisionGenRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
                rectInfos.Add(new AiNodeRectInfo(addRect, offset));
            }
        }

        {
            var levelRect = new Rect(0, 0, 200, 30);
            var lastRect = IAiNodeEditor.CalculateRect(rectInfos.ToArray(), Vector4.zero);
            _DecisionLevelRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            rectInfos.Add(new AiNodeRectInfo(levelRect, new Vector2()
            {
                x = lastRect.center.x - levelRect.width / 2,
                y = lastRect.yMin - 10 - levelRect.height
            } - EditorConfig.LayerInterval));
        }

        _Rects = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_Rects, EditorConfig.BoxPadding);
        return thisRect;
    }
}