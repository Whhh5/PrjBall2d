using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityPropertyMgr : Singleton<EntityPropertyMgr>
{
    public EntityPropertyInfo CreatePropertyInfo(EnEntityProperty target, int[] targetParams)
    {
        var propertyUserData = ClassPoolMgr.Instance.Pull<EntityPropertyInfoUserData>();
        propertyUserData.target = target;
        propertyUserData.arrParams = targetParams;
        EntityPropertyInfo result = target switch
        {
            EnEntityProperty.Gravity => ClassPoolMgr.Instance.Pull<EntityPropertyGravityInfo>(propertyUserData),
            _ => null,
        };
        ClassPoolMgr.Instance.Push(propertyUserData);
        return result;
    }
}