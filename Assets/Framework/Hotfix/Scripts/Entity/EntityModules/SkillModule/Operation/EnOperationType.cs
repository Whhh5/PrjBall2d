public enum EnOperationType
{
    None,
    [EditorFieldName("算数")]
    Compare,
    EnumCount,
}
public enum EnOperationCompareType
{
    None,
    [EditorFieldName("<")]
    Less,
    [EditorFieldName("==")]
    Equal,
    [EditorFieldName(">")]
    Greater,
    EnumCount,
}