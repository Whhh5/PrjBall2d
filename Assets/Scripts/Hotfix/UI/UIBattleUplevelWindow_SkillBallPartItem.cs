using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleUplevelWindow_SkillBallPartItemUserData : IUIBattleUplevelWindow_ItemUserData
{
    public int partId;
    public int star;

    public void OnPoolDestroy()
    {
        partId
            = star
            = -1;
    }
    public IUIBattleUplevelWindow_ItemUserData Copy()
    {
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_SkillBallPartItemUserData>();
        data.partId = partId;
        data.star = star;
        return data;
    }
}


public class UIBattleUplevelWindow_SkillBallPartItem : UIWindowItem, IUIBattleUplevelWindowItem
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

    private readonly List<GameObject> _StarList = new();
    private Action _Callback;

    private List<DescriptionInfo> _DescInfos = new();
    private RectTransform _Rect;
    private int _PartId;
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
        base.Unload();
        _StarList.Clear();
        _DescInfos.Clear();
        _Rect = null;
        _PartId
            = _Star
            = -1;
    }
    public override void LoadCompletion()
    {
        _Rect = GetComponent<RectTransform>();
        base.LoadCompletion();
        _Btn.AddListener(OnClick_Btn);
        _StarGo.SetActive(false);

    }

    public void Setting(int partId, int star, Action onclick)
    {
        _PartId = partId;
        _Star = star;
        _Callback = onclick;

        var starData = GameBattleBallPartsMgr.Instance.GetBallPartStarData(partId, star);
        starData.GetDescriptionList(ref _DescInfos, EnDescriptionLevel.Level1);

        var ballPartCfg = GameSchedule.Instance.GetSkillBallPartCfg0(partId);
        _IconImg.LoadSprirte((EnLoadTarget)ballPartCfg.nIconAssetId);
        _NameTxt.text = LanguageUtility.GetLanguage_BallPartName(partId);

        var level1DescInfoList = _DescInfos.FindAll(item => item.level == EnDescriptionLevel.Level1);

        var strBuilder = new StringBuilder();
        for (int i = 0; i < level1DescInfoList.Count; i++)
        {
            var info = level1DescInfoList[i];
            var content = $"{info.title}:{info.content}";
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
        var curStar = GameBattleBallPartsMgr.Instance.GetPartStar(_PartId);
        var addStar = _Star - curStar;
        GameBattleBallPartsMgr.Instance.AddPart(_PartId, addStar);
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
