using System.Collections.Generic;
using UnityEngine;

public enum EnGameStatus
{
    Normal = 0,
    Lobby = 1 << 0,
    Running = 1 << 1,
    UIWindow = 1 << 2,
}

public class GameStateMgr : Singleton<GameStateMgr>
{
    public Dictionary<EnGameStatus, int> _GameStateDic = new();
    private EnGameStatus _CurGameState = EnGameStatus.Normal;

    public void AddGameState(in EnGameStatus state)
    {
        if (_GameStateDic.ContainsKey(state))
        {
            _GameStateDic[state]++;
            return;
        }
        _GameStateDic.Add(state, 1);
        _CurGameState |= state;
    }
    public void RemoveGameState(in EnGameStatus state)
    {
        if (!_GameStateDic.ContainsKey(state))
            return;
        if (--_GameStateDic[state] != 0)
            return;
        _GameStateDic.Remove(state);
        _CurGameState ^= state;
    }
    public void ResetGameState()
    {

    }
    public bool IsOnlyState(in EnGameStatus state)
    {
        return _CurGameState == state;
    }
    public bool IsContainsState(in EnGameStatus state)
    {
        return (_CurGameState & state) == state;
    }

}
