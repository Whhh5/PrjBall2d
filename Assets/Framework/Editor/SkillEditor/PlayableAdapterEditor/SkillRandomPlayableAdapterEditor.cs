
using System;

public partial class SkillRandomPlayableAdapterEditor : SkillRandomPlayableAdapter, ISkillEditor, ISkillPlayableAdapterGetStageList
{
    private SkillPlayableAdapterDrawStageEditor _PlayableAdapterEditor = new();
    public void OnSkillEditorGUI()
    {
        _PlayableAdapterEditor.SetDrawDataList(this);
        _PlayableAdapterEditor.OnSkillEditorGUI();
    }
    
    public SkillStageInfoEditor[] GetSkillStageInfoEditors()
    {
        return _SkillStageList;
    }

    public void SetSkillStageInfoEditors(SkillStageInfoEditor[] data)
    {
        _SkillStageList = data;
    }


    [EditorField(nameof(_SkillStageList))]
    private SkillStageInfoEditor[] _SkillStageList
    {
        get => this.GetFieldValue<SkillStageInfoEditor[]>(nameof(_SkillStageList));
        set => this.SetFieldValue(nameof(_SkillStageList), value);
    }
}