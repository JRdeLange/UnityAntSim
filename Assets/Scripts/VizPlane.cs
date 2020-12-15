using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VizPlane : MonoBehaviour
{

    Texture2D texture;
    Floor floor;
    int sizeX;
    int sizeY;

    // Start is called before the first frame update
    void Start()
    {
        floor = GameObject.FindGameObjectWithTag("Floor").GetComponent<Floor>();
        transform.localScale = new Vector3(floor.transform.lossyScale.x, 1, floor.transform.lossyScale.z);
        transform.position = floor.transform.position + new Vector3(0, 0.001f, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        sizeX = (int)Mathf.Round(floor.transform.lossyScale.x * 10f);
        sizeY = (int)Mathf.Round(floor.transform.lossyScale.z * 10f);

        texture = new Texture2D(sizeX, sizeY);
        texture.filterMode = FilterMode.Bilinear;
        GetComponent<Renderer>().material.mainTexture = texture;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                texture.SetPixel(x, y, new Color(1, 0, 0, 0));
            }
        }
        texture.Apply();
        
    }

    public void ChangeTransparancy(int x, int y, float transparancy)
    {
        Color newColor = texture.GetPixel(x, y);
        newColor.a = transparancy;
        texture.SetPixel(x, y, newColor);
    }

    public void ApplyTextureChanges()
    {
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
