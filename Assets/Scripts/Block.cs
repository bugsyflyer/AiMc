public enum BlockType
{
    Air,
    Stone,
    Dirt,
    Grass,
    Wood,
    Leaves,
    IronOre
}

public class Block
{
    public BlockType type;
    public int height;

    public Block(BlockType type)
    {
        this.type = type;
        this.height = height;
    }
}