using Cysharp.Threading.Tasks;
using UnityEngine;

public class MapMgr : Singleton<MapMgr>
{
    private int _CurMapId = -1;

    public override void Destroy()
    {
        base.Destroy();
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
    }


    public bool EnterMap(int mapId)
    {
        if (_CurMapId > 0)
            return false;
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        if (mapCfg == null)
            return false;
        _CurMapId = mapId;
        CreateMap(mapId);
        return true;
    }
    public void ExitMap()
    {
        ClearMap();
        _CurMapId = -1;
    }
    private async void CreateMap(int mapId)
    {
    }

    private void ClearMap()
    {

    }

    public Vector3 GetCurMapStartPosition()
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(_CurMapId);
        var pos = new Vector3(mapCfg.fMapLbPos[0], mapCfg.fMapLbPos[1], mapCfg.fMapLbPos[2]);
        return pos;
    }
    public Vector3 GetCurMapPlayerStartPosition()
    {
        var lbPos = GetCurMapStartPosition();
        var mapCfg = GameSchedule.Instance.GetMapCfg0(_CurMapId);
        var localPos = new Vector3(mapCfg.fPlayerLocalPos[0], mapCfg.fPlayerLocalPos[1], mapCfg.fPlayerLocalPos[2]);
        return lbPos + localPos;

    }
}
