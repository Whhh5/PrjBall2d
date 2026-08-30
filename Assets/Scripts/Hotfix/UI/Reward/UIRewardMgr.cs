using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRewardMgr : Singleton<UIRewardMgr>
{


    public void SendReward(int goodsCfgId, int count)
    {
        var goodsId = UIGoodsMgr.Instance.AddGoods(goodsCfgId, count);

        var windowUserData = ClassPoolMgr.Instance.Pull<UIRewardWindowUserData>();
        windowUserData.Setting(new() { new( goodsCfgId, count, goodsId) });
        UIMgr.Instance.ShowWindow<UIRewardWindowData>(windowUserData);
        ClassPoolMgr.Instance.Push(windowUserData);
    }

    public void SendReward(List<GoodsInfo> goodsInfos)
    {
        var ids = UIGoodsMgr.Instance.AddGoods(goodsInfos);
        for (int i = 0; i < ids.Count; i++)
        {
            var goodsInfo = goodsInfos[i];
            goodsInfo.goodsId = ids[i];
            goodsInfos[i] = goodsInfo;
        }

        var windowUserData = ClassPoolMgr.Instance.Pull<UIRewardWindowUserData>();
        windowUserData.Setting(goodsInfos);
        UIMgr.Instance.ShowWindow<UIRewardWindowData>(windowUserData);
        ClassPoolMgr.Instance.Push(windowUserData);
    }
}
