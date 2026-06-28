
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputKeyCodeEventInfo
{
    public Dictionary<KeyCode, HashSet<UnityAction>> _KeyList = new();
    public Dictionary<KeyCode, HashSet<UnityAction>> _AddList = new();
    public Dictionary<KeyCode, HashSet<UnityAction>> _RemoveList = new();

    public void Apply()
    {
        if (_AddList.Count > 0)
        {
            foreach (var item in _AddList)
            {
                if (!_KeyList.TryGetValue(item.Key, out var actionList))
                {
                    actionList = new();
                    _KeyList.Add(item.Key, actionList);
                }
                foreach (var action in item.Value)
                    actionList.Add(action);
            }
            _AddList.Clear();
        }
        if (_RemoveList.Count > 0)
        {
            foreach (var item in _RemoveList)
            {
                if (!_KeyList.TryGetValue(item.Key, out var actionList))
                    continue;
                foreach (var action in item.Value)
                    actionList.Remove(action);
                if (actionList.Count > 0)
                    continue;
                _KeyList.Remove(item.Key);
            }
            _RemoveList.Clear();
        }
    }
}