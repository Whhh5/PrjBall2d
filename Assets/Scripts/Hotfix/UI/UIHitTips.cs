using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIHitTipsUserData : UITipsUserData
{
    public string content;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        content = null;
    }
    public void Setting(string content)
    {
        this.content = content;
    }
}
public class UIHitTipsData : UITipsData<UIHitTips, UIHitTipsUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHitTips;

    private string _Content;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _Content = null;
    }
    protected override void OnPoolInit(UIHitTipsUserData userData)
    {
        _Content = userData.content;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _UITips.Setting(_Content);
    }
}
public class UIHitTips : UITips
{
    [SerializeField, AutoReference]
    private RectTransform _RootRect;
    [SerializeField, AutoReference(nameof(_RootRect))]
    private CanvasGroup _RootCanvasGroup;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _ContentTxt;

    private int DotweenId => _WindowId;
    public override void OnUnload()
    {
        _RootRect.localScale = Vector3.one;
        _RootCanvasGroup.alpha = 1;
        DOTween.Kill(DotweenId);
        base.OnUnload();
    }

    public override bool IsClickClose()
    {
        return false;
    }
    public void Setting(string content)
    {
        _ContentTxt.SetText(content);

        PlayAnimation();
    }
    private async void PlayAnimation()
    {
        var fromScale = 0;
        var toScale = 1;

        var isCancel = await DOTween.To(() => 0f, slider =>
        {
            var scale = Mathf.Lerp(fromScale, toScale, slider);
            _RootRect.localScale = new Vector3(1, scale, 1);

        }, 1f, 0.2f)
            .SetId(DotweenId)
            .ToUniTask(TweenCancelBehaviour.Kill, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            return;
        var isCancel2 = await UniTask.Delay(2000, cancellationToken: GetWindowToken(), cancelImmediately: true).SuppressCancellationThrow();
        if (isCancel2)
            return;
        var isCancel3 = await DOTween.To(() => 0f, slider =>
        {
            var scale = Mathf.Lerp(toScale, fromScale, slider);
            _RootCanvasGroup.alpha = scale;

        }, 1f, 1f)
            .SetId(DotweenId)
            .ToUniTask(TweenCancelBehaviour.Kill, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel3)
            return;

        UIMgr.Instance.HideWindow(_WindowId);
    }
}
