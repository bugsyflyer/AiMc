using UnityEngine;

public enum BiomeType
{
    Normal,
    Plains
}

public class Chunk : MonoBehaviour
{
    public const int ChunkSize = 32; // Size of the chunk in blocks
    public Block[,,] blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
    
    public float perlinScale = 0.1f; // Scale factor for Perlin noise
    public float heightScale = 10f; // Scale factor for terrain height
    public float waterSpawnChance = 0.02f;
    
    public BiomeType biomeType = BiomeType.Normal;
    public float plainsChance = 0.1f;
    
    public GameObject dirtPrefab;
    public GameObject stonePrefab;
    public GameObject grassPrefab;
    public GameObject woodPrefab;
    public GameObject leavesPrefab;
    public GameObject waterPrefab;
    


    // Generate terrain for this chunk
    public void GenerateTerrain(int seed)
    {
        bool isPlains = (Random.value < plainsChance && biomeType != BiomeType.Plains);
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                float perlinValue = Mathf.PerlinNoise((transform.position.x + x + seed) * perlinScale, (transform.position.z + z + seed) * perlinScale);
                int height = Mathf.FloorToInt(perlinValue * heightScale);
                float plainsHeight = 0.0f;
                
                if (isPlains)
                {
                    // Adjust height for plains biome (flatter terrain)
                    plainsHeight = (perlinValue * heightScale * 0.5f); // Example: Half the height
                }
                
                // Smooth transitions based on surrounding chunks
                SmoothTerrainTransition(x, z, plainsHeight);

                for (int y = 0; y < ChunkSize; y++)
                {
                    // Generate grass on top, dirt below grass, and stone below dirt based on height
                    if (y == height - 1)
                    {
                        blocks[x, y, z] = new Block(BlockType.Dirt);
                    }
                    else if (y == height)
                    {
                        blocks[x, y, z] = new Block(BlockType.Grass);
                    }
                    else if (y < height)
                    {
                        blocks[x, y, z] = new Block(BlockType.Stone);
                    }
                    else
                    {
                        blocks[x, y, z] = new Block(BlockType.Air); // Empty space above terrain
                    }
                }
            }
        }

        if (!isPlains)
        {
            GenerateTrees(seed);   
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
                    if (currentBlock.type == BlockType.Air) continue; // Skip air blocks

                    // Check neighboring blocks for air
                    bool exposedToAir = IsBlockExposedToAir(x, y, z);
                
                    // Set visibility based on exposure to air
                    if (exposedToAir)
                    {
                        GameObject blockPrefab = GetPrefabForBlock(currentBlock.type); // Get prefab based on block type
                        if (blockPrefab != null)
                        {
                            Vector3 blockPosition = transform.position + new Vector3(x, y, z); // Calculate block position relative to chunk
                            Instantiate(blockPrefab, blockPosition, Quaternion.identity, transform);
                        }
                    }
                }
            }
        }
    }
    
    private bool IsBlockExposedToAir(int x, int y, int z)
    {
        // Check neighboring blocks for air
        if (x > 0 && blocks[x - 1, y, z].type == BlockType.Air) return true;
        if (x < ChunkSize - 1 && blocks[x + 1, y, z].type == BlockType.Air) return true;
        if (y > 0 && blocks[x, y - 1, z].type == BlockType.Air) return true;
        if (y < ChunkSize - 1 && blocks[x, y + 1, z].type == BlockType.Air) return true;
        if (z > 0 && blocks[x, y, z - 1].type == BlockType.Air) return true;
        if (z < ChunkSize - 1 && blocks[x, y, z + 1].type == BlockType.Air) return true;

        return false; // Block is not exposed to air
    }

    private GameObject GetPrefabForBlock(BlockType type)
    {
        switch (type)
        {
            case BlockType.Air:
                return null; // No visual representation for air
            case BlockType.Stone:
                return stonePrefab; // Assign stone prefab in the inspector
            case BlockType.Grass:
                return grassPrefab;
            case BlockType.Wood:
                return woodPrefab;
            case BlockType.Leaves:
                return leavesPrefab;
            case BlockType.Dirt:
                return dirtPrefab; // Assign dirt prefab in the inspector
            // Add cases for other block types as needed
            default:
                return null; // Return null for unknown block types
        }
    }
    
    private void SmoothTerrainTransition(int x, int z, float height)
    {
        // Get neighboring chunks
        Chunk leftChunk = GetNeighborChunk(x - 1, z);
        Chunk rightChunk = GetNeighborChunk(x + 1, z);
        Chunk frontChunk = GetNeighborChunk(x, z + 1);
        Chunk backChunk = GetNeighborChunk(x, z - 1);

        // Check and interpolate heights based on neighboring chunks
        if (leftChunk != null && leftChunk.biomeType != biomeType)
        {
            height = Mathf.Lerp(height, leftChunk.GetTerrainHeightAtPosition(ChunkSize - 1, z), 0.5f);
        }
        if (rightChunk != null && rightChunk.biomeType != biomeType)
        {
            height = Mathf.Lerp(height, rightChunk.GetTerrainHeightAtPosition(0, z), 0.5f);
        }
        if (frontChunk != null && frontChunk.biomeType != biomeType)
        {
            height = Mathf.Lerp(height, frontChunk.GetTerrainHeightAtPosition(x, 0), 0.5f);
        }
        if (backChunk != null && backChunk.biomeType != biomeType)
        {
            height = Mathf.Lerp(height, backChunk.GetTerrainHeightAtPosition(x, ChunkSize - 1), 0.5f);
        }
    }
    
    private Chunk GetNeighborChunk(int x, int z)
    {
        // Calculate the position of the neighboring chunk based on the current chunk's position
        Vector3 neighborChunkPos = transform.position + new Vector3(x * ChunkSize, 0, z * ChunkSize);

        // Get the Chunk component of the neighboring chunk if it exists
        Collider[] colliders = Physics.OverlapBox(neighborChunkPos + new Vector3(ChunkSize / 2f, 0, ChunkSize / 2f), new Vector3(ChunkSize / 2f, ChunkSize / 2f, ChunkSize / 2f));
        foreach (var collider in colliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();
            if (chunk != null)
            {
                return chunk;
            }
        }

        // Return null if no neighboring chunk is found
        return null;
    }
    
    private int GetTerrainHeightAtPosition(int x, int z)
    {
        // Ensure that x and z are within valid range
        if (x >= 0 && x < ChunkSize && z >= 0 && z < ChunkSize)
        {
            // Access the terrain height at the specified position in the blocks array
            return blocks[x, 0, z].height; // Assuming height information is stored in blocks array
        }
        else
        {
            // Return a default height or handle out-of-range positions
            return 0; // Default height example
        }
    }
    
    private void GenerateTrees(int seed)
{
    int chunkSeed = seed + (int)(Random.Range(0, 100000) + Random.Range(0, 100000)); // Unique seed for each chunk
    // Randomly generate trees on the surface
    System.Random random = new System.Random(chunkSeed); // Use System.Random for random values
    float treeSpawnProbability = 0.01f;

    for (int x = 0; x < ChunkSize; x++)
    {
        for (int z = 0; z < ChunkSize; z++)
        {
            int height = CalculateSurfaceHeight(x, z); // Calculate surface height at (x, z) position

            // Check if this position is suitable for tree generation
            if (height >= 0 && random.NextDouble() < treeSpawnProbability) // Adjust the probability (5%) for tree generation
            {
                GenerateTreeAtPosition(x, height + 1, z, random); // Generate tree at surface level + 1
            }
        }
    }
}

