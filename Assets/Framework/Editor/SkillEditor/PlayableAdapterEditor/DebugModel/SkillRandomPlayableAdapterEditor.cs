


using UnityEngine;

public partial class SkillRandomPlayableAdapterEditor : ISkillDebugModelEditor, ISkillDebugModelUpdateEditor
{
    private SkillPlayableAdapterDrawStageDebugModelEditor _DebugModelEditor = new();


    public void OnDebugModelEnterEditor(GameObject go)
    {
        _DebugModelEditor.SetDrawDataList(this);
        _DebugModelEditor.OnDebugModelEnterEditor(go);
    }

    public void OnDebugModelExitEditor()
    {
        _DebugModelEditor.OnDebugModelExitEditor();
    }

    public void OnGuiDebugModelUpdateEditor()
    {
        _DebugModelEditor.OnGuiDebugModelUpdateEditor();
    }
}