

using System.Collections.Generic;


public enum EnAiPerceptionTestType
{
    None = 0,
    Distance,
    Layer,
}
public interface IAiPerceptionTest : IClassPoolInit, ISerializeToIntArray<AiCommonUserData>
{
    public EnAiPerceptionTestType GetAiPerceptionTestType();
    public bool IsPassable(in int testEntityId);
}

public interface IAiPerceptionTestResult : IClassPoolDestroy
{

}
public interface IAiPerception<TUserData> : IAiPerception, ISerializeToIntArray<TUserData>
    where TUserData : class, ISerializeToIntArrayUserData
{

}
public interface IAiPerception : IClassPoolInit, ISerializeToIntArray
{
    public EnAiPerceptionId GetPerceptionId();
    public bool UpdateAiPerception();
    public ref readonly List<int> GetResultEntityList();
}