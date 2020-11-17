using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{

    // Movement variables
    float speed = 3;
    Vector3 direction = Vector3.forward;
    float wiggleSpeed = 1;
    float wiggleAngle = 90;
    float newMovementAngle = 0;

    // Start is called before the first frame update
    void Start()
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
        direction = Quaternion.Euler(0, directionRotation, 0) * direction;

        // Update how far we still have to rotate
        newMovementAngle -= directionRotation;

        print(newMovementAngle);

    }

    void Move()
    {
        Wiggle();
        transform.position += speed * direction * Time.deltaTime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
    }
}
