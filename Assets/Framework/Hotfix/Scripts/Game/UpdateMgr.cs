using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUpdate
{
    public void Update();
}
public class UpdateMgr : Singleton<UpdateMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private HashSet<IUpdate> m_UpdateHash = new();


    private HashSet<IUpdate> m_AddHash = new();
    private HashSet<IUpdate> m_RemoveHash = new();

    public override void Update()
    {
        base.Update();
        if (m_RemoveHash.Count > 0)
        {
            foreach (var item in m_RemoveHash)
                m_UpdateHash.Remove(item);
            m_RemoveHash.Clear();
        }

        if (m_AddHash.Count > 0)
        {
            foreach (var item in m_AddHash)
                m_UpdateHash.Add(item);
            m_AddHash.Clear();
        }

        foreach (var item in m_UpdateHash)
        {
            if (m_RemoveHash.Contains(item))
                continue;
            item.Update();
        }
    }

    public void Registener(IUpdate update)
    {
        if (m_RemoveHash.Contains(update))
        {
            m_RemoveHash.Remove(update);
            return;
        }
        m_AddHash.Add(update);
    }
    public void Unregistener(IUpdate update)
    {
        if (m_AddHash.Contains(update))
        {
            m_AddHash.Remove(update);
            return;
        }
        m_RemoveHash.Add(update);
    }
}
