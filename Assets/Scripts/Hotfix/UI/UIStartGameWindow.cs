using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class AccountInfo
{
    public ulong account;
    public string password;
    public AccountInfo(ulong account, string password)
    {
        this.account = account;
        this.password = password;
    }
}
public class UIStartGameWindowData : UIWindowData<UIStartGameWindow>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIStartGameWindow;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        EventMgr.Instance.Unregister(EnABBEvent.EVENT_ACCOUNT_LOGIN, OnEvent_Login);
    }
    protected override void OnPoolInit(UIWindowUserData userData)
    {
        base.OnPoolInit(userData);

        EventMgr.Instance.Register(EnABBEvent.EVENT_ACCOUNT_LOGIN, OnEvent_Login);
    }
    private void OnEvent_Login(in int _, in int __)
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }
}
public class UIStartGameWindow : UIWindow
{
    [SerializeField, AutoReference]
    private TMP_InputField _AccountInputField = null;
    [SerializeField, AutoReference]
    private TMP_InputField _PasswordInputField = null;
    [SerializeField, AutoReference]
    private Button _EnterBtn = null;
    [SerializeField, AutoReference]
    private Button _RegisterBtn = null;

    [SerializeField, AutoReference]
    private Button _SystemBtn = null;
    [SerializeField, AutoReference]
    private Button _SystemItem = null;
    private readonly Dictionary<string, Button> _SystemItems = new();

    private bool _IsEntering = false;
    public override void OnUnload()
    {
        _EnterBtn.RemoveListener(OnClick_EnterBtn);
        _RegisterBtn.RemoveListener(OnClick_RegisterBtn);
        base.OnUnload();
        _IsEntering = false;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _EnterBtn.AddListener(OnClick_EnterBtn);
        _RegisterBtn.AddListener(OnClick_RegisterBtn);

        PlatformUtility.PlatformAnalysis.RegisterTmpInputField(_AccountInputField);
        PlatformUtility.PlatformAnalysis.RegisterTmpInputField(_PasswordInputField);

        _SystemItem.gameObject.SetActive(false);
        _SystemBtn.AddListener(() => _SystemItem.transform.parent.gameObject.SetActive(!_SystemItem.transform.parent.gameObject.activeSelf));
        _SystemItem.transform.parent.gameObject.SetActive(false);

        _AccountInputField.characterLimit = GlobalCfgUtility.Get50001_Int32();
        _PasswordInputField.characterLimit = GlobalCfgUtility.Get50002_Int32();

        var lastAccount = PlayerPrefsMgr.Instance.GetString(EnPlayerPrefsId.Account);
        var lastPassword = PlayerPrefsMgr.Instance.GetString(EnPlayerPrefsId.Password);
        _AccountInputField.SetTextWithoutNotify(lastAccount);
        _PasswordInputField.SetTextWithoutNotify(lastPassword);

    }

