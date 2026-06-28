

public interface IAiExecutionUserData : ISerializeToIntArray<AiCommonUserData>
{

}
public interface IAiExecution<T> : IAiExecution, IClassPoolInit<T>
    where T : IClassPoolUserData
{

}

public interface IAiExecution : IClassPoolInit
{
    public EnAiExecutionType GetAiNodeType();
    public void ExecuteAiNode();
}