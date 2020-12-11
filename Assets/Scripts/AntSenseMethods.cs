using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSenseMethods : MonoBehaviour
{
    public static List<Vector3> GenerateSenseRayDirections(float coneWidth, float coneRadius, float coneRayInterval)
    {
        // Divide cone width by 2 to make it a symmetrical problem
        coneWidth = coneWidth/2;
        // The list that will contain all the directions
        List<Vector3> rayDirections = new List<Vector3>();

        // Add the center ray
        Vector3 lastDirection = Vector3.forward;
        rayDirections.Add(lastDirection);
        // Add the rest
        while (coneWidth > 0)
        {
            // Calculate new ray direction
            lastDirection = Quaternion.Euler(0, Mathf.Min(coneWidth, coneRayInterval), 0) * lastDirection;
            lastDirection.Normalize();
            // Update how many degrees we still have to go
            coneWidth -= coneRayInterval;

            // Add both the positive and negative directions
            // Simply add the positive
            rayDirections.Add(lastDirection);
            // Rotate around the forward direction to obtain the negative
            rayDirections.Add(Quaternion.Euler(0, 0, 180) * lastDirection);
        }

        return rayDirections;
    }

    static float CalculateConeRayInterval(float coneWidth, float coneRadius, float smallestToBeSensedObjectWidth)
    {
        // Prevent division by zero
        if (coneRadius < 0.05f) coneRadius = 0.05f;
        // Prevent an infinite amount of rays
        if (smallestToBeSensedObjectWidth < 0.05f) smallestToBeSensedObjectWidth = 0.05f;
        // Some math that makes sure that we are using the minimal amount of rays possible
        // while not missing any objects larger than smallestToBeSensedObjectWidth (TOA van soscastoa)
        return Mathf.Atan(smallestToBeSensedObjectWidth/coneRadius) * Mathf.Rad2Deg;
    }

    public static List<RaycastHit> GetObjectsInVision(Transform antTransform, Vector3 antDirection, float coneWidth, 
                                                      float coneRadius, float smallestToBeSensedObjectWidth)
    {
        float coneRayInterval = CalculateConeRayInterval(coneWidth, coneRadius, smallestToBeSensedObjectWidth);
        List<Vector3> rayDirections = AntSenseMethods.GenerateSenseRayDirections(coneWidth, coneRadius, coneRayInterval);
        // List to put all of the objects in sight in
        List<RaycastHit> objectsInSightRays = new List<RaycastHit>();
        List<GameObject> addedGameObjects = new List<GameObject>();
        // Loop over all the sight rays       
        foreach (Vector3 ray in rayDirections)
        {
            // Rotate the rays to face the ant's direction
            float directionAngle = Vector3.SignedAngle(Vector3.forward, antDirection, Vector3.up);
            Vector3 rotatedRay = Quaternion.Euler(0, directionAngle, 0) * ray;
            rotatedRay.Normalize();

            // Makes the lines visible for debug purposes
            Debug.DrawLine(antTransform.position, antTransform.position + (rotatedRay * coneRadius), Color.white);

            // Get the objects in view by casting the rays
            RaycastHit[] hits;
            hits = Physics.RaycastAll(antTransform.position, rotatedRay, coneRadius);

            foreach (var hit in hits)
            {
                // If the seen Gameobject is not yet in the list or if it is a shorter ray to one already in the list

                int gameObjectidx = addedGameObjects.IndexOf(hit.collider.gameObject);
                if (gameObjectidx == -1)
                {
                    // Add it
                    objectsInSightRays.Add(hit);
                    addedGameObjects.Add(hit.collider.gameObject);
                } else if (objectsInSightRays[gameObjectidx].distance > hit.distance)
                {
                    // Add it
                    objectsInSightRays.RemoveAt(gameObjectidx);
                    addedGameObjects.RemoveAt(gameObjectidx);
                    objectsInSightRays.Add(hit);
                    addedGameObjects.Add(hit.collider.gameObject);
                }
            }

            // Debug printing
            //foreach (var item in objectsInSight){
            //    print(item);
            //}
        }
        
        return objectsInSightRays;
    }
}
