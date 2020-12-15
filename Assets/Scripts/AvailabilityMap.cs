using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailabilityMap : MonoBehaviour
{

    public Floor floor;
    int width;
    int height;
    bool[,] map;
    LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        // Get the floor dimensions
        width = (int)Mathf.Round(floor.transform.lossyScale.x * 10f);
        height = (int)Mathf.Round(floor.transform.lossyScale.z * 10f);

        // Check if the floor is fully covered by the map
        if((floor.transform.lossyScale.x * 10) % 1.0f != 0 || (floor.transform.lossyScale.z * 10) % 1.0f != 0)
        {
            print("Make sure the ground is integer scale!!!!!");
        }

        // The only thing we care about are barriers
        mask = LayerMask.GetMask("Barrier");

        ComputeAvailabilityMap();
    }

    bool TraceRays(int x, int z)
    {
        // Make the ray originate from the center of a square
        Vector3 origin1 = new Vector3(x + 0.25f, 1, z + 0.25f);
        Vector3 origin2 = new Vector3(x + 0.25f, 1, z + 0.75f);
        Vector3 origin3 = new Vector3(x + 0.75f, 1, z + 0.25f);
        Vector3 origin4 = new Vector3(x + 0.75f, 1, z + 0.75f);

        // If there is no hit encountered there is no barrier there
        return ! (Physics.Raycast(origin1, Vector3.down, 1, mask) ||
                Physics.Raycast(origin2, Vector3.down, 1, mask) ||
                Physics.Raycast(origin3, Vector3.down, 1, mask) ||
                Physics.Raycast(origin4, Vector3.down, 1, mask));
    }

    void ComputeAvailabilityMap()
    {
        map = new bool[width,height];

        // Loop over all squares and check if they are available
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x,y] = TraceRays(x, y);
            }
        }
    }

    public bool IsCellAvailable(int x, int y)
    {
        // Return false if the location is out of bounds
        if (x >= width || y >= height || x < 0 || y < 0) return false;
        return map[x, y];
    }

    public bool IsCellAvailable(float x, float y)
    {
        return IsCellAvailable((int)x, (int)y);
    }

    public int GetMapWidth()
    {
        return width;
    }

    public int GetMapHeight()
    {
        return height;
    }

    public bool[,] GetAvailabilityMap()
    {
        return map;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
