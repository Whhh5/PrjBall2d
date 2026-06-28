using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AiTransformDecisionSubGeneratorEditor : AiTransformDecisionSubGenerator,
    IAiDecisionSubGeneratorEditor
{
    public AiTransformDecisionSubGeneratorEditor()
    {
        _BaseEditor = new AiDecisionSubGeneratorEditor(this);
    }

    [EditorField(nameof(_AiExecuteConditions))]
    private IAiConditionEditor[] _AiExecuteConditions
    {
        get => this.GetFieldValue<IAiConditionEditor[]>(nameof(_AiExecuteConditions));
        set => this.SetFieldValue<IAiConditionEditor[]>(nameof(_AiExecuteConditions), value);
    }

    private readonly AiDecisionSubGeneratorEditor _BaseEditor = null;

    public void DrawNodeArea(in Rect rect)
    {
        _BaseEditor.DrawNodeArea(in rect);
    }

    public Rect CalculateLocalNodeArea()
    {
        var thisRect = _BaseEditor.CalculateLocalNodeArea();
        return thisRect;
    }

    public string GetTitle()
    {
        return "朝向目标移动";
    }

    public IAiConditionEditor[] GetConditionsEditor()
    {
        return _AiExecuteConditions;
    }

    public void SetConditionsEditor(IAiConditionEditor[] conditionEditors)
    {
        _AiExecuteConditions = conditionEditors;
    }
}