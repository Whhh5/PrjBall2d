using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBattleMainWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleMainWindow;
}
public class UIBattleMainWindow : UIWindow
{
    [SerializeField]
    private Button _PauseBtn = null;
    [SerializeField]
    private Image _PauseImg = null;
    [SerializeField]
    private Button _PlaySpeedBtn = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _TitleTxt = null;
    [SerializeField]
    private TextMeshProUGUI _PlaySpeedTxt = null;
    [SerializeField]
    private UIBattleMainWindow_SkillBallIconItem _SkillBallIconItem = null;
    [SerializeField]
    private UIBattleMainWindow_SkillBallIconItem _SkillPassivityIconImg = null;
    [SerializeField]
    private RectTransform _SelectFrameRect = null;
    [SerializeField,AutoReference]
    private Button _RestartBtn = null;
    [SerializeField,AutoReference]
    private Button _ExitBtn = null;

    private readonly Dictionary<int, UIBattleMainWindow_SkillBallIconItem> _SkillBallIconItems = new();
    private readonly Dictionary<int, int> _Ball2Index = new();
    private readonly Dictionary<int, UIBattleMainWindow_SkillBallIconItem> _SkillPassivityIconItems = new();
    private readonly Dictionary<int, int> _SkillPassivity2Index = new();


    private int _TipsWindowId = -1;
    private bool _IsSettingTime = false;

