using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class UIBattleSkillBallFussionWindowUserData : UIWindowUserData
{
    public int fussionCfgId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        fussionCfgId = -1;
    }
}
public class UIBattleSkillBallFussionWindowData : UIWindowData<UIBattleSkillBallFussionWindow, UIBattleSkillBallFussionWindowUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleSkillBallFussionWindow;

    public int _FussionCfgId;
    public override void OnPoolDestroy()
    {
        TimeMgr.Instance.SetPause(false, true);
        base.OnPoolDestroy();
        _FussionCfgId = -1;
    }
    protected override void OnPoolInit(UIBattleSkillBallFussionWindowUserData userData)
    {
        _FussionCfgId = userData.fussionCfgId;
        TimeMgr.Instance.SetPause(true, true);
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        //_UIWindow.Setting(_FussionCfgId);
        _UIWindow.Setting2(_FussionCfgId);
    }

}

public class UIBattleSkillBallFussionWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _CloseBtn = null;
    //[SerializeField, AutoReference]
    //private Image _MergeImg = null;
    [SerializeField, AutoReference]
    private Image _Source1Img = null;
    [SerializeField, AutoReference]
    private Image _Source2Img = null;
    [SerializeField, AutoReference]
    private Image _ResultImg = null;

    //private readonly List<LoadHandle> _ImgLoadHandle = new();
    private EnDotweenId _DotweenId = EnDotweenId.UIBattleSkillBallFussionWindow;
    public override void OnUnload()
    {
        DOTween.Kill(_DotweenId);
        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        //for (int i = 0; i < _ImgLoadHandle.Count; i++)
        //{
        //    ABBLoadMgr.Instance.Unload(_ImgLoadHandle[i]);
        //}
        base.OnUnload();
        //_ImgLoadHandle.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _CloseBtn.AddListener(OnClick_CloseBtn);
    }
    private void OnClick_CloseBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }

    public void Setting2(int fussionCfgId)
    {
        var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(fussionCfgId);
        //_MergeImg.gameObject.SetActive(false);

        var ballId1 = fussionCfg.arrSourceBallIds[0];
        var ball1Cfg1 = GameSchedule.Instance.GetSkillBallCfg0(ballId1);
        _Source1Img.LoadSprirte(ball1Cfg1.nIconAssetId);

        var ballId2 = fussionCfg.arrSourceBallIds[1];
        var ball1Cfg2 = GameSchedule.Instance.GetSkillBallCfg0(ballId2);
        _Source2Img.LoadSprirte(ball1Cfg2.nIconAssetId);

        var resultBallCfg = GameSchedule.Instance.GetSkillBallCfg0(fussionCfg.nFinalBallId);
        _ResultImg.LoadSprirte(resultBallCfg.nIconAssetId);

        PlayAnimationAsync();
    }

    private async void PlayAnimationAsync()
    {
        _CloseBtn.interactable = false;
        var imgRect1 = _Source1Img.GetComponent<RectTransform>();
        var imgRect2 = _Source2Img.GetComponent<RectTransform>();
        var resuktRect = _ResultImg.GetComponent<RectTransform>();

        var maxX = 200;
        var maxTime = 1f;
        var moveTiem = 0.5f;
        await DOTween.To(() => 0f, slider =>
        {
            var posSlider = Mathf.Lerp(0, maxTime * moveTiem, slider) / 0.5f;
            var imgPosX = Mathf.Lerp(0, maxX, posSlider);
            var pos1 = new Vector2(maxX - imgPosX, 0);
            var pos2 = new Vector2(-pos1.x, 0);
            imgRect1.anchoredPosition = pos1;
            imgRect2.anchoredPosition = pos2;

            var scaleSlider = Mathf.Clamp01(Mathf.Lerp(slider, maxTime, slider) - moveTiem) / (1 - moveTiem);
            var scale = Mathf.Lerp(0, 1f, scaleSlider);
            resuktRect.localScale = Vector3.one * scale;

        }, 1f, maxTime)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .SetId(_DotweenId);
        _CloseBtn.interactable = true;
    }

    //public async void Setting(int fussionCfgId)
    //{
    //    var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(fussionCfgId);
    //    var targetMat = _MergeImg.material;
    //    _MergeImg.gameObject.SetActive(false);

    //    var ballTextures = new List<Texture2D>(fussionCfg.arrSourceBallIds.Length);

    //    for (int i = 0; i < fussionCfg.arrSourceBallIds.Length; i++)
    //    {
    //        var ballId = fussionCfg.arrSourceBallIds[i];
    //        var ball1Cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
    //        var (isCancel, txt1LoadHandle) = await ABBLoadMgr.Instance.LoadAsync<Texture2D>((EnLoadTarget)ball1Cfg.nIconAssetId, GetWindowToken())
    //            .SuppressCancellationThrow();
    //        if (isCancel)
    //            return;
    //        _ImgLoadHandle.Add(txt1LoadHandle);

    //        var img = txt1LoadHandle.GetObject<Texture2D>();
    //        ballTextures.Add(img);
    //    }
    //    {
    //        var ball3Cfg = GameSchedule.Instance.GetSkillBallCfg0(fussionCfg.nFinalBallId);
    //        var (isCancel, txt1LoadHandle) = await ABBLoadMgr.Instance.LoadAsync<Texture>((EnLoadTarget)ball3Cfg.nIconAssetId, GetWindowToken())
    //            .SuppressCancellationThrow();
    //        if (isCancel)
    //            return;
    //        _ImgLoadHandle.Add(txt1LoadHandle);

    //        var img = txt1LoadHandle.GetObject<Texture>();
    //        targetMat.SetTexture("_BallBig", img);
    //    }

    //    // 构建 Texture2DArray
    //    if (ballTextures != null && ballTextures.Count > 0)
    //    {
    //        int width = ballTextures[0].width;
    //        int height = ballTextures[0].height;
    //        var texArr = new Texture2DArray(width, height, ballTextures.Count, TextureFormat.RGBA32, false);

    //        for (int i = 0; i < ballTextures.Count; i++)
    //        {
    //            texArr.SetPixels(ballTextures[i].GetPixels(), i);
    //        }
    //        texArr.Apply();

    //        // 传入Shader
    //        targetMat.SetTexture("_BallTexArray", texArr);
    //    }

    //    // 设置小球数量和初始分布半径
    //    targetMat.SetInt("_BallCount", ballTextures.Count);

    //    _MergeImg.gameObject.SetActive(true);

    //    {
    //        var isCancel = await DOTween.To(() => 0f, slider =>
    //        {
    //            targetMat.SetFloat("_MergeProgress", slider);

    //        }, 1f, 3f)
    //            .SetEase(Ease.Linear)
    //            .SetUpdate(true)
    //            .ToUniTask(cancellationToken: GetWindowToken()).SuppressCancellationThrow();
    //    }
    //}

}
