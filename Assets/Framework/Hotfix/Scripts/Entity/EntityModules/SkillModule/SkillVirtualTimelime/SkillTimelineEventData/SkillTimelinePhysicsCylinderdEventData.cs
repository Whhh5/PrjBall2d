public abstract class SkillTimelinePhysicsEventData: IClassPoolDestroy
{
    public int attackerEntityId;
    public int hitEntityId;
    public int skillId;
    public int timelineEventId;
    public virtual void OnPoolDestroy()
    {
        attackerEntityId
            = hitEntityId
            = skillId
            = timelineEventId
            = -1;
    }
}
public class SkillTimelinePhysicsCylinderdEventData : SkillTimelinePhysicsEventData
{
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
}