// CharacterCfg
public class CharacterCfg : ICfg
{
	private CharacterCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nCharacterID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nMonsterID;
	public System.Int32 GetID()
	{
		return nCharacterID;
	}
}
