using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleFinishWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleFinishWindow;
    public override void OnPoolDestroy()
    {
        TimeMgr.Instance.SetPause(false, true);
        base.OnPoolDestroy();
    }
    protected override void OnPoolInit(UIWindowUserData userData)
    {
        base.OnPoolInit(userData);
        TimeMgr.Instance.SetPause(true, true);
    }
}
public class UIBattleFinishWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _ReturnBtn = null;
    [SerializeField, AutoReference]
    private Button _RestartBtn = null;
    [SerializeField, AutoReference]
    private RectTransform _RewardGoodsItemRootRect = null;
    [SerializeField, AutoReference]
    private RectTransform _KillInfosRootRect = null;


    private readonly List<UIGoodsItemBase> _GoodsItems = new();
    private readonly List<UIBattleFinishWindow_KillInfoItem> _SkillInfoItems = new();
    public override void OnUnload()
    {
        foreach (var item in _SkillInfoItems)
            UIMgr.Instance.UnloadWindowItem(item);
        foreach (var item in _GoodsItems)
            UIMgr.Instance.UnloadWindowItem(item);
        
        _ReturnBtn.RemoveListener(OnClick_ReturnBtn);
        _RestartBtn.RemoveListener(OnClick_RestartBtn);
        base.OnUnload();
        _GoodsItems.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _ReturnBtn.AddListener(OnClick_ReturnBtn);
        _RestartBtn.AddListener(OnClick_RestartBtn);

        var battleResult = GameBattleMgr.Instance.GetBattleStatus();
        var isSuccess = battleResult == EnGameBattleStatus.Success;
        this.SetComValid(isSuccess);

        _RestartBtn.gameObject.SetActive(!isSuccess);

        var isFirstSuccess = false;
        if (isSuccess)
        {
            var chapterId = GameMapMgr.Instance.GetCurChapterId();
            var maxSuccessChapterId = GameChapterMgr.Instance.GetMaxSuccessChapterId();
            if (chapterId > maxSuccessChapterId)
            {
                isFirstSuccess = true;
                GameChapterMgr.Instance.SetMaxSuccessChapterId(chapterId);
            }
        }
        CreateGoodsItems(isSuccess, isFirstSuccess);
    }

    private async void CreateGoodsItems(bool isSuccess, bool isFirstSuccess)
    {
        var monsterCount = GameBattleMonitorMgr.Instance.GetMonsterDieCount();
        var expCount = Mathf.FloorToInt(monsterCount / 5f);

        var coinCount = monsterCount * 10;
        var coreCount = Mathf.FloorToInt(monsterCount / 15f);
        var gemCount = Mathf.FloorToInt(monsterCount / 10f);

        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);


        var diamondGoodItemId = isFirstSuccess ? 10 : -1;
        var diamondCount = (isFirstSuccess ? 10 : 0) + chapterCfg.nSuccessDiamondCount;
        if (diamondCount > 0 && isSuccess)
            diamondGoodItemId = UIGoodsMgr.Instance.AddGoods(EnGoodsId.Diamond, diamondCount);
        var expGoodItemId = -1;
        if (expCount > 0)
            expGoodItemId = UIGoodsMgr.Instance.AddGoods(EnGoodsId.Exp, expCount);
        var coinGoodItemId = -1;
        if (coinCount > 0)
            coinGoodItemId = UIGoodsMgr.Instance.AddGoods(EnGoodsId.Coin, coinCount);
        List<GoodsInfo> coreGoodsInfos = null;
        if (coreCount > 0)
        {
            coreGoodsInfos = UICoreUtility.RangeGetCoreGoodsCfgId(coreCount);
            UIGoodsMgr.Instance.AddGoods(ref coreGoodsInfos);
        }
        List<GoodsInfo> gemGoodsInfos = null;
        if (gemCount > 0)
        {
            gemGoodsInfos = UIGemUtility.RangeGetGemGoodsCfgId(gemCount);
            UIGoodsMgr.Instance.AddGoods(ref gemGoodsInfos);
        }

        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleFinishWindow_KillInfoItem>(EnLoadTarget.Pre_UIBattleFinishWindow_KillInfoItem, _KillInfosRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var title = LanguageUtility.GetLanguage(10048);
            item.Setting(title, $"{monsterCount}");
            _SkillInfoItems.Add(item);
        }
        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleFinishWindow_KillInfoItem>(EnLoadTarget.Pre_UIBattleFinishWindow_KillInfoItem, _KillInfosRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var title = LanguageUtility.GetLanguage(10052);
            item.Setting(title, $"{expCount}");
            _SkillInfoItems.Add(item);
        }
        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleFinishWindow_KillInfoItem>(EnLoadTarget.Pre_UIBattleFinishWindow_KillInfoItem, _KillInfosRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var title = LanguageUtility.GetLanguage(10049);
            item.Setting(title, $"{coinCount}");
            _SkillInfoItems.Add(item);
        }
        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleFinishWindow_KillInfoItem>(EnLoadTarget.Pre_UIBattleFinishWindow_KillInfoItem, _KillInfosRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var title = LanguageUtility.GetLanguage(10050);
            item.Setting(title, $"{coreCount}");
            _SkillInfoItems.Add(item);
        }
        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleFinishWindow_KillInfoItem>(EnLoadTarget.Pre_UIBattleFinishWindow_KillInfoItem, _KillInfosRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var title = LanguageUtility.GetLanguage(10051);
            item.Setting(title, $"{gemCount}");
            _SkillInfoItems.Add(item);
        }

        if (diamondGoodItemId > 0)
        {
            var (isCancel, item) = await UIGoodsUtility.CreateGoodsItem(new((int)EnGoodsId.Diamond, diamondCount, expGoodItemId), _RewardGoodsItemRootRect, GetWindowToken(), EnSourceId.UIBattleFinishWindow)
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            _GoodsItems.Add(item);
        }
        if (expGoodItemId > 0)
        {
            var (isCancel, item) = await UIGoodsUtility.CreateGoodsItem(new((int)EnGoodsId.Exp, expCount, expGoodItemId), _RewardGoodsItemRootRect, GetWindowToken(), EnSourceId.UIBattleFinishWindow)
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            _GoodsItems.Add(item);
        }
        if (coinGoodItemId > 0)
        {
            var (isCancel, item) = await UIGoodsUtility.CreateGoodsItem(new((int)EnGoodsId.Coin, coinCount, coinGoodItemId), _RewardGoodsItemRootRect, GetWindowToken(), EnSourceId.UIBattleFinishWindow)
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            _GoodsItems.Add(item);
        }
        if (coreCount > 0)
        {
            for (int i = 0; i < coreCount; i++)
            {
                var coreGoodsInfo = coreGoodsInfos[i];
                var (isCancel, item) = await UIGoodsUtility.CreateGoodsItem(coreGoodsInfo, _RewardGoodsItemRootRect, GetWindowToken(), EnSourceId.UIBattleFinishWindow)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                _GoodsItems.Add(item);
            }
        }
        if (gemCount > 0)
        {
            for (int i = 0; i < gemCount; i++)
            {
                var gemGoodsInfo = gemGoodsInfos[i];
                var (isCancel, item) = await UIGoodsUtility.CreateGoodsItem(gemGoodsInfo, _RewardGoodsItemRootRect, GetWindowToken(), EnSourceId.UIBattleFinishWindow)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                _GoodsItems.Add(item);
            }
        }
    }

    private void OnClick_ReturnBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        GameMapMgr.Instance.ExitMap();
        UIMgr.Instance.ShowLobby();
    }
    private void OnClick_RestartBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        GameMapMgr.Instance.ReenterMap();
    }
}
