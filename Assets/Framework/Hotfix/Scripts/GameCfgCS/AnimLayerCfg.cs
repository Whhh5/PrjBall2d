// AnimLayerCfg
public class AnimLayerCfg : ICfg
{
	private AnimLayerCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nLayer;
	// 参数
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
	public System.Int32 GetID()
	{
		return nLayer;
	}
}
