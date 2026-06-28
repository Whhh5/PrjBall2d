using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityPropertyGravityInfo : EntityPropertyInfo
{
    public int[] arrParams = null;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        arrParams = null;
    }
    public override void OnPoolInit(EntityPropertyInfoUserData userData)
    {
        base.OnPoolInit(userData);
        arrParams = userData.arrParams;
    }
}