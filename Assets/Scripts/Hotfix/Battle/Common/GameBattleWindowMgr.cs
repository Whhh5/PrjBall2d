using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public interface IBattleEnergyUplevelItemSelect
{
    public void Init(int mapId);
    public void Destroy();
    public void Reset();
    public bool IsValid();
    public int GetTestValue();
    public bool TrySelect(out IUIBattleUplevelWindow_ItemUserData userData, bool isIgnoreHistory, HashSet<int> extraIgnoree);
}
public class GameBattleWindowMgr : Singleton<GameBattleWindowMgr>, IGameMapLifecycle
{
    private readonly Dictionary<EnUIBattleUplevelWindowItemType, IBattleEnergyUplevelItemSelect> _UplevelItemSelects = new()
    {
        { EnUIBattleUplevelWindowItemType.SkillBall, new GameBattleUplevelSkillBallItemSelect() },
        { EnUIBattleUplevelWindowItemType.SkillPassivity, new GameBattleUplevelSkillPassivityItemSelect() },
        { EnUIBattleUplevelWindowItemType.SkillBallPart, new GameBattleUplevelSkillBallPartItemSelect() },
        { EnUIBattleUplevelWindowItemType.Attribute, new GameBattleUplevelAttributeItemSelect() },
        { EnUIBattleUplevelWindowItemType.SkillBallFussion, new GameBattleUplevelSkillBallFussionItemSelect() },
    };
    private readonly Dictionary<EnUIBattleUplevelWindowItemType, int> _ItemTestValues = new()
    {
        { EnUIBattleUplevelWindowItemType.SkillBall, 10 },
        { EnUIBattleUplevelWindowItemType.SkillPassivity, 10 },
        { EnUIBattleUplevelWindowItemType.SkillBallPart, 10 },
        { EnUIBattleUplevelWindowItemType.Attribute, 1 },
        { EnUIBattleUplevelWindowItemType.SkillBallFussion, 10 },
    };
    private int _DelayUplevelCount = 0;
    private int _UplevelWindowId = -1;
    private int _UplevelEntityId = -1;
    private Vector2 _UplevelEntityWorldPos;
    private List<UIBattleUplevelWindowItemData> _LastUplevelWindowDataList = null;
    private int _BossInfoDialogWindowId = -1;

