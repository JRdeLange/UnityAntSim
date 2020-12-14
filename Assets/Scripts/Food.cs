using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    float respawnChance = 1;
    float invokeRepeatingInterval = 1;
    List<Vector3> directions = new List<Vector3>();
    List<float> rayLengths = new List<float>();

    
    float squareRootOfTwo = Mathf.Sqrt(2);

    // If false only spawn food in the 4 cardinal directions, otherwise all 8
    bool spawnFoodAtAllEightDirs = true;

    LayerMask mask;

    public Food food;

    // Start is called before the first frame update
    void Start()
    {
        // Generate 8 directions, multiply the corners by sqrt(2) because of pythagoras
        InitializeDirections();

        mask = LayerMask.GetMask("Barrier", "Food");
        // Have a random starting offset to make it so that not all food grows at the same time.
        float startingOffset = Random.Range(invokeRepeatingInterval, 2 * invokeRepeatingInterval);
        InvokeRepeating("SpawnFood", startingOffset, startingOffset);
    }

    // Check if we even spawn food this iteration
    bool SpawnFoodThisIteration()
    {
        return Random.Range(0.0f, 1.0f) < respawnChance;
    }

    public void PickUp()
    {
        Destroy(gameObject);
    }

    // Finds a free square if there is one around the food
    Vector3 FindFreeSquare()
    {
        // Starting the iteration of the list of directions at a random place is random enough for us
        int randomOffset = Random.Range(0, directions.Count - 1);
        for (int i = randomOffset; i < directions.Count + randomOffset; ++ i)
        {
            // Calculate an index that is within bounds
            int idx = i % directions.Count;
            // If a direction is clear we can spawn food here
            if(! Physics.Raycast(transform.position, directions[idx], rayLengths[idx], mask))
            {
                return directions[idx];
            }
        }
        return Vector3.zero;
    }

    void SpawnFood()
    {
        // If we spawn food
        if (SpawnFoodThisIteration())
        {
            // Try to find an available position
            Vector3 newPos = FindFreeSquare();
            if (newPos == Vector3.zero) return;
            Instantiate(food, transform.position + newPos, Quaternion.identity);
        }
    }

    // Initializes the directions and raylengths lists
    void InitializeDirections()
    // Sorry for all the nested loops and checks but I assure you they are all important and result in readable code (I swear)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if(!(i == 0 && j == 0))
                {
                    if (i != 0 && j != 0)
                    {
                        // Corners
                        if (spawnFoodAtAllEightDirs)
                        {
                            rayLengths.Add(squareRootOfTwo+0.1f);
                            directions.Add(new Vector3(i, 0, j));
                        }
                    } else
                    {
                        // Non-corners
                        rayLengths.Add(1.1f);
                        directions.Add(new Vector3(i, 0, j));
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
