using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : Singleton<MapMgr>
{
    public void CreateMap(int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
    }
    public void ClearMap(int maoId)
    {

    }

}
