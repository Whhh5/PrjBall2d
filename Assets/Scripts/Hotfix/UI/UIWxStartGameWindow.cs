#if UNITY_WEBGL && UNITY_MINIGAME_WX
#define UNITY_WX
#endif

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WeChatWASM;

public class UIWxStartGameWindowData : UIWindowData<UIWxStartGameWindow>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIWxStartGameWindow;

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
public class UIWxStartGameWindow : UIWindow
{
#if UNITY_WX
    public class LoginOpenIdResponse
    {
        //public string token;
        public string openId;
    }
    private enum EnWxLoginResult
    {
        None,
        Start,
        Success,
        Faild,
        Timeout,
        Used,
    }

    [SerializeField]
    private Button _LoginBtn;
    [SerializeField]
    private Button _TestBtn;
    //private bool _IsNeedShowPrivacy;
    private string _AccountId = null;

    private EnWxLoginResult _LoginResult = EnWxLoginResult.None;

    public override void OnUnload()
    {
        //WX.OffTouchEnd(LoginCallback);
        _LoginBtn.RemoveListener(OnClick_LoginBtn);
        base.OnUnload();
        //_IsNeedShowPrivacy = false;
        _LoginResult = EnWxLoginResult.None;
        _AccountId = null;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _LoginBtn.AddListener(OnClick_LoginBtn);
        Login();
        //WX.OnTouchEnd(LoginCallback);
        //ShowPrivacy();
    }
    private void Login()
    {
        if (_LoginResult != EnWxLoginResult.None && _LoginResult != EnWxLoginResult.Faild)
            return;
        _LoginResult = EnWxLoginResult.Start;
        WX.Login(new() { success = LoginSuccess, fail = LoginFaild });
    }

    private void OnClick_LoginBtn()
    {
        if (_LoginResult == EnWxLoginResult.Faild)
        {
            Login();
            return;
        }
        if (_LoginResult != EnWxLoginResult.Success)
            return;
        var id = AccountMgr.Instance.GetAccoundId();
        AotDebug.LogWarning(id);
        _LoginResult = EnWxLoginResult.Used;

        AccountMgr.Instance.LoginGame(_AccountId);

    }

    public void LoginSuccess(LoginSuccessCallbackResult result)
    {
        AotDebug.LogWarning($"login info code:{result.code}, errMsg:{result.errMsg}");


        ExchangePhone(result.code, GetOpenIdSuccess, GetOpenIdFail);
    }
    public void LoginFaild(LoginFailCallbackErr result)
    {
        _LoginResult = EnWxLoginResult.Faild;
        AotDebug.LogError($"login errno:{result.errno}, error:{result.errMsg}");
    }

    private void GetOpenIdSuccess(LoginOpenIdResponse response)
    {
        AotDebug.LogWarning($"GetOpenIdSuccess: openId:{response.openId}");
        if (!string.IsNullOrWhiteSpace(response.openId))
        {
            _AccountId = response.openId;
            _LoginResult = EnWxLoginResult.Success;
        }
        else
        {
            _LoginResult = EnWxLoginResult.Faild;
        }
    }
    private void GetOpenIdFail(string msg)
    {
        AotDebug.LogWarning($"GetOpenIdFail: {msg}");
        _LoginResult = EnWxLoginResult.Faild;
    }


    /// <summary>
    /// 调用后端接口换取手机号
    /// </summary>
    /// <param name="code">wx.getPhoneNumber 返回的 code</param>
    /// <param name="onSuccess">成功回调，返回手机号数据</param>
    /// <param name="onError">失败回调，返回错误信息</param>
    public void ExchangePhone(string code, Action<LoginOpenIdResponse> onSuccess, Action<string> onError)
    {
        GameStart.Instance.StartCoroutine(ExchangePhoneCoroutine(code, onSuccess, onError));
    }

