using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Notes for future me:
// Implementing multiple kinds of pheromones can easilly be done by using multiple pheromone maps and updating these one by one. 
// Might not be the fastest solution though.

public class PheromoneManager : MonoBehaviour
{
    int mapSizeX = 50;
    int mapSizeZ = 50;
    float scanTimeInterval = 1f;
    float senseThreshold = 0.01f;
    float evaporationFactor = 0.1f;
    float diffuseFactor = 0.1f;
    float [,] pheromoneMap; 
    float [,] pheromoneTransferMap;

    // Evaporates the pheromone value at [xPos,zPos] and diffuse some of that to the 8 surrounding squares.
    void diffuseFromPos(int xPos, int zPos)
    {
        float concentration = pheromoneMap[xPos, zPos];
        for (int x = xPos-1; x < xPos+1; x++)
        {
            for (int z = zPos-1; z < zPos+1; z++)
            {
                if (x == zPos && z == zPos)
                {
                    pheromoneTransferMap[x,z] = pheromoneTransferMap[x,z] + concentration * (1f - 8 * diffuseFactor + evaporationFactor);
                }else
                {
                    pheromoneTransferMap[x,z] = pheromoneMap[x,z] + concentration * diffuseFactor;
                }          
            }
        }
    }

    // Checks whether the square [xPos][zPos] has a pheromone level above a certain threshold. If it does return true, if it doesn't return false.
    bool posHasPheromones(int xPos,int zPos)
    {
        if (pheromoneMap[xPos,zPos] > senseThreshold)
        {
            return true;
        }else
        {
            return false;
        }
    }

    // Goes over each square to check if it has pheromones, and diffuses and evaporates the pheromones at these locations
    void SpreadAndEvaporatePheromones()
    {
        pheromoneTransferMap = new float[mapSizeX,mapSizeZ];
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
        pheromoneMap = pheromoneTransferMap;
    }

    // Start is called before the first frame update
    void Start()
    {
        pheromoneMap = new float[mapSizeX,mapSizeZ];
        InvokeRepeating("SpreadAndEvaporatePheromones", 0f, scanTimeInterval);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //// Functions for interacting with the pheromone manager
    // Drop pheromones at location [xPos, zPos]
    public void dropPheromone(int xPos, int zPos, float concentration)
    {
        pheromoneMap[xPos, zPos] = pheromoneMap[xPos, zPos] + concentration;
    }

    // Sense if there are pheromones at location [xPos, zPos]
    public bool sensePheromone(int xPos, int zPos){
        return posHasPheromones(xPos, zPos);
    }

    // Sense pheromone concentration of location [xPos, zPos]
    public float sensePheromoneConcentration(int xPos, int zPos){
        return pheromoneMap[xPos, zPos];
    }
}