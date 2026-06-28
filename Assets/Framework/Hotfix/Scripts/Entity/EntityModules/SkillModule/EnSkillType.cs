public enum EnSkillType
{
    None,
    [EditorFieldName("连击")]
    Link,
    [EditorFieldName("随机")]
    Random,
    [EditorFieldName("单一")]
    Singleton,
    [EditorFieldName("循环")]
    Loop,
    [EditorFieldName("条件")]
    Select,
    EnumCount,
}