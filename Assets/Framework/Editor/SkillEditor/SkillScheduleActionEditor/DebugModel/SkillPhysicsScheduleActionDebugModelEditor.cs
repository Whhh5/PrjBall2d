using UnityEngine;


public partial class SkillPhysicsScheduleActionEditor : ISkillScheduleActionDebugModelEditor
{
    private Rect _BoxRect = default;
    private Rect _StageRect = default;

    public void OnDebugModelEnterEditor(GameObject go)
    {
        if (_PhysicsResolve is IPhysicsResolveDebugModelEditor physicsResolveDebugModelEditor)
            physicsResolveDebugModelEditor.OnDebugModelEnterEditor(go);
    }

    public void OnDebugModelExitEditor()
    {
        if (_PhysicsResolve is IPhysicsResolveDebugModelEditor physicsResolveDebugModelEditor)
            physicsResolveDebugModelEditor.OnDebugModelExitEditor();
    }

    public void OnSceneDebugModelUpdateEditor(in SkillDebugModelTimelineInfo timeInfo)
    {
        if (_PhysicsResolve is IPhysicsResolveDebugModelEditor physicsResolveDebugModelEditor)
            physicsResolveDebugModelEditor.OnSceneUpdateEditor(in _StageRect, in _BoxRect, in timeInfo, _AtkSchedule);
    }

    #region gui
    public void OnGuiDebugModelUpdateEditor(in Rect stageRect, in int i, ref int ctrlId)
    {
        var (schedule, boxRect) = SkillDebugModelUtil.DrawStageScheduleActionBox(this, in stageRect, _AtkSchedule, i, ref ctrlId);
        _BoxRect = boxRect;
        _StageRect = stageRect;
        if (schedule != _AtkSchedule)
        {
            _AtkSchedule = schedule;
        }

    }
    #endregion
}