    private async void OnClick_EnterBtn()
    {
        if (_IsEntering)
            return;
        if (!CheckAccountPasswordValid())
            return;
        _IsEntering = true;

        var account = ulong.Parse(_AccountInputField.text);
        var accPath = GetAccountFilePath(account);
        var accountInfo = await WebUtil.DownloadServerFileAsync(accPath);
        if (accountInfo.Length == 0)
        {
            _IsEntering = false;
            UIUtil.ShowHitTips(30109);
            return;
        }
        else
        {
            var str = Encoding.UTF8.GetString(accountInfo);
            var accInfo = JsonConvert.DeserializeObject<AccountInfo>(str);
            if (accInfo.password != _PasswordInputField.text)
            {
                _IsEntering = false;
                UIUtil.ShowHitTips(30109);
                return;
            }
        }
        var lastAccount = PlayerPrefsMgr.Instance.GetString(EnPlayerPrefsId.Account);
        if (lastAccount != _AccountInputField.text)
        {
            PlayerPrefsMgr.Instance.SetString(EnPlayerPrefsId.Account, $"{account}");
            PlayerPrefsMgr.Instance.SetString(EnPlayerPrefsId.Password, _PasswordInputField.text);
        }
        EnterGame($"{account}");
        _IsEntering = false;
    }
    private async void OnClick_RegisterBtn()
    {
        if (!CheckAccountPasswordValid())
            return;
        _IsEntering = true;
        var account = ulong.Parse(_AccountInputField.text);
        var password = _PasswordInputField.text;
        var accPath = GetAccountFilePath(account);
        var accountInfo = await WebUtil.DownloadServerFileAsync(accPath);
        if (accountInfo.Length != 0)
        {
            _IsEntering = false;
            UIUtil.ShowHitTips(30111);
            return;
        }
        var content = string.Format(LanguageUtility.GetLanguage(30112), _AccountInputField.text, _PasswordInputField.text);
        UIUtil.ShowControllerDialog(content, EnSourceId.UIStartGameWindow, async () =>
        {
            var accInfo = new AccountInfo(account, password);
            var json = JsonConvert.SerializeObject(accInfo);
            await WebUtil.UploadFileAsync(accPath, json);
            PlayerPrefsMgr.Instance.SetString(EnPlayerPrefsId.Account, $"{account}");
            PlayerPrefsMgr.Instance.SetString(EnPlayerPrefsId.Password, password);
            EnterGame($"{account}");
            _IsEntering = false;

        }, () =>
        {
            _IsEntering = false;
        });
    }

    private void EnterGame(string account)
    {
        AccountMgr.Instance.LoginGame(account);

    }
    private bool CheckAccountPasswordValid()
    {
        var accountCount = GlobalCfgUtility.Get50001_Int32();
        if (_AccountInputField.text.Length != accountCount)
        {
            var content = string.Format(LanguageUtility.GetLanguage(30108), accountCount, _AccountInputField.text.Length);
            UIUtil.ShowHitTips(content);
            return false;
        }
        var minPasswordCount = GlobalCfgUtility.Get50003_Int32();
        if (_PasswordInputField.text.Length < minPasswordCount)
        {
            var content = string.Format(LanguageUtility.GetLanguage(30110), minPasswordCount, _PasswordInputField.text.Length);
            UIUtil.ShowHitTips(content);
            return false;
        }
        return true;
    }
    private string GetAccountFilePath(ulong account)
    {
        var root = GlobalConfigSO.Instance.GetPersistentRoot();
        var path = Path.Combine(root, $"{account}.account");
        return path;
    }
    protected override void Update()
    {
        base.Update();

        if (_SystemItem.transform.parent.gameObject.activeSelf)
        {
            var systems = GameBattleMgr.Instance.GetBattleSystems();
            foreach (var (key, value) in systems)
            {
                if (!_SystemItems.TryGetValue(key, out var btn))
                {
                    var clickKey = key;
                    btn = GameObject.Instantiate(_SystemItem, _SystemItem.transform.parent);
                    btn.gameObject.SetActive(true);
                    _SystemItems.Add(clickKey, btn);
                    btn.onClick.AddListener(() => OnClick_SystemItem(clickKey));
                    var sys = GameBattleMgr.Instance.GetSystem(clickKey);
                    var txtCom = btn.transform.GetComponentInChildren<TextMeshProUGUI>();
                    txtCom.text = $"{sys.GetType().Name.Replace("Game", "").Replace("System", "").Replace("Battle", "")}";
                }
            }
        }
    }

    private void OnClick_SystemItem(string sysKey)
    {
        var btn = _SystemItems[sysKey];
        var isEnable = GameBattleMgr.Instance.IsEnableSystem(sysKey);
        if (isEnable)
        {
            GameBattleMgr.Instance.DisableSystem(sysKey);
        }
        else
        {
            GameBattleMgr.Instance.EnableSystem(sysKey);
        }
        var img = btn.GetComponent<Image>();
        img.color = isEnable ? Color.red : Color.green;
    }
}
