
internal class EventSkillAttackData : IClassPoolDestroy
{
    public int attackerEntityId;
    public int hitEntityId;
    public int value;
    public void OnPoolDestroy()
    {
        attackerEntityId
            = hitEntityId
            = -1;
    }
}
