using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICoreQualitySelectDialogUserData : UIWindowUserData
{
    public List<string> selects;
    public Action<HashSet<int>> callback;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        callback = null;
        selects = null;
    }
    public void Setting(List<string> selects, Action<HashSet<int>> callback)
    {
        this.selects = selects;
        this.callback = callback;
    }
}
public class UICoreQualitySelectDialogData : UIWindowData<UICoreQualitySelectDialog, UICoreQualitySelectDialogUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Dialog;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UICoreQualitySelectDialog;

    private List<string> _Selects;
    private Action<HashSet<int>> _Callback;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _Callback = null;
        _Selects = null;
    }
    protected override void OnPoolInit(UICoreQualitySelectDialogUserData userData)
    {
        _Callback = userData.callback;
        _Selects = userData.selects;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _UIWindow.SetCallback(_Selects, _Callback);
    }
}
public class UICoreQualitySelectDialog : UIWindow
{
    [SerializeField, AutoReference]
    private Button _CloseBtn;
    [SerializeField, AutoReference]
    private Button _EnterBtn;
    [SerializeField, AutoReference]
    private Toggle _ToggleItem;

    private Action<HashSet<int>> _Callback;
    private readonly List<Toggle> _Toggles = new();
    public override void OnUnload()
    {
        foreach (var item in _Toggles)
        {
            GameObject.Destroy(item.gameObject);
        }
        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        _EnterBtn.RemoveListener(OnClick_EnterBtn);
        base.OnUnload();
        _Toggles.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _CloseBtn.AddListener(OnClick_CloseBtn);
        _EnterBtn.AddListener(OnClick_EnterBtn);

        _ToggleItem.gameObject.SetActive(false);


    }
    public void SetCallback(List<string> selects, Action<HashSet<int>> action)
    {
        _Callback = action;
        for (var i = 0; i < selects.Count; i++)
        {
            var item = GameObject.Instantiate(_ToggleItem, _ToggleItem.transform.parent);
            item.gameObject.SetActive(true);
            item.SetIsOnWithoutNotify(false);
            var tmpCom = item.GetComponentInChildren<TextMeshProUGUI>();
            tmpCom.SetText(selects[i]);
            _Toggles.Add(item);
        }
    }
    private void OnClick_CloseBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }
    private void OnClick_EnterBtn()
    {
        if (_Callback != null)
        {
            var selList = new HashSet<int>();
            for (int i = 0; i < _Toggles.Count; i++)
            {
                var item = _Toggles[i];
                if (!item.isOn)
                    continue;
                selList.Add(i);
            }
            _Callback.Invoke(selList);
        }
        UIMgr.Instance.HideWindow(_WindowId);
    }
}
