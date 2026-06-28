using System;



public interface ISkillTypeData<T> : IClassPoolInit<T>
    where T: class, IClassPoolUserData
{

}