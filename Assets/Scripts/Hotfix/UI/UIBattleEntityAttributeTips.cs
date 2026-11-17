using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleEntityAttributeTipsUserData : UITipsUserData
{
    public string title;
    public int targetEntityId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        targetEntityId = -1;
        title = null;
    }
}
public class UIBattleEntityAttributeTipsData : UITipsData<UIBattleEntityAttributeTips, UIBattleEntityAttributeTipsUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleEntityAttributeTips;

    private int _TargetEntityId = -1;
    private string _Title;

    public override void OnPoolDestroy()
    {
        TimeMgr.Instance.SetPause(false, false);
        base.OnPoolDestroy();
        _TargetEntityId = -1;
        _Title = null;
    }
    protected override void OnPoolInit(UIBattleEntityAttributeTipsUserData userData)
    {
        _Title = userData.title;
        _TargetEntityId = userData.targetEntityId;
        TimeMgr.Instance.SetPause(true, true);
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _UITips.Setting(_Title, _TargetEntityId);
    }
}

public class UIBattleEntityAttributeTips : UITips
{
    [SerializeField, AutoReference]
    private TextMeshProUGUI _NameTxt = null;
    [SerializeField, AutoReference]
    private RectTransform _AttributeRootRect = null;
    [SerializeField, AutoReference]
    private Image _IconImg = null;

    private int _TargetEntityId;
    private readonly Dictionary<EnEntityAttributeId, UIBattleEntityAttributeTips_AttributeItem> _AttributeItems = new();

    public override void OnUnload()
    {
        _IconImg.UnloadSprite();
        if (_TargetEntityId > 0)
            EventMgr.Instance.Unregister<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _TargetEntityId, 0, OnEvent_AttributeChange);

        foreach (var (_, attItem) in _AttributeItems)
        {
            UnregisterUIDissolve(attItem);
            UIMgr.Instance.UnloadWindowItem(attItem);
        }
        base.OnUnload();
        _AttributeItems.Clear();
        _TargetEntityId = -1;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }

    public async void Setting(string title, int entityId)
    {
        _TargetEntityId = entityId;
        _NameTxt.SetText(title);
        var infoData = EntityMgr.Instance.GetEntityData<IGameEntityInfo>(entityId);
        var iconAssetId = infoData.GetIconAssetId();
        _IconImg.LoadSprirte(iconAssetId);

        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        for (var i = EnEntityAttributeId.None + 1; i < EnEntityAttributeId.EnumCount; i++)
        {
            var attValue = attCom.GetValue(i);
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleEntityAttributeTips_AttributeItem>(EnLoadTarget.Pre_UIBattleEntityAttributeTips_AttributeItem, _AttributeRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            item.Setting(i, attValue);
            RegisterUIDissolve(item);
            _AttributeItems.Add(i, item);
        }

        EventMgr.Instance.Register<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _TargetEntityId, 0, OnEvent_AttributeChange);
    }

    private void OnEvent_AttributeChange(in int nEntityId, in int enAttributeId, in GameBattleEntityAttributeChangeEventData data)
    {
        var attId = (EnEntityAttributeId)enAttributeId;
        if (!_AttributeItems.TryGetValue(attId, out var iten))
            return;
        iten.SetValue(data.currentValue);
    }
}
