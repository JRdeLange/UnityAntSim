using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneManager : MonoBehaviour
{

    public Ant antPrefab;
    int counter = 0;
    int antfreq = 6;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counter ++;
        if (counter >= antfreq){
            counter = 0;
            Instantiate(antPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
        }
    }
}
