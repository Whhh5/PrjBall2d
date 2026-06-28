// AIModuleCfg
public class AIModuleCfg : ICfg
{
	private AIModuleCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nModuleID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nLevel;
	public System.Int32 GetID()
	{
		return nModuleID;
	}
}
