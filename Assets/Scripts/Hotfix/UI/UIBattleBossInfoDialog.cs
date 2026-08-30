using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleBossInfoDialogUserData : UIWindowUserData
{
    public int bossEntityId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        bossEntityId = -1;
    }
}
public class UIBattleBossInfoDialogData : UIWindowData<UIBattleBossInfoDialog, UIBattleBossInfoDialogUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Dialog;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleBossInfoDialog;

    private int _BossEntityId = -1;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _BossEntityId = -1;
    }
    protected override void OnPoolInit(UIBattleBossInfoDialogUserData userData)
    {
        _BossEntityId = userData.bossEntityId;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _UIWindow.SetBossEntityId(_BossEntityId);
    }
    public override void OnGODestroy()
    {
        base.OnGODestroy();
        _BossEntityId = -1;
    }
}
public class UIBattleBossInfoDialog : UIWindow
{
    [SerializeField, AutoReference]
    private Slider _HpValueSlider = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _HpValueTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _NameTxt;


    private int _BossEntityId;
    private readonly EnDotweenId _DotweenId = EnDotweenId.UIBattleBossInfoDialog;

    public override void OnUnload()
    {
        if (_BossEntityId > 0)
            EventMgr.Instance.Unregister<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _BossEntityId, (int)EnEntityAttributeId.Hp, OnEvent_BossHpChange);
        DOTween.Kill(_DotweenId);
        base.OnUnload();
        _BossEntityId = default;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }

    public void SetBossEntityId(int bossEntityId)
    {
        _BossEntityId = bossEntityId;
        if (_BossEntityId > 0)
        {
            EventMgr.Instance.Register<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, _BossEntityId, (int)EnEntityAttributeId.Hp, OnEvent_BossHpChange);
            var gameMonsterEntityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(_BossEntityId);

            var monsterId = gameMonsterEntityData.GetMonsterCfgId();
            var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
            _NameTxt.SetLanguageText(monsterCfg.nNameLangId);

            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(_BossEntityId, EnSceneEntityComType.Attribute);
            var curHp = attCom.GetValue(EnEntityAttributeId.Hp);
            var maxHp = attCom.GetValue(EnEntityAttributeId.LimitHp);
            _HpValueTxt.SetText(LanguageUtility.GetLanguage_1009(Mathf.CeilToInt(curHp), Mathf.CeilToInt(maxHp)));

            _HpValueSlider.value = Mathf.Clamp01(curHp / maxHp);
        }
    }

    private void OnEvent_BossHpChange(in int entityId, in int enAttributeId, in GameBattleEntityAttributeChangeEventData data)
    {
        DOTween.Kill(_DotweenId);
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(_BossEntityId, EnSceneEntityComType.Attribute);
        var curHp = attCom.GetValue(EnEntityAttributeId.Hp);
        var maxHp = attCom.GetValue(EnEntityAttributeId.LimitHp);
        _HpValueTxt.SetText(LanguageUtility.GetLanguage_1009(Mathf.CeilToInt(curHp), Mathf.CeilToInt(maxHp)));

        _HpValueSlider.value = Mathf.Clamp01(curHp / maxHp);
    }
}