    private IEnumerator ExchangePhoneCoroutine(string code, Action<LoginOpenIdResponse> onSuccess, Action<string> onError)
    {
        // 1. 构建请求体
        //WxPhoneRequest requestData = new WxPhoneRequest { code = code };
        //string jsonPayload = JsonUtility.ToJson(requestData);
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        // 2. 构建 Request
        var server = GlobalConfigSO.Instance.GetServerUrlPath();
        var serverUrl = $"{server}/api/wx/wxlogin?code={code}";
        using var request = new UnityWebRequest(serverUrl, "POST");
        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 10; // 10秒超时

        // 3. 发送请求
        yield return request.SendWebRequest();

        // 4. 处理响应
        if (request.result != UnityWebRequest.Result.Success)
        {
            // 网络错误、超时或 HTTP 状态码错误 (如 404, 500)
            string errorMsg = $"网络错误: {request.error} (Code: {request.responseCode})";
            if (!string.IsNullOrEmpty(request.downloadHandler.text))
            {
                errorMsg += $"\nServer Response: {request.downloadHandler.text}";
            }
            onError?.Invoke(errorMsg);
            yield break;
        }

        // 5. 解析 JSON
        string jsonResult = request.downloadHandler.text;
        AotDebug.LogWarning($"server: {jsonResult}");
        try
        {
            var response = JsonUtility.FromJson<LoginOpenIdResponse>(jsonResult);

            // 检查业务逻辑是否成功 (FastAPI 返回的 success 字段)
            if (response != null)
            {
                onSuccess?.Invoke(response);
            }
            else
            {
                // FastAPI 返回 200 但业务失败 (例如微信 code 无效)
                onError?.Invoke($"业务失败");
            }
        }
        catch (Exception ex)
        {
            // JSON 解析失败
            onError?.Invoke($"JSON解析失败: {ex.Message} | Raw: {jsonResult}");
        }
    }

    //public void GetPhoneNumber()
    //{
    //    var option = new GetPhoneNumberOption()
    //    {
    //        isRealtime = false,
    //        phoneNumberNoQuotaToast = true,
    //        success = GetPhoneNumberSuccessCallback,
    //        fail = GetPhoneNumberFailCallback,
    //    };
    //    WX.GetPhoneNumber(option);
    //}
    //public void GetPhoneNumberFailCallback(GeneralCallbackResult result)
    //{
    //    AotDebug.LogError(result.errMsg);
    //}
    //public void GetPhoneNumberSuccessCallback(GetPhoneNumberSuccessCallbackResult result)
    //{
    //    AotDebug.LogWarning($"GetPhoneNumberSuccessCallback: {result.code}, {result.errno}");
    //    if (result.errno > 0)
    //    {
    //        AotDebug.LogError($"{result.errMsg}");
    //        return;
    //    }
    //    ExchangePhone(result.code, ServerGetPhoneNumberSuccessCallback, ServerGetPhoneNumberFailCallback);
    //}
    //public void ServerGetPhoneNumberSuccessCallback(WxPhoneResponse data)
    //{
    //    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
    //    AotDebug.LogWarning(json);

    //    if (ulong.TryParse(data.purePhoneNumber, out var number))
    //    {
    //        AotDebug.LogWarning($"number: {number}");
    //        AccountMgr.Instance.SetAccountId(number);

    //        AccountMgr.Instance.LoginGame(number);
    //    }
    //}
    //public void ServerGetPhoneNumberFailCallback(string data)
    //{
    //    //UIUtil.ShowHitTips(data);
    //    AotDebug.LogError(data);
    //}
    ///// <summary>
    ///// 调用后端接口换取手机号
    ///// </summary>
    ///// <param name="code">wx.getPhoneNumber 返回的 code</param>
    ///// <param name="onSuccess">成功回调，返回手机号数据</param>
    ///// <param name="onError">失败回调，返回错误信息</param>
    //public void ExchangePhone(string code, Action<WxPhoneResponse> onSuccess, Action<string> onError)
    //{
    //    GameStart.Instance.StartCoroutine(ExchangePhoneCoroutine(code, onSuccess, onError));
    //}

    //private IEnumerator ExchangePhoneCoroutine(string code, Action<WxPhoneResponse> onSuccess, Action<string> onError)
    //{
    //    // 1. 构建请求体
    //    //WxPhoneRequest requestData = new WxPhoneRequest { code = code };
    //    //string jsonPayload = JsonUtility.ToJson(requestData);
    //    //byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

    //    // 2. 构建 Request
    //    var server = GlobalConfigSO.Instance.GetServerUrlPath();
    //    var serverUrl = $"{server}/api/wx/getPhoneNumber?code={code}";
    //    using var request = new UnityWebRequest(serverUrl, "POST");
    //    //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //    request.downloadHandler = new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");
    //    request.timeout = 10; // 10秒超时

