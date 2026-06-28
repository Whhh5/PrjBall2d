// BuffTypeCfg
public class BuffTypeCfg : ICfg
{
	private BuffTypeCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nTypeID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strEnumNameEditor;
	public System.Int32 GetID()
	{
		return nTypeID;
	}
}
