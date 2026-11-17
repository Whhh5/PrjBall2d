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
    [SerializeField, AutoReference]
    private Button _PauseBtn = null;
    [SerializeField, AutoReference]
    private Button _LeftSpeedBtn = null;
    [SerializeField, AutoReference]
    private Button _RightSpeedBtn = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _TitleTxt = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _PlaySpeedTxt = null;
    [SerializeField, AutoReference]
    private UIBattleMainWindow_SkillBallIconItem _SkillBallIconItem = null;
    [SerializeField, AutoReference]
    private UIBattleMainWindow_SkillBallIconItem _SkillPassivityIconImg = null;
    [SerializeField, AutoReference]
    private Slider _HpSlider = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _HpValueTxt = null;
    [SerializeField, AutoReference]
    private Slider _EnergySlider = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _EnergyValueTxt = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _EnergyLevelTxt = null;
    [SerializeField, AutoReference]
    private Slider _WaveSlider = null;
    [SerializeField, AutoReference]
    private Image _PlayerIconImg = null;
    [SerializeField, AutoReference(nameof(_PlayerIconImg))]
    private RectTransform _PlayerIconRect = null;

    private readonly Dictionary<int, UIBattleMainWindow_SkillBallIconItem> _SkillBallIconItems = new();
    private readonly Dictionary<int, int> _Ball2Index = new();
    private readonly Dictionary<int, UIBattleMainWindow_SkillBallIconItem> _SkillPassivityIconItems = new();
    private readonly Dictionary<int, int> _SkillPassivity2Index = new();


    private bool _IsSettingTime = false;
    private int _PlayerId = -1;
    private int _CurSpeedIndex = -1;
    private int _SpeedSetId = -1;
    private float _WaveToSliderValue = 0;
    private readonly float[] _SpeedChangeArray = { 1, 2 };
    public override void OnUnload()
    {
        _PlayerIconImg.UnloadSprite();
        if (_IsSettingTime)
        {
            TimeMgr.Instance.SetPause(false, true);
        }
        if (_SpeedSetId > 0)
        {
            TimeMgr.Instance.CancelGameSpeed(_SpeedSetId, true);
        }

        EventMgr.Instance.Unregister<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _PlayerId, (int)EnEntityAttributeId.Hp, OnEvent_HpChange);
        EventMgr.Instance.Unregister<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _PlayerId, (int)EnEntityAttributeId.LimitHp, OnEvent_HpChange);

        EventMgr.Instance.Unregister<GameBattleEnergyChangeEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_CHANGE, 0, 0, OnEvent_EnergyChange);
        EventMgr.Instance.Unregister<GameBattleEnergyUplevelEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, OnEvent_EnergyLevelChange);

        EventMgr.Instance.Unregister<GameSkillPassivityAddEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_ADD, 0, 0, OnEvent_SkillPassivityAdd);
        EventMgr.Instance.Unregister<GameSkillPassivityRemoveEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_REMOVE, 0, 0, OnEvent_SkillPassivityRemove);

        EventMgr.Instance.Unregister<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_BallAdd);
        EventMgr.Instance.Unregister<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_REMOVE, (int)EnBallType.Skill, 0, OnEvent_BallRemove);

        EventMgr.Instance.Unregister<GameBattleNextWaveEventData>(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, OnEvent_NexeWave);
        EventMgr.Instance.Unregister<GameBattleBossDebutEventData>(EnABBEvent.EVENT_BATTLE_BOSS_DEBUT, 0, 0, OnEvent_BossDebut);
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
        _LeftSpeedBtn.RemoveListener(OnClick_LeftSpeedBtn);
        _RightSpeedBtn.RemoveListener(OnClick_RightSpeedBtn);
        _PauseBtn.RemoveListener(OnClick_PauseBtn);
        base.OnUnload();
        _SkillBallIconItems.Clear();
        _SkillPassivityIconItems.Clear();
        _Ball2Index.Clear();
        _SkillPassivity2Index.Clear();
        _CurSpeedIndex = -1;
        _SpeedSetId = -1;
        _IsSettingTime = false;
        _PlayerId = -1;
        _WaveToSliderValue = 0;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();

        _WaveSlider.gameObject.SetActive(true);
        _TitleTxt.gameObject.SetActive(true);
        _WaveSlider.value = 0;

        _SkillBallIconItem.gameObject.SetActive(false);
        _SkillPassivityIconImg.gameObject.SetActive(false);

        _LeftSpeedBtn.AddListener(OnClick_LeftSpeedBtn);
        _RightSpeedBtn.AddListener(OnClick_RightSpeedBtn);
        _PauseBtn.AddListener(OnClick_PauseBtn);

        _PlayerId = GamePlayerMgr.Instance.GetPlayerEntityId();
        EventMgr.Instance.Register<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _PlayerId, (int)EnEntityAttributeId.Hp, OnEvent_HpChange);
        EventMgr.Instance.Register<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _PlayerId, (int)EnEntityAttributeId.LimitHp, OnEvent_HpChange);

        EventMgr.Instance.Register<GameSkillBallAddEventData>(EnABBEvent.EVENT_SKILL_BALL_ADD, (int)EnBallType.Skill, 0, OnEvent_BallAdd);
        EventMgr.Instance.Register<GameSkillBallRemoveEventData>(EnABBEvent.EVENT_SKILL_BALL_REMOVE, (int)EnBallType.Skill, 0, OnEvent_BallRemove);

        EventMgr.Instance.Register<GameSkillPassivityAddEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_ADD, 0, 0, OnEvent_SkillPassivityAdd);
        EventMgr.Instance.Register<GameSkillPassivityRemoveEventData>(EnABBEvent.EVENT_SKILL_PASSIVITY_REMOVE, 0, 0, OnEvent_SkillPassivityRemove);

        EventMgr.Instance.Register<GameBattleEnergyChangeEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_CHANGE, 0, 0, OnEvent_EnergyChange);
        EventMgr.Instance.Register<GameBattleEnergyUplevelEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, OnEvent_EnergyLevelChange);

        EventMgr.Instance.Register<GameBattleNextWaveEventData>(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, OnEvent_NexeWave);
        EventMgr.Instance.Register<GameBattleBossDebutEventData>(EnABBEvent.EVENT_BATTLE_BOSS_DEBUT, 0, 0, OnEvent_BossDebut);

        var ballIds = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        for (int i = 0; i < ballIds.Count; i++)
        {
            var ballId = ballIds[i];
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            OnEvent_BallAdd(ballCfg.nBallType, ballId, null);
        }
        SetSpeed(0);
        _PauseBtn.SetComValid(_IsSettingTime);

        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        var chapterName = LanguageUtility.GetLanguage(chapterCfg.nNameLangId);
        var titleStr = LanguageUtility.GetLanguage_10014(chapterId, chapterName);
        _TitleTxt.SetText(titleStr);

        var characterId = GamePlayerMgr.Instance.GetCharacterId();
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(characterId);
        var monstercfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterId);
        _PlayerIconImg.LoadSprirte(monstercfg.nIconAssetID);

        var sys = GameBattleMgr.Instance.GetSystem<GameBattleEnergySystem>();
        var cur = sys.GetCurrentEnergyValue();
        var max = sys.GetMaxEnergyValue();
        var level = sys.GetEnergyLevel();
        UpdateEnergySliderInfo(cur, max);
        UpdateEnergyLevelInfo(level);

        UpdateHpSliderInfo();
        UpdateSpeedButtonStatus();
        UpdateWaveSliderValue();
    }

    private void OnClick_LeftSpeedBtn()
    {
        if (_CurSpeedIndex <= 0)
            return;
        SetSpeed(_CurSpeedIndex - 1);
        UpdateSpeedButtonStatus();
    }
    private void OnClick_RightSpeedBtn()
    {
        if (_CurSpeedIndex >= _SpeedChangeArray.Length - 1)
            return;
        SetSpeed(_CurSpeedIndex + 1);
        UpdateSpeedButtonStatus();
    }
    private void UpdateSpeedButtonStatus()
    {
        _LeftSpeedBtn.gameObject.SetActive(_CurSpeedIndex > 0);
        _RightSpeedBtn.gameObject.SetActive(_CurSpeedIndex < _SpeedChangeArray.Length - 1);
    }
    private void OnClick_PauseBtn()
    {
        UIMgr.Instance.ShowWindow<UIBattleSettingWindowData>();
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
        var tipsUserData = ClassPoolMgr.Instance.Pull<UIBattleSkillBallInfoTipsUserData>();
        tipsUserData.ballId = ballId;
        tipsUserData.star = GameBattleBallMgr.Instance.GetBallStar(ballId);
        UIMgr.Instance.ShowWindow<UIBattleSkillBallInfoTipsData>(tipsUserData);
        ClassPoolMgr.Instance.Push(tipsUserData);
    }
    private void OnClick_SkillPassivityIcon(int index, int skillPassivityId)
    {
        var tipsUserData = ClassPoolMgr.Instance.Pull<UIBattleSkillPassivityInfoTipsUserData>();
        tipsUserData.skillPassivityId = skillPassivityId;
        tipsUserData.star = GameSkillPassivityMgr.Instance.GetStar(skillPassivityId);
        UIMgr.Instance.ShowWindow<UIBattleSkillPassivityInfoTipsData>(tipsUserData);
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
    }
    private void OnEvent_HpChange(in int entityId, in int enAttId, in GameBattleEntityAttributeChangeEventData data)
    {
        UpdateHpSliderInfo();
    }
    private void OnEvent_EnergyChange(in int _, in int __, in GameBattleEnergyChangeEventData data)
    {
        UpdateEnergySliderInfo(data.currentValue, data.maxValue);
    }
    private void OnEvent_EnergyLevelChange(in int _, in int __, in GameBattleEnergyUplevelEventData data)
    {
        UpdateEnergyLevelInfo(data.currentLevel);
    }
    private void OnEvent_BossDebut(in int _, in int __, in GameBattleBossDebutEventData data)
    {
        _WaveSlider.gameObject.SetActive(false);
        _TitleTxt.gameObject.SetActive(false);
    }
    private void OnEvent_NexeWave(in int _, in int __, in GameBattleNextWaveEventData data)
    {
        UpdateWaveSliderValue();
    }

    private void UpdateWaveSliderValue()
    {
        var sys = GameBattleMgr.Instance.GetSystem<GameBattleWaveSystem>();
        var maxWave = sys.GetWaveCount();
        var curWaveId = sys.GetCurWaveId();
        var slider = Mathf.Clamp01(curWaveId / (float)maxWave);
        _WaveToSliderValue = slider;
    }

    private void UpdateHpSliderInfo()
    {
        var curHp = GameAttributeUtility.GetValueInt(_PlayerId, EnEntityAttributeId.Hp);
        var maxHp = GameAttributeUtility.GetValueInt(_PlayerId, EnEntityAttributeId.LimitHp);
        var slider = (float)curHp / maxHp;
        _HpSlider.value = Mathf.Clamp01(slider);
        _HpValueTxt.SetText($"{curHp}");
    }
    private void UpdateEnergySliderInfo(int cur, int max)
    {
        _EnergyValueTxt.SetText($"{cur}/{max}");

        var slider = max > 0 ? cur / (float)max : 0;
        _EnergySlider.value = Mathf.Clamp01(slider);
    }
    private void UpdateEnergyLevelInfo(int level)
    {
        _EnergyLevelTxt.SetText($"{level}");
    }

    protected override void Update()
    {
        base.Update();
        
        if (_WaveSlider.gameObject.activeSelf)
        {
            var toSlider = Mathf.Clamp(_WaveSlider.value + ABBUtil.GetTimeDelta() / 10, 0, _WaveToSliderValue);
            _WaveSlider.value = toSlider;

            var angle = Mathf.Sin(ABBUtil.GetGameTimeSeconds() % 1 * 2 * Mathf.PI) * 10 + 5;
            _PlayerIconRect.eulerAngles = Vector3.forward * angle;
        }
    }
}
