public enum BlockType
{
    Air,
    Stone,
    Dirt,
    Wood,
    Leaves,
    IronOre
}

public class Block
{
    public BlockType type;

    public Block(BlockType type)
    {
        this.type = type;
    }
}