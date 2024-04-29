using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public GameObject player;
    public GameObject chunkPrefab;
    public int viewDistance = 2;
    private int seed;

    
    public int chunkCount = 0;

    //public ArrayList<Chunks> chunkList = new ArrayList<Chunks>();
    //2d arraylist of blocks
    public List<List<Block>> blocks = new List<List<Block>>();

    private Vector3 lastPlayerChunkPos;

    void Start()
    {
        GenerateWorld();
        
    }

    public Block getBlock(int x, int z)
    {
        //returns the block in blocks at given x and z
        return blocks[x][z];
    }
    
    void GenerateWorld()
    {
        seed = Random.Range(0, 100000); 
        lastPlayerChunkPos = GetChunkPosition(player.transform.position);
        GenerateChunksAroundPlayer(4, true);
    }

    void Update()
    {
        Vector3 playerChunkPos = GetChunkPosition(player.transform.position);
        if (playerChunkPos != lastPlayerChunkPos)
        {
            lastPlayerChunkPos = playerChunkPos;
            if (!CheckInitialGenInViewDistance())
            {
                GenerateChunksAroundPlayer(viewDistance, false);
            }
        }
    }


    private void GenerateChunksAroundPlayer(int viewDistance, bool isInitialGen)
    {
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector3 chunkPos = lastPlayerChunkPos + new Vector3(x * Chunk.ChunkSize, 0, z * Chunk.ChunkSize);

                // Check if the chunk is within the view distance
                bool isChunkVisible = Mathf.Abs(x) <= viewDistance && Mathf.Abs(z) <= viewDistance;

                if (isChunkVisible)
                {
                    // Generate or update the chunk if it's within view distance
                    GenerateChunk(chunkPos, isInitialGen);
                }
                else
                {
                    // Deactivate or hide the chunk if it's outside view distance
                    DeactivateChunk(chunkPos);
                }
            }
        }
    }
    
    private void DeactivateChunk(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position + new Vector3(Chunk.ChunkSize / 2f, 0, Chunk.ChunkSize / 2f),
            new Vector3(Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f));

        foreach (var collider in colliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();
            if (chunk != null && !chunk.initialGen)
            {
                // Deactivate or hide the chunk (e.g., set it inactive or disable its renderer)
                chunk.gameObject.SetActive(false);
                // Alternatively, if your chunk has a specific script for visibility control, call a method like HideChunk()
                // chunk.HideChunk();
            }
        }
    }

    private void GenerateChunk(Vector3 position, bool isInitialGen)
    {
        Collider[] colliders = Physics.OverlapBox(position + new Vector3(Chunk.ChunkSize / 2f, 0, Chunk.ChunkSize / 2f),
            new Vector3(Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f));

        foreach (var collider in colliders)
        {
            Chunk existingChunk = collider.GetComponent<Chunk>();
            if (existingChunk != null)
            {
                existingChunk.gameObject.SetActive(true);
                return;
            }

            
        }
        // Check if chunk already exists at this position, if not, instantiate it
        // Use chunkPrefab and set its position accordingly
        GameObject newChunkObj = Instantiate(chunkPrefab, position, Quaternion.identity);
        newChunkObj.transform.SetParent(transform); // Attach to TerrainManager object
        Chunk newChunk = newChunkObj.GetComponent<Chunk>();
        newChunk.setChunkNumber(chunkCount);
        //chunkList.Add(newChunk);
        List<Block> newList = new List<Block>();
        blocks.Add(newList);
        newChunk.setInitial(isInitialGen);
        newChunk.GenerateTerrain(seed, chunkCount); // Generate terrain for the chunk
        chunkCount++;
        newChunk.UpdateChunk(); // Update visuals
    }

    public static Vector3 GetChunkPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / Chunk.ChunkSize) * Chunk.ChunkSize;
        int y = Mathf.FloorToInt(position.y / Chunk.ChunkSize) * Chunk.ChunkSize;
        int z = Mathf.FloorToInt(position.z / Chunk.ChunkSize) * Chunk.ChunkSize;
        return new Vector3(x, y, z);
    }
    
    public bool GetInitialGenStatusAtPlayerPosition()
    {
        // Get the position of the chunk where the player is standing
        Vector3 playerChunkPos = GetChunkPosition(player.transform.position);

        // Calculate the position within the chunk for the player
        Vector3 localPos = player.transform.position - playerChunkPos;

        // Convert local position to block coordinates within the chunk
        int x = Mathf.FloorToInt(localPos.x);
        int y = Mathf.FloorToInt(localPos.y);
        int z = Mathf.FloorToInt(localPos.z);

        // Get the Chunk component of the chunk where the player is standing
        Collider[] colliders = Physics.OverlapBox(playerChunkPos + new Vector3(Chunk.ChunkSize / 2f, 0, Chunk.ChunkSize / 2f),
            new Vector3(Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f));
        
        foreach (var collider in colliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();
            if (chunk != null)
            {
                // Check if the block at the player's position is part of the chunk
                if (x >= 0 && x < Chunk.ChunkSize && y >= 0 && y < Chunk.ChunkSize && z >= 0 && z < Chunk.ChunkSize)
                {
                    // Get the initialGen status of the block
                    return chunk.initialGen;
                }
            }
        }

        // Default return value if the player is not standing in a valid chunk
        return false;
    }
    
    public bool CheckInitialGenInViewDistance()
    {
        Vector3 playerChunkPos = GetChunkPosition(player.transform.position);
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector3 chunkPos = playerChunkPos + new Vector3(x * Chunk.ChunkSize, 0, z * Chunk.ChunkSize);
                Collider[] colliders = Physics.OverlapBox(chunkPos + new Vector3(Chunk.ChunkSize / 2f, 0, Chunk.ChunkSize / 2f),
                    new Vector3(Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f, Chunk.ChunkSize / 2f));

                foreach (var collider in colliders)
                {
                    Chunk chunk = collider.GetComponent<Chunk>();
                    if (chunk == null || !chunk.initialGen)
                    {
                        // At least one chunk within viewDistance has initialGen as false
                        Debug.Log("Not all chunks in view distance have initialGen set to true.");
                        return false;
                    }
                }
            }
        }

        // All chunks within viewDistance have initialGen set to true
        Debug.Log("All chunks in view distance have initialGen set to true.");
        return true;
    }
    
}//THE END