private void GenerateTreeAtPosition(int x, int y, int z, System.Random random)
{
    // Determine tree shape and trunk height
    int trunkHeight = random.Next(2, 10); // Random trunk height between 2 and 9 blocks
    int treeType = random.Next(2); // Randomly choose one of three tree types (0, 1, or 2)

    // Generate tree based on type
    switch (treeType)
    {
        case 0:
            GenerateOakTree(x, y, z, trunkHeight);
            break;
        case 1:
            GeneratePineTree(x, y, z, trunkHeight);
            break;
        default:
            break;
    }
}

private void GenerateOakTree(int x, int y, int z, int trunkHeight)
{
    // Generate oak tree trunk
    for (int yOffset = 0; yOffset < trunkHeight; yOffset++)
    {
        int yPos = y + yOffset;
        if (yPos >= ChunkSize) break; // Ensure we stay within the chunk's bounds

        blocks[x, yPos, z] = new Block(BlockType.Wood); // Place wood blocks for trunk
    }

    // Generate oak tree canopy
    int canopyHeight = trunkHeight + 2; // Example: Canopy starts 2 blocks above the trunk
    GenerateOakTreeCanopy(x, y + trunkHeight, z, canopyHeight);
}

    private void GenerateOakTreeCanopy(int x, int y, int z, int canopyHeight)
    {
        int radius = 2; // Example: Canopy radius in blocks

        // Ensure loop bounds stay within the chunk's bounds
        for (int yOffset = 0; yOffset < canopyHeight && y + yOffset < ChunkSize; yOffset++)
        {
            int yPos = y + yOffset;

            for (int xOffset = -radius; xOffset <= radius; xOffset++)
            {
                int xPos = x + xOffset;
                if (xPos < 0 || xPos >= ChunkSize) continue; // Skip if outside chunk bounds

                for (int zOffset = -radius; zOffset <= radius; zOffset++)
                {
                    int zPos = z + zOffset;
                    if (zPos < 0 || zPos >= ChunkSize) continue; // Skip if outside chunk bounds

                    if (IsWithinCanopySphere(x, xPos, y, yPos, z, zPos, radius))
                    {
                        blocks[xPos, yPos, zPos] = new Block(BlockType.Leaves); // Place leaves blocks for canopy
                    }
                }
            }
        }
    }

    private bool IsWithinCanopySphere(int centerX, int x, int centerY, int y, int centerZ, int z, int radius)
    {
        int dx = centerX - x;
        int dy = centerY - y;
        int dz = centerZ - z;
        int distanceSquared = dx * dx + dy * dy + dz * dz;

        return distanceSquared <= (radius * radius);
    }

        private void GeneratePineTree(int x, int y, int z, int trunkHeight)
    {
        // Generate pine tree trunk
        for (int yOffset = 0; yOffset < trunkHeight; yOffset++)
        {
            int yPos = y + yOffset;
            if (yPos >= ChunkSize) break; // Ensure we stay within the chunk's bounds

            blocks[x, yPos, z] = new Block(BlockType.Wood); // Place wood blocks for trunk
        }

        // Generate pine tree canopy
        int canopyHeight = trunkHeight + 3; // Example: Canopy starts 3 blocks above the trunk
        GeneratePineTreeCanopy(x, y + trunkHeight, z, canopyHeight);
    }

        private void GeneratePineTreeCanopy(int x, int y, int z, int canopyHeight)
        {
            int baseRadius = 2; // Base radius of the canopy

            for (int yOffset = 0; yOffset < canopyHeight; yOffset++)
            {
                int yPos = y - yOffset; // Start from the top and move down the trunk
                if (yPos < 0) break; // Stop if we reach the bottom of the chunk

                int radiusOffset = baseRadius - (yOffset / 2); // Decrease radius towards the top of the canopy

                for (int xOffset = -radiusOffset; xOffset <= radiusOffset; xOffset++)
                {
                    int xPos = x + xOffset;
                    if (xPos < 0 || xPos >= ChunkSize) continue; // Skip if outside chunk bounds

                    for (int zOffset = -radiusOffset; zOffset <= radiusOffset; zOffset++)
                    {
                        int zPos = z + zOffset;
                        if (zPos < 0 || zPos >= ChunkSize) continue; // Skip if outside chunk bounds

                        // Calculate distance from current position to the trunk position
                        float distance = Vector3.Distance(new Vector3(xPos, yPos, zPos), new Vector3(x, y, z));

                        // Adjust the base radius based on distance to create a cone shape
                        float adjustedRadius = baseRadius - (distance / canopyHeight) * baseRadius;

                        // Check if the block at yPos is within bounds before accessing
                        if (yPos >= 0 && yPos < ChunkSize && distance <= adjustedRadius)
                        {
                            blocks[xPos, yPos, zPos] = new Block(BlockType.Leaves); // Place leaves blocks for canopy
                        }
                    }
                }
            }
        }


    private bool IsWithinCanopyShape(int centerX, int x, int centerY, int y, int centerZ, int z, int radius)
    {
        int dx = centerX - x;
        int dy = centerY - y;
        int dz = centerZ - z;
        int distanceSquared = dx * dx + dy * dy + dz * dz;

        return distanceSquared <= (radius * radius);
    }

    private int CalculateSurfaceHeight(int x, int z)
    {
        int maxHeight = ChunkSize - 1; // Maximum height in the chunk (assuming 0-based indexing)
    
        // Start from the top of the chunk and check downward
        for (int y = maxHeight; y >= 0; y--)
        {
            if (blocks[x, y, z] != null && blocks[x, y, z].type != BlockType.Air)
            {
                return y; // Found a non-empty block (surface)
            }
        }

        return -1; // No solid block found (shouldn't happen in a valid chunk)
    }
    
}//code end!!!!!!!!!!!!