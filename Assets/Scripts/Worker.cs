using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Ant
{

    List<string> approach = new List<string>();
    List<string> avoid = new List<string>();
    List<string> flee = new List<string>();
    List<string> importanceOrder = new List<string>();
    float avoidThreshold = 1.5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Approach food
        approach.Add("Food");

        // Avoid similar ants
        avoid.Add(this.tag);
        avoid.Add("Barrier");
        
        // Flee from stuff 
        flee.Add("Enemy"); // Still placeholder, change to relevant stuff later

        importanceOrder.Add("Enemy");
        importanceOrder.Add("Barrier");
        importanceOrder.Add(this.tag);
        importanceOrder.Add("Food");

    }

    protected override void ParseSight(List<RaycastHit> objectsInSightRays)
    {
        RaycastHit ray = FindMostImportantObject(objectsInSightRays, importanceOrder);
        // Check if a thing has been found
        if (ray.distance == Mathf.Infinity) return;

        GameObject gameObject = ray.collider.gameObject;

        print(gameObject);

        if (flee.Contains(gameObject.tag))
        {
            // Approach behavior
        } else if (avoid.Contains(gameObject.tag))
        {
            if (ray.distance < avoidThreshold)
            {
                IntelligentSteerAway(ray);
            }
        } else if (approach.Contains(gameObject.tag))
        {
            // Flee behavior
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
