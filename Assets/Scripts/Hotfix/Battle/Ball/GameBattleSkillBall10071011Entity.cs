using UnityEngine;
using Random = UnityEngine.Random;

public class GameBattleSkillBall10071011EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall10071011Entity, GameBattleSkillBall10071011StarData>, IUpdate
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall10071011Entity;

    private int _LastEntityId;
    private EnDirection _LastEntityDirection;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];
    private float _ExtendRadius = 0;
    private float _EffectScale = 0f;
    private float _LastAtkTime = 0;
    public override void Destroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.Destroy();
        _LastEntityId = -1;
        _LastEntityDirection = EnDirection.None;
        _ExtendRadius
            = _EffectScale
            = _LastAtkTime
            = 0;
    }

    protected override void OnPoolInit(GameBattleSkillBallEntityUserData userData)
    {
        base.OnPoolInit(userData);
        UpdateMgr.Instance.Registener(this);

        var starData = GetStarUserData();
        _EffectScale = starData.radius * 2;
        _LastAtkTime = GameUtility.GetGameTime();
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _GameBattleSkillBallEntity.SetEffectScale(_EffectScale);
        _GameBattleSkillBallEntity.UpdateEffectPos();
    }
    public override void Collision(in GamePhysicsHit hit)
    {
        if (_LastEntityId == hit.entityId)
            if (_LastEntityDirection == hit.dir)
                return;

        _LastEntityId = hit.entityId;
        _LastEntityDirection = hit.dir;
        base.Collision(hit);
    }
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        _ExtendRadius += starData.pernetrateRadius;
        SetEffectScale((_ExtendRadius + starData.radius) * 2);
    }

    public void Update()
    {
        var starData = GetStarUserData();
        var interval = starData.interval;
        var curTime = GameUtility.GetGameTime();
        if (_LastAtkTime + interval > curTime)
            return;
        _LastAtkTime = curTime;
        var radius = starData.radius + _ExtendRadius;
        var center = (Vector2)GetWorldPos();

        var count = GamePhysicsUtility.SphereCastAll(in center, in radius, ref _Hits);
        if (count == 0)
            return;


        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var damageValue = ballAtkValue + ownerAtk + starData.damageValue;

        for (int i = 0; i < starData.atkCount; i++)
        {
            var randomIndex = Random.Range(0, count);
            ref readonly var hit = ref _Hits[randomIndex];
            var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(hit.entityId);
            var monsterCenter = pointInfo.GetCenterPointWorldPos();
            var atkPoint = new Vector2()
            {
                x = Random.Range(-0.3f, 0.3f),
                y = Random.Range(-0.3f, 0.3f),
            } + monsterCenter;

            var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, damageValue, atkPoint, starData.damageType);
            PlayHitAudio(damageResult);
            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_buff_Lightning, atkPoint);

            if (GameMonsterUtility.EntityIsValid(in hit.entityId))
            {
                BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, hit.entityId, starData.buff);

                var evUserData = ClassPoolMgr.Instance.Pull<GameBattleSkillBall_10071011AttackEventData>();
                evUserData.entityId = hit.entityId;
                evUserData.damageType = starData.damageType;
                evUserData.damageValue = damageValue;
                EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALLPART10071011_ATTACK, evUserData);
                ClassPoolMgr.Instance.Push(evUserData);
            }
        }
    }
    public void SetEffectScale(float scale)
    {
        _EffectScale = scale;
        if (IsLoadSuccess)
            _GameBattleSkillBallEntity.SetEffectScale(scale);
    }
}
public class GameBattleSkillBall10071011Entity : GameBattleSkillBallEntity
{
    [SerializeField]
    private GameObject _EffectGo;


    public void UpdateEffectPos()
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Effect);
        var pos = _EffectGo.transform.position;
        pos.z = posZ;
        _EffectGo.transform.position = pos;
    }
    public void SetEffectScale(float scale)
    {
        _EffectGo.transform.localScale = Vector3.one * scale;
    }
}
public class GameBattleSkillBall_10071011AttackEventData : IClassPoolDestroy
{
    public int entityId;
    public int damageValue;
    public EnDamageType damageType;
    public void OnPoolDestroy()
    {
        entityId = -1;
        damageValue = default;
        damageType = EnDamageType.None;
    }
}
