using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Notes for future me:
// Implementing multiple kinds of pheromones can easilly be done by using multiple pheromone maps and updating these one by one. 
// Might not be the fastest solution though.

public class PheromoneManager : MonoBehaviour
{
    public VizPlane vizPlanePrefab;
    VizPlane vizPlane;
    public AvailabilityMap AM;
    public Floor floor;
    public PheromoneVizTile pheromoneVizTile;
    int mapSizeX;
    int mapSizeZ;
    float pheromoneCap = 100;
    float scanTimeInterval = .1f;
    float senseThreshold = .00001f;
    float evaporationFactor = .9f;
    float diffuseFactor = .09f;
    float [,] pheromoneMap;
    float [,] pheromoneChangeMap;
    PheromoneVizTile [,] tileMap;

    // Start is called before the first frame update
    void Start()
    {
        // Get the floor dimensions
        mapSizeZ = (int)Mathf.Round(floor.transform.lossyScale.z * 10f);
        mapSizeX = (int)Mathf.Round(floor.transform.lossyScale.x * 10f);

        pheromoneMap = new float[mapSizeX, mapSizeZ];

        vizPlane = Instantiate(vizPlanePrefab, Vector3.zero, Quaternion.identity);
        //CreateTileMap(mapSizeX, mapSizeZ);
        InvokeRepeating("SpreadAndEvaporatePheromones", 0f, scanTimeInterval);
    }

    // Goes over each square to check if it has pheromones, and diffuses and evaporates the pheromones at these locations
    void SpreadAndEvaporatePheromones()
    {
        pheromoneChangeMap = new float[mapSizeX, mapSizeZ];
        // Calculate the difference for each tile
        for (int xPos = 0; xPos < mapSizeX; xPos++)
        {
            for (int zPos = 0; zPos < mapSizeZ; zPos++)
            {
                if (posHasPheromones(xPos, zPos))
                {
                    diffuseFromPos(xPos, zPos);
                }
            }
        }
        // add/substract the difference to/from the tile
        for (int xPos = 0; xPos < mapSizeX; xPos++)
        {
            for (int zPos = 0; zPos < mapSizeZ; zPos++)
            {
                pheromoneMap[xPos,zPos] += pheromoneChangeMap[xPos,zPos];
                if (pheromoneMap[xPos,zPos]<senseThreshold)
                {
                    pheromoneMap[xPos,zPos] = 0;
                }
            }
        }
        UpdateVisuals();
    }

    // Evaporates the pheromone value at [xPos,zPos] and diffuse some of that to the 8 surrounding squares.
    void diffuseFromPos(int xPos, int zPos)
    {
        float concentration = pheromoneMap[xPos, zPos];
        for (int x = xPos - 1; x <= xPos + 1; x++)
        {
            for (int z = zPos - 1; z <= zPos + 1; z++)
            {
                if (z >= mapSizeZ || x >= mapSizeX || z < 0 || x < 0 || !AM.IsCellAvailable(x,z))
                {}
                else if (x == xPos && z == zPos)
                {
                    pheromoneChangeMap[x, z] -= concentration * evaporationFactor;
                }else
                {
                    pheromoneChangeMap[x, z] += concentration * diffuseFactor;
                }          
            }
        }
    }

    // Checks whether the square [xPos][zPos] has a pheromone level above a certain threshold. If it does return true, if it doesn't return false.
    bool posHasPheromones(int xPos, int zPos)
    {
        if (pheromoneMap[xPos, zPos] > senseThreshold)
        {
            return true;
        }else
        {
            return false;
        }
    }

    //// Functions for interacting with the pheromone manager
    // Drop pheromones at location [xPos, zPos]
    public void dropPheromone(int xPos, int zPos, float concentration)
    {
        if (AM.IsCellAvailable(xPos,zPos))
        {
            pheromoneMap[xPos, zPos] += concentration;
        }   
    }

    // Sense if there are pheromones at location [xPos, zPos]
    public bool sensePheromone(int xPos, int zPos)
    {
        return posHasPheromones(xPos, zPos);
    }

    // Sense pheromone concentration of location [xPos, zPos]
    public float GetPheromoneConcentration(int xPos, int zPos)
    {
        return pheromoneMap[xPos, zPos];
    }

    // Returns the direction with the highest pheromone concentration
    public float[] GetDirectionHighestConcentration(int xPos, int zPos)
    {
        float[] direction = new float[2];
        float concentration;

        concentration = 0;

        for (int x = xPos - 1; x <= xPos + 1; x++)
        {
            for (int z = zPos - 1; z <= zPos + 1; z++)
            {
                if (pheromoneMap[x, z] > concentration)
                {
                    concentration = pheromoneMap[x, z];
                    direction[0] = (x-xPos);
                    direction[1] = (z-zPos);
                }
            }
        }
        return direction;
    }

    // Functions used for visualising pheromones
    void UpdateVisuals()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                if (pheromoneMap[x, z] < pheromoneCap)
                {

                    vizPlane.ChangeTransparancy(x, z, Mathf.Pow(pheromoneMap[x, z]/pheromoneCap, 1f/20f));
                    //tileMap[x, z].ChangeTransparancy(Mathf.Pow(pheromoneMap[x, z]/pheromoneCap, 1f/10f));
                }else
                {
                    vizPlane.ChangeTransparancy(x, z, 1);
                    //tileMap[x, z].ChangeTransparancy(1);
                }              
            }
        }
        vizPlane.ApplyTextureChanges();
    }

    // Create a map and spawn all of the tiles for pheromone visibility
    void CreateTileMap(int mapSizeX, int mapSizeZ)
    {
        tileMap = new PheromoneVizTile[mapSizeX, mapSizeZ];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                PheromoneVizTile tile = Instantiate(pheromoneVizTile, new Vector3 (x+0.5f,0,z+0.5f), Quaternion.identity);
                tileMap[x,z] = tile;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}