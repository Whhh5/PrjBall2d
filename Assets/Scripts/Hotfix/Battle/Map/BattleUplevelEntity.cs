using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using TMPro;
using UnityEngine;

public class BattleUplevelEntityData : SceneEntityData<BattleUplevelEntity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleUplevelEntity;
    private int _Count;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Grid);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();

        _Count = -1;
    }
    protected override void OnPoolInit(SceneEntityUserData userData)
    {
        base.OnPoolInit(userData);

        _Count = GameBattleWindowMgr.Instance.GetUplevelCount();


    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _SceneEntity.SetCount(_Count);
    }

    public void SetCount(int count)
    {
        _Count = count;
        if (IsLoadSuccess)
            _SceneEntity.SetCount(count);
    }
}
public class BattleUplevelEntity : SceneEntity
{
    [SerializeField]
    private GameObject _EffectGo;
    [SerializeField]
    private ParticleSystem _CountEffect;


    public void SetCount(int count)
    {

        if (count <= 0)
        {
            _EffectGo.SetActive(false);
            _CountEffect.gameObject.SetActive(false);
        }
        else
        {
            if (!_EffectGo.activeSelf)
            {
                _CountEffect.gameObject.SetActive(true);
                _EffectGo.SetActive(true);
            }

            var mainModule = _CountEffect.main;
            mainModule.maxParticles = count;

            var emission = _CountEffect.emission;
            emission.rateOverTime = count;
        }
    }

}
