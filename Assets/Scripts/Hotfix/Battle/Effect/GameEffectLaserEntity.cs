using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class GameEffectLaserEntityData : SceneEntityData<GameEffectLaserEntity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_Eff_buff_jiguang;

    private Vector3 _Point1;
    private Vector3 _Point2;
    private float _LaserWidth;

    public override void OnPoolDestroy()
    {
        DOTween.Kill(this.GetHashCode());
        base.OnPoolDestroy();
    }
    protected override void OnPoolInit(SceneEntityUserData userData)
    {
        base.OnPoolInit(userData);

        DOTween.To(() => 0f, slider =>
        {
            var scale = Mathf.Sin(slider * Mathf.PI);
            var curScale = GetLocalScale();
            SetLocalScale(new Vector3(curScale.x, scale, curScale.z));

        }, 1f, 0.3f)
            .SetId(this.GetHashCode())
            .OnComplete(() =>
            {
                SceneEntityMgr.Instance.DestroyEntityData(_EntityID);
            });
    }
    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapUtility.GetCurMapDepth() + (int)EnGameEntityOrderLayer.Effect;
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();

        _SceneEntity.SetPoint2(_Point1, _Point2);
        _SceneEntity.SetLaserWidth(_LaserWidth);
    }

    public void SetPoint2(Vector2 point1, Vector2 point2)
    {
        var posZ = GetWorldPos().z;
        _Point1 = new Vector3(point1.x, point1.y, posZ);
        _Point2 = new Vector3(point2.x, point2.y, posZ);

        if (IsLoadSuccess)
            _SceneEntity.SetPoint2(_Point1, _Point2);
    }
    public void SetLaserWidth(float width)
    {
        _LaserWidth = width;
        if (IsLoadSuccess)
            _SceneEntity.SetLaserWidth(width);
    }
}
public class GameEffectLaserEntity : SceneEntity
{
    [SerializeField]
    private List<LineRenderer> _Lines = new();

    private float _LaserWidth;
    public void SetLaserWidth(float width)
    {
        _LaserWidth = width;
    }
    public void SetPoint2(Vector3 point1, Vector3 point2)
    {
        for (int i = 0; i < _Lines.Count; i++)
        {
            var line = _Lines[i];
            line.SetPositions(new Vector3[] { point1, point2 });
        }
    }

    public override void SetLocalScale()
    {
        base.SetLocalScale();

        foreach (var item in _Lines)
        {
            item.startWidth = _EntityData.LocalScale.y * _LaserWidth;
        }
    }
}
