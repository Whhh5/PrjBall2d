// AssetCfg
public class AssetCfg : ICfg
{
	private AssetCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
	// 枚举名称
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	// 路径
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strPath;
	public System.Int32 GetID()
	{
		return nAssetID;
	}
}
