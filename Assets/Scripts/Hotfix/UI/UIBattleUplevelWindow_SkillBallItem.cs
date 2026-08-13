using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleUplevelWindow_SkillBallItemUserData : IUIBattleUplevelWindow_ItemUserData
{
    public int ballId;
    public int star;

    public void OnPoolDestroy()
    {
        ballId
            = star
            = -1;
    }
    public IUIBattleUplevelWindow_ItemUserData Copy()
    {
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_SkillBallItemUserData>();
        data.ballId = ballId;
        data.star = star;
        return data;
    }
}

public class UIBattleUplevelWindow_SkillBallItem : UIWindowItem, IUIBattleUplevelWindowItem
{
    [SerializeField]
    private Button _Btn;
    [SerializeField]
    private Image _IconImg;
    [SerializeField]
    private TextMeshProUGUI _NameTxt;
    [SerializeField]
    private TextMeshProUGUI _DescriptionTxt;
    [SerializeField]
    private GameObject _StarGo;
    [SerializeField, AutoReference]
    private Image _FussionImg;
    [SerializeField, AutoReference]
    private GameObject _FussionInfoGo;

    private readonly List<Image> _FussionImgList = new();
    private readonly List<GameObject> _StarList = new();
    private Action _Callback;

    private List<DescriptionInfo> _DescInfos = new();
    private RectTransform _Rect;
    private int _BallId;
    private int _Star;
    public override void Unload()
    {
        for (int i = 0; i < _DescInfos.Count; i++)
        {
            ClassPoolMgr.Instance.Push(_DescInfos[i]);
        }
        _IconImg.UnloadSprite();
        _Btn.RemoveListener(OnClick_Btn);
        for (int i = 0; i < _StarList.Count; i++)
        {
            GameObject.Destroy(_StarList[i]);
        }
        for (int i = 0; i < _FussionImgList.Count; i++)
        {
            var img = _FussionImgList[i];
            img.UnloadSprite();
            GameObject.Destroy(img.gameObject);
        }
        base.Unload();
        _StarList.Clear();
        _DescInfos.Clear();
        _FussionImgList.Clear();
        _Rect = null;
        _BallId
            = _Star
            = -1;
    }
    public override void LoadCompeletion()
    {
        _Rect = GetComponent<RectTransform>();
        base.LoadCompeletion();
        _Btn.AddListener(OnClick_Btn);
        _StarGo.SetActive(false);
        _FussionImg.gameObject.SetActive(false);

    }

    public void Setting(int ballId, int star, Action onclick)
    {
        _BallId = ballId;
        _Star = star;
        _Callback = onclick;

        var starData = GameBattleBallMgr.Instance.GetBallStarData(ballId, star);
        starData.GetDescriptionList(ref _DescInfos, EnDescriptionLevel.Level1);

        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        _IconImg.LoadSprirte((EnLoadTarget)ballCfg.nIconAssetId);
        _NameTxt.text = LanguageUtility.GetLanguage_BallName(ballId);

        var level1DescInfoList = _DescInfos.FindAll(item => item.level == EnDescriptionLevel.Level1);

        var strBuilder = new StringBuilder();
        for (int i = 0; i < level1DescInfoList.Count; i++)
        {
            var info = level1DescInfoList[i];
            var content = string.IsNullOrWhiteSpace(info.title)
                ? $"{info.content}"
                : $"{info.title}:{info.content}";
            if (i < level1DescInfoList.Count - 1)
                strBuilder.AppendLine(content);
            else
                strBuilder.Append(content);
        }
        _DescriptionTxt.SetText(strBuilder.ToString());

        for (int i = 0; i < star; i++)
        {
            var starItem = GameObject.Instantiate(_StarGo, _StarGo.transform.parent);
            starItem.SetActive(true);
            _StarList.Add(starItem);
        }

        var ballIds = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        foreach (var testBallId in ballIds)
        {
            if (testBallId == ballId)
                continue;
            var isFussion = GameBattleBallMgr.Instance.IsFussion(ballId, testBallId);
            if (!isFussion)
                continue;
            var img = GameObject.Instantiate(_FussionImg, _FussionImg.transform.parent);
            _FussionImgList.Add(img);
            img.gameObject.SetActive(true);

            var testBallCfg = GameSchedule.Instance.GetSkillBallCfg0(testBallId);
            img.LoadSprirte((EnLoadTarget)testBallCfg.nIconAssetId);
        }

        _FussionInfoGo.SetActive(_FussionImgList.Count > 0);
    }

    private void OnClick_Btn()
    {
        _Callback?.Invoke();
    }

    public List<DescriptionInfo> GetDescriptionList(EnDescriptionLevel startLevel, EnDescriptionLevel endLevel)
    {
        var result = _DescInfos.FindAll(item => item.level >= startLevel && item.level <= endLevel);
        return result;
    }

    public void Select()
    {
        var curStar = GameBattleBallMgr.Instance.GetBallStar(_BallId);
        var addStar = _Star - curStar;
        GameBattleBallMgr.Instance.AddBall(_BallId, addStar);
    }
    public Vector3 GetWorldPos()
    {
        return _Rect.position;
    }

    public float GetSizeDeltaY()
    {
        return _Rect.rect.height;
    }
}
