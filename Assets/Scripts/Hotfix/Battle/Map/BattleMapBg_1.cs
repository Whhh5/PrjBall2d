using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class BattleMapBg_1Data : GameEntityData<BattleMapBg_1>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleMapBg_1;

    public override void SetPosition(in Vector3 worldPos)
    {
        var depth = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Background);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, depth));
    }

    private float _HpSliderValue;
    private float _EnergySliderValue;
    private int _EnergyLevel;
    private int _EnergyValue;

    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameBattlePlayerCreateEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, 0, 0, OnEvent_PlayerCreate);
        EventMgr.Instance.Unregister<GameBattlePlayerDestroyEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_DESTROY, 0, 0, OnEvent_PlayerDestroy);
        EventMgr.Instance.Unregister<GameBattleEnergyChangeEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_CHANGE, 0, 0, OnEvent_EnergyChange);
        EventMgr.Instance.Unregister<GameBattleEnergyUplevelEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, OnEvent_EnergyUplevel);
        EventMgr.Instance.Unregister<GameBattleMapReenterEventData>(EnABBEvent.EVENT_BATTLE_MAP_REENTER, 0, 0, OnEvent_MapReenter);

        base.OnPoolDestroy();
        _HpSliderValue
            = _EnergySliderValue
            = _EnergyLevel
            = _EnergyValue
            = 0;
    }
    public override void OnPoolInit(GameEntityUserData userData)
    {
        base.OnPoolInit(userData);

        EventMgr.Instance.Register<GameBattlePlayerCreateEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, 0, 0, OnEvent_PlayerCreate);
        EventMgr.Instance.Register<GameBattlePlayerDestroyEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_DESTROY, 0, 0, OnEvent_PlayerDestroy);
        EventMgr.Instance.Register<GameBattleEnergyChangeEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_CHANGE, 0, 0, OnEvent_EnergyChange);
        EventMgr.Instance.Register<GameBattleEnergyUplevelEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, OnEvent_EnergyUplevel);
        EventMgr.Instance.Register<GameBattleEnergyUplevelEventData>(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, OnEvent_EnergyUplevel);
        EventMgr.Instance.Register<GameBattleMapReenterEventData>(EnABBEvent.EVENT_BATTLE_MAP_REENTER, 0, 0, OnEvent_MapReenter);

    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _Entity.SetHpSliderValue(_HpSliderValue);
        _Entity.SetEnergySliderValue(_EnergySliderValue);
        _Entity.SetEnergyLevel(_EnergyLevel);
        _Entity.SetEnergyValue(_EnergyValue);
    }
    private void OnEvent_MapReenter(in int _, in int __, in GameBattleMapReenterEventData data)
    {
        _HpSliderValue
            = _EnergySliderValue
            = _EnergyLevel
            = _EnergyValue
            = 0;
        _Entity.SetHpSliderValue(_HpSliderValue);
        _Entity.SetEnergySliderValue(_EnergySliderValue);
        _Entity.SetEnergyLevel(_EnergyLevel);
        _Entity.SetEnergyValue(_EnergyValue);
    }
    public void SetHpSliderValue(float sliderValue)
    {
        _HpSliderValue = sliderValue;
        if (IsLoadSuccess)
            _Entity.SetHpSliderValue(sliderValue);
    }
    
    public void SetEnergySliderValue(float sliderValue)
    {
        _EnergySliderValue = sliderValue;
        if (IsLoadSuccess)
            _Entity.SetEnergySliderValue(sliderValue);
    }
    public void SetEnergyLevel(int level)
    {
        _EnergyLevel = level;
        if (IsLoadSuccess)
            _Entity.SetEnergyLevel(level);
    }
    public void SetEnergyValue(int level)
    {
        _EnergyValue = level;
        if (IsLoadSuccess)
            _Entity.SetEnergyValue(level);
    }

    private void OnEvent_PlayerCreate(in int _, in int __, in GameBattlePlayerCreateEventData data)
    {
        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(playerEntityId, EnSceneEntityComType.Attribute);
        var curHp = attCom.GetValue(EnEntityAttributeId.Hp);
        var maxHp = attCom.GetValue(EnEntityAttributeId.LimitHp);
        SetHpSliderValue(curHp / maxHp);

        EventMgr.Instance.Register<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, data.entityId, (int)EnEntityAttributeId.Hp, OnEvent_EntityHpAttributeChange);
        EventMgr.Instance.Register<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, data.entityId, (int)EnEntityAttributeId.LimitHp, OnEvent_EntityHpAttributeChange);

    }
    private void OnEvent_PlayerDestroy(in int _, in int __, in GameBattlePlayerDestroyEventData data)
    {
        EventMgr.Instance.Unregister<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, data.entityId, (int)EnEntityAttributeId.Hp, OnEvent_EntityHpAttributeChange);
        EventMgr.Instance.Unregister<GameBattleEntityAttributeChangeEventData>(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, data.entityId, (int)EnEntityAttributeId.LimitHp, OnEvent_EntityHpAttributeChange);
    }

    private void OnEvent_EntityHpAttributeChange(in int nEntityId, in int nAttribtueId, in GameBattleEntityAttributeChangeEventData data)
    {
        var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(playerEntityId, EnSceneEntityComType.Attribute);
        var curHp = attCom.GetValue(EnEntityAttributeId.Hp);
        var maxHp = attCom.GetValue(EnEntityAttributeId.LimitHp);
        SetHpSliderValue(curHp / maxHp);
    }

    private void OnEvent_EnergyUplevel(in int _, in int __, in GameBattleEnergyUplevelEventData data)
    {
        SetEnergyLevel(data.currentLevel);
    }
    private void OnEvent_EnergyChange(in int _, in int __, in GameBattleEnergyChangeEventData data)
    {
        SetEnergyValue(data.currentValue);
        SetEnergySliderValue(data.currentValue / (float)data.maxValue);
    }

}

public class BattleMapBg_1 : GameEntity
{

    [SerializeField]
    private SpriteRenderer _HpSlider;
    [SerializeField]
    private float _HpMaxSliderValue;
    [SerializeField]
    private SpriteRenderer _EnergySlider;
    [SerializeField]
    private float _EnergyMaxSliderValue;
    [SerializeField]
    private TextMeshPro _EnergyLevelTxt;
    [SerializeField]
    private TextMeshPro _EnergyValue;

    private int _Level;
    public void SetHpSliderValue(float value)
    {
        _HpSlider.size = new Vector2(_HpMaxSliderValue * value, _HpSlider.size.y);
    }
    public void SetEnergySliderValue(float value)
    {
        _EnergySlider.size = new Vector2(_EnergyMaxSliderValue * value, _EnergySlider.size.y);
    }
    public void SetEnergyLevel(int value)
    {
        _Level = value;
        _EnergyLevelTxt.text = $"{value}";
    }
    public void SetEnergyValue(int value)
    {
        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        if (chapterCfg.nMaxEnergyStage > 0 && _Level >= chapterCfg.nMaxEnergyStage)
        {
            _EnergyValue.text = $"MAX";
        }
        else
        {
            _EnergyValue.text = $"{value}";
        }
    }
}
