// BuffCfg
public class BuffCfg : ICfg
{
	private BuffCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nBuffID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nBuffType;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strEnumNameEditor;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nIconAssetID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDesc;
	public System.Int32 GetID()
	{
		return nBuffID;
	}
}
