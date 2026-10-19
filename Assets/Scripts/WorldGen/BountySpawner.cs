using UnityEngine;

public class BountySpawner : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public int numberOfInstances = 1;

    public void GenBounties(int[,] mazeGen, float[,] heights)
    {
        float xPos = 0;
        float zPos = 0;
        float yPos = 0;
        float X = 0;
        float Z = 0;
        float varyX = 0;
        float varyZ = 0;
        float mzMag = 0;
        while((mzMag > 0.025f)||(yPos<15f)||(yPos>22f))
        {
            xPos = 1f*Random.Range(1,499);
            zPos = 1f*Random.Range(1,499);
            yPos = 50f*heights[(int)(4.098*zPos), (int)(4.098*xPos)];

            X = (((float)xPos/500f)*(2f*6f+2f)-1f);
            Z = (((float)zPos/500f)*(2f*6f+2f)-1f);

            varyX = 1.5f*(0.5f*perlin(0.37f*X, 0.37f*(Z-34f)) + 0.45f*perlin(0.511f*X, 0.511f*(Z-81f)));
            varyZ = 1.5f*(0.5f*perlin(0.37f*(X-43f), 0.37f*Z) + 0.45f*perlin(0.511f*(X+73f), 0.511f*Z));

            mzMag = mazeMag(X + varyX, Z + varyZ, mazeGen);
        }
        GameObject bounty1 = Instantiate(prefab1, new Vector3(xPos, yPos+8, zPos), Quaternion.identity, transform);

        //bounty1.transform.localPosition = new Vector3(xPos, yPos+10, zPos);

        xPos = 0;
        zPos = 0;
        yPos = 0;
        X = 0;
        Z = 0;
        varyX = 0;
        varyZ = 0;
        mzMag = 0;
        while((mzMag > 0.025f)||(yPos<15f)||(yPos>22f))
        {
            xPos = 1f*Random.Range(1,499);
            zPos = 1f*Random.Range(1,499);
            yPos = 50f*heights[(int)(4.098*zPos), (int)(4.098*xPos)];

            X = (((float)xPos/500f)*(2f*6f+2f)-1f);
            Z = (((float)zPos/500f)*(2f*6f+2f)-1f);

            varyX = 1.5f*(0.5f*perlin(0.37f*X, 0.37f*(Z-34f)) + 0.45f*perlin(0.511f*X, 0.511f*(Z-81f)));
            varyZ = 1.5f*(0.5f*perlin(0.37f*(X-43f), 0.37f*Z) + 0.45f*perlin(0.511f*(X+73f), 0.511f*Z));

            mzMag = mazeMag(X + varyX, Z + varyZ, mazeGen);
        }
        GameObject bounty2 = Instantiate(prefab2, new Vector3(xPos, yPos+8, zPos), Quaternion.identity, transform);

        //bounty2.transform.localPosition = new Vector3(xPos, yPos+10, zPos);

        xPos = 0;
        zPos = 0;
        yPos = 0;
        X = 0;
        Z = 0;
        varyX = 0;
        varyZ = 0;
        mzMag = 0;
        while((mzMag > 0.025f)||(yPos<15f)||(yPos>22f))
        {
            xPos = 1f*Random.Range(1,499);
            zPos = 1f*Random.Range(1,499);
            yPos = 50f*heights[(int)(4.098*zPos), (int)(4.098*xPos)];

            X = (((float)xPos/500f)*(2f*6f+2f)-1f);
            Z = (((float)zPos/500f)*(2f*6f+2f)-1f);

            varyX = 1.5f*(0.5f*perlin(0.37f*X, 0.37f*(Z-34f)) + 0.45f*perlin(0.511f*X, 0.511f*(Z-81f)));
            varyZ = 1.5f*(0.5f*perlin(0.37f*(X-43f), 0.37f*Z) + 0.45f*perlin(0.511f*(X+73f), 0.511f*Z));

            mzMag = mazeMag(X + varyX, Z + varyZ, mazeGen);
        }
        GameObject bounty3 = Instantiate(prefab3, new Vector3(xPos, yPos+8, zPos), Quaternion.identity, transform);

        //bounty3.transform.localPosition = new Vector3(xPos, yPos+8, zPos);
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