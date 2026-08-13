using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IUIBattleUplevelWindow_ItemUserData : IClassPoolDestroy
{
    public IUIBattleUplevelWindow_ItemUserData Copy();
}

public class UIBattleUplevelWindow_AttributeItemUserData : IUIBattleUplevelWindow_ItemUserData
{
    public EnEntityAttributeId attId;
    public int star;
    public EnValueType valueType;
    public float addValue;

    public IUIBattleUplevelWindow_ItemUserData Copy()
    {
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_AttributeItemUserData>();
        data.attId = attId;
        data.star = star;
        data.valueType = valueType;
        data.addValue = addValue;
        return data;
    }

    public void OnPoolDestroy()
    {
        valueType = EnValueType.None;
        attId = EnEntityAttributeId.None;
        star = -1;
        addValue = -1;
    }
}

public class UIBattleUplevelWindow_AttributeItem : UIWindowItem, IUIBattleUplevelWindowItem
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
    private TextMeshProUGUI _CurVlaueTxt;
    [SerializeField]
    private GameObject _StarGo;

    private readonly List<GameObject> _StarList = new();
    private Action _Callback;

    private readonly List<DescriptionInfo> _DescInfos = new();
    private RectTransform _Rect;
    private EnEntityAttributeId _AttId;
    private float _AddValue;
    private EnValueType _AddValueType;
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
        _AddValue = -1;
        _AttId = EnEntityAttributeId.None;
        _AddValueType = EnValueType.None;
    }
    public override void LoadCompeletion()
    {
        _Rect = GetComponent<RectTransform>();
        base.LoadCompeletion();
        _Btn.AddListener(OnClick_Btn);
        _StarGo.SetActive(false);

    }

    public void Setting(EnEntityAttributeId attId, int star, float addValue, EnValueType valueType, Action onclick)
    {
        _AttId = attId;
        _Callback = onclick;

        _AddValue = addValue;
        _AddValueType = valueType;

        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        _IconImg.LoadSprirte((EnLoadTarget)attCfg.nIconAssetId);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
        var titleStr = LanguageUtility.GetLanguage_10003();
        _NameTxt.SetText(string.Format(titleStr, attName));

        var contentStr = valueType switch
        {
            EnValueType.Percentage => string.Format(LanguageUtility.GetLanguage_10005(), attName, addValue * 100),
            _ => string.Format(LanguageUtility.GetLanguage_10004(), attName, addValue),
        };
        var descLevelInfo = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descLevelInfo.title = attName;
        descLevelInfo.level = EnDescriptionLevel.Level1;
        descLevelInfo.content = contentStr;

        _DescInfos.Add(descLevelInfo);

        _DescriptionTxt.SetText(contentStr);


        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(playerEntityId, EnSceneEntityComType.Attribute);
        var curValueDesc = LanguageUtility.GetLanguage_10006();
        var curValue = attCom.GetValue(attId);
        var valueStr = (EnValueType)attCfg.nValueType switch
        {
            EnValueType.Percentage => string.Format(LanguageUtility.GetLanguage_10008(), curValue * 100),
            _ => string.Format(LanguageUtility.GetLanguage_10007(), curValue),
        };
        _CurVlaueTxt.SetText(string.Format(curValueDesc, valueStr));

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
        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var worldPos = EntityMgr.Instance.GetEntityWorldPos2D(playerEntityId);

        switch (_AttId)
        {
            case EnEntityAttributeId.LimitHp:
                var lastValue = GameAttributeUtility.GetValue(playerEntityId, EnEntityAttributeId.LimitHp);
                switch (_AddValueType)
                {
                    case EnValueType.Value:
                        GameAttributeUtility.AddAddtionalValue(playerEntityId, EnEntityAttributeId.LimitHp, _AddValue);
                        break;
                    case EnValueType.Percentage:
                        GameAttributeUtility.AddAddtionalPercentage(playerEntityId, EnEntityAttributeId.LimitHp, _AddValue);
                        break;
                    default:
                        break;
                }
                var curValue = GameAttributeUtility.GetValue(playerEntityId, EnEntityAttributeId.LimitHp);
                GameAttackMgr.Instance.RestoreHealth(in playerEntityId, in playerEntityId, curValue - lastValue, in worldPos);
                break;
            case EnEntityAttributeId.Hp:
                switch (_AddValueType)
                {
                    case EnValueType.Value:
                        GameAttackMgr.Instance.RestoreHealth(in playerEntityId, in playerEntityId, _AddValue, in worldPos);
                        break;
                    case EnValueType.Percentage:
                        var maxHp = GameAttributeUtility.GetValue(playerEntityId, EnEntityAttributeId.LimitHp);
                        GameAttackMgr.Instance.RestoreHealth(in playerEntityId, in playerEntityId, maxHp * _AddValue, in worldPos);
                        break;
                    default:
                        break;
                }
                break;
            default:
                switch (_AddValueType)
                {
                    case EnValueType.Value:
                        GameAttributeUtility.AddAddtionalValue(playerEntityId, _AttId, _AddValue);
                        break;
                    case EnValueType.Percentage:
                        GameAttributeUtility.AddAddtionalPercentage(playerEntityId, _AttId, _AddValue);
                        break;
                    default:
                        break;
                }
                break;
        }
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
