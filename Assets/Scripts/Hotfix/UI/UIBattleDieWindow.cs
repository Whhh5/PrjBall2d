using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleDieWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleDieWindow;
}
public class UIBattleDieWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _FinishBtn = null;
    [SerializeField, AutoReference]
    private Button _ResurrectionBtn = null;

    public override void OnUnload()
    {
        _FinishBtn.RemoveListener(OnClick_FinishBtn);
        _ResurrectionBtn.RemoveListener(OnClick_ResurrectionBtn);
        base.OnUnload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _FinishBtn.AddListener(OnClick_FinishBtn);
        _ResurrectionBtn.AddListener(OnClick_ResurrectionBtn);
    }

    private void OnClick_FinishBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        GameBattleMgr.Instance.SetBattleStatus(EnGameBattleStatus.Faild);
    }
    private void OnClick_ResurrectionBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        GameBattleMgr.Instance.SetBattleStatus(EnGameBattleStatus.Battle);
        GameBattleMgr.Instance.Resurrection();
    }
}
