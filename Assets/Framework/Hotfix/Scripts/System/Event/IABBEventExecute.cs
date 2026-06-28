using System;
public interface IABBEventExecute
{
    public void EventExecute(EnABBEvent enEvent, int sourceID, int type, IClassPool userData);
}

public interface IABBEventExecute<T>: IABBEventExecute
    where T: class, IClassPool
{
    void IABBEventExecute.EventExecute(EnABBEvent enEvent, int sourceID, int type, IClassPool userData)
    {
        EventExecute(enEvent, sourceID, type, userData as T);
    }
    public void EventExecute(EnABBEvent enEvent, int sourceID, int type, T userData);
}
