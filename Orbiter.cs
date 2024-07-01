using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public int maxSatellites = 8;
    public float orbitRadius = 2f;

    public float orbitSpeed = 5f;
    public float fireAdjust = 8f;
    public float adjustSpeed = 20f;

    public GameObject satellitePrefab;

    public int powerCost = 14;

    public string launchKey = "v";
    public string createKey = "b";

    public Transform orbitron;

    public AudioSource fireSound;
    public AudioSource createSound;

    private List<GameObject> satellites = new List<GameObject>();
    private List<float> timeParams = new List<float>();
    private float spaceBetweenSat;
    private int satToFire = 0;

    private VitalsManager power;

    void Start()
    {
        power = gameObject.GetComponent<VitalsManager>();
        CreateSatellite();

        orbitron.parent = null;
        orbitron.tag = gameObject.tag;
    }


    void Update()
    {
        orbitron.position = transform.position; 

        float speed = orbitSpeed;
        if(satToFire > 0)
        {
            speed = fireAdjust;
            if (satellites.Count < 1) satToFire = 0;
        }

        float[] newParams = new float[satellites.Count];
        for (int i = 1; i < satellites.Count; i++)
        {
            int previousIndex = i - 1;

            float speedToUse = speed;
            float distanceFPre = timeParams[i] - timeParams[previousIndex];
            if(distanceFPre < 0) 
            {
                distanceFPre += 2 * (float)(System.Math.PI);
            }
            if((float)System.Math.Round(distanceFPre, 1) != spaceBetweenSat) speedToUse = adjustSpeed;
  
            newParams[i] = timeParams[i] + Time.deltaTime * speedToUse; 
        }

        if(newParams.Length > 0)
        { 
            newParams[0] = timeParams[0] + Time.deltaTime * speed;
        }

        for (int i = 0; i < timeParams.Count; i++)
        {
            timeParams[i] = newParams[i] % (2 * (float)(System.Math.PI));
            satellites[i].transform.localPosition = new Vector3(orbitRadius * (float)System.Math.Cos(timeParams[i]), orbitRadius * (float)System.Math.Sin(timeParams[i]), 0);
        }

        if(satToFire > 0)
        {
            for(int i = 0; i < satellites.Count; i++)
            {
                if((float)System.Math.Round(timeParams[i], 1) == (float)System.Math.Round(((transform.eulerAngles.z + 90) % 360) * ((float)(System.Math.PI) / 180), 1))
                {
                    FireSatellite(satellites[i]);
                    satellites.Remove(satellites[i]);
                    timeParams.Remove(timeParams[i]);
                    //UpdateTargetPositions();
                }
            }
        }



        if(Input.GetKeyDown(launchKey) && satToFire < maxSatellites)
        {
            if(satToFire < satellites.Count) satToFire += 1;
        }
        if(Input.GetKeyDown(createKey) && satellites.Count < maxSatellites)
        {
            if(power.UsePower(powerCost)) CreateSatellite();
        }
    }

    public void RemoveSatellite(GameObject satellite)
    {
        for (int i = 0; i < satellites.Count; i++)
        {
            if(satellite == satellites[i]) 
            {
                satellites.Remove(satellites[i]);
                timeParams.Remove(timeParams[i]);
                //UpdateTargetPositions();
                return;
            }
        }
    }

    void FireSatellite(GameObject satellite)
    {
        satellite.transform.parent = null;
        satToFire--;

        satellite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z));
        satellite.GetComponent<PhaeramProjectile>().LaunchProjectile();
        fireSound.Play();

        //UpdateTargetPositions();
    }

    void CreateSatellite()
    {
        GameObject newSat = Instantiate(satellitePrefab, new Vector3(orbitron.position.x, orbitron.position.y + orbitRadius, 0), Quaternion.identity, orbitron);
        newSat.GetComponent<PhaeramProjectile>().orbitManager = this;
        newSat.tag = gameObject.tag;
        satellites.Add(newSat);
        timeParams.Add(0f);

        createSound.Play();

        UpdateTargetPositions();
    }

    void UpdateTargetPositions()
    {
        if(satellites.Count != 0)
        {
            spaceBetweenSat = (float)System.Math.Round((2f * (float)System.Math.PI) / satellites.Count, 1);
        }
        else
        {
            spaceBetweenSat = 0;
        }
    }
}
