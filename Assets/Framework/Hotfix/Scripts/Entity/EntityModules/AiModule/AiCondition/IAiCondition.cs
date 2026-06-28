
public enum EnAiConditionType
{
    None = 0,
    Distance,
    Rotation,
}
public interface IAiCondition : ISerializeToIntArray<AiCommonUserData>
{
    public EnAiConditionType GetConditionType();
    public bool IsPass(in int inputId);
}