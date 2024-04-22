using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int ChunkSize = 32; // Size of the chunk in blocks
    public Block[,,] blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
    
    public float perlinScale = 0.1f; // Scale factor for Perlin noise
    public float heightScale = 10f; // Scale factor for terrain height
    
    public GameObject dirtPrefab;
    public GameObject stonePrefab;


    // Generate terrain for this chunk
    public void GenerateTerrain(int seed)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                float perlinValue = Mathf.PerlinNoise((transform.position.x + x + seed) * perlinScale, (transform.position.z + z + seed) * perlinScale);
                int height = Mathf.FloorToInt(perlinValue * heightScale);

                for (int y = 0; y < ChunkSize; y++)
                {
                    // Example: Generate dirt on top of stone based on height
                    if (y <= height - 2)
                    {
                        blocks[x, y, z] = new Block(BlockType.Stone);
                    }
                    else if (y <= height)
                    {
                        blocks[x, y, z] = new Block(BlockType.Dirt);
                    }
                    else
                    {
                        blocks[x, y, z] = new Block(BlockType.Air); // Empty space above terrain
                    }
                }
            }
        }
    }

    // Update chunk visuals based on blocks array
    public void UpdateChunk()
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                    Block currentBlock = blocks[x, y, z];
                    GameObject blockPrefab = GetPrefabForBlock(currentBlock.type); // Get prefab based on block type
                    if (blockPrefab != null)
                    {
                        Vector3 blockPosition = transform.position + new Vector3(x, y, z); // Calculate block position
                        Instantiate(blockPrefab, blockPosition, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    private GameObject GetPrefabForBlock(BlockType type)
    {
        switch (type)
        {
            case BlockType.Air:
                return null; // No visual representation for air
            case BlockType.Stone:
                return stonePrefab; // Assign stone prefab in the inspector
            case BlockType.Dirt:
                return dirtPrefab; // Assign dirt prefab in the inspector
            // Add cases for other block types as needed
            default:
                return null; // Return null for unknown block types
        }
    }
}