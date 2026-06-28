// MonsterCfg
public class MonsterCfg : ICfg
{
	private MonsterCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nMonsterID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strName;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nEntityTypeId;
	// 动作组id-skillcfg
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrSkillGroup;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetCfgID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrLayer;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrEnemyLayer;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrFriendLayer;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nKeyCodeControllerID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nIdleCmdID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nWalkCmdId;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nRunCmdId;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nDieCmdID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAIID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nIconAssetID;
	public System.Int32 GetID()
	{
		return nMonsterID;
	}
}
