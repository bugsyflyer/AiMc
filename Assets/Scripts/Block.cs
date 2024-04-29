
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

    public int blockNumber;
    public int chunkNumber;


    public Block(BlockType type)
    {
        this.type = type;
    }

    public Block(BlockType type, int height, Chunk chunk, int blockNumber, int chunkNumber)
    {
        this.type = type;
        this.height = height;
        parentChunk = chunk;
        this.blockNumber = blockNumber;
        this.chunkNumber = chunkNumber;
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
        parentChunk.UpdateChunk();
    }
}