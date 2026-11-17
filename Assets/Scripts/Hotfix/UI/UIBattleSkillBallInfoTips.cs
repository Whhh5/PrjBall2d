using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIBattleSkillBallInfoTipsUserData : UITipsUserData
{
    public int ballId;
    public int star;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ballId
            = star
            = -1;
    }
}
public class UIBattleSkillBallInfoTipsData : UITipsData<UIBattleSkillBallInfoTips, UIBattleSkillBallInfoTipsUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleSkillBallInfoTips;


    private int _BallId;
    private int _Star;
    public override void OnPoolDestroy()
    {
        TimeMgr.Instance.SetPause(false, false);
        base.OnPoolDestroy();
        _BallId
            = _Star
            = -1;
    }
    protected override void OnPoolInit(UIBattleSkillBallInfoTipsUserData userData)
    {
        _BallId = userData.ballId;
        _Star = userData.star;

        TimeMgr.Instance.SetPause(true, true);
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(_BallId);
        var starData = GameBattleBallMgr.Instance.GetBallStarData(_BallId, _Star);
        var list = new List<DescriptionInfo>(10);
        starData.GetDescriptionList(ref list, EnDescriptionLevel.Level1);

        var nameStr = LanguageUtility.GetLanguage(ballCfg.nNameLangId);
        _UITips.SettingBallInfo(nameStr, (EnLoadTarget)ballCfg.nIconAssetId, list, _Star);

        for (int i = 0; i < list.Count; i++)
        {
            ClassPoolMgr.Instance.Push(list[i]);
        }
    }
}

public class UIBattleSkillBallInfoTips : UITips
{
    [SerializeField]
    private TextMeshProUGUI _NameTxt;
    [SerializeField]
    private GameObject _StarGo;
    [SerializeField]
    private TextMeshProUGUI _DescLevel1Txt;
    [SerializeField]
    private TextMeshProUGUI _DescLevel2Txt;
    [SerializeField]
    private RectTransform _DescRootRect;
    [SerializeField, AutoReference]
    private Image _IconImg;


    private readonly List<UIDescriptionLevel3WindowItem> _DescLevelTxts = new();
    private readonly List<GameObject> _StarGods = new();
    public override void OnUnload()
    {
        _IconImg.UnloadSprite();
        for (int i = 0; i < _DescLevelTxts.Count; i++)
        {
            var item = _DescLevelTxts[i];
            UnregisterUIDissolve(item);
            UIMgr.Instance.UnloadWindowItem(item);
        }
        for (int i = 0; i < _StarGods.Count; i++)
            GameObject.Destroy(_StarGods[i]);
        base.OnUnload();
        _StarGods.Clear();
        _DescLevelTxts.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _StarGo.SetActive(false);
    }
    public async void SettingBallInfo(string title, EnLoadTarget iconAssetId, List<DescriptionInfo> descInfos, int star)
    {
        _IconImg.LoadSprirte(iconAssetId);
        _NameTxt.SetText(title);
        var level2Description = new Dictionary<EnDescriptionLevel, List<DescriptionInfo>>(descInfos.Count);
        foreach (var item in descInfos)
        {
            if (!level2Description.TryGetValue(item.level, out var levelList))
            {
                levelList = new();
                level2Description.Add(item.level, levelList);
            }
            levelList.Add(item);
        }
        var descStr = new StringBuilder();
        if (level2Description.TryGetValue(EnDescriptionLevel.Level1, out var descLevel1InfoList))
        {
            for (int i = 0; i < descLevel1InfoList.Count; i++)
            {
                var info = descLevel1InfoList[i];
                var content = string.IsNullOrWhiteSpace(info.title)
                    ? $"{info.content}"
                    :$"{info.title}:{info.content}";
                if (i < descLevel1InfoList.Count - 1)
                    descStr.AppendLine(content);
                else
                    descStr.Append(content);
            }
            _DescLevel1Txt.text = descStr.ToString();
        }
        descStr.Clear();
        if (level2Description.TryGetValue(EnDescriptionLevel.Level2, out var descLevel2InfoList))
        {
            for (int i = 0; i < descLevel2InfoList.Count; i++)
            {
                var info = descLevel2InfoList[i];
                var content = string.IsNullOrWhiteSpace(info.title)
                    ? $"{info.content}"
                    : $"{info.title}:{info.content}";
                if (i < descLevel2InfoList.Count - 1)
                    descStr.AppendLine(content);
                else
                    descStr.Append(content);
            }
        }
        _DescLevel2Txt.text = descStr.ToString();

        for (var j = EnDescriptionLevel.Level3; j < EnDescriptionLevel.Count; j++)
        {
            descStr.Clear();
            if (!level2Description.TryGetValue(j, out var descLevel3InfoList))
                break;
            for (int i = 0; i < descLevel3InfoList.Count; i++)
            {
                var info = descLevel3InfoList[i];
                var content = string.IsNullOrWhiteSpace(info.title)
                    ? $"{info.content}"
                    : $"{info.title}:{info.content}";
                if (i < descLevel3InfoList.Count - 1)
                    descStr.AppendLine(content);
                else
                    descStr.Append(content);
            }
            var (isCancel, level3Item) = await UIMgr.Instance.LoadWindowItemAsync<UIDescriptionLevel3WindowItem>(EnLoadTarget.Pre_UIDescriptionLevel3WindowItem, _DescRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            level3Item.Setting(descStr.ToString());
            RegisterUIDissolve(level3Item);
            _DescLevelTxts.Add(level3Item);
        }

        for (int i = 0; i < star; i++)
        {
            var starIns = GameObject.Instantiate(_StarGo, _StarGo.transform.parent);
            starIns.SetActive(true);
            _StarGods.Add(starIns);
        }
    }
}
