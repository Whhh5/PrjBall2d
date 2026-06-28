// CmdLevelCfg
public class CmdLevelCfg : ICfg
{
	private CmdLevelCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nLevelID;
	public System.Int32 GetID()
	{
		return nLevelID;
	}
}
