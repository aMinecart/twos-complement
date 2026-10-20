using UnityEngine;

public class BountySpawner : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public int numberOfInstances = 1;

    [Header("Valid bounty height (normalized 0-1)")]
    public float minNormHeight = 0.30f;
    public float maxNormHeight = 0.44f;

    public float heightOffset = 8f;
 
    public int maxAttempts = 5000;

    public void GenBounties(int[,] mazeGen, float[,] heights)
    {
        SpawnBounty(prefab1, mazeGen, heights);
        SpawnBounty(prefab2, mazeGen, heights);
        SpawnBounty(prefab3, mazeGen, heights);
    }

    void SpawnBounty(GameObject prefab, int[,] mazeGen, float[,] heights)
    {
        if(prefab == null)
        {
           Debug.LogWarning("BountySpawner: a bounty prefab is not assigned.");
            return; 
        }

        //uses terrain height scale 
        float heightScale = 50f;
        if (Terrain.activeTerrain != null)
        {
            heightScale = Terrain.activeTerrain.terrainData.size.y;
        }
 
        float xPos = 0f;
        float zPos = 0f;
        float normH = 0f;
        bool found = false;

          for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            xPos = 1f * Random.Range(1, 499);
            zPos = 1f * Random.Range(1, 499);
            normH = heights[(int)(4.098 * zPos), (int)(4.098 * xPos)];
 
            float X = (((float)xPos / 500f) * (2f * 6f + 2f) - 1f);
            float Z = (((float)zPos / 500f) * (2f * 6f + 2f) - 1f);
 
            float varyX = 1.5f * (0.5f * perlin(0.37f * X, 0.37f * (Z - 34f)) + 0.45f * perlin(0.511f * X, 0.511f * (Z - 81f)));
            float varyZ = 1.5f * (0.5f * perlin(0.37f * (X - 43f), 0.37f * Z) + 0.45f * perlin(0.511f * (X + 73f), 0.511f * Z));
 
            float mzMag = mazeMag(X + varyX, Z + varyZ, mazeGen);
 
            if (mzMag <= 0.025f && normH >= minNormHeight && normH <= maxNormHeight)
            {
                found = true;
                break;
            }
        }

         if (!found)
        {
            Debug.LogWarning("BountySpawner: no ideal spot found, using last candidate.");
        }

        float yPos = normH * heightScale;

        Instantiate(prefab, new Vector3(xPos, yPos + heightOffset, zPos), Quaternion.identity, transform);
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