using DG.Tweening;
using TMPro;
using UnityEngine;


public class GameBattleDamageValueUserData : GameEntityUserData
{
    public EnDamageType damageType;
    public EnDamageValueType damageValueType;
    public int damageValue;

    public void Init(EnDamageType damageType, EnDamageValueType damageValueType, int damageValue)
    {
        this.damageType = damageType;
        this.damageValueType = damageValueType;
        this.damageValue = damageValue;
    }
}
public class GameBattleDamageValueData : GameEntityData<GameBattleDamageValue, GameBattleDamageValueUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleDamageValue;

    private EnDamageType _DamageType;
    private EnDamageValueType _DamageValueType;
    private int _DamageValue;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.DamageTxt);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
    public override void Destroy()
    {
        base.Destroy();
        _DamageType = EnDamageType.None;
        _DamageValueType = EnDamageValueType.None;
        _DamageValue = -1;
    }
    protected override void OnPoolInit(GameBattleDamageValueUserData userData)
    {
        _DamageType = userData.damageType;
        _DamageValueType = userData.damageValueType;
        _DamageValue = userData.damageValue;
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _Entity.Setting(_DamageValue, _DamageType, _DamageValueType);
    }
}
public class GameBattleDamageValue : GameEntity
{
    [SerializeField]
    private TextMeshPro _ValueTxt;

    [SerializeField]
    private Color _ColorPhysic;
    [SerializeField]
    private Color _ColorMagic;
    [SerializeField]
    private Color _ColorTrue;


    private readonly float _DefFontSize = 5;


    public override void OnUnload()
    {
        DOTween.Kill(GetHashCode());
        base.OnUnload();
    }

    public void Setting(int value, EnDamageType damageType, EnDamageValueType damageValueType)
    {
        _ValueTxt.SetText($"{value}");

        _ValueTxt.color = damageType switch
        {
            EnDamageType.Physic => _ColorPhysic,
            EnDamageType.Magic => _ColorMagic,
            EnDamageType.True => _ColorTrue,
            _ => Color.black,
        };

        _ValueTxt.fontSize = damageValueType switch
        {
            EnDamageValueType.Critical => _DefFontSize * 1.3f,
            _ => _DefFontSize,
        };

        PlayAnim(damageType, damageValueType);
    }

    private void PlayAnim(EnDamageType damageType, EnDamageValueType damageValueType)
    {
        var from = _ValueTxt.fontSize * 2;
        var to = _ValueTxt.fontSize;
        DOTween.To(() => 0, slider =>
        {
            _ValueTxt.fontSize = Mathf.Lerp(from, to, slider);

        }, 1f, 0.2f)
            .SetId(GetHashCode());
    }
}
