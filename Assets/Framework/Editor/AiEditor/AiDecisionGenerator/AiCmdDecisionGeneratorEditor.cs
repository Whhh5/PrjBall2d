using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class AiCmdDecisionGeneratorEditor : AiCmdDecisionGenerator, IAiDecisionGeneratorEditor
{
    [EditorField(nameof(_CmdType))]
    private EnEntityCmd _CmdType
    {
        get => this.GetFieldValue<EnEntityCmd>(nameof(_CmdType));
        set => this.SetFieldValue<EnEntityCmd>(nameof(_CmdType), value);
    }
    [EditorField(nameof(_SubDecisionGenerators))]
    private IAiDecisionSubGeneratorEditor[] _SubDecisionGenerators
    {
        get => this.GetFieldValue<IAiDecisionSubGeneratorEditor[]>(nameof(_SubDecisionGenerators));
        set => this.SetFieldValue<IAiDecisionSubGeneratorEditor[]>(nameof(_SubDecisionGenerators), value);
    }
    [EditorField(nameof(_AiPerceptionMatchInfos))]
    private IAiPerceptionMatchInfoEditor[] _AiPerceptionMatchInfos
    {
        get => this.GetFieldValue<IAiPerceptionMatchInfoEditor[]>(nameof(_AiPerceptionMatchInfos));
        set => this.SetFieldValue<IAiPerceptionMatchInfoEditor[]>(nameof(_AiPerceptionMatchInfos), value);
    }


    private AiNodeRectIndexInfo _CmdTypeRectIndex;
    private AiNodeRectIndexInfo _SubDecisionGeneratorsRectIndex;
    private AiNodeRectIndexInfo _AiDecisionGeneratorPerceptionInfosRectIndex;
    private AiNodeRectIndexInfo _AddSubGenRectIndex;
    private AiNodeRectIndexInfo _AddPerInfoRectIndex;
    private AiNodeRectIndexInfo _ArrowRectIndex;
    private AiNodeRectIndexInfo _SubGenRootRectIndex;

    private AiNodeRectInfo[] _Rects = null;
    
    
    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);

        Rect decisionGenPerceptionInfoRect;
        {
            var subGenOffset = new Vector2();
            var count = _AiPerceptionMatchInfos?.Length ?? 0;
            _AiDecisionGeneratorPerceptionInfosRectIndex = new AiNodeRectIndexInfo(rectInfos.Count, count);
            for (var i = 0; i < count; i++)
            {
                var genEditor = _AiPerceptionMatchInfos[i];
                var genRect = genEditor.CalculateLocalNodeArea();
                rectInfos.Add(new AiNodeRectInfo(genRect, subGenOffset));
                subGenOffset.x += genRect.width + 5;
            }

            if (count == 0)
            {
                var addRect = new Rect(0, 0, 50, 50);
                _AddPerInfoRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
                rectInfos.Add(new AiNodeRectInfo(addRect, subGenOffset));
            }

            decisionGenPerceptionInfoRect = IAiNodeEditor.CalculateRect(rectInfos.ToArray(), Vector4.zero);
        }

        var arrowSize = new Vector2(100, 100);
        {
            var arrowRect = new Rect(0, 0, 80, 40);
            _ArrowRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            rectInfos.Add(new AiNodeRectInfo(arrowRect, new Vector2(decisionGenPerceptionInfoRect.xMax + (arrowSize.x - arrowRect.width) / 2, decisionGenPerceptionInfoRect.center.y - arrowRect.height / 2)));
        }

        {
            var cmdRect = new Rect(0, 0, 200, 30);
            var lastRect = decisionGenPerceptionInfoRect;
            var cmdRectOffset = new Vector2(lastRect.center.x - cmdRect.width / 2, lastRect.y - cmdRect.height) - EditorConfig.LayerInterval;
            _CmdTypeRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            rectInfos.Add(new AiNodeRectInfo(cmdRect, cmdRectOffset));
        }

        {
            var startX = decisionGenPerceptionInfoRect.xMax + arrowSize.x;
            var cmdRect = rectInfos[_CmdTypeRectIndex.GetIndex()].GetRect(Vector2.zero);
            var subGenOffset = new Vector2(0, 0) 
                               + new Vector2(startX, cmdRect.yMax)
                               + EditorConfig.LayerInterval;
            var count = _SubDecisionGenerators?.Length ?? 0;
            _SubDecisionGeneratorsRectIndex = new AiNodeRectIndexInfo(rectInfos.Count, count);
            for (var i = 0; i < count; i++)
            {
                var genEditor = _SubDecisionGenerators[i];
                var genRect = genEditor.CalculateLocalNodeArea();
                rectInfos.Add(new AiNodeRectInfo(genRect, subGenOffset));
                subGenOffset.x += genRect.width + 5;
            }

            if (count == 0)
            {
                var addRect = new Rect(0, 0, 50, 50);
                _AddSubGenRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
                rectInfos.Add(new AiNodeRectInfo(addRect, subGenOffset));
            }

            var subGenAreaRect = IAiNodeEditor.CalculateRect(rectInfos.ToArray(), _SubDecisionGeneratorsRectIndex);
            var rootRect = new Rect(0, 0, 100, 30);
            
            _SubGenRootRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            rectInfos.Add(new AiNodeRectInfo(rootRect, new Vector2()
            {
                x = Mathf.Max(startX, subGenAreaRect.center.x - rootRect.width / 2),
                y = subGenAreaRect.yMin - rootRect.height,

            } - EditorConfig.LayerInterval));
        }

        _Rects = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_Rects, EditorConfig.BoxPadding);
        return thisRect;
    }
    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");
        // 
        var cmdRect = _Rects[_CmdTypeRectIndex.GetIndex()].GetRect(rect.position);
        EditorGuiUtil.DrawEnum(cmdRect, _CmdType, value => _CmdType = value);
        //
        var subGenRoot = _Rects[_SubGenRootRectIndex.GetIndex()].GetRect(rect.position);
        if (GUI.Button(subGenRoot, "子行为全部满足"))
        {
            
        }
        //
        var arrowRect = _Rects[_ArrowRectIndex.GetIndex()].GetRect(rect.position);
        GUI.Button(arrowRect, "---->");
        // 
        var perInfoCount = _AiPerceptionMatchInfos?.Length ?? 0;
        for (var i = 0; i < perInfoCount; i++)
        {
            ref readonly var rectInfo = ref _Rects[_AiDecisionGeneratorPerceptionInfosRectIndex.GetIndex(i)];
            _AiPerceptionMatchInfos[i].DrawNodeArea(rectInfo.GetRect(rect.position));
        }

        // 
        var subDecGenCount = _SubDecisionGenerators?.Length ?? 0;
        for (var i = 0; i < subDecGenCount; i++)
        {
            ref readonly var rectInfo = ref _Rects[_SubDecisionGeneratorsRectIndex.GetIndex(i)];
            _SubDecisionGenerators[i].DrawNodeArea(rectInfo.GetRect(rect.position));
        }

        
        DrawLines(in rect);
        DrawOther(in rect);

        if (perInfoCount == 0)
        {
            var addPerInfoRect = _Rects[_AddPerInfoRectIndex.GetIndex()].GetRect(rect.position);
            EditorGuiUtil.DrawCreateBtn<EnAiPerceptionMatchInfoType>(addPerInfoRect, value =>
            {
                var editor = SkillEditorDefine.CreateAiPerceptionMatchInfoEditor(value);
                _AiPerceptionMatchInfos = EditorUtil.AddArrayElement(_AiPerceptionMatchInfos, editor);
            });
        }
        if (subDecGenCount == 0)
        {
            var addSubGenRect = _Rects[_AddSubGenRectIndex.GetIndex()].GetRect(rect.position);
            EditorGuiUtil.DrawCreateBtn<EnAiDecisionSubGeneratorType>(addSubGenRect, value =>
            {
                var editor = SkillEditorDefine.CreateAiDecisionSubGeneratorEditor(value);
                _SubDecisionGenerators = EditorUtil.AddArrayElement(_SubDecisionGenerators, editor);
            });
        }
    }

    private void DrawLines(in Rect rect)
    {        
        var perInfoCount = _AiPerceptionMatchInfos?.Length ?? 0;
        var cmdRect = _Rects[_CmdTypeRectIndex.GetIndex()].GetRect(rect.position);    
        var subDecGenCount = _SubDecisionGenerators?.Length ?? 0;        
        var subGenRoot = _Rects[_SubGenRootRectIndex.GetIndex()].GetRect(rect.position);
        if (perInfoCount == 0)
        {
            var addPerInfoRect = _Rects[_AddPerInfoRectIndex.GetIndex()].GetRect(rect.position);    
            EditorGuiUtil.LinkRectA2b(cmdRect, addPerInfoRect, 0);
        }
        for (var i = 0; i < perInfoCount; i++)
        {
            ref readonly var rectInfo = ref _Rects[_AiDecisionGeneratorPerceptionInfosRectIndex.GetIndex(i)];
            EditorGuiUtil.LinkRectA2b(cmdRect, rectInfo.GetRect(rect.position), 0);
        }        
        for (var i = 0; i < subDecGenCount; i++)
        {
            ref readonly var rectInfo = ref _Rects[_SubDecisionGeneratorsRectIndex.GetIndex(i)];
            EditorGuiUtil.LinkRectA2b(rectInfo.GetRect(rect.position), subGenRoot, 0);
        }
        EditorGuiUtil.LinkRectA2b(subGenRoot, cmdRect, 0);
    }

    private void DrawOther(in Rect rect)
    {        
        var subDecGenCount = _SubDecisionGenerators?.Length ?? 0;
        EditorGuiUtil.DrawAreaDelAndAddBtn<EnAiDecisionSubGeneratorType>(in rect, subDecGenCount, _Rects, _SubDecisionGeneratorsRectIndex, delIndex =>
        {
            _SubDecisionGenerators = EditorUtil.RemoveArrayElement(_SubDecisionGenerators, delIndex);
        }, subGenType =>
        {
            var editor =  SkillEditorDefine.CreateAiDecisionSubGeneratorEditor(subGenType);
            _SubDecisionGenerators = EditorUtil.AddArrayElement(_SubDecisionGenerators, editor);
        });
        
        var matchInfoCount = _AiPerceptionMatchInfos?.Length ?? 0;
        EditorGuiUtil.DrawAreaDelAndAddBtn<EnAiPerceptionMatchInfoType>(in rect, matchInfoCount, _Rects, _AiDecisionGeneratorPerceptionInfosRectIndex, delIndex =>
        {
            _AiPerceptionMatchInfos = EditorUtil.RemoveArrayElement(_AiPerceptionMatchInfos, delIndex);
        }, matchType =>
        {
            var editor =  SkillEditorDefine.CreateAiPerceptionMatchInfoEditor(matchType);
            _AiPerceptionMatchInfos = EditorUtil.AddArrayElement(_AiPerceptionMatchInfos, editor);
        });
    }
    
}