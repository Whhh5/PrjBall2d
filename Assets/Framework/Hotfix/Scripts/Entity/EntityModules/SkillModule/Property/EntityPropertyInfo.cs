using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityPropertyInfoUserData : IClassPoolUserData
{
    public EnEntityProperty target = EnEntityProperty.None;
    public int[] arrParams = null;
    public void OnPoolDestroy()
    {
        target = EnEntityProperty.None;
        arrParams = null;
    }
}
public abstract class EntityPropertyInfo : IClassPool<EntityPropertyInfoUserData>
{
    public EnEntityProperty target = EnEntityProperty.None;

    public virtual void OnPoolDestroy()
    {
        target = EnEntityProperty.None;
    }

    public virtual void OnPoolInit(EntityPropertyInfoUserData userData)
    {
        target = userData.target;
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }

    public void OnPoolEnable()
    {
    }
}