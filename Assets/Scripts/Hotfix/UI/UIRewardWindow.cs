using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardWindowUserData : UIWindowUserData
{
    public List<GoodsInfo> goodsInfos;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        goodsInfos = null;
    }
    public void Setting(List<GoodsInfo> goodsInfos)
    {
        this.goodsInfos = goodsInfos;
    }
}
public class UIRewardWindowData : UIWindowData<UIRewardWindow, UIRewardWindowUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIRewardWindow;

    private List<GoodsInfo> _GoodsInfos;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _GoodsInfos = null;
    }
    protected override void OnPoolInit(UIRewardWindowUserData userData)
    {
        _GoodsInfos = userData.goodsInfos;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _UIWindow.Setting(_GoodsInfos);
    }
}

public class UIRewardWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _CloseBtn;
    [SerializeField, AutoReference]
    private RectTransform _GoodsItemRootRect;


    private readonly List<UIGoodsItemBase> _GoodsItems = new();

    public override void OnUnload()
    {
        foreach (var item in _GoodsItems)
        {
            UIMgr.Instance.UnloadWindowItem(item);
        }
        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        base.OnUnload();
        _GoodsItems.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _CloseBtn.AddListener(OnClick_CloseBtn);
    }
    public async void Setting(List<GoodsInfo> goodsInfos)
    {
        for (int i = 0; i < goodsInfos.Count; i++)
        {
            var cancel = await UniTask.WaitForSeconds(0.05f, cancelImmediately: true, cancellationToken: GetWindowToken())
                .SuppressCancellationThrow();
            if (cancel)
                break;
            var goodsInfo = goodsInfos[i];
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsCommonItem>(EnLoadTarget.Pre_UIGoodsCommonItem, _GoodsItemRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            item.Setting(goodsInfo, EnSourceId.UIRewardWindow);
            _GoodsItems.Add(item);
        }
    }
    private void OnClick_CloseBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }
}
