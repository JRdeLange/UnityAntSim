using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Ant
{

    List<string> approach = new List<string>();
    List<string> avoid = new List<string>();
    List<string> flee = new List<string>();
    List<string> importanceOrder = new List<string>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Approach food
        approach.Add("Food");

        // Avoid similar ants
        avoid.Add(this.tag);
        
        // Flee from stuff 
        flee.Add("enemy"); // Still placeholder, change to relevant stuff later

        importanceOrder.Add("enemy");
        importanceOrder.Add(this.tag);
        importanceOrder.Add("Food");

    }

    RaycastHit FindMostImportantObject(List<RaycastHit> objectsInSightRays)
    {
        RaycastHit mostImportantInView = new RaycastHit();
        mostImportantInView.distance = Mathf.Infinity;
        float mostImportantIdx = Mathf.Infinity;
        foreach (RaycastHit hit in objectsInSightRays){
            float currentImportanceIdx = importanceOrder.IndexOf(hit.collider.gameObject.tag);
            if (currentImportanceIdx != -1 && currentImportanceIdx <= mostImportantIdx && hit.distance < mostImportantInView.distance){
                mostImportantInView = hit;
                mostImportantIdx = currentImportanceIdx;
            }
        }

        return mostImportantInView;
    }

    protected override void ParseSight(List<RaycastHit> objectsInSightRays)
    {
        RaycastHit ray = FindMostImportantObject(objectsInSightRays);
        // Check if a thing has been found
        if (ray.distance == Mathf.Infinity) return;

        GameObject gameObject = ray.collider.gameObject;

        print(gameObject);

        if (flee.Contains(gameObject.tag)){
            // Approach behavior
        } else if (avoid.Contains(gameObject.tag)){
            // Avoid behavior
        } else if (approach.Contains(gameObject.tag)){
            // Flee behavior
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
