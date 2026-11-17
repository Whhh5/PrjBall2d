using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIHallMainCharacterWindowData : UIWindowData<UIHallMainCharacterWindow>
{
    public override EnUIWindowType WindowType => EnUIWindowType.SubWindow;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallMainCharacterWindow;

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        var activeCharacterId = UICharacterUtility.GetActiveCharacterId();
        _UIWindow.Setting(activeCharacterId);
    }
}
public class UIHallMainCharacterWindow : UIWindow
{
    [SerializeField, AutoReference]
    private RectTransform _AttributeRootRect;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _NameTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _LevelTxt;
    [SerializeField, AutoReference]
    private Button _UplevelBtn;
    [SerializeField, AutoReference]
    private GameObject _MaxLevelGo;
    [SerializeField, AutoReference]
    private GameObject _LockGo;
    [SerializeField, AutoReference]
    private Button _UnlockBtn;
    [SerializeField, AutoReference]
    private Button _LeftBtn;
    [SerializeField, AutoReference]
    private Button _RightBtn;
    [SerializeField, AutoReference]
    private Button _ActiveBtn;
    [SerializeField, AutoReference]
    private GameObject _ActiveGo;
    [SerializeField, AutoReference]
    private RectTransform _AptitudeItemRootRect;
    [SerializeField, AutoReference]
    private Button _AptitudeBtn;
    [SerializeField, AutoReference]
    private Button _RefreshAptitudeBtn;
    [SerializeField, AutoReference]
    private RectTransform _AptitudeBtnDirRect;
    [SerializeField, AutoReference]
    private RectTransform _AptitudeNeedGoodsTipsRootRect;
    [SerializeField, AutoReference]
    private RectTransform _UplevelNeedGoodsTipsRootRect;
    [SerializeField, AutoReference]
    private RectTransform _CharacterRootRect;

