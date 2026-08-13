using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleEntityAttributeWindowUserData : UIWindowUserData
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
public class UIBattleEntityAttributeWindowData : UIWindowData<UIBattleEntityAttributeWindow, UIBattleEntityAttributeWindowUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Tips;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleEntityAttributeWindow;

    private int _TargetEntityId = -1;
    private string _Title;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _TargetEntityId = -1;
        _Title = null;
    }
    protected override void OnPoolInit(UIBattleEntityAttributeWindowUserData userData)
    {
        _Title = userData.title;
        _TargetEntityId = userData.targetEntityId;
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _UIWindow.Setting(_Title, _TargetEntityId);
    }
}

public class UIBattleEntityAttributeWindow : UIWindow
{
    [SerializeField]
    private TextMeshProUGUI _NameTxt = null;
    [SerializeField]
    private RectTransform _AttributeRootRect = null;

    private int _TargetEntityId;
    private readonly Dictionary<EnEntityAttributeId, UIBattleEntityAttributeWindow_AttributeItem> _AttributeItems = new();

    public override void OnUnload()
    {
        if(_TargetEntityId > 0)
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
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
    }

    public async void Setting(string title, int entityId)
    {
        _TargetEntityId = entityId;
        _NameTxt.SetText(title);
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        for (var i = EnEntityAttributeId.None + 1; i < EnEntityAttributeId.EnumCount; i++)
        {
            var attValue = attCom.GetValue(i);
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleEntityAttributeWindow_AttributeItem>(EnLoadTarget.Pre_UIBattleEntityAttributeWindow_AttributeItem, _AttributeRootRect, GetWindowToken())
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
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            if (UIMgr.Instance.IsValid(_WindowId))
            {
                UIMgr.Instance.HideWindow(_WindowId);
            }
        }
    }
}
