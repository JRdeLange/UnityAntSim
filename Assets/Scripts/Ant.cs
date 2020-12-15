using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
	// Add Pheromone Manager
	protected PheromoneManager pheromoneManager;
	
    // Movement variables
    float speed = 5;
    Vector3 direction = Vector3.forward;
    float wiggleSpeed = 360;
    float wiggleAngle = 20;
    protected float newMovementAngle;
    bool stopped = false;

    // Sense variables
    float coneWidth = 120;
    float coneRadius = 4;
    float smallestToBeSensedObjectWidth = 1;
    protected LayerMask getObjectsInVisionMask;

    // Intelligent steer away variables
    float ISAconeWidth = 360;
    float ISAconeRadius = 2f;
    float ISAconeInterval = 30;

    List<Collision> newCollisionsThisFrame = new List<Collision>();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        pheromoneManager = GameObject.FindGameObjectWithTag("PheromoneManager").GetComponent<PheromoneManager>();
    }

    // Cast rays in order to find a clear direction
    Vector3 FindClearDirection(List<Vector3> possibleAngles, Vector3 rotationAngle)
    {
        LayerMask mask = LayerMask.GetMask("Barrier");
        foreach (Vector3 dir in possibleAngles)
        {
            float directionAngle = Vector3.SignedAngle(Vector3.forward, rotationAngle, Vector3.up);
            Vector3 rotatedDir = Quaternion.Euler(0, directionAngle, 0) * dir;
            rotatedDir.Normalize();

            if (! Physics.Raycast(transform.position, rotatedDir, ISAconeRadius, mask))
            {
                return rotatedDir;
            }
            //Debug.DrawLine(transform.position, transform.position + rotatedDir * ISAconeRadius, Color.gray);
        }
        return Vector3.zero;
    }

    // Get the direction with the highest pheromone concentration and set the current movement angle towards it
    protected void FollowPheromone()
    {
        // Get the direction with the highest concentration
        float[] direction;
        direction = pheromoneManager.GetDirectionHighestConcentration((int)(transform.position.x), (int)(transform.position.z));
        Vector3 directionVector = new Vector3(direction[0], 0, direction[1]);

        // Move in that direction
        float angle = AntSenseMethods.VectorToDirectionAngle(transform, directionVector);
        newMovementAngle = angle;
    }

    protected void IntelligentSteerAway(RaycastHit hit, bool cannotIntersect)
    {
        // Get all the possible angles
        List<Vector3> possibleAngles = AntSenseMethods.GenerateRayDirections(ISAconeWidth, ISAconeInterval);
        // Find a clear one
        // Start by finding the vector in a right angle with the ray we hit the object with
        Vector3 antToHit = (hit.point - transform.position).normalized;
        float worldAngle = Vector3.Angle(transform.forward, antToHit);
        if (Vector3.Dot(antToHit, transform.right) > 0){
            worldAngle *= -1;
        }
        Vector3 localRotationAngle = Vector3.forward;
        if (worldAngle < 0) localRotationAngle = Quaternion.Euler(0,270,0) * antToHit;
        if (worldAngle >= 0) localRotationAngle = Quaternion.Euler(0,90,0) * antToHit;
        localRotationAngle.Normalize();

        Vector3 clearAngle = FindClearDirection(possibleAngles, localRotationAngle);
        // If there is no clear angle, stop moving
        if (clearAngle == Vector3.zero)
        {
            stopped = true;
            return;
        }
        stopped = false;

        if (cannotIntersect && hit.distance < 1.0F){
            transform.forward = clearAngle;
        }

        float angle = AntSenseMethods.VectorToDirectionAngle(transform, clearAngle);
        newMovementAngle = angle;
        //transform.forward = clearAngle;

        //Debug.DrawLine(transform.position, transform.position + clearAngle * ISAconeRadius);
        //Debug.DrawLine(transform.position, hit.point, Color.red);

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
        float turnSpeed = wiggleSpeed * Time.deltaTime;
        if (newMovementAngle > 0)
        {
            directionRotation = Mathf.Min(turnSpeed, newMovementAngle);
        } else 
        {
            directionRotation = Mathf.Max(-turnSpeed, newMovementAngle);
        }

        // Rotate
        transform.Rotate(0, directionRotation, 0);

        // Update how far we still have to rotate
        newMovementAngle -= directionRotation;
    }

    protected RaycastHit FindMostImportantObject(List<RaycastHit> objectsInSightRays, Dictionary<string, int> importanceOrder)
    {
        RaycastHit mostImportantInView = new RaycastHit();
        mostImportantInView.distance = Mathf.Infinity;
        float mostImportantIdx = Mathf.Infinity;
        foreach (RaycastHit hit in objectsInSightRays)
        {
            int currentImportanceIdx;
            importanceOrder.TryGetValue(hit.collider.gameObject.tag, out currentImportanceIdx);
            if (currentImportanceIdx != 0 && (currentImportanceIdx < mostImportantIdx || 
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
                                                                             coneRadius, smallestToBeSensedObjectWidth, 
                                                                             getObjectsInVisionMask);
        
        ParseSight(objectsInSightRays);
    }

    void Move()
    {
        if (stopped) return;

        Wiggle();
        transform.position += speed * transform.forward * Time.deltaTime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        See();
        Move();
        
    }
}