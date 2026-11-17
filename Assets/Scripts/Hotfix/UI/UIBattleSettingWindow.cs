using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleSettingWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleSettingWindow;
}
public class UIBattleSettingWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _RestartBtn = null;
    [SerializeField, AutoReference]
    private Button _ExitBtn = null;
    [SerializeField, AutoReference]
    private Button _ContinueBtn = null;
    [SerializeField, AutoReference]
    private Toggle _AutoFireToggle = null;
    [SerializeField, AutoReference]
    private Toggle _DamageTxtToggle = null;

    public override void OnUnload()
    {
        TimeMgr.Instance.SetPause(false, true);
        _RestartBtn.RemoveListener(OnClick_RestartBtn);
        _ExitBtn.RemoveListener(OnClick_ExitBtn);
        _ContinueBtn.RemoveListener(OnClick_ContinueBtn);
        _AutoFireToggle.RemoveListener(OnClick_AutoFireToggle);
        _DamageTxtToggle.RemoveListener(OnClick_DamageTxtToggle);
        base.OnUnload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _RestartBtn.AddListener(OnClick_RestartBtn);
        _ExitBtn.AddListener(OnClick_ExitBtn);
        _ContinueBtn.AddListener(OnClick_ContinueBtn);
        _AutoFireToggle.AddListener(OnClick_AutoFireToggle);
        _DamageTxtToggle.AddListener(OnClick_DamageTxtToggle);
        TimeMgr.Instance.SetPause(true, true);

        var pd = PersistentMgr.Instance.GetData<PersistentSettingData>(EnPersistentId.Setting);
        var value = pd.GetAutoFireBall();
        _AutoFireToggle.SetIsOnWithoutNotify(value);

        var value2 = pd.GetIsShowDamageTxt();
        _DamageTxtToggle.SetIsOnWithoutNotify(value2);
    }
    private void OnClick_RestartBtn()
    {
        if (UIMgr.Instance.ContainsWindow<UIControllerDialogData>())
            return;
        var content = LanguageUtility.GetLanguage(30103);
        UIUtil.ShowControllerDialog(content, EnSourceId.UIBattleMainWindow, RestartLevel);
    }
    private void OnClick_ExitBtn()
    {
        if (UIMgr.Instance.ContainsWindow<UIControllerDialogData>())
            return;
        var content = LanguageUtility.GetLanguage(30104);
        UIUtil.ShowControllerDialog(content, EnSourceId.UIBattleMainWindow, ExitLevel);
    }
    private void OnClick_ContinueBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }
    private void OnClick_AutoFireToggle(bool isOn)
    {
        var pd = PersistentMgr.Instance.GetData<PersistentSettingData>(EnPersistentId.Setting);
        pd.SetAutoFireBall(isOn);
    }
    private void OnClick_DamageTxtToggle(bool isOn)
    {
        GameDamageMgr.Instance.SetIsShowDamageTxt(isOn);
    }

    private void RestartLevel()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        GameMapMgr.Instance.ReenterMap();
    }
    private void ExitLevel()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        GameMapMgr.Instance.ExitMap();
        UIMgr.Instance.ShowLobby();
    }
}
