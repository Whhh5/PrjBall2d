using System;


public class SkillAttribute : Attribute
{
    public EnPhysicsType type;

    public SkillAttribute(EnPhysicsType type)
    {
        this.type = type;
    }
}
public class EnumNameAttribute : Attribute
{
    public string name;

    public EnumNameAttribute(string name)
    {
        this.name = name;
    }
}


public class EditorFieldNameAttribute : Attribute
{
    public string fieldName;
    public EditorFieldNameAttribute(string name)
    {
        fieldName = name;
    }
}
public class HideEditorAttribute : Attribute
{

}