    private readonly Dictionary<EnEntityAttributeId, UIHallMainCharacterWindow_AttributeItem> _AttItems = new();
    private readonly Dictionary<EnEntityAttributeId, UIHallMainCharacterWindow_AptitudeItem> _AptitudeItems = new();
    private readonly Dictionary<int, UIGoodsItemCountLeftTip> _AptitudeNeedGoodsItems = new();
    private readonly Dictionary<int, UIGoodsItemCountLeftTip> _UplevelNeedGoodsItems = new();
    private readonly Dictionary<int, int> _Character2Index = new();
    private CancellationTokenSource _CreateToken;
    private CancellationTokenSource _CreateAptiudeToken;
    private int _CurCharacterId = -1;
    private int _CurCharacterEntityId;
    private bool _IsShowAptitude = false;
    public override void OnUnload()
    {
        if (_CreateToken != null)
        {
            _CreateToken.Cancel();
            _CreateToken = null;
        }
        if (_CreateAptiudeToken != null)
        {
            _CreateAptiudeToken.Cancel();
            _CreateAptiudeToken = null;
        }
        foreach (var (_, item) in _AttItems)
            UIMgr.Instance.UnloadWindowItem(item);
        foreach (var (_, item) in _AptitudeItems)
            UIMgr.Instance.UnloadWindowItem(item);
        foreach (var (_, item) in _AptitudeNeedGoodsItems)
            UIMgr.Instance.UnloadWindowItem(item);
        foreach (var (_, item) in _UplevelNeedGoodsItems)
            UIMgr.Instance.UnloadWindowItem(item);

        _UplevelBtn.RemoveListener(OnClick_UplvelBtn);
        _UnlockBtn.RemoveListener(OnClick_UnlockBtn);
        _LeftBtn.RemoveListener(OnClick_LeftBtn);
        _RightBtn.RemoveListener(OnClick_RightBtn);
        _ActiveBtn.RemoveListener(OnClick_ActiveBtn);
        _AptitudeBtn.RemoveListener(OnClick_AptitudeBtn);
        _RefreshAptitudeBtn.RemoveListener(OnClick_RefreshAptitudeBtn);

        EventMgr.Instance.Unregister(EnABBEvent.EVENT_CHARACTER_UNLOCK, OnEvent_CharacterUnlock);
        EventMgr.Instance.Unregister<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, OnEvent_GoodsChange);
        if (_CurCharacterId > 0)
        {
            SceneEntityMgr.Instance.RemoveCom(_CurCharacterEntityId, EnSceneEntityComType.Animation);
            SceneEntityMgr.Instance.RemoveCom(_CurCharacterEntityId, EnSceneEntityComType.Cmd);
            SceneEntityMgr.Instance.DestroyEntityData(_CurCharacterEntityId);
            EventMgr.Instance.Unregister<GameCharacterUplevelEventData>(EnABBEvent.EVENT_CHARACTER_UPLEVEL, _CurCharacterId, 0, OnEvent_CharacterUplevel);
            EventMgr.Instance.Unregister<GameCharacterAptitudeChangeEventData>(EnABBEvent.EVENT_CHARACTER_APTITUDE_CHANGE, _CurCharacterId, 0, OnEvent_CharacterAptitude);
            EventMgr.Instance.Unregister(EnABBEvent.EVENT_CHARACTER_ACTIVE, _CurCharacterId, 0, OnEvent_CharacterActive);
            EventMgr.Instance.Unregister(EnABBEvent.EVENT_CHARACTER_APTITUDE_REFRESH, _CurCharacterId, 0, OnEvent_CharacterAptitudeRefresh);
        }
        base.OnUnload();
        _AttItems.Clear();
        _CurCharacterId = -1;
        _CurCharacterEntityId = -1;
        _Character2Index.Clear();
        _AptitudeItems.Clear();
        _AptitudeNeedGoodsItems.Clear();
        _UplevelNeedGoodsItems.Clear();
        _IsShowAptitude = false;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _UplevelBtn.AddListener(OnClick_UplvelBtn);
        _UnlockBtn.AddListener(OnClick_UnlockBtn);
        _LeftBtn.AddListener(OnClick_LeftBtn);
        _RightBtn.AddListener(OnClick_RightBtn);
        _ActiveBtn.AddListener(OnClick_ActiveBtn);
        _AptitudeBtn.AddListener(OnClick_AptitudeBtn);
        _RefreshAptitudeBtn.AddListener(OnClick_RefreshAptitudeBtn);

