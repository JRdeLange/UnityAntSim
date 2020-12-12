using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Ant
{

    List<string> approach = new List<string>();
    List<string> avoid = new List<string>();
    List<string> cannotIntersect = new List<string>();
    List<string> flee = new List<string>();
    //List<string[]> importanceOrder = new List<string[]>();
    Dictionary<string, int> importanceOrder = new Dictionary<string, int>();
    Dictionary<string, System.Action<RaycastHit>> functionMapping = new Dictionary<string, System.Action<RaycastHit>>();
    float avoidThreshold = 1.5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Approach food
        approach.Add("Food");

        // Avoid similar ants
        avoid.Add(this.tag);

        // Make sure to not intersect barriers
        cannotIntersect.Add("Barrier");
        
        // Flee from stuff 
        flee.Add("Enemy"); // Still placeholder, change to relevant stuff later

        int currentImportance = 1;
        importanceOrder.Add("Enemy", currentImportance);
        importanceOrder.Add("Barrier", currentImportance);
        currentImportance++;
        importanceOrder.Add(this.tag, currentImportance);
        currentImportance++;
        importanceOrder.Add("Food", currentImportance);

        functionMapping.Add("Barrier", CannotIntersectBehavior);
        functionMapping.Add(this.tag, AvoidBehavior);

    }

    void AvoidBehavior(RaycastHit hit)
    {
            if (hit.distance < avoidThreshold)
            {
                IntelligentSteerAway(hit, false);
            }
    }

    void ApproachBehavior(RaycastHit hit)
    {

    }

    void CannotIntersectBehavior(RaycastHit hit)
    {
        if (hit.distance < avoidThreshold)
            {
                IntelligentSteerAway(hit, true);
            }
    }

    void FleeBehavior(RaycastHit hit)
    {

    }

    protected override void ParseSight(List<RaycastHit> objectsInSightRays)
    {
        RaycastHit hit = FindMostImportantObject(objectsInSightRays, importanceOrder);
        // Check if a thing has been found
        if (hit.distance == Mathf.Infinity) return;

        GameObject gameObject = hit.collider.gameObject;

        functionMapping[gameObject.tag].DynamicInvoke(hit);

        /**
        if (flee.Contains(gameObject.tag))
        {
            // Flee behavior
        } else if (avoid.Contains(gameObject.tag))
        {
            if (ray.distance < avoidThreshold)
            {
                IntelligentSteerAway(ray, false);
            }
        } else if (cannotIntersect.Contains(gameObject.tag))
        {
            if (ray.distance < avoidThreshold)
            {
                IntelligentSteerAway(ray, true);
            }
        } else if (approach.Contains(gameObject.tag))
        {
            // Approach behavior
        }
        **/

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
