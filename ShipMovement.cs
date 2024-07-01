using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    
    public float thrustForce = 1;
    public float maxSpeed;
    public float turnSpeed = 5f;

    public int turnAmount = 5; //In degrees

    private bool readyToTurn = true;

    [Header("Key Bindings")]

    [System.NonSerialized] public bool verticalStuck = false;
    public bool vertical1 = true;
    public bool horizontal1 = true;

    void Update()
    {
        float yAxis = Input.GetAxis("Vertical2");
        if(vertical1) yAxis = Input.GetAxis("Vertical");

        float xAxis = Input.GetAxis("Horizontal2");
        if(horizontal1) xAxis = Input.GetAxis("Horizontal");

        if(verticalStuck) yAxis = 1;

        if(yAxis > 0)
        {
            float xComp = 0f;
            if((transform.up.x > 0 && rb.velocity.x > 0) && rb.velocity.x > maxSpeed)
            {
                xComp = (transform.up.x * maxSpeed - rb.velocity.x) * 0.25f;
            }
            else if((transform.up.x < 0 && rb.velocity.x < 0) && rb.velocity.x < -maxSpeed)
            {
                xComp = (transform.up.x * maxSpeed - rb.velocity.x) * 0.25f;
            }
            else
            {
                xComp = transform.up.x * thrustForce;
            }

            float yComp = 0f;
            if((transform.up.y > 0 && rb.velocity.y > 0) && rb.velocity.y > maxSpeed)
            {
                yComp = (transform.up.y * maxSpeed - rb.velocity.y) * 0.25f;
            }
            else if((transform.up.y < 0 && rb.velocity.y < 0) && rb.velocity.y < -maxSpeed)
            {
                yComp = (transform.up.y * maxSpeed - rb.velocity.y) * 0.25f;
            }
            else
            {
                yComp = transform.up.y * thrustForce;
            }

            rb.AddForce(new Vector3(xComp,yComp,0));
        }

        if(readyToTurn)
        {
            if(xAxis > 0)
            {
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, (float)System.Math.Round(transform.eulerAngles.z - turnAmount, 0));
                readyToTurn = false;
                StartCoroutine(TurnCoolDown());
            }
            else if(xAxis < 0)
            {
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, (float)System.Math.Round(transform.eulerAngles.z + turnAmount, 0));
                readyToTurn = false;
                StartCoroutine(TurnCoolDown());
            }
        }

        //ClampVelocity();
    }

    private void ClampVelocity()
    {
        float x = Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
        float y = Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);
        rb.velocity = new Vector3(x, y, 0);
    }
    private void ThrustForward(float amount)
    {
        Vector3 force = transform.up * amount;
        rb.AddForce(force);
    }

    private IEnumerator TurnCoolDown()
    {
        yield return new WaitForSeconds(1/(turnSpeed));
        readyToTurn = true;
    }
}
