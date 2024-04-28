
using UnityEngine;

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
    private BlockType type;
    public int height;
    private Chunk parentChunk;


    public Block(BlockType type)
    {
        this.type = type;
    }

    public Block(BlockType type, int height, Chunk chunk)
    {
        this.type = type;
        this.height = height;
        parentChunk = chunk;
    }

    public BlockType getType()
    {
        return type;
    }
    
    public Chunk GetChunk()
    {
        return parentChunk;
    }

    public void Break()
    {
        type = BlockType.Air;
        Debug.Log("broke");
    }
}