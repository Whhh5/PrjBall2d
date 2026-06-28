using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed class EntityBuffComData : Entity3DComData
{

    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.Buff;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
    public override void OnPoolInit(Entity3DComDataUserData userData)
    {
        base.OnPoolInit(userData);
    }

    public void AddBuff(int addKey)
    {
        
    }
    public void RemoveBuff(int addKey)
    {

    }
}
