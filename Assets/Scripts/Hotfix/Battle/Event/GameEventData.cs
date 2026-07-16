


public class GameSkillBallOnCollisionEventData : IClassPoolDestroy
{
    public int entityId;
    public GamePhysicsHit hit;
    public void OnPoolDestroy()
    {
        entityId = -1;
        hit = default;
    }
}
public class GameSkillBallRemoveEventData : IClassPoolDestroy
{
    public int ballId;
    public void OnPoolDestroy()
    {
        ballId = -1;
    }
}
public class GameSkillBallAddEventData : IClassPoolDestroy
{
    public int ballId;
    public int star;
    public void OnPoolDestroy()
    {
        ballId
            = star
            = -1;
    }
}
public class GameSkillBallUpStarEventData : IClassPoolDestroy
{
    public int ballId;
    public int star;
    public int addStar;
    public void OnPoolDestroy()
    {
        ballId
            = star
            = addStar
            = -1;
    }
}
public class GameBattleNextWaveEventData : IClassPoolDestroy
{
    public int lastWaveId;
    public int curWaveId;
    public void OnPoolDestroy()
    {
        lastWaveId
            = curWaveId
            = -1;
    }
}