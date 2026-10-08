using System.Text;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    [Header("Terrain")]
    public int resolution = 2049;
    public float width = 1000f;
    public float heightScale = 200f;

    [Header("Generation")]
    public float sampleScale = 1.0f;
    public int lvlSeed = 42;
    public int mazeSize = 6;

    private Terrain terrain;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        // Create TerrainData
        TerrainData data = new TerrainData();

        data.heightmapResolution = resolution;
        data.size = new Vector3(width, heightScale, width);

        // Generate heights
        float[,] heights = new float[resolution, resolution];

        int[,] mazeGen = generateMaze(mazeSize, lvlSeed);

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float worldX = x * sampleScale;
                float worldZ = z * sampleScale;

                float h = HeightFunction(worldX, worldZ, mazeGen);

                heights[z, x] = Mathf.Clamp01(h);
            }
        }

        data.SetHeights(0, 0, heights);

        // Create Terrain GameObject
        GameObject terrainObject = Terrain.CreateTerrainGameObject(data);
        terrain = terrainObject.GetComponent<Terrain>();

        // Optional: generate colors/material
        GenerateTerrainTexture(data, heights, mazeGen);
    }

    float HeightFunction(float x, float z, int[,] mazeGen)
    {
        float X = (x/(float)resolution)*(2f*(float)mazeSize+2f)-1f;
        float Z = (z/(float)resolution)*(2f*(float)mazeSize+2f)-1f;

        float varyX = 1.5f*(0.5f*perlin(0.37f*X, 0.37f*(Z-34f)) + 0.45f*perlin(0.511f*X, 0.511f*(Z-81f)));
        float varyZ = 1.5f*(0.5f*perlin(0.37f*(X-43f), 0.37f*Z) + 0.45f*perlin(0.511f*(X+73f), 0.511f*Z));

        float mzMag = mazeMag(X + varyX, Z + varyZ, mazeGen);

	float mntVal = perlin(0.5f*X, 0.5f*Z) + 1.7f*perlin(0.897f*X, 0.897f*(Z+41f))+1.4f*perlin(1.77f*X, 1.77f*(Z+83f))+1.25f*perlin(2.57f*X, 2.57f*(Z+127f))+1.1f*perlin(4.157f*X, 4.157f*(Z+171f))+0.9f*perlin(7.157f*X, 7.157f*(Z+221f));
        float baseVal = 0.25f+mzMag*(3f*mzMag+3f+mntVal)+0.2f*mntVal;
        float wVal = perlin(0.5f*X+51f, 0.5f*Z)+1.7f*perlin(0.897f*X, 0.897f*(Z+147f))+1.4f*perlin(1.77f*X, 1.77f*(Z+183f))+1.25f*perlin(2.57f*X, 2.57f*(Z+227f))+1.1f*perlin(4.157f*X, 4.157f*(Z+271f))+0.9f*perlin(7.157f*X, 7.157f*(Z+321f));

        float height = (baseVal - Mathf.Pow(0.25f*wVal, 2))*0.05f;
        height += (Mathf.Sqrt(Mathf.Pow(height,2) + 0.0004f)-height)*(0.5f/(1f+Mathf.Pow(50f*mzMag, 4)));
        return height + 0.3f;
    }

    float mazeMag(float x, float z, int[,] mazeGen)
    {
        if((x<0f)||(x>=2f*(float)mazeSize)||(z<0f)||(z>=2f*(float)mazeSize)) return 1f;

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

    int[,] generateMaze(int size, int seed)
    {
        Random.InitState(lvlSeed);

        int[,] maze = new int[2*size+1,2*size+1];
        int is_filled = 0;

        int[] loc = {2*Random.Range(1,size+1)-1, 2*Random.Range(1,size+1)-1};
        maze[loc[0], loc[1]] = 1;
        int ornt = 0;
        int[] nloc = {0, 0};
        int[,] ornts = {{-1, 0}, {1, 0}, {0, -1}, {0, 1}};

        for(int i=0; i<2000; i++)
        {
            for(int j=0; j<200; j++)
            {
                ornt = Random.Range(0,4);
                nloc = new int[] {loc[0]+2*ornts[ornt, 0], loc[1]+2*ornts[ornt, 1]};
                if((nloc[0]>=0)&&(nloc[0]<=2*size)&&(nloc[1]>=0)&&(nloc[1]<=2*size)) break;
                else
                {
                    for(int k=0; k<200; k++)
                    {
                        loc = new int[] {2*Random.Range(1,size+1)-1, 2*Random.Range(1,size+1)-1};
                        if(maze[loc[0], loc[1]]==1) break;
                        if(k==199) Debug.Log("error 3");
                    }
                }
                if(j==199) Debug.Log("error 2");
            }
            
            if(maze[nloc[0], nloc[1]]==0)
            {
                maze[loc[0]+ornts[ornt, 0], loc[1]+ornts[ornt, 1]] = 1;
                maze[nloc[0], nloc[1]] = 1;
                loc = new int[] {nloc[0], nloc[1]};
            }
            else
            {
                for(int j=0; j<2000; j++)
                {
                    loc = new int[] {2*Random.Range(1,size+1)-1, 2*Random.Range(1,size+1)-1};
                    if(maze[loc[0], loc[1]]==1) break;
                    if(j==1999) Debug.Log("error 4");
                }
            }
            
            is_filled = 1;
            for(int n=0; n<size; n++)
            {
                for(int m=0; m<size; m++)
                {
                    if(maze[2*n+1, 2*m+1]==0) is_filled = 0;
                }
            }
            if(is_filled==1) break;
            if(i==1999) Debug.Log("error 1");
        }

        var outStr = new StringBuilder();
        for(int n=0; n<2*size+1; n++)
        {
            for(int m=0; m<2*size+1; m++)
            {
                if(maze[n, m]==0) outStr.Append("##");
                else outStr.Append("    ");
            }
            outStr.Append("\n");
        }
        Debug.Log(outStr.ToString());
        return maze;
    }

    Color ColorFunction(int x, int z, float height, int[,] mazeGen)
    {
        float X = ((float)x/(float)resolution)*(2f*(float)mazeSize+2f)-1f;
        float Z = ((float)z/(float)resolution)*(2f*(float)mazeSize+2f)-1f;

        float varyX = 1.5f*(0.5f*perlin(0.37f*X, 0.37f*(Z-34f)) + 0.45f*perlin(0.511f*X, 0.511f*(Z-81f)));
        float varyZ = 1.5f*(0.5f*perlin(0.37f*(X-43f), 0.37f*Z) + 0.45f*perlin(0.511f*(X+73f), 0.511f*Z));

        float mzMag = mazeMag(X + varyX, Z + varyZ, mazeGen);

        if (mzMag < 0.005f) return new Color(1f, 0.8f, 0.4f);

        float h = height + 0.022f*perlin(7.157f*X, 7.157f*(Z-171f))+0.018f*perlin(13.157f*X, 13.157f*(Z-221f));

        if (height < 0.29f) return new Color(1f, 0.8f, 0.4f);
        if (height < 0.31f) return new Color(1f-((height-0.29f)/0.02f), 0.8f+0.2f*((height-0.29f)/0.02f), 0.4f-0.4f*((height-0.29f)/0.02f));
        if (h < 0.45f) return new Color(0f, 1f, 0f);
        if (h < 0.48f) return new Color(0f+0.5f*((h-0.45f)/0.03f), 1f-0.7f*((h-0.45f)/0.03f), 0f+0.1f*((h-0.45f)/0.03f));
        if (h < 0.6f) return new Color(0.5f, 0.3f, 0.1f);
        if (h < 0.65f) return new Color(0.5f+0.5f*((h-0.6f)/0.05f), 0.3f+0.7f*((h-0.6f)/0.05f), 0.1f+0.9f*((h-0.6f)/0.05f));
        return new Color(1f, 1f, 1f);
    }

    void GenerateTerrainTexture(TerrainData data, float[,] heights, int[,] mazeGen)
    {
        int textureResolution = resolution;

        Texture2D texture = new Texture2D(
            textureResolution,
            textureResolution,
            TextureFormat.RGBA32,
            false
        );

        for (int z = 0; z < textureResolution; z++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                float h = heights[z, x];

                Color color = ColorFunction(x, z, h, mazeGen);

                texture.SetPixel(x, z, color);
            }
        }

        texture.Apply();

        // For a simple prototype, use a material
        Material material = new Material(
            Shader.Find("Universal Render Pipeline/Lit")
        );

        material.mainTexture = texture;

        terrain.materialTemplate = material;
    }
}