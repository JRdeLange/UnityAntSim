using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    // All the variables
    // // // Ant.cs
    public float speed = 5;
    public float wiggleSpeed = 360;
    public float wiggleAngle = 20;

    // Sense variables
    public float coneWidth = 120;
    public float coneRadius = 4;
    public float smallestToBeSensedObjectWidth = 1;
    public LayerMask getObjectsInVisionMask;

    // Intelligent steer away variables
    public float ISAconeWidth = 360;
    public float ISAconeRadius = 2f;
    public float ISAconeInterval = 30;
    public bool visualizeSight = true;

    public float pheromoneSpeed = 9;

    // // // Worker.cs
    public float avoidThreshold = 1.5f;
    public float foodPickupThreshold = 1.0f;
    public int amountOfCarriedFood = 0;
    public float maxCarriedFood = Mathf.Infinity;

    // // // PheromoneManager.cs
    public float pheromoneCap = 100;
    public float scanTimeInterval = .1f;
    public float senseThreshold = .00001f;
    public float evaporationFactor = .9f;
    public float diffuseFactor = .09f;

    // // // VizPlane.cs
    public Color color;

    // // // Food.cs
    public float respawnChance = 1;
    public float invokeRepeatingInterval = 1;
    public bool spawnFoodAtAllEightDirs = true;
    public float offset1 = 1f / 18f;
    public float offset2 = 1f / 1f;


    // Start is called before the first frame update
    void Awake() {
        color = new Color(1, 0.3f, 1, 0);
        getObjectsInVisionMask = LayerMask.GetMask("Food", "Barrier", "Ant");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
