
using UnityEngine;

public class GameBattleSkillBallDestroyEventData : IClassPoolDestroy
{
    public int unvalidEntityId;
    public Vector2 pos;
    public Vector2 moveDir;
    public void OnPoolDestroy()
    {
        unvalidEntityId = -1;
        pos
            = moveDir
            = Vector2.zero;
    }
}