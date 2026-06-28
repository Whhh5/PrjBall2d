// AICfg
public class AICfg : ICfg
{
	private AICfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAIID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrAIModuleID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrPerception;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrDecision;
	public System.Int32 GetID()
	{
		return nAIID;
	}
}
