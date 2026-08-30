using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainBattleWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.SubWindow;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallMainBattleWindow;
}
public class UIHallMainBattleWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _EnterBattleBtn;
    [SerializeField, AutoReference]
    private Button _ChangeLeftChapterBtn = null;
    [SerializeField, AutoReference]
    private Button _ChangeRightChapterBtn = null;
    [SerializeField, AutoReference]
    private RectTransform _ChapterRootRect = null;
    [SerializeField, AutoReference]
    private RectTransform _EnterGoodsInfoRootRect = null;

    private UIGoodsItemCountRightTip _GoodsCountTips = null;
    private int _CurSelectChapterId = 0;
    private readonly List<UIHallMainBattleWindow_ChapterItem> _ChapterItems = new();

    private readonly EnDotweenId _DotweenId = EnDotweenId.UIHallMainBattleWindow;
    private readonly float _ChapterItemWidth = 800f;
    private readonly float _ChapterDefaultScale = 0.7f;
    public override void OnUnload()
    {
        if (_GoodsCountTips != null)
        {
            UIMgr.Instance.UnloadWindowItem(_GoodsCountTips);
            _GoodsCountTips = null;
        }
        DOTween.Kill(_DotweenId);
        foreach (var item in _ChapterItems)
        {
            UIMgr.Instance.UnloadWindowItem(item);
        }
        _EnterBattleBtn.RemoveListener(OnClick_EnterBattleBtn);
        _ChangeLeftChapterBtn.RemoveListener(OnClick_ChangeLeftChapterBtn);
        _ChangeRightChapterBtn.RemoveListener(OnClick_ChangeRightChapterBtn);
        EventMgr.Instance.Unregister<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);
        base.OnUnload();
        _CurSelectChapterId = 0;
        _ChapterItems.Clear();
    }
    public override async void LoadCompletion()
    {
        base.LoadCompletion();
        _EnterBattleBtn.AddListener(OnClick_EnterBattleBtn);
        _ChangeLeftChapterBtn.AddListener(OnClick_ChangeLeftChapterBtn);
        _ChangeRightChapterBtn.AddListener(OnClick_ChangeRightChapterBtn);

        var isCancel = await InitChapterItems().SuppressCancellationThrow();
        if (!isCancel)
        {
            SetChapterId(1);
            UpdateChapterItemRect(true);
        }

        CreateEnergyIconItem();

        EventMgr.Instance.Register<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);
    }

    private void OnClick_EnterBattleBtn()
    {
        var chapterId = _CurSelectChapterId;
        UIMgr.Instance.HideLobby();
        GameMapMgr.Instance.EnterMap(chapterId);
    }
    private void OnClick_ChangeLeftChapterBtn()
    {
        SetChapterId(_CurSelectChapterId - 1);
    }
    private void OnClick_ChangeRightChapterBtn()
    {
        SetChapterId(_CurSelectChapterId + 1);
    }
    private void OnEvent_GoodsChange(in int goodsCfgId, in int _, in GameGoodsChangeEventData data)
    {
        if (goodsCfgId == (int)EnGoodsId.Energy)
        {
            UpdateEnterButtonStatus();
        }
    }

    private async void CreateEnergyIconItem()
    {
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountRightTip>(EnLoadTarget.Pre_UIGoodsItemCountRightTip, _EnterGoodsInfoRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            return;
        var enterEnergyCount = GlobalCfgUtility.Get10011_Int32();
        item.Setting(new((int)EnGoodsId.Energy, enterEnergyCount), EnSourceId.UIHallMainBattleWindow);
        _GoodsCountTips = item;
        UpdateEnterButtonStatus();
    }

    private void SetChapterId(int chapterId)
    {
        if (_CurSelectChapterId == chapterId)
            return;
        var maxChapter = _ChapterItems.Count;
        var leftBtnActive = chapterId > 1;
        var rightBtnActive = chapterId < maxChapter;
        if (_ChangeLeftChapterBtn.gameObject.activeSelf != leftBtnActive)
            _ChangeLeftChapterBtn.gameObject.SetActive(leftBtnActive);
        if (_ChangeRightChapterBtn.gameObject.activeSelf != rightBtnActive)
            _ChangeRightChapterBtn.gameObject.SetActive(rightBtnActive);
        if (_CurSelectChapterId > 0)
        {
            var lastItem = _ChapterItems[_CurSelectChapterId - 1];
            lastItem.SetSelectStatus(false);
        }
        _CurSelectChapterId = chapterId;
        var curItem = _ChapterItems[chapterId - 1];
        curItem.SetSelectStatus(true);
        UpdateEnterButtonStatus();
    }
    protected override void Update()
    {
        base.Update();
        if (_CurSelectChapterId > 0)
            UpdateChapterItemRect();
    }
    private float GetAnimationSpeed()
    {
        return ABBUtil.GetTimeDelta() * 10;
    }
    private void UpdateChapterItemRect(bool immediate = false)
    {
        for (int i = 0; i < _ChapterItems.Count; i++)
        {
            var item = _ChapterItems[i];
            var index = i - (_CurSelectChapterId - 1);
            var toPosX = index * _ChapterItemWidth;
            var curPosX = item.GetLocalPosX();
            var posX = Mathf.Lerp(curPosX, toPosX, immediate ? 1 : GetAnimationSpeed());
            item.SetLocalPos(posX);

            var toScale = _CurSelectChapterId == i + 1 ? 1 : _ChapterDefaultScale;
            var curScale = item.GetLocalScale();
            var scale = Mathf.Lerp(curScale, toScale, immediate ? 1 : GetAnimationSpeed());
            item.SetLocalScale(scale);
        }
    }
    private async UniTask InitChapterItems()
    {
        var count = GameSchedule.Instance.GetChapterCfgCount();
        for (int i = 0; i < count; i++)
        {
            var chapterCfg = GameSchedule.Instance.GetChapterCfg(i);
            if (!GameChapterMgr.Instance.IsActive(chapterCfg.nChapterId))
                break;
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainBattleWindow_ChapterItem>(EnLoadTarget.Pre_UIHallMainBattleWindow_ChapterItem, _ChapterRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                throw new System.OperationCanceledException();
            var isLock = GameChapterMgr.Instance.IsLock(chapterCfg.nChapterId);
            item.Init(chapterCfg.nChapterId, isLock);
            _ChapterItems.Add(item);
        }
    }

    private void UpdateEnterButtonStatus()
    {
        var isLock = GameChapterMgr.Instance.IsLock(_CurSelectChapterId);
        var energyCount = UIGoodsMgr.Instance.GetGoodsCount((int)EnGoodsId.Energy);
        var isEnergy = energyCount >= GlobalCfgUtility.Get10011_Int32();
        _EnterBattleBtn.interactable = !isLock & isEnergy;
        if (_GoodsCountTips != null)
            _GoodsCountTips.SetValid(isEnergy);
    }
}
