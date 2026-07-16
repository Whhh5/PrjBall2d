using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBallPartPosData : IClassPoolDestroy
{
    public int posEntityId;
    public Vector2 worldPos;
    public int partEntityId;

    public void OnPoolDestroy()
    {
        posEntityId
            = partEntityId
            = -1;
        worldPos = Vector2.zero;
    }
}
public class GameBattleBallPartsMgr : Singleton<GameBattleBallPartsMgr>, IABBEventExecute, IGameMapLifecycle
{
    private readonly Dictionary<int, GameBallPartPosData> _PartPosDatas = new();
    private readonly Dictionary<int, GameBattleSkillBallPartData> _BallPartDatas = new();

    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        ABBEventMgr.Instance.Register(EnABBEvent.EVENT_SKILL_BALL_REMOVE, this);
        ABBEventMgr.Instance.Register(EnABBEvent.EVENT_SKILL_BALL_ONCOLLISION, this);
        ABBEventMgr.Instance.Register(EnABBEvent.EVENT_SKILL_BALL_ADD, this);
        ABBEventMgr.Instance.Register(EnABBEvent.EVENT_SKILL_BALL_UPSTAR, this);
        GameMapMgr.Instance.RegisterLifecycle(this);
    }
    public override void OnDisable()
    {
        base.OnDisable();

        GameMapMgr.Instance.UnregisterLifecycle(this);
        ABBEventMgr.Instance.Unregister(EnABBEvent.EVENT_SKILL_BALL_REMOVE, this);
        ABBEventMgr.Instance.Unregister(EnABBEvent.EVENT_SKILL_BALL_ONCOLLISION, this);
        ABBEventMgr.Instance.Unregister(EnABBEvent.EVENT_SKILL_BALL_ADD, this);
        ABBEventMgr.Instance.Unregister(EnABBEvent.EVENT_SKILL_BALL_UPSTAR, this);
    }

    public EnGameMapLifecycleOrder GetMapLifecycleOrder()
    {
        return EnGameMapLifecycleOrder.GameBattleBallPartsMgr;
    }
    public void EnterMap(int mapId)
    {
        var gridLbPos = GameMapUtility.GetGridLbPosition(mapId);
        var gridRuPos = GameMapUtility.GetGridRuPosition(mapId);
        var gridAreaSize = GameMapUtility.GetBattleAreaBoundSize(mapId);

        var posCount = 6;
        var interval = gridAreaSize.y / (posCount + 1);

        CreateColPartPosData(gridLbPos.x - 0.8f);
        CreateColPartPosData(gridRuPos.x + 0.8f);

        void CreateColPartPosData(float posX)
        {
            for (int i = 0; i < posCount; i++)
            {
                var posY = interval * (i + 1) + gridLbPos.y;
                var data = ClassPoolMgr.Instance.Pull<GameBallPartPosData>();
                data.worldPos = new Vector2(posX, posY);
                _PartPosDatas.Add(_PartPosDatas.Count, data);

                var posEntityId = SceneEntityMgr.Instance.CreateEntityData<GameBalletBallPartPosEntityData>();
                EntityMgr.Instance.SetEntityWorldPos(posEntityId, data.worldPos);
                data.posEntityId = posEntityId;
                EntityMgr.Instance.LoadEntity(posEntityId);
            }
        }
    }

    public void ExitMap(int mapId)
    {
        foreach (var (_, data) in _PartPosDatas)
        {
            SceneEntityMgr.Instance.DestroyEntityData(data.posEntityId);
            ClassPoolMgr.Instance.Push(data);
        }
        _PartPosDatas.Clear();
    }

    public void ReenterMap(int mapId)
    {
    }
    public void EventExecute(EnABBEvent enEvent, int sourceID, int type, IClassPool userData)
    {
        switch (enEvent)
        {
            case EnABBEvent.EVENT_SKILL_BALL_REMOVE:
                {
                    OnEvent_SkillBallRemove(sourceID);
                }
                break;
            case EnABBEvent.EVENT_SKILL_BALL_ADD:
                {
                    var data = userData as GameSkillBallAddEventData;
                    OnEvent_SkillBallStar(in sourceID, in data.star, in data.star);
                }
                break;
            case EnABBEvent.EVENT_SKILL_BALL_UPSTAR:
                {
                    var data = userData as GameSkillBallUpStarEventData;
                    OnEvent_SkillBallStar(in sourceID, in data.addStar, in data.star);
                }
                break;
            case EnABBEvent.EVENT_SKILL_BALL_ONCOLLISION:
                {
                    var data = userData as GameSkillBallOnCollisionEventData;
                    OnEvent_SkillBallOnCollision(in sourceID, in data.entityId, in data.hit);
                }
                break;
            default:
                break;
        }
    }

    private bool TryGetIdlePosIndex(out int posIndex)
    {
        foreach (var (index, data) in _PartPosDatas)
        {
            if (data.partEntityId > 0)
                continue;
            posIndex = index;
            return true;
        }
        posIndex = -1;
        return false;
    }
    public void OnEvent_SkillBallStar(in int ballId, in int addStar, in int star)
    {
        var starData = GameBattleBallMgr.Instance.GetBallStarData<IBattleSkillBallPartInfo>(ballId, star);

        var partCount = starData.GetPartCount();
        for (int i = 0; i < partCount; i++)
        {
            var (partId, partStar) = starData.GetPartInfo(i);

            if (!TryGetIdlePosIndex(out var posIndex))
                break;

            if (_BallPartDatas.TryGetValue(ballId, out var data))
            {
                var curStar = data.GetStar();
                if (curStar != partStar)
                {
                    data.SetStar(partStar);
                }
            }
            else
            {
                var cfg = GameSchedule.Instance.GetSkillBallPartCfg0(partId);
                var ownerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();

                var userData2 = ClassPoolMgr.Instance.Pull<GameBattleSkillBallPartUserData>();
                userData2.posIndex = posIndex;
                userData2.star = partStar;
                userData2.partId = partId;
                data = TypeDefine.CreateInstance<GameBattleSkillBallPartData>(cfg.nClassTypeId, userData2);
                ClassPoolMgr.Instance.Push(userData2);
                _BallPartDatas.Add(ballId, data);

                var posData = _PartPosDatas[posIndex];

                var partEntityId = data.CreatePartEntity(ownerEntityId, ballId, in posData.worldPos);

                posData.partEntityId = partEntityId;
            }
        }
    }
    private void OnEvent_SkillBallRemove(int ballId)
    {
        if (!_BallPartDatas.TryGetValue(ballId, out var data))
            return;

        var posData = _PartPosDatas[data.GetPosIndex()];
        posData.partEntityId = -1;
        ClassPoolMgr.Instance.Push(data);
        _BallPartDatas.Remove(ballId);
    }
    private void OnEvent_SkillBallOnCollision(in int ballId, in int entityId, in GamePhysicsHit hit)
    {
        if (!_BallPartDatas.TryGetValue(ballId, out var data))
            return;
        if (!data.ContainsEntity())
            return;
        var count = data.GetEntityCount();
        for (int i = 0; i < count; i++)
        {
            var partEntityId = data.GetPartEntityId(i);
            var entityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallPartEntityData>(partEntityId);
            entityData.GameBallOnCollision(in entityId, in hit);
        }
    }

    public int GetPartStar(int partId)
    {
        if (!_BallPartDatas.TryGetValue(partId, out var partData))
            return 0;
        var star = partData.GetStar();
        return star;
    }

    public T GetBallPartStarData<T>(int partId)
        where T : ISerializeToIntArray, new()
    {
        var star = GetPartStar(partId);
        var value = GetBallPartStarData<T>(partId, star);
        return value;
    }
    public T GetBallPartStarData<T>(int partId, int star)
    {
        if (!_BallPartDatas.TryGetValue(partId, out var partData))
            return default;
        var starData = partData.GetStarData(star);
        if (starData is not T value)
            return default;
        return value;
    }
    public T CreatePartStarData<T>(int partId, int starLevel, ISerializeToIntArrayUserData userData)
        where T : ISerializeToIntArray, new()
    {
        var ballCfg = GameSchedule.Instance.GetSkillBallPartCfg0(partId);
        var startIndex = 0;
        for (int i = 0; i < starLevel - 1; i++)
        {
            var itemCount = ballCfg.arrStarParams[startIndex++];
            startIndex += itemCount;
        }
        var count = ballCfg.arrStarParams[startIndex++];
        userData.arrParams = ballCfg.arrStarParams;
        userData.startIndex = startIndex;
        userData.count = count;
        var starUserData = ClassPoolMgr.Instance.Pull<T>(userData);
        return starUserData;
    }
}
