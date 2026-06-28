using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ABBEventMgr : Singleton<ABBEventMgr>
{
    private Dictionary<EnABBEvent, ABBEventData> m_ActionList = new();


    public void Register(EnABBEvent ev, int sourceID, int typeID, IABBEventExecute action)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
        {
            var data = ClassPoolMgr.Instance.Pull<ABBEventDataUserData>();
            data.abbevent = ev;
            evData = ClassPoolMgr.Instance.Pull<ABBEventData>(data);
            ClassPoolMgr.Instance.Push(data);
            m_ActionList.Add(ev, evData);
        }
        evData.AddEvent(sourceID, typeID, action);
    }
    public void Unregister(EnABBEvent ev, int sourceID, int typeID, IABBEventExecute action)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
            return;
        evData.RemoveEvent(sourceID, typeID, action);
        if (evData.Count == 0)
        {
            m_ActionList.Remove(ev);
            ClassPoolMgr.Instance.Push(evData);
        }
    }
    public void FireExecute(EnABBEvent ev, int sourceID, int typeID, IClassPool userData)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
            return;
        evData.FireEvent(sourceID, typeID, userData);
    }
    public void FireExecute(EnABBEvent ev, int sourceID, IClassPool userData)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
            return;
        evData.FireEvent(sourceID, 0, userData);
    }
    public void FireExecute(EnABBEvent ev, int sourceID)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
            return;
        evData.FireEvent(sourceID, 0, null);
    }
    public void FireExecute(EnABBEvent ev)
    {
        FireExecute(ev, 0, 0, null);
    }
    public void FireExecute(EnABBEvent ev, IClassPool userData)
    {
        FireExecute(ev, 0, 0, userData);
    }

    public void Register(EnABBEvent ev, IABBEventExecute action)
    {
        Register(ev, 0, 0, action);
    }
    public void Unregister(EnABBEvent ev, IABBEventExecute action)
    {
        Unregister(ev, 0, 0, action);
    }
}

