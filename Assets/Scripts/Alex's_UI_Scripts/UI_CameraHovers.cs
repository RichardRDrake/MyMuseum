using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CameraHovers : MonoBehaviour
{
    //This script attaches to the hover fields at the sides of the screen during build mode.
    //Determines when and how to rotate the 3rd person camera
    [SerializeField] private GameObject Camera;
    private CamController camController;

    //If this is attached to the left hover field, isLeft is true.
    [SerializeField] private bool isLeft;
    private bool cooldown = false;

    //note that this is public as the CamController must access it
    public bool isHovering = false;
    private float timer = 0.8f;
    private float timerCurrent;

    // Start is called before the first frame update
    void Start()
    {
        camController = Camera.GetComponent<CamController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Forces a grace period between camera changes
        if(cooldown == true)
        {
            if(timerCurrent < timer)
            {
                timerCurrent += Time.deltaTime;
            }
            else
            {
                cooldown = false;
                timerCurrent = 0.0f;
            }
        }

        //Determines what actions to take, should a hover be in progress
        if(isHovering && !cooldown)
        {
            TurnCamera();
        }
    }

    //Turns camera
    private void TurnCamera()
    {
        if (isLeft)
        {
            //Just... trust me.
            camController.rotateRight();
            cooldown = true;
        }
        else if (!isLeft)
        {
            camController.rotateLeft();
            cooldown = true;
        }
    }

    public void HoverIn()
    {
        isHovering = true;
    }

    public void HoverOut()
    {
        isHovering = false;
        cooldown = false;
        timerCurrent = 0.0f;
    }
}
