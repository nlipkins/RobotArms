using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{

    public float ArmSpeed;
    public int ArmMoveTimeInMs;
    public int ArmMaxAngle;
    
    public float HeadSpeed;
    public int HeadMoveTimeInMs;
    public int HeadMaxAngle;

  

    private bool ArmReturns = false;
    private bool ArmInMotion = false;
    private bool RobotStops = false;

    /// <summary>
    /// The class variables that we are concerned with 
    /// for this exercise
    /// Left, Right arms, and the Head
    /// </summary>
    private Transform Left, Right;
    private Transform Head;


    private float ArmMoveTimeF;
    private float HeadMoveTimeF;

    // Start is called before the first frame update
    void Start()
    {
        ArmMoveTimeF = ArmMoveTimeInMs / 1000f;
        HeadMoveTimeF = HeadMoveTimeInMs / 1000f;

        CreateArmReferences();
        CreateHeadReference();

        Debug.Log($"Left Arm = {Left.transform.position}");
        Debug.Log($"Right Arm = {Right.transform.position}");
        Debug.Log("Good work.");
    }
    
    /// <summary>
    /// Each group has to implement this method.
    /// Use CreateArmReference as a guide.
    /// </summary>
    private void CreateHeadReference()
    {
        bool fnd = false;

        Transform currentChild = null;
        for (int i = 0; i < transform.childCount && !fnd; i++)
        {
            currentChild = transform.GetChild(i);
            if (currentChild.CompareTag("Head"))
            {
                Head = currentChild;
                fnd = true;
            }
        }
    }
    
    private void CreateArmReferences()
    {
        Transform currentChild = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            currentChild = transform.GetChild(i);
            if (currentChild.CompareTag("Arm"))
            {
                if (currentChild.name == "LeftArm")
                    Left = currentChild;

                else if (currentChild.name == "RightArm")
                    Right = currentChild;
            }
        }
    }

    

    // Update is called once per frame
    private void Update()
    {
        if (!ArmInMotion && !RobotStops)
        {
            StartCoroutine("RotateHead");

            if (!ArmReturns)
            {
                StartCoroutine("ForwardArm", Left);
                StartCoroutine("ForwardArm", Right);
            }
            else
            {
                StartCoroutine("BackwardArm", Left);
                StartCoroutine("BackwardArm", Right);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && !RobotStops)
        {
            RobotStops = true;
            StartCoroutine("ResetToNeutral", Left);
            StartCoroutine("ResetToNeutral", Right);
        }
    }

    private IEnumerator RotateHead()
    {
        if (Head == null)
            yield return null;

        WaitForSeconds waitTime = new WaitForSeconds(HeadMoveTimeF);
        float currentSpeed = HeadSpeed;

        while (!RobotStops)
        {
            Head.Rotate(Vector3.up, currentSpeed);
            yield return waitTime;
        }

        if (RobotStops)
            Head.eulerAngles = new Vector3(Head.eulerAngles.x, 0f, Head.eulerAngles.z);

        yield return null;
    }

    private IEnumerator ForwardArm(Transform arm)
    {
        WaitForSeconds waitTime = new WaitForSeconds(ArmMoveTimeF);
        ArmInMotion = true;
        while ((arm.eulerAngles.z >= 360 - ArmMaxAngle || arm.eulerAngles.z <= ArmMaxAngle))
        {
            if (RobotStops)
                break;

            arm.Rotate(Vector3.forward, ArmSpeed);
            yield return waitTime;
        }


        if (!RobotStops)
            arm.eulerAngles = new Vector3(arm.eulerAngles.x, arm.eulerAngles.y, ArmMaxAngle);

        ArmReturns = true;
        ArmInMotion = false;
        yield return null;
    }

    private IEnumerator BackwardArm(Transform arm)
    {
        WaitForSeconds waitTime = new WaitForSeconds(ArmMoveTimeF);
        ArmInMotion = true;

        while ((arm.eulerAngles.z >= 360 - ArmMaxAngle || arm.eulerAngles.z <= ArmMaxAngle))
        {
            if (RobotStops)
                break;

            arm.Rotate(Vector3.back, ArmSpeed);
            yield return waitTime;
        }

        if (!RobotStops)
            arm.eulerAngles = new Vector3(arm.eulerAngles.x, arm.eulerAngles.y, 360 - ArmMaxAngle);
        ArmReturns = false;
        ArmInMotion = false;
        yield return null;
    }


    private IEnumerator ResetToNeutral(Transform arm)
    {
        WaitForSeconds waitTime = new WaitForSeconds(ArmMoveTimeF * 4);

        if (arm.localEulerAngles.z >= 360 - ArmMaxAngle)
        {
            int count = 360 - Mathf.RoundToInt(arm.localEulerAngles.z);
            for (int i = 0; i < count; i++)
            {
                arm.Rotate(Vector3.forward);
                yield return waitTime;
            }
        }
        else
        {
            int count = Mathf.RoundToInt(arm.localEulerAngles.z);
            for (int i = count; i > 0; i--)
            {
                arm.Rotate(Vector3.back);
                yield return waitTime;
            }

        }
        arm.eulerAngles = new Vector3(arm.eulerAngles.x, arm.eulerAngles.y, 0);
        yield return null;
    }


}
