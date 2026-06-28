// MapCfg
public class MapCfg : ICfg
{
	private MapCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nMapId;
	// 指令类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
	public System.Int32 GetID()
	{
		return nMapId;
	}
}
