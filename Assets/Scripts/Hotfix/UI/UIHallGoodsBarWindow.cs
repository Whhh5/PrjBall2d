using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallGoodsBarWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.SubWindow;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallGoodsBarWindow;
}

public class UIHallGoodsBarWindow : UIWindow
{
    [SerializeField, AutoReference]
    private RectTransform _GoodsInfoRootRect = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _LevelTxt = null;
    [SerializeField, AutoReference]
    private Image _LevelImg = null;

    private readonly EnDotweenId _DotweenId = EnDotweenId.UIHallGoodsBarWindow;
    private readonly Dictionary<int, UIHallGoodsBarWindow_GoodsItem> _GoodsCfgId2GoodsItem = new();
    private readonly HashSet<UIGoodsItemIconBase> _MoveIcons = new();
    public override void OnUnload()
    {
        DOTween.Kill(_DotweenId);
        foreach (var item in _MoveIcons)
        {
            UIMgr.Instance.UnloadWindowItem(item);
        }
        foreach (var item in _GoodsCfgId2GoodsItem)
        {
            UIMgr.Instance.UnloadWindowItem(item.Value);
        }
        EventMgr.Instance.Unregister<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);
        EventMgr.Instance.Unregister<GameAccountExpChangeEventData>(EnABBEvent.EVENT_ACCOUNT_EXP_CHANGE, 0, 0, OnEvent_AccountExpChange);
        EventMgr.Instance.Unregister<GameAccountLevelChangeEventData>(EnABBEvent.EVENT_ACCOUNT_LEVEL_CHANGE, 0, 0, OnEvent_AccountLevelChange);
        base.OnUnload();
        _GoodsCfgId2GoodsItem.Clear();
        _MoveIcons.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        EventMgr.Instance.Register<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);
        EventMgr.Instance.Register<GameAccountExpChangeEventData>(EnABBEvent.EVENT_ACCOUNT_EXP_CHANGE, 0, 0, OnEvent_AccountExpChange);
        EventMgr.Instance.Register<GameAccountLevelChangeEventData>(EnABBEvent.EVENT_ACCOUNT_LEVEL_CHANGE, 0, 0, OnEvent_AccountLevelChange);

        UpdateLevel();
        UpdateExp();
        CreateGoodsItems();
    }

    private void OnEvent_GoodsChange(in int goodsCfgId, in int _, in GameGoodsChangeEventData data)
    {
        if (!_GoodsCfgId2GoodsItem.TryGetValue(goodsCfgId, out var item))
            return;
        item.SetCount(data.curValue);
    }
    private void OnEvent_AccountExpChange(in int _, in int __, in GameAccountExpChangeEventData data)
    {
        UpdateExp();
    }
    private void OnEvent_AccountLevelChange(in int _, in int __, in GameAccountLevelChangeEventData data)
    {
        UpdateLevel();
        UpdateExp();
    }
    private void UpdateExp()
    {
        var slider = AccountMgr.Instance.GetExpSliderValue();
        _LevelImg.fillAmount = slider;
    }
    private void UpdateLevel()
    {
        var level = AccountMgr.Instance.GetLevel();
        _LevelTxt.SetText($"{level}");
    }

    private async void CreateGoodsItems()
    {
        var count = GameSchedule.Instance.GetGoodsCfgCount();
        for (int i = 0; i < count; i++)
        {
            var cfg = GameSchedule.Instance.GetGoodsCfg(i);
            if (cfg.bShowTopBar <= 0)
                continue;
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIHallGoodsBarWindow_GoodsItem>(EnLoadTarget.Pre_UIHallGoodsBarWindow_GoodsItem, _GoodsInfoRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            item.Setting(cfg.nGoodsId);
            _GoodsCfgId2GoodsItem.Add(cfg.nGoodsId, item);
        }
    }
}
