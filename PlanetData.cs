using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="new planet", menuName="Planet Data")]
public class PlanetData : ScriptableObject
{
    [Header("Extra Info")]
    public string planetName;//in words
    public float pressure;// in atm
    public float radius;//in km
    public float mass;//in yottagrams (kg * 10^24)
    public float surfaceGravity;//in gs
    public float tilt;//degrees
    public float year;//earth years
    public float day;//earth days


    [Header("Relevant Info")]
    public float windSpeed;//km/h
    public string weatherType;
    public float temp;//celsius

    [Header("Materials")]
    public Material planetPreviewMat;

    [Header("Orbiting Bodies")]
    public PlanetData[] orbitData;

    [Header("Surface Info")]
    public Sprite surfaceTexture;
    public PlanetEntity[] planetEntities;
    public Vector3[] entityPositions;
}
