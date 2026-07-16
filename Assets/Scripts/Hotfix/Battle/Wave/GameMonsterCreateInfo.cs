
public class GameMonsterCreateInfo : IClassPoolDestroy
{
    public int index;
    public int monsterId;
    public int row;
    public int col;
    public int attValueCfgId;
    public void OnPoolDestroy()
    {
        monsterId
            = row
            = col
            = attValueCfgId
            = index
            = -1;
    }
    public void Init(in int index, int monsterId, int row, int col, in int attValueCfgId)
    {
        this.index = index;
        this.monsterId = monsterId;
        this.row = row;
        this.col = col;
        this.attValueCfgId = attValueCfgId;
    }
}