using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOperationInfo : IClassPoolInit<ISerializeToIntArrayUserData>, ISerializeToIntArray
{
    public bool CompareResult(int target);
}