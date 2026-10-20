using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public Transform player;
    public PlayerSpawnManager manager; 
    [Header("Valid spawn height (normalized 0-1)")]
    public float minNormHeight = 0.30f;
    public float maxNormHeight = 0.44f;
    public int maxAttempts = 5000; 
    public void SetSpawn(int[,] mazeGen, float[,] heights)
    {
        //using the terrain height scale instead of hardcoded val
        float heightScale = 50f; 
        if(Terrain.activeTerrain != null)
        {
            heightScale = Terrain.activeTerrain.terrainData.size.y; 
        }

        float xPos = 0f; 
        float zPos = 0f; 
        float normH = 0f; 
        float mzMag = 1f; 
        bool found = false; 

       for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            xPos = 1f * Random.Range(1, 249);
            zPos = 1f * Random.Range(1, 249);
            normH = heights[(int)(4.098 * zPos), (int)(4.098 * xPos)];
 
            float X = (((float)xPos / 500f) * (2f * 6f + 2f) - 1f);
            float Z = (((float)zPos / 500f) * (2f * 6f + 2f) - 1f);
 
            float varyX = 1.5f * (0.5f * perlin(0.37f * X, 0.37f * (Z - 34f)) + 0.45f * perlin(0.511f * X, 0.511f * (Z - 81f)));
            float varyZ = 1.5f * (0.5f * perlin(0.37f * (X - 43f), 0.37f * Z) + 0.45f * perlin(0.511f * (X + 73f), 0.511f * Z));
 
            mzMag = mazeMag(X + varyX, Z + varyZ, mazeGen);
 
            if (mzMag <= 0.075f && normH >= minNormHeight && normH <= maxNormHeight)
            {
                found = true;
                break;
            }
        }
 
        if (!found)
        {
            Debug.LogWarning("PlayerSpawnPoint: no ideal spawn found, using last candidate.");
        }
 
        float yPos = normH * heightScale;
        Debug.Log($"PlayerSpawnPoint: x={xPos}, z={zPos}, y={yPos}, heightScale={heightScale}");
 
        transform.localPosition = new Vector3(xPos, yPos + 2f, zPos);
        Physics.SyncTransforms();
 
        if (manager != null)
        {
            // Manager moves the player (saved position or this spawn point)
            manager.BeginSpawn();
        }
        else if (player != null)
        {
            // Fallback: old behavior if no manager is assigned
            player.position = transform.position;
        }
    }

    float mazeMag(float x, float z, int[,] mazeGen)
    {
        if((x<0f)||(x>=2f*6f)||(z<0f)||(z>=2f*6f)) return 1f;

        float mi_x = (float)mazeGen[(int)x, (int)z]*(1f-intr(x)) + (float)mazeGen[(int)x+1, (int)z]*intr(x);
        float mi_x1 = (float)mazeGen[(int)x, (int)z+1]*(1f-intr(x)) + (float)mazeGen[(int)x+1, (int)z+1]*intr(x);
        float mi_xz = 1f - (mi_x*(1f-intr(z)) + mi_x1*intr(z));

        return mi_xz;
    }

    float intr(float x)
    {
        return 3f*Mathf.Pow(Mod(x, 1f), 2) - 2f*Mathf.Pow(Mod(x, 1f), 3);
    }

    float perlin(float x, float z)
    {
        return 2f*Mathf.PerlinNoise(x, z) - 1f;
    }

    float Mod(float x, float m)
    {
        return ((x % m) + m) % m;
    }
}
