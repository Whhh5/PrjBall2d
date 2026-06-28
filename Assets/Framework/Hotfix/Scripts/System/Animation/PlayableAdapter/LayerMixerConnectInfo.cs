

public enum EnAnimLayerStatus
{
    None,
    Entering,
    Playing,
    Exiting,
    Nothing,
}
public class LayerMixerConnectInfo : IClassPool<PoolNaNUserData>
{
    public int port;
    public EnAnimLayer layer;
    public int skillId;

    public void OnPoolDestroy()
    {
        port = -1;
        layer = EnAnimLayer.None;
        skillId = -1;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(PoolNaNUserData userData)
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}