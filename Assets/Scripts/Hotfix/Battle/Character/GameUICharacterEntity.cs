using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameUICharacterEntityData : SceneEntityData<GameUICharacterEntity>
{
    private bool _IsUnlock = true;

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _SceneEntity.SetIsUnlock(_IsUnlock);
    }
    public void SetIsUnlock(bool isUnlock)
    {
        _IsUnlock = isUnlock;
        if (IsLoadSuccess)
            _SceneEntity.SetIsUnlock(isUnlock);
    }
}

public abstract class GameUICharacterEntity : SceneEntity, IEntityAnimCom
{
    private Animator _Anim;

    public Animator GetAnimator()
    {
        if (_Anim == null)
            if (!gameObject.TryGetComponent<Animator>(out _Anim))
                _Anim = gameObject.AddComponent<Animator>();
        return _Anim;
    }
    public void SetIsUnlock(bool isUnlock)
    {
        var coms = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var item in coms)
        {
            item.color = isUnlock ? Color.white : Color.black;
        }
    }
}
