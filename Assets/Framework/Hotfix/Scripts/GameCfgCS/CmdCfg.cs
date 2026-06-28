// CmdCfg
public class CmdCfg : ICfg
{
	private CmdCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nCmdID;
	// 备注
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	// 指令类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nCmdType;
	// 指令参数
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrParams;
	public System.Int32 GetID()
	{
		return nCmdID;
	}
}
