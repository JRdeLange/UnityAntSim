using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    // Movement variables
    float speed = 3;
    Vector3 direction = Vector3.forward;
    float wiggleSpeed = 1;
    float wiggleAngle = 45;
    float newMovementAngle;

    // Sense variables
    float coneWidth = 120;
    float coneRadius = 5;
    float smallestToBeSensedObjectWidth = 1;

    List<Collision> newCollisionsThisFrame = new List<Collision>();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
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