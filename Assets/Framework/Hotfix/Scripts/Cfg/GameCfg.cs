using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public abstract class CfgGameBase
{
    
}
public class GameCfgInfo
{
    public Dictionary<int, CfgGameBase> Cfg = new();
}
public sealed partial class GameCfgMgr: Singleton<GameCfgMgr>
{
    private Dictionary<Type, GameCfgInfo> m_TableCfg = new();

    public T GetCfg<T>(int nID)
        where T: CfgGameBase
    {
        var type = typeof(T);
        var cfgInfo = m_TableCfg[type];
        var data = cfgInfo.Cfg[nID];
#if UNITY_EDITOR
        if (data == null)
            throw new Exception($" get cfg error type:{type}, id:{nID}");
#endif
        return data as T;
    }
    private void AddCfg<T>(T cfgInfo)
        where T: GameCfgInfo
    {
        m_TableCfg.Add(typeof(T), cfgInfo);
    }
}



public sealed class CfgPath : CfgGameBase
{
    public int nID;
    public string strValue;
}
public sealed class CfgString : CfgGameBase
{
    public int nID;
    public string StrChina;
}
public partial class GameCfgMgr
{
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        AddCfg(InitStringCfg());
    }
    private GameCfgInfo InitStringCfg()
    {
        return new GameCfgInfo()
        {
            Cfg = new()
            {
                {1, new CfgPath() {
                    nID = 1,
                    strValue = "",
                } },
                {2, new CfgPath() {
                    nID = 1,
                    strValue = "",
                } },
                {3, new CfgPath() {
                    nID = 1,
                    strValue = "",
                } },
                {4, new CfgPath() {
                    nID = 1,
                    strValue = "",
                } },
                {5, new CfgPath() {
                    nID = 1,
                    strValue = "",
                } },
            }
        };

    }
}

