
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

    public int x;
    public int y;
    public int z;


    public Block(BlockType type)
    {
        this.type = type;
    }

    public Block(BlockType type, int height, Chunk chunk, int blockNumber, int chunkNumber, int arrX, int arrY, int arrZ)
    {
        this.type = type;
        this.height = height;
        parentChunk = chunk;
        this.blockNumber = blockNumber;
        this.chunkNumber = chunkNumber;
        x = arrX;
        y = arrY;
        z = arrZ;
    }

    public BlockType getType()
    {
        return type;
    }
    
    public Chunk GetChunk()
    {
        return parentChunk;
    }

    public int getArrX()
    {
        return x;
    }
    
    public int getArrY()
    {
        return y;
    }
    
    public int getArrZ()
    {
        return z;
    }

    public void Break()
    {
        type = BlockType.Air;
        Debug.Log("broke");
    }
}