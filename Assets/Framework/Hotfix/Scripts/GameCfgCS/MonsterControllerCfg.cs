// MonsterControllerCfg
public class MonsterControllerCfg : ICfg
{
	private MonsterControllerCfg() {}
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nControllerID;
	// keycode-cmdID
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrParams;
	public System.Int32 GetID()
	{
		return nControllerID;
	}
}
