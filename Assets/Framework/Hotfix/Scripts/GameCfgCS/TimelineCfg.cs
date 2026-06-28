// TimelineCfg
public class TimelineCfg : ICfg
{
	private TimelineCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nCmdID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nTypeId;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Single fDurationTime;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrParams;
	// 指令类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
	public System.Int32 GetID()
	{
		return nCmdID;
	}
}
