using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject player;
    public GameObject chunkPrefab;
    public int viewDistance = 1;
    private int seed;

    private Vector3 lastPlayerChunkPos;

    void Start()
    {
        GenerateWorld();
        
    }
    
    void GenerateWorld()
    {
        seed = Random.Range(0, 100000); 
        lastPlayerChunkPos = GetChunkPosition(player.transform.position);
        GenerateChunksAroundPlayer();
    }

    void Update()
    {
        Vector3 playerChunkPos = GetChunkPosition(player.transform.position);
        if (playerChunkPos != lastPlayerChunkPos)
        {
            lastPlayerChunkPos = playerChunkPos;
            //GenerateChunksAroundPlayer();
        }
    }

    private void GenerateChunksAroundPlayer()
    {
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector3 chunkPos = lastPlayerChunkPos + new Vector3(x * Chunk.ChunkSize, 0, z * Chunk.ChunkSize);
                GenerateChunk(chunkPos);
            }
        }
    }

    private void GenerateChunk(Vector3 position)
    {
        // Check if chunk already exists at this position, if not, instantiate it
        // Use chunkPrefab and set its position accordingly
        GameObject newChunkObj = Instantiate(chunkPrefab, position, Quaternion.identity);
        newChunkObj.transform.SetParent(transform); // Attach to TerrainManager object
        Chunk newChunk = newChunkObj.GetComponent<Chunk>();
        newChunk.GenerateTerrain(seed); // Generate terrain for the chunk
        newChunk.UpdateChunk(); // Update visuals
    }

    private Vector3 GetChunkPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / Chunk.ChunkSize) * Chunk.ChunkSize;
        int y = Mathf.FloorToInt(position.y / Chunk.ChunkSize) * Chunk.ChunkSize;
        int z = Mathf.FloorToInt(position.z / Chunk.ChunkSize) * Chunk.ChunkSize;
        return new Vector3(x, y, z);
    }
}