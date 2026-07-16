
using System.Collections.Generic;

public class GameBattleWaveData : IClassPoolDestroy
{
    public int CreateCount => monsters.Count - delayCreateList.Count;
    private int nextIndex = 0;
    private readonly List<int> delayCreateList = new();
    private readonly List<GameMonsterCreateInfo> monsters = new();
    public int MaxMonsterCount => monsters.Count;
    public void OnPoolDestroy()
    {
        foreach (var info in monsters)
        {
            ClassPoolMgr.Instance.Push(info);
        }
        monsters.Clear();
        delayCreateList.Clear();
        nextIndex = 0;
    }
    public void AddMonsterCreateInfo(GameMonsterCreateInfo monsterInfo)
    {
        monsterInfo.index = this.monsters.Count;
        this.delayCreateList.Add(this.monsters.Count);
        this.monsters.Add(monsterInfo);
    }
    public bool TryGetNextCreateIndex(out GameMonsterCreateInfo createInfo)
    {
        createInfo = null;
        if (this.delayCreateList.Count == 0)
            return false;
        nextIndex = (nextIndex + 1) % this.delayCreateList.Count;
        var index = this.delayCreateList[nextIndex];
        createInfo = monsters[index];
        return true;
    }
    public void CreateFinish(int index)
    {
        delayCreateList.Remove(index);
    }
}