using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillBehaviour : IClassPoolInit<ISerializeToIntArrayUserData>, ISerializeToIntArray
{
    public EnSkillBehaviourType SkillBehavioueType();
    public void Execute(int entityID);
}
