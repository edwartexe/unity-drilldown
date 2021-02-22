using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //Allows us to use UI.

public static class controls{
    public static Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
    public static bool h_isAxisInUse = false;
    public static bool v_isAxisInUse = false;
    public static float SpawnRate = 0.25F;
    public static float timestamp = 0F;
    public static int horizontal;
    public static int vertical;

    public static void calculate() {
        horizontal = 0;     //Used to store the horizontal move direction.
        vertical = 0;       //Used to store the vertical move direction.


        //empanada
        //UnityEngine.WebGLInput.captureAllKeyboardInput = true; // or false

        //Check if we are running either in the Unity editor or in a standalone build.  
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL

        for (int i = 0; i < 20; i++) {
            if (Input.GetKeyDown("joystick button " + i)) {
                Debug.Log("joystick button " + i);

            }
        }

        //Get input from the input manager, round it to an integer and store it
        //> 0.4f was originaly !=0 but it was too sensitive
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.4f || Mathf.Abs(Input.GetAxisRaw("HorizontalJ")) > 0.4f || Mathf.Abs(Input.GetAxisRaw("HorizontalD")) > 0.4f) {
            if (h_isAxisInUse == false || Time.time >= timestamp) {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.4f) {
                    horizontal = (int)(Input.GetAxisRaw("Horizontal"));
                } else {
                    if (Mathf.Abs(Input.GetAxisRaw("HorizontalJ")) > 0.4f) {
                        horizontal = (int)(Input.GetAxisRaw("HorizontalJ"));
                    } else {
                        horizontal = (int)(Input.GetAxisRaw("HorizontalD"));
                    }
                }
                h_isAxisInUse = true;
                timestamp = Time.time + SpawnRate;
            }
        }
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("HorizontalJ") == 0 && Input.GetAxisRaw("HorizontalD") == 0) {
            h_isAxisInUse = false;
        }

        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.4f || Mathf.Abs(Input.GetAxisRaw("VerticalJ")) > 0.4f || Mathf.Abs(Input.GetAxisRaw("VerticalD")) > 0.4f) {
            if (v_isAxisInUse == false || Time.time >= timestamp) {
                //vertical = (int)(Mathf.Max(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("VerticalJ")));
                
                if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.4f) {
                    vertical = (int)(Input.GetAxisRaw("Vertical"));
                } else {
                    if (Mathf.Abs(Input.GetAxisRaw("VerticalJ")) > 0.4f) {
                        vertical = (int)(Input.GetAxisRaw("VerticalJ"));
                    } else {
                        vertical = (int)(Input.GetAxisRaw("VerticalD"));
                    }
                }
                v_isAxisInUse = true;
                timestamp = Time.time + SpawnRate;
            }
        }
        if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("VerticalJ") == 0 && Input.GetAxisRaw("VerticalD") == 0) {
            v_isAxisInUse = false;
        }
        //Check if moving horizontally, if so set vertical to zero.
        /*if (horizontal != 0) {
            vertical = 0;
        }*/

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0){
                //Store the first touch detected.
                Touch myTouch = Input.touches[0];

                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began){
                    //If so, set touchOrigin to the position of that touch
                    touchOrigin = myTouch.position;
                }

                //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
                    //Set touchEnd to equal the position of this touch
                    Vector2 touchEnd = myTouch.position;

                    //Calculate the difference between the beginning and end of the touch on the x axis.
                    float x = touchEnd.x - touchOrigin.x;

                    //Calculate the difference between the beginning and end of the touch on the y axis.
                    float y = touchEnd.y - touchOrigin.y;

                    //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                    touchOrigin.x = -1;

                    //Check if the difference along the x axis is greater than the difference along the y axis.
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                        horizontal = x > 0 ? 1 : -1;
                    else
                        //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                        vertical = y > 0 ? 1 : -1;
                }
            }

#endif //End of mobile platform dependendent compilation section started above with #elif
    }

}