        EventMgr.Instance.Register(EnABBEvent.EVENT_CHARACTER_UNLOCK, OnEvent_CharacterUnlock);
        EventMgr.Instance.Register<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, OnEvent_GoodsChange);

        var count = GameSchedule.Instance.GetCharacterCfgCount();
        for (int i = 0; i < count; i++)
        {
            var cfg = GameSchedule.Instance.GetCharacterCfg(i);
            _Character2Index.Add(cfg.nCharacterID, i);
        }
    }
    private void OnClick_UplvelBtn()
    {
        var needGoodsList = UICharacterUtility.GetUplevelNeedGoodsInfos(_CurCharacterId);
        foreach (var item in needGoodsList)
        {
            UIGoodsMgr.Instance.RemoveGoods(item.goodsCfgId, item.goodsCount);
        }

        var level = UICharacterUtility.GetLevel(_CurCharacterId);
        var toLevel = level + 1;
        UICharacterMgr.Instance.SetLevel(_CurCharacterId, toLevel);
    }
    private void OnClick_UnlockBtn()
    {
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(_CurCharacterId);
        var goodsCfgId = characterCfg.arrUnlockGoods[0];
        var goodsCount = characterCfg.arrUnlockGoods[1];
        var goodsInfos = new List<GoodsInfo>(characterCfg.arrUnlockGoods.Length / 2 + 1);
        for (int i = 0; i < characterCfg.arrUnlockGoods.Length; i += 2)
        {
            var goodsInfo = new GoodsInfo(characterCfg.arrUnlockGoods[i], characterCfg.arrUnlockGoods[i + 1]);
            goodsInfos.Add(goodsInfo);
        }

        var content = LanguageUtility.GetLanguage(30106);
        UIUtil.ShowControllerDialog(content, EnSourceId.UIHallMainCharacterWindow, () =>
        {
            for (int i = 0; i < goodsInfos.Count; i++)
            {
                var goodsInfo = goodsInfos[i];
                var curCount = UIGoodsMgr.Instance.GetGoodsCount(goodsInfo.goodsCfgId);
                if (curCount < goodsInfo.goodsCount)
                {
                    UIUtil.ShowHitTips(30107);
                    return;
                }
            }
            for (int i = 0; i < goodsInfos.Count; i++)
            {
                var goodsInfo = goodsInfos[i];
                UIGoodsMgr.Instance.RemoveGoods(goodsInfo.goodsCfgId, goodsInfo.goodsCount);
            }

            UICharacterMgr.Instance.SetIsLock(_CurCharacterId, false);

        }, () =>
        {

        }, goodsInfos);
    }
    private void OnClick_LeftBtn()
    {
        var curIndex = _Character2Index[_CurCharacterId];
        var cfg = GameSchedule.Instance.GetCharacterCfg(curIndex - 1);
        UpdateCharacterInfo(cfg.nCharacterID);
    }
    private void OnClick_RightBtn()
    {
        var curIndex = _Character2Index[_CurCharacterId];
        var cfg = GameSchedule.Instance.GetCharacterCfg(curIndex + 1);
        UpdateCharacterInfo(cfg.nCharacterID);
    }
    private void OnClick_ActiveBtn()
    {
        UICharacterMgr.Instance.SetActiveCharacterId(_CurCharacterId);
    }
    private void OnClick_AptitudeBtn()
    {
        _IsShowAptitude = !_IsShowAptitude;
        if (_IsShowAptitude)
            UpdateAptitudeInfo();
        UpdateAptitudeStatus();
    }
    private void OnClick_RefreshAptitudeBtn()
    {
        var needGoodsList = UICharacterUtility.GetAptitudeNeedGoodsInfos(_CurCharacterId);
        foreach (var item in needGoodsList)
        {
            UIGoodsMgr.Instance.RemoveGoods(item.goodsCfgId, item.goodsCount);
        }
        UICharacterMgr.Instance.RefreshAptitude(_CurCharacterId);
    }
    private void OnEvent_CharacterUnlock(in int characterId, in int _)
    {
        var level = UICharacterUtility.GetLevel(characterId);
        SetLevel(level);
        UpdateAttributeInfo();
        UpdateAptitudeInfo();
        UpdateButtonStatus();
        UpdateAptitudeStatus();
        UpdateEntityUnlockStatus();
    }
    private void OnEvent_CharacterUplevel(in int characterId, in int _, in GameCharacterUplevelEventData data)
    {
        SetLevel(data.curLevel);
        UpdateAttributeInfo();
        UpdateButtonStatus();
    }
    private void OnEvent_CharacterActive(in int characterId, in int _)
    {
        UpdateButtonStatus();
    }

    private void OnEvent_CharacterAptitudeRefresh(in int characterId, in int _)
    {
        UpdateAptitudeInfo();
        UpdateAttributeInfo();
    }

    private void OnEvent_CharacterAptitude(in int characterId, in int nAttId, in GameCharacterAptitudeChangeEventData data)
    {
        var attId = (EnEntityAttributeId)nAttId;
        if (!_AptitudeItems.TryGetValue(attId, out var item))
            return;
        item.SetSliderValue(data.curValue);
    }
    private void OnEvent_GoodsChange(in int goodsCfgId, in int _, in GameGoodsChangeEventData data)
    {
        if (_UplevelNeedGoodsItems.ContainsKey(goodsCfgId))
        {
            UpdateButtonStatus();
        }
        if (_AptitudeNeedGoodsItems.ContainsKey(goodsCfgId))
        {
            UpdateAptitudeStatus();
        }
    }

    private void UpdateEntityUnlockStatus()
    {
        var isLock = UICharacterUtility.IsLock(_CurCharacterId);
        var data = EntityMgr.Instance.GetEntityData<GameUICharacterEntityData>(_CurCharacterEntityId);
        data.SetIsUnlock(!isLock);
    }

    public void Setting(int characterId)
    {
        UpdateCharacterInfo(characterId);
    }
    private void SetLevel(int level)
    {
        var isLock = UICharacterUtility.IsLock(_CurCharacterId);
        var levelStr = LanguageUtility.GetLanguage_10033(isLock ? LanguageUtility.GetLanguage_10035() : level + 1);
        _LevelTxt.SetText(levelStr);
    }
    private void UpdateCharacterInfo(int characterId)
    {
        if (characterId <= 0 || characterId == _CurCharacterId)
            return;

        if (_CurCharacterId > 0)
        {
            SceneEntityMgr.Instance.RemoveCom(_CurCharacterEntityId, EnSceneEntityComType.Animation);
            SceneEntityMgr.Instance.RemoveCom(_CurCharacterEntityId, EnSceneEntityComType.Cmd);
            SceneEntityMgr.Instance.DestroyEntityData(_CurCharacterEntityId);

            EventMgr.Instance.Unregister(EnABBEvent.EVENT_CHARACTER_ACTIVE, _CurCharacterId, 0, OnEvent_CharacterActive);
            EventMgr.Instance.Unregister(EnABBEvent.EVENT_CHARACTER_APTITUDE_REFRESH, _CurCharacterId, 0, OnEvent_CharacterAptitudeRefresh);
            EventMgr.Instance.Unregister<GameCharacterUplevelEventData>(EnABBEvent.EVENT_CHARACTER_UPLEVEL, _CurCharacterId, 0, OnEvent_CharacterUplevel);
            EventMgr.Instance.Unregister<GameCharacterAptitudeChangeEventData>(EnABBEvent.EVENT_CHARACTER_APTITUDE_CHANGE, _CurCharacterId, 0, OnEvent_CharacterAptitude);
        }
        _CurCharacterId = characterId;
        EventMgr.Instance.Register<GameCharacterUplevelEventData>(EnABBEvent.EVENT_CHARACTER_UPLEVEL, characterId, 0, OnEvent_CharacterUplevel);
        EventMgr.Instance.Register<GameCharacterAptitudeChangeEventData>(EnABBEvent.EVENT_CHARACTER_APTITUDE_CHANGE, _CurCharacterId, 0, OnEvent_CharacterAptitude);
        EventMgr.Instance.Register(EnABBEvent.EVENT_CHARACTER_ACTIVE, _CurCharacterId, 0, OnEvent_CharacterActive);
        EventMgr.Instance.Register(EnABBEvent.EVENT_CHARACTER_APTITUDE_REFRESH, _CurCharacterId, 0, OnEvent_CharacterAptitudeRefresh);

        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(characterId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterId);

        _CurCharacterEntityId = SceneEntityMgr.Instance.CreateEntityData(characterCfg.nUIClassTypeId);
        EntityMgr.Instance.SetEntityWorldPos(_CurCharacterEntityId, _CharacterRootRect.position);
        EntityMgr.Instance.SetEntityScale(_CurCharacterEntityId, _CharacterRootRect.localScale);
        EntityMgr.Instance.LoadEntity(_CurCharacterEntityId);

        SceneEntityMgr.Instance.AddCom<EntityCmdComData>(_CurCharacterEntityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.AddCom<EntityCmdComData>(_CurCharacterEntityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.AddCmd(_CurCharacterEntityId, monsterCfg.nIdleCmdID);
        SceneEntityMgr.Instance.AddCmd(_CurCharacterEntityId, monsterCfg.nFlyDown);

        var level = UICharacterUtility.GetLevel(characterId);
        SetLevel(level);
        _NameTxt.SetLanguageText(monsterCfg.nNameLangId);
        if (_IsShowAptitude)
        {
            UpdateAptitudeInfo();
        }
        UpdateButtonStatus();
        UpdateAptitudeStatus();
        UpdateAttributeInfo();
        UpdateEntityUnlockStatus();
    }

    private async void UpdateAttributeInfo()
    {
        _CreateToken?.Cancel();
        _CreateToken = new();
        foreach (var item in _UplevelNeedGoodsItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        _UplevelNeedGoodsItems.Clear();

        var isLock = UICharacterUtility.IsLock(_CurCharacterId);

        var needGoodsInfos = UICharacterUtility.GetUplevelNeedGoodsInfos(_CurCharacterId);
        for (int i = 0; i < needGoodsInfos.Count; i++)
        {
            var goodsInfo = needGoodsInfos[i];
            var goodsCount = isLock ? -1 : goodsInfo.goodsCount;
            if (!_UplevelNeedGoodsItems.TryGetValue(goodsInfo.goodsCfgId, out var tipsItem))
            {
                var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountLeftTip>(EnLoadTarget.Pre_UIGoodsItemCountLeftTip, _UplevelNeedGoodsTipsRootRect, _CreateToken.Token)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                item.Setting(new(goodsInfo.goodsCfgId, goodsCount), EnSourceId.UIHallMainCharacterWindow);
                tipsItem = item;
                _UplevelNeedGoodsItems.Add(goodsInfo.goodsCfgId, tipsItem);
            }
            else
            {
                tipsItem.SetCount(goodsCount);
            }
            if (!isLock)
            {
                var curGoodsCount = UIGoodsMgr.Instance.GetGoodsCount(goodsInfo.goodsCfgId);
                var isPass = curGoodsCount >= goodsCount;
                tipsItem.SetComValid(isPass);
            }
        }


        var list = UICharacterUtility.GetAttributeValues(_CurCharacterId);
        foreach (var info in list)
        {
            var value = isLock
                ? LanguageUtility.GetLanguage_10035()
                : LanguageUtility.GetLanguage_AttributeValue(info.attId, info.value);
            if (!_AttItems.TryGetValue(info.attId, out var item))
            {
                var (isCancel, attItem) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainCharacterWindow_AttributeItem>(EnLoadTarget.Pre_UIHallMainCharacterWindow_AttributeItem, _AttributeRootRect, _CreateToken.Token)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                attItem.Setting(info.attId, value);
                item = attItem;
                _AttItems.Add(info.attId, item);
            }
            else
            {
                item.SetValue(value);
            }
        }
        _CreateToken = null;
    }
    private async void UpdateAptitudeInfo()
    {
        _CreateAptiudeToken?.Cancel();
        _CreateAptiudeToken = new();

        foreach (var item in _AptitudeNeedGoodsItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        _AptitudeNeedGoodsItems.Clear();

        var isLock = UICharacterUtility.IsLock(_CurCharacterId);

        var needGoodsInfos = UICharacterUtility.GetAptitudeNeedGoodsInfos(_CurCharacterId);
        for (int i = 0; i < needGoodsInfos.Count; i++)
        {
            var goodsInfo = needGoodsInfos[i];
            var goodsCount = isLock ? -1 : goodsInfo.goodsCount;
            if (!_AptitudeNeedGoodsItems.TryGetValue(goodsInfo.goodsCfgId, out var tipsItem))
            {
                var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountBottomTip>(EnLoadTarget.Pre_UIGoodsItemCountBottomTip, _AptitudeNeedGoodsTipsRootRect, _CreateAptiudeToken.Token)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                item.SetAutoSize();
                item.Setting(new(goodsInfo.goodsCfgId, goodsCount), EnSourceId.UIHallMainCharacterWindow);
                tipsItem = item;
                _AptitudeNeedGoodsItems.Add(goodsInfo.goodsCfgId, tipsItem);
            }
            else
            {
                tipsItem.SetCount(goodsCount);
            }

            if (!isLock)
            {
                var curGoodsCount = UIGoodsMgr.Instance.GetGoodsCount(goodsInfo.goodsCfgId);
                var isPass = curGoodsCount >= goodsCount;
                tipsItem.SetComValid(isPass);
            }
        }

        var list = UICharacterUtility.GetAptitudeAttributeIds();
        foreach (var attId in list)
        {
            var value = isLock
                ? 0
                : UICharacterUtility.GetAptitudeValue(_CurCharacterId, attId);
            if (!_AptitudeItems.TryGetValue(attId, out var item))
            {
                var (isCancel, attItem) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainCharacterWindow_AptitudeItem>(EnLoadTarget.Pre_UIHallMainCharacterWindow_AptitudeItem, _AptitudeItemRootRect, _CreateAptiudeToken.Token)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                attItem.Setting(attId, value);
                item = attItem;
                _AptitudeItems.Add(attId, item);
            }
            else
            {
                item.SetSliderValue(value);
            }
        }
        _CreateAptiudeToken = null;
    }
    private void UpdateButtonStatus()
    {
        var isLock = UICharacterUtility.IsLock(_CurCharacterId);
        var level = UICharacterUtility.GetLevel(_CurCharacterId);
        var maxLevel = UICharacterUtility.GetMaxLevel(_CurCharacterId);
        var isMax = level >= maxLevel - 1;
        _MaxLevelGo.SetActive(!isLock && isMax);
        _UplevelBtn.gameObject.SetActive(!isLock && !isMax);
        _LockGo.SetActive(isLock);

        var curActiveId = UICharacterUtility.GetActiveCharacterId();
        _ActiveBtn.gameObject.SetActive(!isLock && curActiveId != _CurCharacterId);
        _ActiveGo.SetActive(!isLock && curActiveId == _CurCharacterId);

        var index = _Character2Index[_CurCharacterId];
        var count = GameSchedule.Instance.GetCharacterCfgCount();
        _LeftBtn.gameObject.SetActive(index > 0);
        _RightBtn.gameObject.SetActive(index < count - 1);


        if (!isLock)
        {
            var btnEnable = true;
            var needGoodsInfos = UICharacterUtility.GetUplevelNeedGoodsInfos(_CurCharacterId);
            foreach (var item in needGoodsInfos)
            {
                var curCount = UIGoodsMgr.Instance.GetGoodsCount(item.goodsCfgId);
                var goodsValid = curCount >= item.goodsCount;
                if (_UplevelNeedGoodsItems.TryGetValue(item.goodsCfgId, out var needGoodsItem))
                {
                    needGoodsItem.SetComValid(goodsValid);
                }
                btnEnable &= goodsValid;
            }
            _UplevelBtn.interactable = btnEnable;
        }
    }
    private void UpdateAptitudeStatus()
    {
        var isLock = UICharacterUtility.IsLock(_CurCharacterId);
        _AptitudeItemRootRect.gameObject.SetActive(!isLock && _IsShowAptitude);
        _AptitudeBtnDirRect.eulerAngles = (_IsShowAptitude ? -1 : 1) * 90 * Vector3.forward;
        _AptitudeBtn.gameObject.SetActive(!isLock);

        if (!isLock)
        {
            var btnEnable = true;
            var needGoodsInfos = UICharacterUtility.GetAptitudeNeedGoodsInfos(_CurCharacterId);
            foreach (var item in needGoodsInfos)
            {
                var curCount = UIGoodsMgr.Instance.GetGoodsCount(item.goodsCfgId);
                var goodsValid = curCount >= item.goodsCount;
                if (_AptitudeNeedGoodsItems.TryGetValue(item.goodsCfgId, out var needGoodsItem))
                {
                    needGoodsItem.SetComValid(goodsValid);
                }
                btnEnable &= goodsValid;
            }
            _RefreshAptitudeBtn.interactable = btnEnable;
        }
    }
}
