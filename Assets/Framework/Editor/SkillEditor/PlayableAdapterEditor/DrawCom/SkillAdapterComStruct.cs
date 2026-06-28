using UnityEngine;

public interface ISkillPlayableAdapterGetStageList
{
    public SkillStageInfoEditor[] GetSkillStageInfoEditors();
    public void SetSkillStageInfoEditors(SkillStageInfoEditor[] data);
}
public interface ISkillPlayableAdapterGetBuffEditor
{
    public IEntityBuffParamsEditor[] GetSkillBuffParamsEditor();
    public void SetSkillBuffParamsEditor(IEntityBuffParamsEditor[] data);
}

public struct SkillDebugModelTimelineInfo
{
    public float skillTime;
    public float skillSlider;
    public float skillMaxTime;
    public float stageStartTime;
    public float stageTime;
    public float stageMaxTime;
    public float stageLocalSlider;
    public readonly float StageLocalSliderToSkillTime(float slider)
    {
        var result = stageStartTime + stageMaxTime * slider;
        return result;
    }
}
public class DrawSkillScheduleInfo
{
    public Rect rect;
}
