
public class GameMonsterCreateInfo : IClassPoolDestroy
{
    public int index;
    public int monsterId;
    public int row;
    public int col;
    public float attCofficient;
    public void OnPoolDestroy()
    {
        attCofficient
            = monsterId
            = row
            = col
            = index
            = -1;
    }
    public void Init(in int index, int monsterId, int row, int col, in float attCofficient)
    {
        this.attCofficient = attCofficient;
        this.index = index;
        this.monsterId = monsterId;
        this.row = row;
        this.col = col;
    }
}