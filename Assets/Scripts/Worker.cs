﻿using System.Collections;
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
    float foodPickupThreshold = 1.0f;
    int amountOfCarriedFood = 0;
    float maxCarriedFood = Mathf.Infinity;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        getObjectsInVisionMask = LayerMask.GetMask("Food", "Barrier", "Ant");

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
        importanceOrder.Add("Food", currentImportance);
        currentImportance++;
        importanceOrder.Add(this.tag, currentImportance);
        currentImportance++;
        

        functionMapping.Add("Barrier", CannotIntersectBehavior);
        functionMapping.Add(this.tag, AvoidBehavior);
        functionMapping.Add("Food", ApproachBehavior);

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
        Vector3 antToHit = (hit.point - transform.position).normalized;
        newMovementAngle = AntSenseMethods.VectorToDirectionAngle(transform, antToHit);
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

    void PickUpFood(RaycastHit hit)
    {
        hit.collider.gameObject.GetComponent<Food>().PickUp();
        amountOfCarriedFood++;
    }

    bool SpecialConditions(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag == "Food" && hit.distance < foodPickupThreshold
            && amountOfCarriedFood < maxCarriedFood)
        {
            PickUpFood(hit);
            return true;
        }
        return false;
    }

    protected override void ParseSight(List<RaycastHit> objectsInSightRays)
    {
        RaycastHit hit = FindMostImportantObject(objectsInSightRays, importanceOrder);
        // Check if a thing has been found
        if (hit.distance == Mathf.Infinity) return;

        //Debug.DrawLine(transform.position, hit.point, Color.red);

        GameObject gameObject = hit.collider.gameObject;

        if (!SpecialConditions(hit))
        {
            functionMapping[gameObject.tag].DynamicInvoke(hit);
        }

        

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
