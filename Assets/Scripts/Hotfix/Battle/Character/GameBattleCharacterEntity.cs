using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameEntityHpChange
{
    public void SetHpSliderValue(float sliderValue);
}
public interface IGameEntityInfo
{
    public int GetIconAssetId();
}
public class GameBattleCharacterEntityUserData : SceneEntityUserData
{
    public int characterId;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        characterId = -1;
    }
}
public abstract class GameBattleCharacterEntityData : SceneEntityData<GameBattleCharacterEntity, GameBattleCharacterEntityUserData>, IGameEntityHpChange, IEntityGridPointInfo, IGameEntityInfo
{
    private int _CharacterId;
    private float _SliderValue;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _SliderValue = 0;
        _CharacterId = -1;
    }
    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Monster);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
    protected override void OnPoolInit(GameBattleCharacterEntityUserData userData)
    {
        _CharacterId = userData.characterId;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _SceneEntity.SetHpSliderValue(_SliderValue);
    }
    public void SetHpSliderValue(float sliderValue)
    {
        _SliderValue = sliderValue;
        if (IsLoadSuccess)
            _SceneEntity.SetHpSliderValue(sliderValue);
    }

    public Vector2 GetCenterPointWorldPos()
    {
        return WorldPos;
    }
    
    public int GetIconAssetId()
    {
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(_CharacterId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterId);
        return monsterCfg.nIconAssetID;
    }
    public ref readonly GameBattleGridIndex GetMainGridIndex()
    {
        throw new System.NotImplementedException();
    }

    public ref readonly int[] GetGridIndexs()
    {
        throw new System.NotImplementedException();
    }

    public ref readonly int[] GetGridValidEdges()
    {
        throw new System.NotImplementedException();
    }

    public Vector2 GetMainGridPos()
    {
        throw new System.NotImplementedException();
    }
}
public abstract class GameBattleCharacterEntity : SceneEntity, IEntityAnimCom
{
    [SerializeField]
    private SpriteRenderer _HpSlider;

    [SerializeField]
    private float _HpMaxSliderValue;


    private Animator _Anim;
    public Animator GetAnimator()
    {
        if (_Anim == null)
            if (!gameObject.TryGetComponent<Animator>(out _Anim))
                _Anim = gameObject.AddComponent<Animator>();
        return _Anim;
    }

    public void SetHpSliderValue(float value)
    {
        _HpSlider.size = new Vector2(_HpMaxSliderValue * value, _HpSlider.size.y);
    }
}