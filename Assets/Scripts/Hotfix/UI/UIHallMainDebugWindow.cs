using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIHallMainDebugWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.SubWindow;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallMainDebugWindow;
}

public class UIHallMainDebugWindow : UIWindow
{
    [SerializeField, AutoReference]
    private TMP_InputField _GoodsIdInput;
    [SerializeField, AutoReference]
    private TMP_InputField _GoodsCountInput;
    [SerializeField, AutoReference]
    private Button _GoodsApplyBtn;

    [SerializeField, AutoReference]
    private TMP_InputField _CoreIdInput;
    [SerializeField, AutoReference]
    private TMP_InputField _CoreCountInput;
    [SerializeField, AutoReference]
    private Button _CoreApplyBtn;

    [SerializeField, AutoReference]
    private TMP_InputField _GemIdInput;
    [SerializeField, AutoReference]
    private TMP_InputField _GemCountInput;
    [SerializeField, AutoReference]
    private Button _GemApplyBtn;

    [SerializeField, AutoReference]
    private TMP_InputField _ExpCountInput;
    [SerializeField, AutoReference]
    private Button _ExpApplyBtn;

    public override void OnUnload()
    {
        _GoodsApplyBtn.RemoveListener(OnClick_GoodsApplyBtn);
        _CoreApplyBtn.RemoveListener(OnClick_CoreApplyBtn);
        _ExpApplyBtn.RemoveListener(OnClick_ExpApplyBtn);
        _GemApplyBtn.RemoveListener(OnClick_GemApplyBtn);
        base.OnUnload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();

        _GoodsApplyBtn.AddListener(OnClick_GoodsApplyBtn);
        _CoreApplyBtn.AddListener(OnClick_CoreApplyBtn);
        _ExpApplyBtn.AddListener(OnClick_ExpApplyBtn);
        _GemApplyBtn.AddListener(OnClick_GemApplyBtn);

    }
    private void OnClick_GoodsApplyBtn()
    {
        if (!int.TryParse(_GoodsIdInput.text, out var goodsCfgId))
            return;
        if (!int.TryParse(_GoodsCountInput.text, out var count))
            return;
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        if (goodsCfg == null)
        {
            AotDebug.LogError($"添加失败不存在物品:{goodsCfgId}");
            return;
        }
        var curCount = UIGoodsMgr.Instance.GetGoodsCount(goodsCfgId);
        var changeCount = Mathf.Abs(count - curCount);
        if (changeCount == 0)
            return;
        if (count > curCount)
            UIRewardMgr.Instance.SendReward(goodsCfgId, changeCount);
        else
            UIGoodsMgr.Instance.RemoveGoods(goodsCfgId, changeCount);
    }
    private void OnClick_CoreApplyBtn()
    {
        if (!int.TryParse(_CoreCountInput.text, out var count))
            count = 1;
        if (!int.TryParse(_CoreIdInput.text, out var goodsCfgId))
        {
            var goodsInfoList = UICoreUtility.RangeGetCoreGoodsCfgId(count);
            UIRewardMgr.Instance.SendReward(goodsInfoList);
        }
        else
        {
            var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
            if (goodsCfg == null)
                return;
            for (int i = 0; i < count; i++)
            {
                UIRewardMgr.Instance.SendReward(goodsCfgId, 1);
            }
        }
    }
    private void OnClick_GemApplyBtn()
    {
        if (!int.TryParse(_GemCountInput.text, out var count))
            count = 1;
        if (!int.TryParse(_GemIdInput.text, out var goodsCfgId))
        {
            var goodsInfoList = UIGemUtility.RangeGetGemGoodsCfgId(count);
            UIRewardMgr.Instance.SendReward(goodsInfoList);
        }
        else
        {
            var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
            if (goodsCfg == null)
                return;
            for (int i = 0; i < count; i++)
            {
                UIRewardMgr.Instance.SendReward(goodsCfgId, 1);
            }
        }
    }

    private void OnClick_ExpApplyBtn()
    {
        if (!int.TryParse(_ExpCountInput.text, out var count))
            return;
        var curExp = UIGoodsMgr.Instance.GetGoodsCount((int)EnGoodsId.Exp);
        var changeExp = count - curExp;
        UIGoodsMgr.Instance.AddGoods((int)EnGoodsId.Exp, changeExp);
    }
}
