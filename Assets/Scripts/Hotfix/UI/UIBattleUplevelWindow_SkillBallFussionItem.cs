using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleUplevelWindow_SkillBallFussionItemUserData : IUIBattleUplevelWindow_ItemUserData
{
    public int fussionId;

    public void OnPoolDestroy()
    {
        fussionId = -1;
    }
    public IUIBattleUplevelWindow_ItemUserData Copy()
    {
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_SkillBallFussionItemUserData>();
        data.fussionId = fussionId;
        return data;
    }
}

public class UIBattleUplevelWindow_SkillBallFussionItem : UIWindowItem, IUIBattleUplevelWindowItem
{
    [SerializeField, AutoReference(null, SearchScope.Self)]
    private Button _Btn;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _NameTxt;
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference(nameof(_IconImg))]
    private Button _IconBtn;
    [SerializeField, AutoReference]
    private Image _SourceImg;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _DescriptionTxt;

    private Action _Callback;
    private int _FussionCfgId;
    private readonly List<Image> _SourceImgs = new();
    private List<DescriptionInfo> _DescInfos = new();
    public override void Unload()
    {
        foreach (var item in _SourceImgs)
        {
            item.UnloadSprite();
            GameObject.Destroy(item.gameObject);
        }
        for (int i = 0; i < _DescInfos.Count; i++)
        {
            ClassPoolMgr.Instance.Push(_DescInfos[i]);
        }
        _Btn.RemoveListener(OnClick_Btn);
        _IconBtn.RemoveListener(OnClick_IconBtn);

        base.Unload();

        _FussionCfgId = -1;
        _SourceImgs.Clear();
        _DescInfos.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _Btn.AddListener(OnClick_Btn);
        _IconBtn.AddListener(OnClick_IconBtn);
        _SourceImg.gameObject.SetActive(false);
    }

    public void Setting(int fussionId, Action onclick)
    {
        _FussionCfgId = fussionId;
        _Callback = onclick;


        var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(fussionId);
        var finalBallCfg = GameSchedule.Instance.GetSkillBallCfg0(fussionCfg.nFinalBallId);
        _IconImg.LoadSprirte((EnLoadTarget)finalBallCfg.nIconAssetId);
        var name = LanguageUtility.GetLanguage(finalBallCfg.nNameLangId);
        var title = LanguageUtility.GetLanguage_1011(name);
        _NameTxt.SetText(title);
        for (int i = 0; i < fussionCfg.arrSourceBallIds.Length; i++)
        {
            var ballId = fussionCfg.arrSourceBallIds[i];
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            var img = GameObject.Instantiate(_SourceImg, _SourceImg.transform.parent);
            img.LoadSprirte((EnLoadTarget)ballCfg.nIconAssetId);
            img.gameObject.SetActive(true);
            _SourceImgs.Add(img);
        }

        var starData = GameBattleBallMgr.Instance.GetBallStarData(fussionCfg.nFinalBallId, fussionCfg.nFinalBallStar);
        starData.GetDescriptionList(ref _DescInfos, EnDescriptionLevel.Level1);
        var level1DescInfoList = _DescInfos.FindAll(item => item.level == EnDescriptionLevel.Level1);

        var strBuilder = new StringBuilder();
        for (int i = 0; i < level1DescInfoList.Count; i++)
        {
            var info = level1DescInfoList[i];
            var content = $"{info.content}";
            if (i < level1DescInfoList.Count - 1)
                strBuilder.AppendLine(content);
            else
                strBuilder.Append(content);
        }
        _DescriptionTxt.SetText(strBuilder.ToString());
    }

    private void OnClick_Btn()
    {
        _Callback?.Invoke();
    }
    private void OnClick_IconBtn()
    {
        var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(_FussionCfgId);
        var ud = ClassPoolMgr.Instance.Pull<UIBattleSkillBallInfoTipsUserData>();
        ud.ballId = fussionCfg.nFinalBallId;
        ud.star = 1;
        UIMgr.Instance.ShowWindow<UIBattleSkillBallInfoTipsData>(ud);
        ClassPoolMgr.Instance.Push(ud);
    }

    public List<DescriptionInfo> GetDescriptionList(EnDescriptionLevel startLevel, EnDescriptionLevel endLevel)
    {
        var result = _DescInfos.FindAll(item => item.level >= startLevel && item.level <= endLevel);
        return result;
    }

    public void Select()    
    {
        GameBattleBallMgr.Instance.Fussion(_FussionCfgId);

        var data = ClassPoolMgr.Instance.Pull<UIBattleSkillBallFussionWindowUserData>();
        data.fussionCfgId = _FussionCfgId;
        UIMgr.Instance.ShowWindow<UIBattleSkillBallFussionWindowData>(data);
        ClassPoolMgr.Instance.Push(data);
    }

    public float GetSizeDeltaY()
    {
        return _Rect.rect.height;
    }
}
