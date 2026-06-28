using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class LayerMixerInfo : IClassPool<LayerMixerInfoUserData>
{
    private EnAnimLayer _Layer = EnAnimLayer.None;
    private ScriptPlayable<BridgePlayableAdapter> _LayerAdapter;
    private List<int> _UnuseIndex = new(GlobalConfig.Int2);
    private int _InputPortCount = 0;
    public EnAnimLayerStatus _Status = EnAnimLayerStatus.None;
    private Dictionary<int, IPlayableAdapter> _Port2Adapter = new();

    public void OnPoolDestroy()
    {
        _Port2Adapter.Clear();
        _Status = EnAnimLayerStatus.None;
        _LayerAdapter = default;
        _InputPortCount = 0;
        _Layer = EnAnimLayer.None;
        _UnuseIndex.Clear();
    }
    public void PoolConstructor()
    {
    }

    public void OnPoolInit(LayerMixerInfoUserData userData)
    {
        _LayerAdapter = userData.layerAdapter;
        _Layer = userData.layer;
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public int GetConnectCount()
    {
        var count = _InputPortCount - _UnuseIndex.Count;
        return count;
    }
    private int GetInputPort()
    {
        if (_UnuseIndex.Count == 0)
        {
            var index = _InputPortCount++;
            _LayerAdapter.SetInputCount(_InputPortCount);
            return index;
        }
        var port = _UnuseIndex[^1];
        _UnuseIndex.RemoveAt(_UnuseIndex.Count - 1);
        return port;
    }
    private void RecycleInputPort(int index)
    {
        _UnuseIndex.Add(index);
    }
    public IPlayableAdapter GetAdapter(int portID)
    {
        if (!_Port2Adapter.TryGetValue(portID, out var adapter))
            return null;
        return adapter;
    }
    public void Connect(int portID, IPlayableAdapter adapter, int weight = GlobalConfig.Int1)
    {
        var playable = adapter.GetPlayable();
        _LayerAdapter.ConnectInput(portID, playable, GlobalConfig.Int0, weight);
        _Port2Adapter.Add(portID, adapter);
    }
    public int Connect(IPlayableAdapter adapter, int weight = GlobalConfig.Int1)
    {
        var portID = GetInputPort();
        Connect(portID, adapter, weight);
        return portID;
    }
    public void Disconnect(int inputPortID)
    {
        var adapter = DisconnectNoDestroy(inputPortID);
        IPlayableAdapter.Destroy(adapter);
    }

    public IPlayableAdapter DisconnectNoDestroy(int inputPortID)
    {
        if (!_Port2Adapter.TryGetValue(inputPortID, out var adapter))
            return null;
        _Port2Adapter.Remove(inputPortID);
        _LayerAdapter.DisconnectInput(inputPortID);
        RecycleInputPort(inputPortID);
        return adapter;
    }
    public void DisconnectAll()
    {
        var idList = new List<int>(_Port2Adapter.Count);
        foreach (var item in _Port2Adapter)
            idList.Add(item.Key);
        foreach (var item in idList)
            Disconnect(item);
    }
    public bool IsStatus(EnAnimLayerStatus target)
    {
        return _Status == target;
    }
    public void SetStatus(EnAnimLayerStatus status)
    {
        _Status = status;
    }
    public void SetSpeed(int portID, float speed)
    {
        if (!_Port2Adapter.TryGetValue(portID, out var adapter))
            return;
        adapter.SetSpeed(speed);
    }
    public bool ContainsPortID(int portID)
    {
        var result = _Port2Adapter.ContainsKey(portID);
        return result;
    }

}