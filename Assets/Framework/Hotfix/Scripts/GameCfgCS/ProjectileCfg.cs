// ProjectileCfg
public class ProjectileCfg : ICfg
{
	private ProjectileCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nProjectileId;
	// 数据类类型id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nDataTypeId;
	// 指令类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
	public System.Int32 GetID()
	{
		return nProjectileId;
	}
}
