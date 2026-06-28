

using UnityEngine;

public class AiLookAtDecisionSubGeneratorEditor : AiLookAtDecisionSubGenerator, IAiDecisionSubGeneratorEditor
{    
    public AiLookAtDecisionSubGeneratorEditor()
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
        return "看向目标";
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