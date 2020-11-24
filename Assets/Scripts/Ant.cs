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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Wiggle by choosing a new direction and turning by wigglespeed towards that direction
    // If the direction is achieved, simply choose a new direction.
    void Wiggle()
    {
        if (newMovementAngle == 0){
            newMovementAngle = Random.Range(-wiggleAngle, wiggleAngle);
        }

        // The amount we need to turn this frame
        float directionRotation = 0;
        if (newMovementAngle > 0){
            directionRotation = Mathf.Min(wiggleSpeed, newMovementAngle);
        } else {
            directionRotation = Mathf.Max(-wiggleSpeed, newMovementAngle);
        }

        // Rotate
        transform.Rotate(0, directionRotation, 0);

        // Update how far we still have to rotate
        newMovementAngle -= directionRotation;
    }

    void Move()
    {
        Wiggle();
        transform.position += speed * transform.forward * Time.deltaTime;
    }

    void See()
    {
        // List to put all of the objects in sight in
        List<GameObject> objectsInSight = AntSenseMethods.GetObjectsInVision(transform, transform.forward, coneWidth, 
                                                                             coneRadius, smallestToBeSensedObjectWidth);
    }

    // Update is called once per frame
    protected virtual void Update()
    {        
        See();
        Move();
    }
}
