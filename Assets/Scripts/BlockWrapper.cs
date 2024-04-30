using UnityEngine;

public class BlockWrapper : MonoBehaviour
{
    //public Block blockInstance;
    public int blockNumber;
    public int chunkNumber;

    public float getBlockX()
    {
        return transform.position.x;
    }
    public float getBlockY()
    {
        return transform.position.y;
    }
    public float getBlockZ()
    {
        return transform.position.z;
    }

    public void Break()
    {
        Debug.Log("deleted");
        Destroy(gameObject);
    }

    public BlockWrapper(int blockNumber, int chunkNumber)
    {
        this.blockNumber = blockNumber;
        this.chunkNumber = chunkNumber;
    }

    public int getBlockNumber()
    {
        return blockNumber;
    }

    public int getChunkNumber()
    {
        return chunkNumber;
    }

    public void setBlockNumber(int blockNumber)
    {
        this.blockNumber = blockNumber;
    }

    public void setChunkNumber(int chunkNumber)
    {
        this.chunkNumber = chunkNumber;
    }
}