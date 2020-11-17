using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSenses : MonoBehaviour
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
        while (coneWidth > 0){
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

        foreach (var item in rayDirections)
        {
            print(item);
        }

        return rayDirections;
    }
}
