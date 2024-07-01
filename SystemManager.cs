using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    
    public Transform[] planets;
    private float[] timeParams;
    public float[] radii;
    public float[] orbitSpeeds;

    [Header("Orbit Outlines")]
    public int subdivisions = 25;
    public float lineWidth = 0.05f;
    public int lineSortingOrder = -3;

    public float redRange, greenRange, blueRange;

    public Material lineMaterial;

    void Start()
    {
        timeParams = new float[planets.Length];
        for (int i = 0; i < timeParams.Length; i++)
        {
            timeParams[i] = Random.Range(0, 2 * Mathf.PI);
        }

        for (int i = 0; i < planets.Length; i++)
        {

            LineRenderer orbitOutline = planets[i].gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;

            orbitOutline.startWidth = lineWidth;
            orbitOutline.material = lineMaterial;
            orbitOutline.sortingOrder = lineSortingOrder;
            orbitOutline.textureMode = LineTextureMode.Tile;
            Color outlineColor;
            if (radii[i] < redRange) { outlineColor = Color.red; }
            else if(radii[i] < greenRange) { outlineColor = new Color(1 - ((radii[i] - redRange) / (greenRange - redRange)), (radii[i] - redRange) / (greenRange - redRange), 0); }
            else { outlineColor = new Color(0, 1 - ((radii[i] - greenRange) / (blueRange - greenRange)), (radii[i] - greenRange) / (blueRange - greenRange)); }
            orbitOutline.startColor = outlineColor;
            orbitOutline.endColor = outlineColor;

            orbitOutline.positionCount = subdivisions + 1;
            for (int j = 0; j < subdivisions; j++)
            {
                orbitOutline.SetPosition(j, new Vector3(radii[i] * Mathf.Cos(2 * j * Mathf.PI / subdivisions), radii[i] * Mathf.Sin(2 * j * Mathf.PI / subdivisions), planets[i].position.z));
            }
            orbitOutline.SetPosition(subdivisions, new Vector3(radii[i],0,planets[i].position.z));
        }
    }

    void Update()
    {
        for (int i = 0; i < timeParams.Length; i++)
        {
            timeParams[i] = (timeParams[i] + Time.deltaTime * orbitSpeeds[i]) % (2 * (float)(System.Math.PI));
            planets[i].localPosition = new Vector3(radii[i] * (float)System.Math.Cos(timeParams[i]), radii[i] * (float)System.Math.Sin(timeParams[i]), planets[i].position.z);
        }
    }
}