    //    // 3. 发送请求
    //    yield return request.SendWebRequest();

    //    // 4. 处理响应
    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        // 网络错误、超时或 HTTP 状态码错误 (如 404, 500)
    //        string errorMsg = $"网络错误: {request.error} (Code: {request.responseCode})";
    //        if (!string.IsNullOrEmpty(request.downloadHandler.text))
    //        {
    //            errorMsg += $"\nServer Response: {request.downloadHandler.text}";
    //        }
    //        onError?.Invoke(errorMsg);
    //        yield break;
    //    }

    //    // 5. 解析 JSON
    //    string jsonResult = request.downloadHandler.text;
    //    try
    //    {
    //        WxPhoneResponse response = JsonUtility.FromJson<WxPhoneResponse>(jsonResult);

    //        // 检查业务逻辑是否成功 (FastAPI 返回的 success 字段)
    //        if (response != null && response.success)
    //        {
    //            onSuccess?.Invoke(response);
    //        }
    //        else
    //        {
    //            // FastAPI 返回 200 但业务失败 (例如微信 code 无效)
    //            string errDetail = response?.detail ?? "未知业务错误";
    //            onError?.Invoke($"业务失败: {errDetail} | Raw: {jsonResult}");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // JSON 解析失败
    //        onError?.Invoke($"JSON解析失败: {ex.Message} | Raw: {jsonResult}");
    //    }
    //}

    //private void LoginCallback(OnTouchStartListenerResult response)
    //{
    //    AotDebug.LogWarning("LoginCallback");
    //    if (_IsNeedShowPrivacy)
    //    {
    //        ShowPrivacy();
    //        return;
    //    }
    //    var option = new GetPhoneNumberOption()
    //    {
    //        isRealtime = false,
    //        phoneNumberNoQuotaToast = true,
    //        success = GetPhoneNumberSuccessCallback,
    //        fail = GetPhoneNumberFailCallback,
    //    };
    //    WX.GetPhoneNumber(option);
    //}
    //private void ShowPrivacy()
    //{
    //    AotDebug.LogWarning($"ShowPrivacy");
    //    //WX.OnNeedPrivacyAuthorization(OnNeedPrivacyAuthorization);

    //    var option = new GetPrivacySettingOption()
    //    {
    //        success = GetPrivacySettingSuccessCallback,
    //        fail = GetPrivacySettingFailCallback,
    //    };
    //    WX.GetPrivacySetting(option);
    //}

    //private void GetPrivacySettingSuccessCallback(GetPrivacySettingSuccessCallbackResult result)
    //{
    //    AotDebug.LogWarning($"GetPrivacySettingSuccessCallback: {result.needAuthorization}, {result.privacyContractName}, {result.errMsg}");
    //    if (result.needAuthorization)
    //    {
    //        _IsNeedShowPrivacy = true;
    //        var option = new RequirePrivacyAuthorizeOption()
    //        {
    //            success = OpenPrivacyContractSuccess,
    //            fail = OpenPrivacyContractFail,
    //            //complete = RequirePrivacyAuthorize,
    //        };
    //        WX.RequirePrivacyAuthorize(option);
    //    }
    //    else
    //    {
    //        _IsNeedShowPrivacy = false;
    //    }
    //}

    //private void GetPrivacySettingFailCallback(GeneralCallbackResult result)
    //{
    //    AotDebug.LogError($"GetPrivacySettingFailCallback: {result.errMsg}");
    //    _IsNeedShowPrivacy = true;
    //}

    //private void OpenPrivacyContractSuccess(GeneralCallbackResult result)
    //{
    //    AotDebug.LogWarning($"OpenPrivacyContractSuccess: {result.errMsg}");
    //    var option = new PrivacyAuthorizeResolveOption()
    //    {
    //        eventString = "agree",
    //    };
    //    WX.PrivacyAuthorizeResolve(option);
    //    _IsNeedShowPrivacy = false;
    //}
    //private void OpenPrivacyContractFail(GeneralCallbackResult result)
    //{
    //    AotDebug.LogError($"OpenPrivacyContractFail: {result.errMsg}");
    //    _IsNeedShowPrivacy = true;
    //}

#endif
}
