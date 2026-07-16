using System.Collections.Generic;
using UnityEngine;


public class GameBattleSkillBallPart_1009EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1009Entity, GameBattleSkillBallPart_1009StarData>, IUpdate
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1009Entity;

    private readonly float _FireRadius = 0.3f;
    private Vector2 _Dir = Vector2.up;
    private float _ChargeSliderValue = 0;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];
    private int _DelayFireCount = 0;
    private Vector2[] _BallPosArray = new Vector2[5];
    private List<int> _TempIntList = new(10);

    private float _LastFireTime;

    public override void Destroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.Destroy();
        _ChargeSliderValue
            = _DelayFireCount
            = 0;
        _Dir = Vector2.up;
    }
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        UpdateMgr.Instance.Registener(this);
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _GameSkillBallPartEntity.SetDirection(in _Dir);
        _GameSkillBallPartEntity.SetChargeSlider(in _ChargeSliderValue);
    }

    public void Update()
    {
        if (_DelayFireCount > 0)
        {
            var starData = GetBallPartStarData();
            var curTime = GameUtility.GetGameTime();
            if (_LastFireTime + starData.atkInterval < curTime)
            {
                Fire();
                _LastFireTime = curTime;
            }
        }
    }
    private bool Fire()
    {
        var ballCount = GameBattleBallMgr.Instance.GetBallPos(_SourceBallId, ref _BallPosArray);
        if (ballCount == 0)
            return false;

        var mapId = GameMapMgr.Instance.GetCurMapId();
        _TempIntList.Clear();
        for (int i = 0; i < ballCount; i++)
        {
            if (!GameMapUtility.IsValidWorldPos(in mapId, in _BallPosArray[i]))
                continue;
            _TempIntList.Add(i);
        }
        if (_TempIntList.Count == 0)
            return false;
        var target = _BallPosArray[_TempIntList[Random.Range(0, _TempIntList.Count - 1)]];
        var dir = (target - (Vector2)WorldPos).normalized;

        SetDirection(dir);

        var starData = GetBallPartStarData();

        // 发射激光
        var point1 = dir * _FireRadius + (Vector2)WorldPos;
        var validEage = dir.x > 0
            ? EnDirection.Top | EnDirection.Bottom | EnDirection.Right
            : EnDirection.Top | EnDirection.Bottom | EnDirection.Left;
        var pointCount = GameMapUtility.GetMapEagePoint(mapId, in point1, in dir, validEage, ref _Hits);
        if (pointCount == 0)
            return false;
        var point2 = _Hits[0].point;

        var count = GamePhysicsUtility.TryLineCast(in point1, in point2, starData.width / 2, ref _Hits);
        if (count == 0)
            return false;

        var effectAtkDamage = starData.damageValue + Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        for (int i = 0; i < count; i++)
        {
            ref readonly var atkHit = ref _Hits[i];
            GameAttackMgr.Instance.Attack(_OwnerEntityId, atkHit.entityId, effectAtkDamage, atkHit.point, in starData.damageType);
        }

        var effectEntityId = SceneEntityMgr.Instance.CreateEntityData<GameEffectLaserEntityData>();
        var effectEntityData = EntityMgr.Instance.GetEntityData<GameEffectLaserEntityData>(effectEntityId);
        effectEntityData.SetPoint2(point1, point2);
        effectEntityData.SetLaserWidth(starData.width);
        EntityMgr.Instance.SetEntityWorldPos(effectEntityId, WorldPos);
        EntityMgr.Instance.LoadEntity(effectEntityId);

        RemoveChargeCount(1);

        return true;
    }
    private void AddChargeCount(int value)
    {
        var starData = GetBallPartStarData();
        _DelayFireCount = Mathf.Min(starData.maxChargeCount, _DelayFireCount + value);

        var sliderValue = _DelayFireCount / (float)starData.maxChargeCount;
        SetChargeSlider(sliderValue);
    }
    private void RemoveChargeCount(int value)
    {
        var starData = GetBallPartStarData();
        _DelayFireCount = Mathf.Max(0, _DelayFireCount - value); ;

        var sliderValue = _DelayFireCount / (float)starData.maxChargeCount;
        SetChargeSlider(sliderValue);
    }
    public void SetDirection(in Vector2 dir)
    {
        _Dir = dir;
        if (IsLoadSuccess)
            _GameSkillBallPartEntity.SetDirection(in dir);
    }
    public void SetChargeSlider(in float value)
    {
        _ChargeSliderValue = value;
        if (IsLoadSuccess)
            _GameSkillBallPartEntity.SetChargeSlider(in value);
    }

    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
        var starData = GetBallPartStarData();
        AddChargeCount(starData.chargeCount);
    }
}
public class GameBattleSkillBallPart_1009Entity : GameBattleSkillBallPartEntity
{
    [SerializeField, AutoReference]
    private Transform _DirTran;
    [SerializeField, AutoReference]
    private SpriteMask2DSlider _ChargeSlider;

    public void SetDirection(in Vector2 dir)
    {
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _DirTran.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void SetChargeSlider(in float slider)
    {
        _ChargeSlider.SetSliderValue(slider);
    }
}
