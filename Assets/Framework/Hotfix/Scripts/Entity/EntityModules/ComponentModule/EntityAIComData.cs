
using System.Collections.Generic;
using UnityEngine;


public sealed class EntityAIComData : Entity3DComData, IUpdate
{
    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.AI;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnPoolInit(Entity3DComDataUserData userData)
    {
        base.OnPoolInit(userData);
    }

    public void Update()
    {
        
    }
}