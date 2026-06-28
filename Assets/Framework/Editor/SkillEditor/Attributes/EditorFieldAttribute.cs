using System;

public class EditorFieldAttribute : Attribute
{
    public string fieldName;

    public EditorFieldAttribute(string fieldName)
    {
        this.fieldName = fieldName;
    }
}