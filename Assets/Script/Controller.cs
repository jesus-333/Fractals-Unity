using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Material mat;
    public Vector2 pos;
    public float scale;

    [Range(-10.0f, 10.0f)]
    public float slider_a = 1f, slider_b = 1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        moveFunction();
        updateShader();
    }

    void moveFunction(){
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) { scale *= 0.9f; }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) { scale *= 1.1f; }

        if(Input.GetKey(KeyCode.UpArrow)){ scale *= 0.99f; }
        if(Input.GetKey(KeyCode.DownArrow)){ scale *= 1.01f; }

        float movement_value = 0.1f/2;

        if(Input.GetKey(KeyCode.W)){ pos.y += movement_value * scale; }
        if(Input.GetKey(KeyCode.S)){ pos.y -= movement_value * scale; }

        if(Input.GetKey(KeyCode.A)){ pos.x -= movement_value * scale; }
        if(Input.GetKey(KeyCode.D)){ pos.x += movement_value * scale; }
    }

    void updateShader(){
        float aspect = (float) Screen.width / (float) Screen.height;
        float scaleX = scale, scaleY = scale;

        if(aspect > 1) scaleY /= aspect;
        else scale*= aspect;

        mat.SetVector("_Area", new Vector4(pos.x, pos.y, scaleX, scaleY));
        mat.SetFloat("scale_time_a", slider_a);
        mat.SetFloat("scale_time_b", slider_b);

    }
}