    private CancellationTokenSource _TokenSource = null;

    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        EventMgr.Instance.Register<GameBattleEnergyUplevelEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, OnEvent_EnergyUplevel);
        GameMapMgr.Instance.RegisterLifecycle(this);
    }
    public override void OnDisable()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        EventMgr.Instance.Unregister<GameBattleEnergyUplevelEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, OnEvent_EnergyUplevel);
        base.OnDisable();
    }

    public EnGameMapLifecycleOrder GetMapLifecycleOrder() => EnGameMapLifecycleOrder.GameBattleMgr;

    public void EnterMap(int mapId)
    {
        _TokenSource = new();
        foreach (var item in _UplevelItemSelects)
            item.Value.Init(mapId);

        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        _UplevelEntityWorldPos = new Vector2(mapCfg.arrUplevelPos[0], mapCfg.arrUplevelPos[1]);

        _UplevelEntityId = SceneEntityMgr.Instance.CreateEntityData<BattleUplevelEntityData>();
        EntityMgr.Instance.SetEntityWorldPos(_UplevelEntityId, _UplevelEntityWorldPos);
        EntityMgr.Instance.LoadEntity(_UplevelEntityId);

        EventMgr.Instance.Register<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);
        EventMgr.Instance.Register<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_SkillBallAdd);
        EventMgr.Instance.Register<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_SkillBallRemove);

        EventMgr.Instance.Register<GameBattleBossDebutEventData>(EnABBEvent.EVENT_BATTLE_BOSS_DEBUT, 0, 0, OnEvent_BossDebut);
    }

    public void ExitMap(int mapId)
    {
        _TokenSource.Cancel();
        EventMgr.Instance.Unregister<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_SkillBallAdd);
        EventMgr.Instance.Unregister<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_SkillBallRemove);
        EventMgr.Instance.Unregister<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);

        EventMgr.Instance.Unregister<GameBattleBossDebutEventData>(EnABBEvent.EVENT_BATTLE_BOSS_DEBUT, 0, 0, OnEvent_BossDebut);

        if (_BossInfoDialogWindowId > 0)
            UIMgr.Instance.HideWindow(_BossInfoDialogWindowId);

        SceneEntityMgr.Instance.DestroyEntityData(_UplevelEntityId);
        if (UIMgr.Instance.IsValid(_UplevelWindowId))
        {
            UIMgr.Instance.HideWindow(_UplevelWindowId);
        }
        if (_LastUplevelWindowDataList != null)
        {
            foreach (var item in _LastUplevelWindowDataList)
            {
                ClassPoolMgr.Instance.Push(item);
            }
        }

        foreach (var item in _UplevelItemSelects)
            item.Value.Destroy();
        _DelayUplevelCount = 0;
        _UplevelWindowId = -1;
        _UplevelEntityId = -1;
        _LastUplevelWindowDataList = null;
        _TokenSource = null;
        _BossInfoDialogWindowId = -1;
    }

    public void ReenterMap(int mapId)
    {
        _TokenSource.Cancel();
        _TokenSource = new();
        if (UIMgr.Instance.IsValid(_UplevelWindowId))
        {
            UIMgr.Instance.HideWindow(_UplevelWindowId);
        }

        if (_LastUplevelWindowDataList != null)
        {
            foreach (var item in _LastUplevelWindowDataList)
            {
                ClassPoolMgr.Instance.Push(item);
            }
        }
        foreach (var item in _UplevelItemSelects)
        {
            item.Value.Destroy();
            item.Value.Init(mapId);
        }

        if (_BossInfoDialogWindowId > 0)
            UIMgr.Instance.HideWindow(_BossInfoDialogWindowId);
        _BossInfoDialogWindowId = -1;
        _DelayUplevelCount = 0;
        _UplevelWindowId = -1;
        _LastUplevelWindowDataList = null;

        SceneEntityMgr.Instance.DestroyEntityData(_UplevelEntityId);
        _UplevelEntityId = SceneEntityMgr.Instance.CreateEntityData<BattleUplevelEntityData>();
        EntityMgr.Instance.SetEntityWorldPos(_UplevelEntityId, _UplevelEntityWorldPos);
        EntityMgr.Instance.LoadEntity(_UplevelEntityId);
    }

    private void OnEvent_EnergyUplevel(in int _, in int __, in GameBattleEnergyUplevelEventData data)
    {
        _DelayUplevelCount++;
        var entityData = EntityMgr.Instance.GetEntityData<BattleUplevelEntityData>(_UplevelEntityId);
        entityData.SetCount(_DelayUplevelCount);

        if (_DelayUplevelCount == 1)
        {
            ShowWindow();
        }
        else
        {
            PlayEnergyAnimAsync();
        }
    }
    public int GetUplevelCount()
    {
        return _DelayUplevelCount;
    }
    public void ReduceDelatUplevelCount(int count)
    {
        _DelayUplevelCount -= count;
    }

    public void ShowWindow()
    {
        if (_DelayUplevelCount <= 0)
            return;
        if (UIMgr.Instance.IsValid(_UplevelWindowId))
            return;
        _DelayUplevelCount--;
        _UplevelWindowId = ShowUplevelWindow();

        TimeMgr.Instance.SetPause(true, false);
        EventMgr.Instance.Register(EnABBEvent.EVENT_WINDOW_HIDE, _UplevelWindowId, 0, OnEvent_WindowHide);
    }

    private void OnEvent_WindowHide(in int windowId, in int nHideType)
    {
        EventMgr.Instance.Unregister(EnABBEvent.EVENT_WINDOW_HIDE, _UplevelWindowId, 0, OnEvent_WindowHide);

        var hideType = (EnWindowHideType)nHideType;
        if (hideType == EnWindowHideType.SelectItem)
        {
            var entityData = EntityMgr.Instance.GetEntityData<BattleUplevelEntityData>(_UplevelEntityId);
            entityData.SetCount(_DelayUplevelCount);

            if (_DelayUplevelCount > 0)
            {
                _DelayUplevelCount--;
                _UplevelWindowId = ShowUplevelWindow();
                EventMgr.Instance.Register(EnABBEvent.EVENT_WINDOW_HIDE, _UplevelWindowId, 0, OnEvent_WindowHide);
            }
            else
            {
                TimeMgr.Instance.SetPause(false, false);
            }
        }
        else
        {
            // 缓存数据
            _DelayUplevelCount++;
            var uplevelWindow = UIMgr.Instance.GetWindow<UIBattleUplevelWindowData>(_UplevelWindowId);
            _LastUplevelWindowDataList = uplevelWindow.CopyItemData();
            PlayEnergyAnimAsync();
            TimeMgr.Instance.SetPause(false, false);
        }
    }

    public List<UIBattleUplevelWindowItemData> GetUplevelWindowItemData()
    {
        var result = new List<UIBattleUplevelWindowItemData>();
        foreach (var item in _UplevelItemSelects)
            item.Value.Reset();

        var probabilityMap = new List<(EnUIBattleUplevelWindowItemType itemType, int testValue)>();
        var maxTestValue = 0;
        for (var i = EnUIBattleUplevelWindowItemType.None + 1; i < EnUIBattleUplevelWindowItemType.EnumCount; i++)
        {
            var selectItem = _UplevelItemSelects[i];
            if (!selectItem.IsValid())
                continue;
            var testValue = selectItem.GetTestValue() * _ItemTestValues[i];
            probabilityMap.Add((i, testValue));
            maxTestValue += testValue;
        }

        probabilityMap.Sort((item1, item2) =>
        {
            if (item1.testValue < item2.testValue)
                return -1;
            if (item1.testValue > item2.testValue)
                return 1;
            return 0;
        });

        while (result.Count < GameGlobal.UpLevelItemCount)
        {
            var testIndex = Random.Range(0, maxTestValue);
            var index = -1;
            for (int i = 0; i < probabilityMap.Count; i++)
            {
                var testInfo = probabilityMap[i];
                if (testIndex >= testInfo.testValue)
                {
                    testIndex -= testInfo.testValue;
                    continue;
                }
                index = i;
                break;
            }
            if (index < 0)
                break;
            
            var (itemType, testValue) = probabilityMap[index];
            var itemSelect = _UplevelItemSelects[itemType];
            if (!itemSelect.IsValid() || !itemSelect.TrySelect(out var userData, true, null))
            {
                maxTestValue -= testValue;
                probabilityMap.RemoveAt(index);
                if (probabilityMap.Count == 0)
                    break;
                continue;
            }

            var itemData = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindowItemData>();
            itemData.itemType = itemType;
            itemData.userData = userData;
            result.Add(itemData);
        }
        return result;
    }
    private int ShowUplevelWindow()
    {
        var windowUserData = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindowUserData>();
        if (_LastUplevelWindowDataList != null)
        {
            windowUserData.itemDataList = _LastUplevelWindowDataList;
            _LastUplevelWindowDataList = null;
        }
        else
        {
            var itemDatas = GetUplevelWindowItemData();
            windowUserData.itemDataList = itemDatas;
        }

        var windowId = UIMgr.Instance.ShowWindow<UIBattleUplevelWindowData>(windowUserData);
        ClassPoolMgr.Instance.Push(windowUserData);
        return windowId;
    }

    private async void PlayEnergyAnimAsync()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var fromPos = new Vector2(mapCfg.arrEnergyCollisionPoint[0], mapCfg.arrEnergyCollisionPoint[1]);
        var toPos = _UplevelEntityWorldPos;

        var eff1EntityId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_battle_energy_1, fromPos);
        AudioMgr.Instance.Play(EnAudioId.Reward2);
        var isCancel = await DOTween.To(() => 0f, slider =>
        {
            var posOffsetY = Mathf.Sin(Mathf.PI * slider);
            var pos = Vector2.Lerp(fromPos, toPos, slider) + Vector2.up * posOffsetY;
            EntityMgr.Instance.SetEntityWorldPos(eff1EntityId, pos);

        }, 1f, 1f)
            .ToUniTask(cancellationToken: _TokenSource.Token).SuppressCancellationThrow();
        if (!isCancel)
        {
            AudioMgr.Instance.Play(EnAudioId.Reward1);
            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_battle_energy_2, toPos);
        }
        GameEffectMgr.Instance.DestroyEffect(eff1EntityId);
    }


    private void OnEvent_MouseClick(in int _, in int __, in GameBattleMouseClickEventData data)
    {
        if (Vector2.SqrMagnitude(data.worldPos - _UplevelEntityWorldPos) < 1)
        {
            ShowWindow();
        }

    }
    private void OnEvent_SkillBallAdd(in int nBallType, in int ballId, in GameSkillBallAddEventData data)
    {
        if (_LastUplevelWindowDataList != null)
        {
            foreach (var item in _LastUplevelWindowDataList)
            {
                ClassPoolMgr.Instance.Push(item);
            }
            _LastUplevelWindowDataList = null;
        }
    }
    private void OnEvent_SkillBallRemove(in int nBallType, in int ballId, in GameSkillBallRemoveEventData data)
    {
        if (_LastUplevelWindowDataList != null)
        {
            foreach (var item in _LastUplevelWindowDataList)
            {
                ClassPoolMgr.Instance.Push(item);
            }
            _LastUplevelWindowDataList = null;
        }
    }

    private void OnEvent_BossDebut(in int _, in int __, in GameBattleBossDebutEventData data)
    {
        var windowUserData = ClassPoolMgr.Instance.Pull<UIBattleBossInfoDialogUserData>();
        windowUserData.bossEntityId = data.entityId;
        _BossInfoDialogWindowId = UIMgr.Instance.ShowWindow<UIBattleBossInfoDialogData>(windowUserData);
        ClassPoolMgr.Instance.Push(windowUserData);
    }
}
