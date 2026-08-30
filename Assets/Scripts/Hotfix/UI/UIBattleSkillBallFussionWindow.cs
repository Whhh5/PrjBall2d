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
        base.OnPoolDestroy();
        _FussionCfgId = -1;
    }
    protected override void OnPoolInit(UIBattleSkillBallFussionWindowUserData userData)
    {
        _FussionCfgId = userData.fussionCfgId;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _UIWindow.Setting(_FussionCfgId);
    }

}

public class UIBattleSkillBallFussionWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _CloseBtn = null;
    [SerializeField, AutoReference]
    private Image _MergeImg = null;

    private readonly List<LoadHandle> _ImgLoadHandle = new();

    public override void OnUnload()
    {
        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        for (int i = 0; i < _ImgLoadHandle.Count; i++)
        {
            ABBLoadMgr.Instance.Unload(_ImgLoadHandle[i]);
        }
        base.OnUnload();
        _ImgLoadHandle.Clear();
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

    public async void Setting(int fussionCfgId)
    {
        var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(fussionCfgId);
        var targetMat = _MergeImg.material;
        _MergeImg.gameObject.SetActive(false);

        var ballTextures = new List<Texture2D>(fussionCfg.arrSourceBallIds.Length);

        for (int i = 0; i < fussionCfg.arrSourceBallIds.Length; i++)
        {
            var ballId = fussionCfg.arrSourceBallIds[i];
            var ball1Cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            var (isCancel, txt1LoadHandle) = await ABBLoadMgr.Instance.LoadAsync<Texture2D>((EnLoadTarget)ball1Cfg.nIconAssetId, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            _ImgLoadHandle.Add(txt1LoadHandle);

            var img = txt1LoadHandle.GetObject<Texture2D>();
            ballTextures.Add(img);
        }
        {
            var ball3Cfg = GameSchedule.Instance.GetSkillBallCfg0(fussionCfg.nFinalBallId);
            var (isCancel, txt1LoadHandle) = await ABBLoadMgr.Instance.LoadAsync<Texture>((EnLoadTarget)ball3Cfg.nIconAssetId, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            _ImgLoadHandle.Add(txt1LoadHandle);

            var img = txt1LoadHandle.GetObject<Texture>();
            targetMat.SetTexture("_BallBig", img);
        }

        // 构建 Texture2DArray
        if (ballTextures != null && ballTextures.Count > 0)
        {
            int width = ballTextures[0].width;
            int height = ballTextures[0].height;
            var texArr = new Texture2DArray(width, height, ballTextures.Count, TextureFormat.RGBA32, false);

            for (int i = 0; i < ballTextures.Count; i++)
            {
                texArr.SetPixels(ballTextures[i].GetPixels(), i);
            }
            texArr.Apply();

            // 传入Shader
            targetMat.SetTexture("_BallTexArray", texArr);
        }

        // 设置小球数量和初始分布半径
        targetMat.SetInt("_BallCount", ballTextures.Count);

        _MergeImg.gameObject.SetActive(true);

        {
            var isCancel = await DOTween.To(() => 0f, slider =>
            {
                targetMat.SetFloat("_MergeProgress", slider);

            }, 1f, 3f)
                .SetEase(Ease.Linear)
                .ToUniTask(cancellationToken: GetWindowToken()).SuppressCancellationThrow();
        }
    }

}
