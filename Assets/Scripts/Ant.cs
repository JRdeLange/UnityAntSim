using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
	// Add Pheromone Manager
	public PheromoneManager pheromoneManager;
	
    // Movement variables
    float speed = 4;
    Vector3 direction = Vector3.forward;
    float wiggleSpeed = 2;
    float wiggleAngle = 15;
    float newMovementAngle;
    bool stopped = false;

    // Sense variables
    float coneWidth = 120;
    float coneRadius = 5;
    float smallestToBeSensedObjectWidth = 1;

    // Intelligent steer away variables
    float ISAconeWidth = 360;
    float ISAconeRadius = 3;
    float ISAconeInterval = 10;
    float ISArayLength = 3;

    List<Collision> newCollisionsThisFrame = new List<Collision>();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    protected void IntelligentSteerAway(RaycastHit hit)
    {
        List<Vector3> possibleAngles = AntSenseMethods.GenerateRayDirections(ISAconeWidth, ISAconeRadius, ISAconeInterval);
        foreach (Vector3 dir in possibleAngles)
        {
            
        }
    }

    // Steers away from the hit object by 90 degrees in the direction we are heading
    protected void SteerAway(RaycastHit hit)
    {
        // Find the angle relative to our forward angle
        Vector3 antToHit = (hit.point - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, antToHit);
        if (Vector3.Dot(antToHit, transform.right) < 0){
            angle *= -1;
        }

        //print(angle);
        Vector3 newDir = Vector3.forward;
        if (angle < 0) newDir = Quaternion.Euler(0,270,0) * antToHit;
        if (angle >= 0) newDir = Quaternion.Euler(0,90,0) * antToHit;

        //Vector3 newDir = Quaternion.Euler(0,90,0) * antToHit;
        angle = Vector3.Angle(transform.forward, newDir);
        if (Vector3.Dot(newDir, transform.right) > 0){
            angle *= -1;
        }

        print(angle);
        newMovementAngle = angle;

        Debug.DrawLine(transform.position, hit.point);
    }

    // Wiggle by choosing a new direction and turning by wigglespeed towards that direction
    // If the direction is achieved, simply choose a new direction.
    void Wiggle()
    {
        if (newMovementAngle == 0)
        {
            newMovementAngle = Random.Range(-wiggleAngle, wiggleAngle);
        }

        // The amount we need to turn this frame
        float directionRotation = 0;
        if (newMovementAngle > 0)
        {
            directionRotation = Mathf.Min(wiggleSpeed, newMovementAngle);
        } else 
        {
            directionRotation = Mathf.Max(-wiggleSpeed, newMovementAngle);
        }

        // Rotate
        transform.Rotate(0, directionRotation, 0);

        // Update how far we still have to rotate
        newMovementAngle -= directionRotation;
    }

    protected RaycastHit FindMostImportantObject(List<RaycastHit> objectsInSightRays, List<string> importanceOrder)
    {
        RaycastHit mostImportantInView = new RaycastHit();
        mostImportantInView.distance = Mathf.Infinity;
        float mostImportantIdx = Mathf.Infinity;
        foreach (RaycastHit hit in objectsInSightRays)
        {
            float currentImportanceIdx = importanceOrder.IndexOf(hit.collider.gameObject.tag);
            if (currentImportanceIdx != -1 && (currentImportanceIdx < mostImportantIdx || 
                (currentImportanceIdx == mostImportantIdx && hit.distance < mostImportantInView.distance)))
            {
                mostImportantInView = hit;
                mostImportantIdx = currentImportanceIdx;
            }
        }

        return mostImportantInView;
    }

    protected virtual void ParseSight(List<RaycastHit> objectsInSightRays)
    {
        print("This should not be called, the subclass should handle this");
    }

    void See()
    {
        // List to put all of the objects in sight in
        List<RaycastHit> objectsInSightRays = AntSenseMethods.GetObjectsInVision(transform, transform.forward, coneWidth, 
                                                                             coneRadius, smallestToBeSensedObjectWidth);
        
        ParseSight(objectsInSightRays);
    }

    void Move()
    {
        if (stopped) return;

        Wiggle();
        transform.position += speed * transform.forward * Time.deltaTime;
    }

    //Move towards a specifik direction
    void MoveInDirection(float xDir, float zDir)
    {

    }

    // Get the direction with the highest pheromone concentration and move towards it
    void FollowPheromone()
    {
        // Get the direction with the highest concentration
        float[] direction;
        direction = pheromoneManager.GetDirectionHighestConcentration((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

        // Move in that direction
        MoveInDirection(direction[0], direction[1]);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        See();
        Move();
    }
}

/** Methods below here are from failed attempts but they could prove useful in the future

    void SteerAway()
    {
        // Find the most important collision (future example: walls are more important than ants)
        // We simply take the first ant for now

        // This bit is a bit jank but otherwise I run into complaints from the compiler
        Collision collision = newCollisionsThisFrame[0];
        bool foundOne = false;
        foreach (Collision testCollision in newCollisionsThisFrame){
            if (testCollision.gameObject.tag == "RedWorkerAnt"){
                collision = testCollision;
                foundOne = true;
                break;
            }
        }
        if (!foundOne) return;
        
        // Find the angle relative to our forward angle
        Vector3 antToCollider = (collision.gameObject.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, antToCollider);
        if (Vector3.Dot(antToCollider, transform.right) < 0){
            //angle *= -1;
        }
        print(antToCollider * 2);
        print(collision.gameObject);
        Debug.DrawLine(transform.position, transform.position + antToCollider * 2);
    }

    void OnCollisionEnter(Collision other) {
        newCollisionsThisFrame.Add(other);
        print("hit something");
    }

    void Feel()
    {
        foreach (Collision collision in newCollisionsThisFrame){
            print(collision);
        }
        print("nextFrame");
    }

**/