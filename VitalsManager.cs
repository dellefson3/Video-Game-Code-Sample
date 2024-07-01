using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalsManager : MonoBehaviour
{
    [Header("Max/Set Values")]
    public int maxHealth = 10;
    public int maxPower = 10;

    public int powerRegenPerTick = 1;
    public float powerTickSpeed = 0.2f;

    private SliderManager sliders;

    [Header("Current Values")]
    public int health = 1;
    public int power = 0;

    [Header("Destruction Sequence")]

    public AudioSource winningTheme;

    public float deathTime = 0.6f;
    public GameObject deathEffect;

    public MonoBehaviour[] disableItems;
    public Transform[] deathPoints;
    private BattleManager bManager;

    void Start()
    {
        bManager = GameObject.Find("Main Camera").GetComponent<BattleManager>();
        bManager.AssignTarget(gameObject);

        sliders = GameObject.Find(gameObject.tag + "Slides").GetComponent<SliderManager>();

        health = maxHealth;
        power = maxPower;

        sliders.SetMaxHealth(maxHealth);
        sliders.SetMaxPower(maxPower);
        sliders.SetHealth(maxHealth);
        sliders.SetPower(maxPower);

        InvokeRepeating("RegenPower", powerTickSpeed, powerTickSpeed);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health < 0) health = 0;
        sliders.SetHealth(health);
        if(health < 1)
        {
            BlowUp();
        }
    }

    public void BlowUp()
    {
        foreach (MonoBehaviour item in disableItems)
        {
            item.enabled = false;
        }
        Instantiate(deathEffect, transform.position, Quaternion.identity, transform);
        StartCoroutine(PlayEffect(deathTime / deathPoints.Length, 0));
    }

    public bool UsePower(int powerToUse)
    {
        if(powerToUse <= power)
        {
            power -= powerToUse;
            sliders.SetPower(power);
            return true;
        }
        return false;
    }

    private void RegenPower()
    {
        if(power < maxPower)
        {
            power += powerRegenPerTick;
            sliders.SetPower(power);
            if(power > maxPower) power = maxPower;
        }
    }

    private IEnumerator PlayEffect(float waitTime, int iteration) 
    {
        yield return new WaitForSeconds(waitTime);
        if(iteration >= deathPoints.Length) 
        {
            bManager.RoundEnd(this);

            GameObject[] itemsToDestroy = GameObject.FindGameObjectsWithTag(gameObject.tag);
            foreach (GameObject item in itemsToDestroy)
            {
                Destroy(item);
            }
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        else 
        {
            Instantiate(deathEffect, deathPoints[iteration].position, Quaternion.identity, transform);
            StartCoroutine(PlayEffect(waitTime, iteration + 1));
        }
    }
}
