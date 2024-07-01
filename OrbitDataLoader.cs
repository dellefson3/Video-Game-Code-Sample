using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrbitDataLoader : MonoBehaviour
{
    public bool debugOn = false;
    public static PlanetData mainPlanet;
    public PlanetData planetData;
    public EnterOrbitAnim orbitAnimation;
    public MeshRenderer planetMesh;
    public Transform planetTransform;

    public GameObject planetUI;
    public GameObject planetSelectUI;

    public Image surfacePreviewRenderer;
    public LaunchController launchControl;

    public GameObject backToOrbitButton;
    public GameObject iconPrefab;
    public GameObject previewPrefab;

    public TMP_Text nameText, windText, temperatureText, weatherText, pressureText, radiusText, massText, gravityText, tiltText, yearText, dayText;

    void Start()
    {
        if(debugOn) mainPlanet = planetData;
        planetMesh.material = mainPlanet.planetPreviewMat;
        planetTransform.eulerAngles = new Vector3(0, 0, mainPlanet.tilt);
        if (mainPlanet.orbitData.Length < 1)
        {
            backToOrbitButton.SetActive(false);
            SelectPlanet(new Vector3(), mainPlanet);
        }
        else
        {
            planetTransform.gameObject.GetComponent<PlanetSelect>().data = mainPlanet;
            int i = 0;
            foreach (PlanetData orbitingObject in mainPlanet.orbitData)
            {
                float randomTime = Random.Range(0, 2 * (float)(System.Math.PI));
                if (i > 3)
                {
                    while((randomTime > 0.785 && randomTime < 2.356) || (randomTime > 3.927 && randomTime < 5.498))//makes sure the planet is on the wider portion of the screen
                    {
                        randomTime = Random.Range(0, 2 * (float)(System.Math.PI));
                    }
                }
                GameObject newObject = Instantiate(previewPrefab, new Vector3((5 + 3 * i) * (float)System.Math.Cos(randomTime), (5 + 3 * i) * (float)System.Math.Sin(randomTime), planetTransform.position.z), Quaternion.identity);
                float radiusScaler = orbitingObject.radius / mainPlanet.radius;
                if (radiusScaler < 0.15f) radiusScaler = 0.15f;
                newObject.transform.localScale *= radiusScaler;
                newObject.transform.eulerAngles = new Vector3(0, 0, orbitingObject.tilt);
                newObject.GetComponent<MeshRenderer>().material = orbitingObject.planetPreviewMat;
                newObject.GetComponent<PlanetSelect>().data = orbitingObject;

                i++;
            }
        }

    }
    public void SelectPlanet(Vector3 planetPosition, PlanetData planetToLoad)
    {
        planetData = planetToLoad;
        LoadPreloadData();
        orbitAnimation.StartAnimation(planetPosition.x, planetPosition.y);
    }
    public void LoadPreloadData()
    {
        surfacePreviewRenderer.sprite = planetData.surfaceTexture;
        for (int i = 0; i < planetData.planetEntities.Length; i++)
        {
            Image newIconRenderer = Instantiate(iconPrefab, new Vector3(), Quaternion.identity, surfacePreviewRenderer.gameObject.transform).GetComponent<Image>();
            newIconRenderer.gameObject.transform.localPosition = launchControl.CalculateIconPlacement(planetData.entityPositions[i]);
            newIconRenderer.sprite = planetData.planetEntities[i].scannerIcon;
            newIconRenderer.color = planetData.planetEntities[i].scanColor;
            if(planetData.planetEntities[i].hazard || planetData.planetEntities[i].life) newIconRenderer.gameObject.transform.localScale *= planetData.planetEntities[i].size;
        }
        planetUI.SetActive(true);
        planetSelectUI.SetActive(false);
        nameText.text = planetData.planetName;
    }
    public void LoadData()
    {
        windText.text += planetData.windSpeed + " km/h";
        temperatureText.text += planetData.temp + " °C";
        weatherText.text += planetData.weatherType;
        pressureText.text += planetData.pressure + " atm";
        radiusText.text += planetData.radius + " km";
        float mass = planetData.mass;
        int expPower = 24;
        bool numberFormated = false;
        while (!numberFormated)
        {
            if (mass >= 10)
            {
                mass /= 10;
                expPower++;
            }
            else if (mass < 1)
            {
                mass *= 10;
                expPower--;
            }
            else {
                numberFormated = true;
            }
        }
        massText.text += mass + "e" + expPower + " kg";
        gravityText.text += planetData.surfaceGravity + " g";
        tiltText.text += planetData.tilt + "°";
        if (planetData.year == 1) yearText.text += planetData.year + " year";
        else yearText.text += planetData.year + " years";
        if (planetData.day == 1) dayText.text += planetData.day + " day";
        else dayText.text += planetData.day + " days";
    }

    private void UnloadData()
    {
        windText.text = "Wind Speed: ";
        temperatureText.text = "Temperature: ";
        weatherText.text = "Weather Type: ";
        pressureText.text = "Atmospheric Pressure: ";
        radiusText.text = "Radius: ";
        massText.text = "Mass: ";
        gravityText.text = "Surface Gravity: ";
        tiltText.text = "Tilt: ";
        yearText.text = "Year: ";
        dayText.text = "Day: ";
    }

    public void BackToOrbit()
    {
        UnloadData();
        planetUI.SetActive(false);
        orbitAnimation.reseting = true;
    }
    public void AllowPlanetSelection()
    {
        planetSelectUI.SetActive(true);
    }
}
