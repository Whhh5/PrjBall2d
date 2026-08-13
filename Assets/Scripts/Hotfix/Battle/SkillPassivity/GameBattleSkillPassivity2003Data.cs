
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GameBattleSkillPassivity2003StarData : GameBattleSkillPassivityStarData
{
    public float radius;
    public int damageValue;
    public EnDamageType damageType;
    public float attPercentage01;
    public EnEntityAttributeId attId;

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        return list;
    }
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_SkillPassivityName(_SkillPassivityId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_SkillPassivityDescription(_SkillPassivityId),
            radius, damageValue, attPercentage01 * 100, attName, damageTypeName);

    }
}
public class GameBattleSkillPassivity2003Data : GameBattleSkillPassivityData<GameBattleSkillPassivity2003StarData>
{
    private int _DamageValue = 0;
    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];
    public override void OnDisable()
    {
        EventMgr.Instance.Unregister<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);
        _DamageValue = 0;
    }

    public override void OnEnable(int star)
    {
        EventMgr.Instance.Register<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);
    }

    public override void OnUpstar(int lastStar, int curStar)
    {

    }

    private void OnEvent_MonsterDie(in int entityId, in int _, in GameBattleMonsterDieEventData data)
    {
        var starData = GetStarUserData();
        var monsterEntityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(data.entityId);
        var volume = monsterEntityData.GetVolume();
        var radius = starData.radius * volume / 2;
        var attValue = GameAttributeUtility.GetValue(data.entityId, starData.attId);
        var damage = Mathf.CeilToInt(attValue * starData.attPercentage01);
        var worldPos = monsterEntityData.GetCenterPointWorldPos();
        var count = GamePhysicsUtility.SphereCastAll(in worldPos, in radius, ref _Hits);
        for (int i = 0; i < count; i++)
        {
            ref readonly var hit = ref _Hits[i];
            var damageResult = GameAttackMgr.Instance.AttackMonster(in _OwnerEntityId, in hit.entityId, in damage, in starData.damageType);
            _DamageValue += damageResult.value;
        }
        AudioMgr.Instance.Play(EnAudioId.BallPart1002_Effect2);
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_passive_zhakai, worldPos, radius * 2);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_10013();
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_SkillPassivityTipsDescription(_SkillPassivityId),
            _DamageValue);
    }

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillPassivity2003StarData()
            {
                radius = 1.5f,
                damageType = EnDamageType.Physic,
                attPercentage01 = 0.05f,
                attId = EnEntityAttributeId.LimitHp,
            },
            new GameBattleSkillPassivity2003StarData()
            {
                radius = 1.5f,
                damageType = EnDamageType.Physic,
                attPercentage01 = 0.1f,
                attId = EnEntityAttributeId.LimitHp,
            },
            new GameBattleSkillPassivity2003StarData()
            {
                radius = 1.5f,
                damageType = EnDamageType.Physic,
                attPercentage01 = 0.2f,
                attId = EnEntityAttributeId.LimitHp,
            },
        };
    }
}