using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public float camDistance = 2f;
    public float zoomCap = 20f;
    public float camSizeMod = 1f;

    public float dampeningSpeed = 0.7f;

    private Transform target1;
    private Transform target2;
    [System.NonSerialized]public float wallX, wallY;

    public Camera mainCam;
    public GameObject XCam,YCam,XYCam;
    private bool lockedToPlayer = false;

    void Start()
    {
        FindTargets();
        wallX = 2 * zoomCap * mainCam.aspect;
        wallY = 2 * zoomCap;
    }

    public void FindTargets()
    {
        lockedToPlayer = false;
        target1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Transform>();
        target2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Transform>();
    }

    void Update()
    {
        if (target1 == null && target2 == null)
        {
            //freeze camera
            return;
        }
        else if (target1 == null)
        {
            FollowTarget(target2.position);
        }
        else if(target2 == null)
        {
            FollowTarget(target1.position);
        }
        else
        {
            Vector3 targetPos = AdjustedPosition(target1.position, target2.position);

            float camSize = Vector3.Distance(target1.position, targetPos) * 2 + camSizeMod;

            if (camSize > zoomCap) { camSize = zoomCap; }

            transform.position = new Vector3(targetPos.x, targetPos.y, camDistance);

            SetCameraSizes(camSize);

            XCam.transform.position = ReflectPosition(true, false);
            YCam.transform.position = ReflectPosition(false, true);
            XYCam.transform.position = ReflectPosition(true, true);
        }
    }

    private void FollowTarget(Vector3 target)
    {
        Vector3 targetPos = new Vector3(target.x, target.y, camDistance);
        if (lockedToPlayer)
        {
            transform.position = new Vector3(target.x, target.y, camDistance);
        }
        else
        {
            //transform.position = Vector3.MoveTowards(transform.position, AdjustedPosition(targetPos, transform.position), Time.deltaTime * dampeningSpeed);
            Vector3 directionToMove = AdjustedPosition(transform.position, targetPos) - transform.position;
            directionToMove.Normalize();
            transform.position += directionToMove * Time.deltaTime * dampeningSpeed;
            if (Vector2.Distance(transform.position, AdjustedPosition(transform.position, targetPos)) < 0.01) { lockedToPlayer = true; }
        }
        float camSize = camSizeMod + (Vector2.Distance(transform.position, AdjustedPosition(transform.position, targetPos)) * 2);
        if (camSize > zoomCap) { camSize = zoomCap; }
        SetCameraSizes(camSize);

        XCam.transform.position = ReflectPosition(true, false);
        YCam.transform.position = ReflectPosition(false, true);
        XYCam.transform.position = ReflectPosition(true, true);
    }

    private Vector3 AdjustedPosition(Vector3 target1, Vector3 target2) 
    {
        Vector3 targetPos = new Vector3();

        float XRefVal = 0;
        if (target2.x < 0)
        {
            XRefVal = (target2.x + wallX) * 2;
        }
        else
        {
            XRefVal = (target2.x - wallX) * 2;
        }

        float YRefVal = 0;
        if (target2.y < 0)
        {
            YRefVal = (target2.y + wallY) * 2;
        }
        else
        {
            YRefVal = (target2.y - wallY) * 2;
        }

        Vector3 target2XPosRef = new Vector3(-target2.x + XRefVal, target2.y, 0);
        float XRefDis = Vector3.Distance(target1, target2XPosRef);
        Vector3 target2YPosRef = new Vector3(target2.x, -target2.y + YRefVal, 0);
        float YRefDis = Vector3.Distance(target1, target2YPosRef);
        Vector3 target2XYPosRef = new Vector3(-target2.x + XRefVal, -target2.y + YRefVal, 0);
        float XYRefDis = Vector3.Distance(target1, target2XYPosRef);
        float NoRefDis = Vector3.Distance(target1, target2);

        if (XRefDis < YRefDis && XRefDis < XYRefDis && XRefDis < NoRefDis) //a X reflection is needed
        {
            targetPos = target1 + target2XPosRef;
        }
        else if (YRefDis < XYRefDis && YRefDis < NoRefDis) //a Y reflection is needed
        {
            targetPos = target1 + target2YPosRef;
        }
        else if (XYRefDis < NoRefDis) //a X and Y reflection is needed
        {
            targetPos = target1 + target2XYPosRef;
        }
        else //no reflection is needed
        {
            targetPos = target1 + target2;
        }

        return new Vector3(targetPos.x / 2, targetPos.y / 2, 0);
    }

    private void SetCameraSizes(float camSize)
    {
        mainCam.orthographicSize = camSize;
        XCam.GetComponent<Camera>().orthographicSize = camSize;
        YCam.GetComponent<Camera>().orthographicSize = camSize;
        XYCam.GetComponent<Camera>().orthographicSize = camSize;
    }

    private Vector3 ReflectPosition(bool xRef, bool yRef)
    {
        float xPos = transform.position.x;
        if (xRef)
        {
            if (transform.position.x < 0)
            {
                xPos = transform.position.x + (wallX * 2);
            }
            else
            {
                xPos = transform.position.x - (wallX * 2);
            }
        }

        float yPos = transform.position.y;
        if (yRef)
        {
            if (transform.position.y < 0)
            {
                yPos = transform.position.y + (wallY * 2);
            }
            else
            {
                yPos = transform.position.y - (wallY * 2);
            }
        }

        return new Vector3(xPos, yPos, camDistance);
    }
}
