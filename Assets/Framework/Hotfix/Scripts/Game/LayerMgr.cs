using UnityEngine;


public class LayerMgr : Singleton<LayerMgr>
{
    public int GetLayer(EnGameLayer layer)
    {
        return 1 << (int)layer;
    }
    public int GetLayer(params EnGameLayer[] layer)
    {
        var value = 0;
        for (int i = 0; i < layer.Length; i++)
        {
            value += 1 << (int)layer[i];
        }
        return value;
    }
}
