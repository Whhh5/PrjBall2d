// EffectCfg
public class EffectCfg : ICfg
{
	private EffectCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nEffectID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Single fDelayDestroyTime;
	// 指令类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
	public System.Int32 GetID()
	{
		return nEffectID;
	}
}
