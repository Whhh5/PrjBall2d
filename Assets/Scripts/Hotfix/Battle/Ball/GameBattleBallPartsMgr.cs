using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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
public class GameBattleBallPartsMgr : Singleton<GameBattleBallPartsMgr>, IGameMapLifecycle
{
    private readonly Dictionary<int, GameBallPartPosData> _PartPosDatas = new();
    private readonly Dictionary<int, GameBattleSkillBallPartData> _BallPartDatas = new();
    private readonly Dictionary<int, GameBatteSkillBallPartRuntimeData> _PartId2RuntimeDatas = new();
    private readonly Dictionary<int, List<int>> _BallId2PartIds = new();


    public override void Destroy()
    {
        EventMgr.Instance.Unregister<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);
        EventMgr.Instance.Unregister<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_REMOVE, (int)EnBallType.Skill, 0, OnEvent_SkillBallRemove);
        EventMgr.Instance.Unregister<GameSkillBallOnCollisionEventData>(EnABBEvent.EVENT_SKILL_BALL_ONCOLLISION, 0, 0, OnEvent_SkillBallOnCollision);
        EventMgr.Instance.Unregister<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_SkillBallStar);
        EventMgr.Instance.Unregister<GameSkillBallUpStarEventData>(EnABBEvent.EVENT_SKILL_BALL_UPSTAR, (int)EnBallType.Skill, 0, OnEvent_SkillBallUpstar);
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.Destroy();

    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
        EventMgr.Instance.Register<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_REMOVE, (int)EnBallType.Skill, 0, OnEvent_SkillBallRemove);
        EventMgr.Instance.Register<GameSkillBallOnCollisionEventData>(EnABBEvent.EVENT_SKILL_BALL_ONCOLLISION, 0, 0, OnEvent_SkillBallOnCollision);
        EventMgr.Instance.Register<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_SkillBallStar);
        EventMgr.Instance.Register<GameSkillBallUpStarEventData>(EnABBEvent.EVENT_SKILL_BALL_UPSTAR, (int)EnBallType.Skill, 0, OnEvent_SkillBallUpstar);
        EventMgr.Instance.Register<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);
        GameMapMgr.Instance.RegisterLifecycle(this);
    }

    public EnGameMapLifecycleOrder GetMapLifecycleOrder()
    {
        return EnGameMapLifecycleOrder.GameBattleBallPartsMgr;
    }
    public void EnterMap(int mapId, int chapterId)
    {
        ref readonly var gridLbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var gridRuPos = ref GameMapMgr.Instance.GetGridRuPosition();
        ref readonly var gridAreaSize = ref GameMapMgr.Instance.GetBattleAreaBoundSize();

        var posCount = 6;
        var interval = gridAreaSize.y / (posCount + 1);

        CreateColPartPosData(gridLbPos.x - 0.8f, gridLbPos.y);
        CreateColPartPosData(gridRuPos.x + 0.8f, gridLbPos.y);

        void CreateColPartPosData(float posX, float lbY)
        {
            for (int i = 0; i < posCount; i++)
            {
                var posY = interval * (i + 1) + lbY;
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

    public void ExitMap()
    {
        foreach (var (_, data) in _PartPosDatas)
        {
            SceneEntityMgr.Instance.DestroyEntityData(data.posEntityId);
            ClassPoolMgr.Instance.Push(data);
        }
        _PartPosDatas.Clear();
    }

    public void ReenterMap()
    {
    }
    public List<int> GetPartIds()
    {
        return _PartId2RuntimeDatas.Keys.ToList();
    }
    public GameBattleSkillBallPartData GetPartData(in int partId)
    {
        if (!_BallPartDatas.TryGetValue(partId, out var data))
        {
            var cfg = GameSchedule.Instance.GetSkillBallPartCfg0(partId);

            data = TypeDefine.CreateInstance<GameBattleSkillBallPartData>(cfg.nClassTypeId, null);
            data.SetPartId(partId);
            _BallPartDatas.Add(partId, data);
        }
        return data;
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
    private void SkillBallUplevel(int ballId, int star)
    {
        var starData = GameBattleBallMgr.Instance.GetBallStarData<IBattleSkillBallPartInfo>(ballId, star);

        var partCount = starData.GetPartCount();
        for (int i = 0; i < partCount; i++)
        {
            var (partId, partStar) = starData.GetPartInfo(i);
            AddPart(partId, partStar);

            if (!_BallId2PartIds.TryGetValue(ballId, out var partIds))
            {
                partIds = new();
                _BallId2PartIds.Add(ballId, partIds);
            }
            if (!partIds.Contains(partId))
            {
                partIds.Add(partId);
            }
        }
    }
    public void OnEvent_SkillBallStar(in int nBallType, in int ballId, in GameSkillBallAddEventData data)
    {
        SkillBallUplevel(ballId, data.star);
    }
    public void OnEvent_SkillBallUpstar(in int nBallType, in int ballId, in GameSkillBallUpStarEventData data)
    {
        SkillBallUplevel(ballId, data.star);
    }
    private void OnEvent_SkillBallRemove(in int nBallType, in int ballId, in GameSkillBallRemoveEventData data)
    {
        if (!_BallId2PartIds.TryGetValue(ballId, out var partIds))
            return;
        for (int i = 0; i < partIds.Count; i++)
        {
            var partId = partIds[i];
            RemovePart(partId);
        }
        _BallId2PartIds.Remove(ballId);
    }
    private void OnEvent_SkillBallOnCollision(in int ballId, in int ballEntityId, in GameSkillBallOnCollisionEventData data)
    {
        if (!_BallId2PartIds.TryGetValue(ballId, out var partIds))
            return;
        for (int i = 0; i < partIds.Count; i++)
        {
            var partId = partIds[i];
            if (!_PartId2RuntimeDatas.TryGetValue(partId, out var ballData))
                return;
            if (!ballData.ContainsEntity())
                return;
            var count = ballData.GetEntityCount();
            for (int k = 0; k < count; k++)
            {
                var partEntityId = ballData.GetPartEntityId(k);
                var entityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallPartEntityData>(partEntityId);
                entityData.GameBallOnCollision(in data.entityId, in data.hit);
            }
        }
    }

    public void RemovePart(int partId)
    {
        if (!_PartId2RuntimeDatas.TryGetValue(partId, out var rtData))
            return;
        var partData = GetPartData(partId);
        var entityCount = rtData.GetEntityCount();
        for (int i = 0; i < entityCount; i++)
        {
            var entityId = rtData.GetPartEntityId(i);
            partData.DestroyPartEntity(entityId);
        }

        var posData = _PartPosDatas[rtData.GetPosIndex()];
        posData.partEntityId = -1;
        ClassPoolMgr.Instance.Push(rtData);
        _PartId2RuntimeDatas.Remove(partId);
    }
    public void AddPart(int partId)
    {
        AddPart(partId, 1);
    }
    public void AddPart(int partId, int star)
    {
        var partCfg = GameSchedule.Instance.GetSkillBallPartCfg0(partId);
        if (_PartId2RuntimeDatas.TryGetValue(partId, out var rtData))
        {
            var curStar = rtData.GetStar();
            var toStar = rtData.GetStar() + star;
            if (toStar > partCfg.nMaxStar)
                return;
            if (curStar != toStar)
            {
                rtData.SetStar(toStar);
            }
        }
        else
        {
            if (star > partCfg.nMaxStar)
                return;
            if (TryGetIdlePosIndex(out var posIndex))
            {
                var ownerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();

                var userData2 = ClassPoolMgr.Instance.Pull<GameBattleSkillBallPartRuntimeUserData>();
                userData2.posIndex = posIndex;
                userData2.star = star;
                userData2.partId = partId;
                rtData = ClassPoolMgr.Instance.Pull<GameBatteSkillBallPartRuntimeData>(userData2);
                ClassPoolMgr.Instance.Push(userData2);
                _PartId2RuntimeDatas.Add(partId, rtData);

                var posData = _PartPosDatas[posIndex];
                var partData = GetPartData(partId);
                var partEntityId = partData.CreatePartEntity(ownerEntityId, partId, in posData.worldPos);
                rtData.AddEntityId(partEntityId);
                posData.partEntityId = partEntityId;
            }
        }

    }
    public int GetPartStar(int partId)
    {
        if (!_PartId2RuntimeDatas.TryGetValue(partId, out var partData))
            return 0;
        var star = partData.GetStar();
        return star;
    }
    public bool TryGetCurrentCanUplevelPartIds(ref List<int> uplevelPartIds)
    {
        var partIds = GameBattleBallPartsMgr.Instance.GetPartIds();
        foreach (var partId in partIds)
        {
            var partCfg = GameSchedule.Instance.GetSkillBallPartCfg0(partId);
            var partStar = GameBattleBallPartsMgr.Instance.GetPartStar(partId);
            if (partCfg.nMaxStar == partStar)
                continue;
            uplevelPartIds.Add(partId);
        }
        return uplevelPartIds.Count > 0;
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
        var partData = GetPartData(partId);
        var starData = partData.GetStarData(star);
        if (starData is not T value)
            return default;
        return value;
    }
    public GameBattleSkillBallPartStarData GetBallPartStarData(int partId, int star)
    {
        var partData = GetPartData(partId);
        var starData = partData.GetStarData(star);
        return starData;
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

    private void OnEvent_MouseClick(in int _, in int __, in GameBattleMouseClickEventData data)
    {
        foreach (var (partId, rtData) in _PartId2RuntimeDatas)
        {
            var posIndex = rtData.GetPosIndex();
            var pos = _PartPosDatas[posIndex];
            var dis = Vector3.SqrMagnitude(data.worldPos - pos.worldPos);
            if (dis > 1)
                continue;

            var userData = ClassPoolMgr.Instance.Pull<UIBattleSkillBallPartInfoTipsUserData>();
            userData.partId = partId;
            userData.star = rtData.GetStar();
            UIMgr.Instance.ShowWindow<UIBattleSkillBallPartInfoTipsData>(userData);
            break;
        }
    }
}
