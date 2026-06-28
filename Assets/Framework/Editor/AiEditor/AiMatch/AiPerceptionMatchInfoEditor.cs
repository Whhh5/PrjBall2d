using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class AiPerceptionMatchInfoEditor : AiPerceptionMatchInfo, IAiPerceptionMatchInfoEditor
{
    [EditorField(nameof(_PerceptionId))]
    private EnAiPerceptionId _PerceptionId
    {
        get => this.GetFieldValue<EnAiPerceptionId>(nameof(_PerceptionId));
        set => this.SetFieldValue<EnAiPerceptionId>(nameof(_PerceptionId), value);
    }
    [EditorField(nameof(_MatchType))]
    private EnPerceptionMatchType _MatchType
    {
        get => this.GetFieldValue<EnPerceptionMatchType>(nameof(_MatchType));
        set => this.SetFieldValue<EnPerceptionMatchType>(nameof(_MatchType), value);
    }
    [EditorField(nameof(_PerceptionTestData))]
    private IAiPerceptionTestEditor[] _PerceptionTestData
    {
        get => this.GetFieldValue<IAiPerceptionTestEditor[]>(nameof(_PerceptionTestData));
        set => this.SetFieldValue<IAiPerceptionTestEditor[]>(nameof(_PerceptionTestData), value);
    }


    private AiNodeRectIndexInfo _PerceptionTestDataRectIndex;
    private AiNodeRectIndexInfo _MatchTypeRectIndex;
    private AiNodeRectIndexInfo _PerceptionIdRectIndex;
    private AiNodeRectIndexInfo _AddPerceptionTestDataRectIndex;

    private AiNodeRectInfo[] _Rects = null;



    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>();

        Rect testDataRect;
        {
            var offset = new Vector2(0, 0);
            var count = _PerceptionTestData?.Length ?? 0;
            _PerceptionTestDataRectIndex = new AiNodeRectIndexInfo(rectInfos.Count, count);
            for (int i = 0; i < count; i++)
            {
                var itemRect = _PerceptionTestData[i].CalculateLocalNodeArea();
                rectInfos.Add(new AiNodeRectInfo(itemRect, offset));
                offset.x += itemRect.width + 5;
            }

            if (count == 0)
            {
                var addPerTestDataRect = new Rect(Vector2.zero, new Vector2(50, 50));
                _AddPerceptionTestDataRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
                rectInfos.Add(new AiNodeRectInfo(addPerTestDataRect, offset));
            }

            testDataRect = IAiNodeEditor.CalculateRect(rectInfos.ToArray(), Vector4.zero);
        }

        AiNodeRectInfo perIdRectInfo;
        {
            var perIdRect = new Rect(Vector2.zero, new Vector2(150, 30));
            _PerceptionIdRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            perIdRectInfo = new AiNodeRectInfo(perIdRect, new Vector2()
            {
                x = testDataRect.center.x - perIdRect.width / 2,
                y = testDataRect.yMin - 10 - perIdRect.height
            } - EditorConfig.LayerInterval);
            rectInfos.Add(perIdRectInfo);
        }

        {
            var matchTypeRect = new Rect(Vector2.zero, new Vector2(100, 30));
            _MatchTypeRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            var perIdRect = perIdRectInfo.GetRect(Vector2.zero);
            rectInfos.Add(new AiNodeRectInfo(matchTypeRect, new Vector2(perIdRect.xMax, perIdRect.yMin) + new Vector2(10, 0)));
        }


        _Rects = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_Rects, EditorConfig.BoxPadding);
        return thisRect;
    }
    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");

        // nodes
        var perIdRect = _Rects[_PerceptionIdRectIndex.GetIndex()].GetRect(rect.position);
        EditorGuiUtil.DrawEnum(perIdRect, _PerceptionId, value => _PerceptionId = value);
        //
        var matchTypeRect = _Rects[_MatchTypeRectIndex.GetIndex()].GetRect(rect.position);
        EditorGuiUtil.DrawEnum(matchTypeRect, _MatchType, value => _MatchType = value);
        //
        var perTestDataCount = _PerceptionTestData?.Length ?? 0;
        for (var i = 0; i < perTestDataCount; i++)
        {
            var perTestDataRect = _Rects[_PerceptionTestDataRectIndex.GetIndex(i)].GetRect(rect.position);
            _PerceptionTestData[i].DrawNodeArea(perTestDataRect);
        }
        // 
        if (perTestDataCount == 0)
        {
            var addPerTestDataRect = _Rects[_AddPerceptionTestDataRectIndex.GetIndex()].GetRect(rect.position);
            EditorGuiUtil.DrawCreateBtn<EnAiPerceptionTestType>(addPerTestDataRect, value =>
            {
                var newTest = SkillEditorDefine.CreateAiPerceptionTestEditor(value);
                _PerceptionTestData = EditorUtil.AddArrayElement(_PerceptionTestData, newTest);
            });
        }
        // lines
        DrawLines(in rect);
        // other area
        DrawOtherArea(in rect);
    }
    private void DrawOtherArea(in Rect rect)
    {
        // if (!EditorUtil.ViewModelIs(EnEditorViewModelType.Details))
        //     return;

        var perTestDataCount = _PerceptionTestData?.Length ?? 0;
        
        EditorGuiUtil.DrawAreaDelAndAddBtn<EnAiPerceptionTestType>(in rect, perTestDataCount, _Rects, _PerceptionTestDataRectIndex, delIndex =>
        {
            _PerceptionTestData = EditorUtil.RemoveArrayElement(_PerceptionTestData, delIndex);
        }, testType =>
        {
            var newTest = SkillEditorDefine.CreateAiPerceptionTestEditor(testType);
            _PerceptionTestData = EditorUtil.AddArrayElement(_PerceptionTestData, newTest);
        });
    }
    private void DrawLines(in Rect rect)
    {
        var perIdRect = _Rects[_PerceptionIdRectIndex.GetIndex()].GetRect(rect.position);
        var perTestDataCount = _PerceptionTestData?.Length ?? 0;
        // lines
        if (perTestDataCount == 0)
        {
            var addPerTestDataRect = _Rects[_AddPerceptionTestDataRectIndex.GetIndex()].GetRect(rect.position);
            EditorGuiUtil.LinkRectA2b(perIdRect, addPerTestDataRect, 0);
        }
        //
        for (var i = 0; i < perTestDataCount; i++)
        {
            ref readonly var rectInfo = ref _Rects[_PerceptionTestDataRectIndex.GetIndex(i)];
            EditorGuiUtil.LinkRectA2b(perIdRect, rectInfo.GetRect(rect.position), 0);
        }
    }

}