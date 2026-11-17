


using UnityEngine;

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
    public int star;
    public void OnPoolDestroy()
    {
        ballId
            = star
            = -1;
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
public class GameBattleEntityAttributeChangeEventData : IClassPoolDestroy
{
    public float currentValue;

    public void OnPoolDestroy()
    {
        currentValue = -1;
    }
}
public class GameBattleBossHpChangeEventData : IClassPoolDestroy
{
    public float currentValue;

    public void OnPoolDestroy()
    {
        currentValue = -1;
    }
}
public class GameBattlePlayerCreateEventData : IClassPoolDestroy
{
    public int entityId;

    public void OnPoolDestroy()
    {
        entityId = -1;
    }
}
public class GameBattlePlayerDestroyEventData : IClassPoolDestroy
{
    public int entityId;

    public void OnPoolDestroy()
    {
        entityId = -1;
    }
}
public class GameBattleEnergyChangeEventData : IClassPoolDestroy
{
    public int lastValue;
    public int currentValue;
    public int maxValue;

    public void OnPoolDestroy()
    {
        lastValue
            = currentValue
            = maxValue
            = -1;
    }
}
public class GameBattleEnergyUplevelEventData : IClassPoolDestroy
{
    public int currentLevel;
    public int maxEnergyValue;

    public void OnPoolDestroy()
    {
        currentLevel
            = maxEnergyValue
            = -1;
    }
}
public class GameBattleMonsterDieEventData : IClassPoolDestroy
{
    public int entityId;
    public void OnPoolDestroy()
    {
        entityId = -1;
    }
}
public class GameBattleMapReenterEventData : IClassPoolDestroy
{
    public int mapId;
    public void OnPoolDestroy()
    {
        mapId = -1;
    }
}

public class GameBattleMouseClickEventData : IClassPoolDestroy
{
    public Vector2 worldPos;
    public void OnPoolDestroy()
    {
        worldPos = default;
    }
}

public class GameBattleBossDebutEventData : IClassPoolDestroy
{
    public int entityId;
    public void OnPoolDestroy()
    {
        entityId = default;
    }
}

public class GameBattleBossDieEventData : IClassPoolDestroy
{
    public int entityId;
    public void OnPoolDestroy()
    {
        entityId = default;
    }
}

public class GameBattleBall10091010AttackEventData : IClassPoolDestroy
{
    public float width;
    public int damageValue;
    public EnDamageType damageType;
    public int ballEntityId;
    public void OnPoolDestroy()
    {
        width
            = damageValue
            = ballEntityId
            = -1;
        damageType = EnDamageType.None;
    }
}

public class GameBattleBall10021006AttackEventData : IClassPoolDestroy
{
    public int ownerEntityId;
    public int lciclcEntityId;
    public int targetEntityId;
    public int damageValue;
    public int fireIndex;
    public void OnPoolDestroy()
    {
        targetEntityId
            = lciclcEntityId
            = ownerEntityId
            = damageValue
            = fireIndex
            = -1;
    }
}

public class GameBattleBall10011008AttackEventData : IClassPoolDestroy
{
    public int ownerEntityId;
    public int ballEntityId;
    public int damageValue;
    public Vector2 point;

    public void OnPoolDestroy()
    {
        ownerEntityId
            = damageValue
            = ballEntityId
            = -1;
        point = default;
    }
}

public class GameLeachFinishDieEventData : IClassPoolDestroy
{
    public int leechEntityId;
    public int curStoringValue;
    public int depEntityId;

    public void OnPoolDestroy()
    {
        leechEntityId
            = curStoringValue
            = depEntityId
            = -1;
    }
}

public class GameSkillPassivityAddEventData : IClassPoolDestroy
{
    public int skillPassivityId;
    public int star;

    public void OnPoolDestroy()
    {
        skillPassivityId
            = star
            = -1;
    }
}
public class GameSkillPassivityRemoveEventData : IClassPoolDestroy
{
    public int skillPassivityId;

    public void OnPoolDestroy()
    {
        skillPassivityId
            = -1;
    }
}
public class GameSkillPassivityUpstarEventData : IClassPoolDestroy
{
    public int skillPassivityId;
    public int lastStar;
    public int currentStar;

    public void OnPoolDestroy()
    {
        lastStar
            = skillPassivityId
            = currentStar
            = -1;
    }
}

public class GameSkillBallReflectEventData : IClassPoolDestroy
{
    public int ballEntityId;

    public void OnPoolDestroy()
    {
        ballEntityId
            = -1;
    }
}


public class GameCoreAddEventData : IClassPoolDestroy
{
    public int coreId;
    public void OnPoolDestroy()
    {
        coreId = -1;
    }
    public void Setting(in int coreId)
    {
        this.coreId = coreId;
    }
}
public class GameCoreRemoveEventData : IClassPoolDestroy
{
    public int coreId;
    public void OnPoolDestroy()
    {
        coreId = -1;
    }
    public void Setting(in int coreId)
    {
        this.coreId = coreId;
    }
}

public class GameCoreWearEventData : IClassPoolDestroy
{
    public int coreId;
    public int wearIndex;
    public void OnPoolDestroy()
    {
        coreId
            = wearIndex
            = -1;
    }
    public void Setting(in int coreId, in int wearIndex)
    {
        this.coreId = coreId;
        this.wearIndex = wearIndex;
    }
}
public class GameCoreUnwearEventData : IClassPoolDestroy
{
    public int coreId;
    public int lastWearIndex;
    public void OnPoolDestroy()
    {
        coreId
            = lastWearIndex
            = -1;
    }
    public void Setting(in int coreId, in int lastWearIndex)
    {
        this.coreId = coreId;
        this.lastWearIndex = lastWearIndex;
    }
}

public class GameCoreChangeEffectValueEventData : IClassPoolDestroy
{
    public int coreId;
    public float lastEffectValue;
    public float curEffectValue;
    public void OnPoolDestroy()
    {
        lastEffectValue
            = curEffectValue
            = coreId
            = -1;
    }
    public void Setting(in int coreId, in float lastEffectValue, in float curEffectValue)
    {
        this.coreId = coreId;
        this.lastEffectValue = lastEffectValue;
        this.curEffectValue = curEffectValue;
    }
}
public class GameCoreInlayGemEventData : IClassPoolDestroy
{
    public int grooveIndex;
    public int gemId;
    public void OnPoolDestroy()
    {
        grooveIndex
            = gemId
            = -1;
    }
    public void Setting(in int grooveIndex, in int gemId)
    {
        this.grooveIndex = grooveIndex;
        this.gemId = gemId;
    }
}
public class GameCharacterUplevelEventData : IClassPoolDestroy
{
    public int characterId;
    public int lastLevel;
    public int curLevel;
    public void OnPoolDestroy()
    {
        characterId
            = lastLevel
            = curLevel
            = -1;
    }
    public void Setting(in int characterId, in int lastLevel, in int curLevel)
    {
        this.characterId = characterId;
        this.lastLevel = lastLevel;
        this.curLevel = curLevel;
    }
}
public class GameCharacterAptitudeChangeEventData : IClassPoolDestroy
{
    public int lastValue;
    public int curValue;
    public void OnPoolDestroy()
    {
        lastValue
            = curValue
            = -1;
    }
    public void Setting(in int lastValue, in int curValue)
    {
        this.lastValue = lastValue;
        this.curValue = curValue;
    }
}
public class GameGoodsChangeEventData : IClassPoolDestroy
{
    public int lastValue;
    public int curValue;
    public void OnPoolDestroy()
    {
        lastValue
            = curValue
            = -1;
    }
    public void Setting(in int lastValue, in int curValue)
    {
        this.lastValue = lastValue;
        this.curValue = curValue;
    }
}
public class GameAccountLevelChangeEventData : IClassPoolDestroy
{
    public int lastValue;
    public int curValue;
    public void OnPoolDestroy()
    {
        lastValue
            = curValue
            = -1;
    }
    public void Setting(in int lastValue, in int curValue)
    {
        this.lastValue = lastValue;
        this.curValue = curValue;
    }
}
public class GameGemInlayEventData : IClassPoolDestroy
{
    public int coreId;
    public int grooveIndex;
    public void OnPoolDestroy()
    {
        coreId
            = grooveIndex
            = -1;
    }
    public void Setting(in int coreId, in int grooveIndex)
    {
        this.coreId = coreId;
        this.grooveIndex = grooveIndex;
    }
}