    private int _CurSpeedIndex = -1;
    private int _SpeedSetId = -1;
    private readonly float[] _SpeedChangeArray = { 1, 2, 5, 10, 15, 20 };
    public override void OnUnload()
    {
        _PauseImg.UnloadSprite();
        if (_IsSettingTime)
        {
            TimeMgr.Instance.SetPause(false, true);
        }
        if (_SpeedSetId > 0)
        {
            TimeMgr.Instance.CancelGameSpeed(_SpeedSetId, true);
        }
        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        EventMgr.Instance.Unregister<GameSkillPassivityAddEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_ADD,0, 0, OnEvent_SkillPassivityAdd);
        EventMgr.Instance.Unregister<GameSkillPassivityRemoveEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_REMOVE, 0, 0, OnEvent_SkillPassivityRemove);

        EventMgr.Instance.Unregister<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_BallAdd);
        EventMgr.Instance.Unregister<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_REMOVE, (int)EnBallType.Skill, 0, OnEvent_BallRemove);
        foreach (var (_, img) in _SkillBallIconItems)
        {
            img.Uninit();
            GameObject.Destroy(img.gameObject);
        }
        foreach (var (_, img) in _SkillPassivityIconItems)
        {
            img.Uninit();
            GameObject.Destroy(img.gameObject);
        }
        _PlaySpeedBtn.RemoveListener(OnClick_PlaySpeedBtn);
        _PauseBtn.RemoveListener(OnClick_PauseBtn);
        _RestartBtn.RemoveListener(OnClick_RestartBtn);
        _ExitBtn.RemoveListener(OnClick_ExitBtn);
        base.OnUnload();
        _SkillBallIconItems.Clear();
        _SkillPassivityIconItems.Clear();
        _Ball2Index.Clear();
        _SkillPassivity2Index.Clear();
        _TipsWindowId = -1;
        _CurSpeedIndex = -1;
        _SpeedSetId = -1;
        _IsSettingTime = false;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();

        _SkillBallIconItem.gameObject.SetActive(false);
        _SkillPassivityIconImg.gameObject.SetActive(false);
        _SelectFrameRect.gameObject.SetActive(false);

        _PlaySpeedBtn.AddListener(OnClick_PlaySpeedBtn);
        _PauseBtn.AddListener(OnClick_PauseBtn);
        _RestartBtn.AddListener(OnClick_RestartBtn);
        _ExitBtn.AddListener(OnClick_ExitBtn);

        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        EventMgr.Instance.Register<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_BallAdd);
        EventMgr.Instance.Register<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_REMOVE, (int)EnBallType.Skill, 0, OnEvent_BallRemove);
        
        EventMgr.Instance.Register<GameSkillPassivityAddEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_ADD, 0, 0, OnEvent_SkillPassivityAdd);
        EventMgr.Instance.Register<GameSkillPassivityRemoveEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_REMOVE, 0, 0, OnEvent_SkillPassivityRemove);


        var ballIds = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        for (int i = 0; i < ballIds.Count; i++)
        {
            var ballId = ballIds[i];
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            OnEvent_BallAdd(ballCfg.nBallType, ballId, null);
        }
        SetSpeed(0);
        _PauseImg.LoadSprirte(_IsSettingTime ? EnLoadTarget.Sp_img_pause1 : EnLoadTarget.Sp_img_pause2);

        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        var chapterName = LanguageUtility.GetLanguage(chapterCfg.nNameLangId);
        var titleStr = LanguageUtility.GetLanguage_10014(chapterId, chapterName);
        _TitleTxt.SetText(titleStr);
    }

    private void OnClick_PlaySpeedBtn()
    {
        SetSpeed((_CurSpeedIndex + 1) % _SpeedChangeArray.Length);
    }
    private void OnClick_PauseBtn()
    {
        _IsSettingTime = !_IsSettingTime;
        TimeMgr.Instance.SetPause(_IsSettingTime, false);
        _PauseImg.LoadSprirte(_IsSettingTime ? EnLoadTarget.Sp_img_pause1 : EnLoadTarget.Sp_img_pause2);
    }
    private void OnClick_RestartBtn()
    {
        GameMapMgr.Instance.ReenterMap();
    }
    private void OnClick_ExitBtn()
    {
        GameMapMgr.Instance.ExitMap();

        UIMgr.Instance.ShowLobby();
    }

    private void SetSpeed(int index)
    {
        if (_CurSpeedIndex == index)
            return;
        if (_CurSpeedIndex >= 0)
        {
            TimeMgr.Instance.CancelGameSpeed(_SpeedSetId, true);
        }
        _CurSpeedIndex = index;
        var speed = _SpeedChangeArray[index];
        _SpeedSetId = TimeMgr.Instance.SetGameSpeed(speed, false);
        _PlaySpeedTxt.SetText($"{speed}");
    }

    private void OnClick_BallIcon(int index, int ballId)
    {
        var item = _SkillBallIconItems[ballId];
        if (!_SelectFrameRect.gameObject.activeSelf)
        {
            _SelectFrameRect.position = item.GetWorldPos();
            _SelectFrameRect.gameObject.SetActive(true);
        }

        var tipsUserData = ClassPoolMgr.Instance.Pull<UIBattleSkillBallInfoTipsUserData>();
        tipsUserData.ballId = ballId;
        tipsUserData.star = GameBattleBallMgr.Instance.GetBallStar(ballId);
        _TipsWindowId = UIMgr.Instance.ShowWindow<UIBattleSkillBallInfoTipsData>(tipsUserData);
        ClassPoolMgr.Instance.Push(tipsUserData);
    }
    private void OnClick_SkillPassivityIcon(int index, int skillPassivityId)
    {
        var item = _SkillPassivityIconItems[skillPassivityId];
        if (!_SelectFrameRect.gameObject.activeSelf)
        {
            _SelectFrameRect.position = item.GetWorldPos();
            _SelectFrameRect.gameObject.SetActive(true);
        }

        var tipsUserData = ClassPoolMgr.Instance.Pull<UIBattleSkillPassivityInfoTipsUserData>();
        tipsUserData.skillPassivityId = skillPassivityId;
        tipsUserData.star = GameSkillPassivityMgr.Instance.GetStar(skillPassivityId);
        _TipsWindowId = UIMgr.Instance.ShowWindow<UIBattleSkillPassivityInfoTipsData>(tipsUserData);
        ClassPoolMgr.Instance.Push(tipsUserData);
    }

    private void OnEvent_BallAdd(in int nBallType, in int nBallId, in GameSkillBallAddEventData data)
    {
        var index = _SkillBallIconItems.Count;

        var ballId = nBallId;
        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        var iconItem = GameObject.Instantiate(_SkillBallIconItem, _SkillBallIconItem.transform.parent);
        iconItem.Init((EnLoadTarget)ballCfg.nIconAssetId, () => OnClick_BallIcon(index, ballId));
        iconItem.gameObject.SetActive(true);
        _SkillBallIconItems.Add(ballId, iconItem);
        _Ball2Index.Add(ballId, index);

        UpdatePos();
    }
    private void OnEvent_BallRemove(in int nBallType, in int nBallId, in GameSkillBallRemoveEventData data)
    {
        var index = _Ball2Index[nBallId];

        var item = _SkillBallIconItems[nBallId];
        item.Uninit();
        GameObject.Destroy(item.gameObject);
        _SkillBallIconItems.Remove(nBallId);
        _Ball2Index.Remove(nBallId);
        foreach (var ballId in _SkillBallIconItems.Keys)
        {
            var ballIndex = _Ball2Index[ballId];
            if (ballIndex > index)
            {
                _Ball2Index[ballId] = ballIndex - 1;
            }
        }
        UpdatePos();
    }
    private void OnEvent_SkillPassivityAdd(in int skillPAssivityId, in int _, in GameSkillPassivityAddEventData data)
    {
        var index = _SkillPassivityIconItems.Count;

        var ballId = data.skillPassivityId;
        var ballCfg = GameSchedule.Instance.GetSkillPassivityCfg0(ballId);
        var iconItem = GameObject.Instantiate(_SkillPassivityIconImg, _SkillPassivityIconImg.transform.parent);
        iconItem.Init((EnLoadTarget)ballCfg.nIconAssetId, () => OnClick_SkillPassivityIcon(index, ballId));
        iconItem.gameObject.SetActive(true);
        _SkillPassivityIconItems.Add(ballId, iconItem);
        _SkillPassivity2Index.Add(ballId, index);

        UpdatePos();
    }
    private void OnEvent_SkillPassivityRemove(in int skillPAssivityId, in int _, in GameSkillPassivityRemoveEventData data)
    {
        var index = _SkillPassivity2Index[data.skillPassivityId];

        var item = _SkillPassivityIconItems[data.skillPassivityId];
        item.Uninit();
        GameObject.Destroy(item.gameObject);
        _SkillPassivityIconItems.Remove(data.skillPassivityId);
        _SkillPassivity2Index.Remove(data.skillPassivityId);
        foreach (var ballId in _SkillPassivityIconItems.Keys)
        {
            var ballIndex = _SkillPassivity2Index[ballId];
            if (ballIndex > index)
            {
                _SkillPassivity2Index[ballId] = ballIndex - 1;
            }
        }
        UpdatePos();
    }

    private void UpdatePos()
    {
        //var width = GetComponent<RectTransform>().rect.width;

        //var col = Mathf.FloorToInt(Mathf.Min(width * 0.7f / _IconSize.x, _SkillBallIconItems.Count));
        //if (col == 0)
        //    return;
        //var interval = new Vector2(30, 30);
        //var startLocalPosX = -(col * _IconSize.x + (col - 1) * interval.x) / 2;
        //var startLocalPosY = 0;

        //foreach (var (ballId, iconItem) in _SkillBallIconItems)
        //{
        //    var index = _Ball2Index[ballId];
        //    var i = Mathf.FloorToInt(index / col);
        //    var j = index % col;

        //    var localPos = new Vector2(_IconSize.x * (j + 0.5f), -_IconSize.y * i) + new Vector2(j, -i) * interval;
        //    iconItem.SetLocalPos(localPos + new Vector2(startLocalPosX, startLocalPosY));
        //}
    }

    protected override void Update()
    {
        base.Update();

        if (_TipsWindowId > 0 && !UIMgr.Instance.IsValid(_TipsWindowId))
        {
            if (_SelectFrameRect.gameObject.activeSelf)
            {
                _SelectFrameRect.gameObject.SetActive(false);
            }
        }
    }
}
