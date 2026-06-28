using System.Collections.Generic;
using UnityEngine;



public class AiDecisionSubGeneratorEditor
{
    private AiNodeRectIndexInfo _AiExecuteConditionsRectIndex;
    private AiNodeRectIndexInfo _AddAiExecuteConditionsRectIndex;
    private AiNodeRectIndexInfo _TitleRectIndex;
    private AiNodeRectInfo[] _RectInfos = null;

    private IAiDecisionSubGeneratorEditor _Editor;
    public AiDecisionSubGeneratorEditor(IAiDecisionSubGeneratorEditor editor)
    {
        _Editor = editor;
    }
    private IAiConditionEditor[] _AiExecuteConditions
    {
        get => _Editor.GetConditionsEditor();
        set => _Editor.SetConditionsEditor(value);
    }

    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);

        var count = _AiExecuteConditions?.Length ?? 0;
        var offset = new Vector2() + EditorConfig.LayerInterval;
        _AiExecuteConditionsRectIndex = new AiNodeRectIndexInfo(rectInfos.Count, count);

        for (var i = 0; i < count; i++)
        {
            var conditionRect = _AiExecuteConditions[i].CalculateLocalNodeArea();
            rectInfos.Add(new AiNodeRectInfo(conditionRect, offset));
            offset.x += conditionRect.width + 5;
        }


        if (count == 0)
        {
            var addRect = new Rect(0, 0, 50, 50);
            _AddAiExecuteConditionsRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
            rectInfos.Add(new AiNodeRectInfo(addRect, offset));
        }

        var conditionAreaRect = IAiNodeEditor.CalculateRect(rectInfos.ToArray(), Vector4.zero);

        var titleRect = new Rect(0, 0, 200, 30);
        _TitleRectIndex = new AiNodeRectIndexInfo(rectInfos.Count);
        rectInfos.Add(new AiNodeRectInfo(titleRect, new Vector2()
        {
            x = conditionAreaRect.center.x - titleRect.width / 2,
            y = conditionAreaRect.yMin - titleRect.height,
        } - EditorConfig.LayerInterval));


        _RectInfos = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_RectInfos, EditorConfig.BoxPadding);
        return thisRect;
    }

    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");
        //
        var titleRect = _RectInfos[_TitleRectIndex.GetIndex()].GetRect(rect.position);
        if (GUI.Button(titleRect, _Editor.GetTitle()))
        {
        }

        //
        var count = _AiExecuteConditions?.Length ?? 0;
        for (var i = 0; i < count; i++)
        {
            var conditionRect = _RectInfos[_AiExecuteConditionsRectIndex.GetIndex(i)].GetRect(rect.position);
            _AiExecuteConditions[i].DrawNodeArea(conditionRect);
        }


        DrawLines(in rect);
        DrawOther(in rect);

        //
        if (count == 0)
        {
            var addRect = _RectInfos[_AddAiExecuteConditionsRectIndex.GetIndex()].GetRect(rect.position);
            EditorGuiUtil.DrawCreateBtn<EnAiConditionType>(addRect, AddAiExecuteConditions);
        }
    }

    private void DrawLines(in Rect rect)
    {
        var count = _AiExecuteConditions?.Length ?? 0;
        var titleRect = _RectInfos[_TitleRectIndex.GetIndex()].GetRect(rect.position);
        if (count == 0)
        {
            var addRect = _RectInfos[_AddAiExecuteConditionsRectIndex.GetIndex()].GetRect(rect.position);
            EditorGuiUtil.LinkRectA2b(titleRect, addRect, 0);
        }

        for (var i = 0; i < count; i++)
        {
            var conditionRect = _RectInfos[_AiExecuteConditionsRectIndex.GetIndex(i)].GetRect(rect.position);
            EditorGuiUtil.LinkRectA2b(titleRect, conditionRect, 0);
        }
    }

    private void DrawOther(in Rect rect)
    {
        var count = _AiExecuteConditions?.Length ?? 0;
        EditorGuiUtil.DrawAreaDelAndAddBtn<EnAiConditionType>(in rect, count, _RectInfos, _AiExecuteConditionsRectIndex,
            delIndex => { _AiExecuteConditions = EditorUtil.RemoveArrayElement(_AiExecuteConditions, delIndex); },
            AddAiExecuteConditions);
    }

    private void AddAiExecuteConditions(EnAiConditionType exeType)
    {
        var editor = SkillEditorDefine.CreateAiConditionEditor(exeType);
        _AiExecuteConditions = EditorUtil.AddArrayElement(_AiExecuteConditions, editor);
    }
}