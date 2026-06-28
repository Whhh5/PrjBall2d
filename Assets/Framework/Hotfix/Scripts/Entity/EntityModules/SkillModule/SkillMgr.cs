

public class SkillMgr : Singleton<SkillMgr>
{
    public EnEntitySkillLevel GetSkillLevel(int skillId)
    {
        var cmdCfg = GameSchedule.Instance.GetSkillCfg0(skillId);
        return (EnEntitySkillLevel)cmdCfg.nLevel;
    }
}