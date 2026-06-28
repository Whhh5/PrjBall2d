using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ABBInputMgr : Singleton<ABBInputMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private InputKeyCodeEventInfo m_OnKeyDownList = new();
    private InputKeyCodeEventInfo m_OnKeyUpList = new();
    private InputKeyCodeEventInfo m_OnKeyList = new();

    public override void Update()
    {
        base.Update();

        m_OnKeyDownList.Apply();
        m_OnKeyUpList.Apply();
        m_OnKeyList.Apply();


        foreach (var item in m_OnKeyDownList._KeyList)
            if (Input.GetKeyDown(item.Key))
                foreach (var action in item.Value)
                    action.Invoke();
        foreach (var item in m_OnKeyList._KeyList)
            if (Input.GetKey(item.Key))
                foreach (var action in item.Value)
                    action.Invoke();
        foreach (var item in m_OnKeyUpList._KeyList)
            if (Input.GetKeyUp(item.Key))
                foreach (var action in item.Value)
                    action.Invoke();
    }

    private void Listaner(ref InputKeyCodeEventInfo eventInfo, KeyCode keyCode, UnityAction action)
    {
        if (eventInfo._RemoveList.TryGetValue(keyCode, out var removeAction))
        {
            if (removeAction.Contains(action))
            {
                removeAction.Remove(action);
                return;
            }
        }
        if (!eventInfo._AddList.TryGetValue(keyCode, out var actionList))
        {
            actionList = new();
            eventInfo._AddList.Add(keyCode, actionList);
        }
        actionList.Add(action);
    }
    private void Unlistaner(ref InputKeyCodeEventInfo eventInfo, KeyCode keyCode, UnityAction action)
    {
        if (eventInfo._AddList.TryGetValue(keyCode, out var addAction))
        {
            if (addAction.Contains(action))
            {
                addAction.Remove(action);
                return;
            }
        }
        if (!eventInfo._RemoveList.TryGetValue(keyCode, out var actionList))
        {
            actionList = new();
            eventInfo._RemoveList.Add(keyCode, actionList);
        }
        actionList.Add(action);
    }
    public bool ContainsAnyKeys(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            ref var keyCode = ref keyCodes[i];
            if (Input.GetKey(keyCode))
                return true;
        }
        return false;
    }
    public void AddListanerDown(KeyCode keyCode, UnityAction action)
    {
        Listaner(ref m_OnKeyDownList, keyCode, action);
    }
    public void RemoveListanerDown(KeyCode keyCode, UnityAction action)
    {
        Unlistaner(ref m_OnKeyDownList, keyCode, action);
    }
    public void AddListaner(KeyCode keyCode, UnityAction action)
    {
        Listaner(ref m_OnKeyList, keyCode, action);
    }
    public void RemoveListaner(KeyCode keyCode, UnityAction action)
    {
        Unlistaner(ref m_OnKeyList, keyCode, action);
    }
    public void AddListanerUp(KeyCode keyCode, UnityAction action)
    {
        Listaner(ref m_OnKeyUpList, keyCode, action);
    }
    public void RemoveListanerUp(KeyCode keyCode, UnityAction action)
    {
        Unlistaner(ref m_OnKeyUpList, keyCode, action);
    }
    public bool GetKey(in KeyCode keyCode)
    {
        return Input.GetKey(keyCode);
    }
    public bool GetKeyDown(in KeyCode keyCode)
    {
        return Input.GetKeyDown(keyCode);
    }
    public bool GetKeyUp(in KeyCode keyCode)
    {
        return Input.GetKeyUp(keyCode);
    }
}
