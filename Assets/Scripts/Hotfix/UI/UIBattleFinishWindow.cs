using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleFinishWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleFinishWindow;
}
public class UIBattleFinishWindow : UIWindow
{
    [SerializeField, AutoReference]
    private TextMeshProUGUI _TitleTxt = null;
    [SerializeField, AutoReference]
    private Button _ReturnBtn = null;
    [SerializeField, AutoReference]
    private Button _RestartBtn = null;

    public override void OnUnload()
    {
        _ReturnBtn.RemoveListener(OnClick_ReturnBtn);
        _RestartBtn.RemoveListener(OnClick_RestartBtn);
        base.OnUnload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _ReturnBtn.AddListener(OnClick_ReturnBtn);
        _RestartBtn.AddListener(OnClick_RestartBtn);

        var battleResult = GameBattleMgr.Instance.GetBattleStatus();
        var isSuccess = battleResult == EnGameBattleStatus.Success;
        var titleLangId = isSuccess ? 10017 : 10018;
        _TitleTxt.SetLanguageText(titleLangId);

        _RestartBtn.gameObject.SetActive(!isSuccess);
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
