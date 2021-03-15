using System;
using System.Collections;
using System.Collections.Generic;
using Complex = System.Numerics.Complex;
using UnityEngine;
using UnityEngine.UI;

public class fractalTexture2d : MonoBehaviour
{
    private Texture2D fractal, fractal_copy;
    public RawImage img;
    public int width = 800, height = 600;
    public float x_min = -1, x_max = 1, y_min = -1, y_max = 1;

    [Range(1, 255)]
    public int max_iteration = 255;

    bool inside_image = false, color_inside;
    float percentage_r, percentage_g, percentage_b, escape_radius;
    Vector2 start_click, end_click, start_coord_for_calculus, end_coord_for_calculus;

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    void Start()
    {
        percentage_r = percentage_g = percentage_b = 1f;
        color_inside = true;
        escape_radius = 4;

        fractal = createFractal();
        fractal_copy = new Texture2D(width, height);
        Graphics.CopyTexture(fractal, fractal_copy);
        setTexture(fractal);

        start_coord_for_calculus = new Vector2(0f, 0f);
        end_coord_for_calculus = new Vector2(0f, 0f);
    }

    void Update(){

    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Create Fractal functions

    Texture2D createFractal(){
        Texture2D fractal = new Texture2D(width, height);

        float[] x_points = linspace(x_min, x_max, width), y_points = linspace(y_min, y_max, height);
        Complex z, c;
        float count =0, color_inside_value = 1f;

        for (int i = 0; i < width; i ++){
            for(int j = 0; j < height; j++){
                z = new Complex(0, 0);
                c = new Complex(x_points[i], y_points[j]);
                count = 0;

                // if(i < 10 && j < 10) print(c);

                for(int k = 0; k < max_iteration; k++){
                    z = z * z + c;

                    if(Complex.Abs(z) > escape_radius) break;

                    count++;
                }

                // fractal.SetPixel(i, j, new Color(count/255f, 0f, 0f));
                // fractal.SetPixel(i, j, new Color(count/255f * (float)Complex.Abs(z), count/255f * (float)Complex.Abs(z), count/255f * (float)Complex.Abs(z)));

                if(color_inside) color_inside_value = (float)Complex.Abs(z);
                else color_inside_value = 1f;

                fractal.SetPixel(i, j, new Color(count/255f * color_inside_value * percentage_r, count/255f * color_inside_value * percentage_g, count/255f * color_inside_value * percentage_b));

            }
        }

        return fractal;
    }


    private float[] linspace(float start, float end, int n){
        float[] return_vet = new float[n];
        float step = (end - start) / (float) n, tmp_val = start;

        for(int i = 0; i < n; i++){
            return_vet[i] = tmp_val;
            tmp_val += step;
        }

        return return_vet;
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Color fractal related functions

    public void setcolorInside(bool tmp_value){
        color_inside = tmp_value;
    }

    public void setPercentageRed(float tmp_percentage_r){
        percentage_r = tmp_percentage_r;
    }

    public void setPercentageGreen(float tmp_percentage_g){
        percentage_g = tmp_percentage_g;
    }

    public void setPercentageBlue(float tmp_percentage_b){
        percentage_b = tmp_percentage_b;
    }

    public void setEscapeRadius(float tmp_escape_radius){
        escape_radius = tmp_escape_radius;

        GameObject.Find("Text Escape Radius").GetComponent<Text>().text = "" + ((int)tmp_escape_radius);
    }

    public void drawnFractalThroughFunction(){
        fractal = createFractal();
        setTexture(fractal);
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Manage Click Functions

    public void OnDrag(){
        // Function used during the drag of the mouse.
        // Create a texture that overlap and show the selected area with inverted color

        if(inside_image){
            Vector3 mousePos = Input.mousePosition;
            end_click = new Vector2(mousePos.x, mousePos.y);
            correctCordinates(start_click, end_click);

            Graphics.CopyTexture(fractal, fractal_copy);
            createOverlay();
            setTexture(fractal_copy);
        }
    }

    public void OnClickDown(){
        if(inside_image){
            Vector3 mousePos = Input.mousePosition;
            start_click = new Vector2(mousePos.x, mousePos.y);
        }
    }

    public void OnClickUp(){
        if(inside_image){
            Vector3 mousePos = Input.mousePosition;
            end_click = new Vector2(mousePos.x, mousePos.y);
            correctCordinates(start_click, end_click);

            // Clean the selection when I stop to click the mouse button (Used during debug)
            // Graphics.CopyTexture(fractal, fractal_copy);
            // setTexture(fractal_copy);

            // Zoom in temporary variable
            float tmp_x_min, tmp_x_max, tmp_y_min, tmp_y_max;

            // Rescale x values
            tmp_x_min = (start_coord_for_calculus.x / width) * (x_max - x_min) + x_min;
            tmp_x_max = (end_coord_for_calculus.x / width) * (x_max - x_min) + x_min;

            // Rescale y values
            tmp_y_min = start_coord_for_calculus.y / (height) * (y_max - y_min) + y_min;
            tmp_y_max = end_coord_for_calculus.y / (height) * (y_max - y_min) + y_min;

            // Assign rescale values
            x_min = tmp_x_min;
            x_max = tmp_x_max;
            y_min = tmp_y_min;
            y_max = tmp_y_max;

            // Redrawn fractal
            drawnFractalThroughFunction();
        }
    }

    public void OnEnter(){
        inside_image = true;
    }

    public void OnExit(){
        inside_image = false;
        Graphics.CopyTexture(fractal, fractal_copy);
        setTexture(fractal);
    }

    private void correctCordinates(Vector2 start_coord, Vector2 end_coord){
        // Functions used to correct the cordinates according to Unity System where the origin is the bottom left corner.
        // The correction also consider the fact that the resolution of the texture can be different from the resolution of the screen

        float tmp_min, tmp_max;

        // correct x values
        tmp_min = Mathf.Min(start_coord.x, end_coord.x);
        tmp_max = Mathf.Max(start_coord.x, end_coord.x);

        start_coord_for_calculus.x = (int)(((float)tmp_min / (float)Screen.width) * (float)width);
        end_coord_for_calculus.x   = (int)(((float)tmp_max / (float)Screen.width) * (float)width);

        // correct y values
        tmp_min = Mathf.Min(start_coord.y, end_coord.y);
        tmp_max = Mathf.Max(start_coord.y, end_coord.y);

        start_coord_for_calculus.y = (int)(((float)tmp_min / (float)Screen.height) * (float)height);
        end_coord_for_calculus.y   = (int)(((float)tmp_max / (float)Screen.height) * (float)height);
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    private void createOverlay(){
        // Create an overlay on the selected area

        // Evaluate width and height of the current selection
        int tmp_width = (int)end_coord_for_calculus.x - (int)start_coord_for_calculus.x, tmp_height = (int)end_coord_for_calculus.y - (int)start_coord_for_calculus.y;

        // Get the color of the original pixels and change them
        Color[] pixels = GetPixelsHandmade(fractal_copy, (int)start_coord_for_calculus.x, (int)start_coord_for_calculus.y, tmp_width, tmp_height);
        pixels = changeColorsVector(pixels);

        // Set the selected pixels to the new color
        setPixelsHandmade(fractal_copy, (int)start_coord_for_calculus.x, (int)start_coord_for_calculus.y, tmp_width, tmp_height, pixels);
    }

    private Color[] changeColorsVector(Color[] colors){
        // Function to change the color of the pixels during the overlap

        for(int i = 0; i < colors.Length; i++){
            // colors[i] = new Color(1f - colors[i].r, 1f, 1f - colors[i].b);

            colors[i].r = 1 - colors[i].r;
            colors[i].g = 1 - colors[i].g;
            colors[i].b = 1 - colors[i].b;
            colors[i].a = 0.8f;
        }

        return colors;
    }

    private Color[] GetPixelsHandmade(Texture2D img, int x, int y, int width, int height){
        Color[] tmp_colors = new Color[width * height];
        int k = 0;

        for(int i = x; i < x + width; i++){
            for(int j = y; j < y + height; j++){
                tmp_colors[k] = img.GetPixel(i, j);
                k++;
            }
        }

        return tmp_colors;
    }

    private void setPixelsHandmade(Texture2D img, int x, int y, int width, int height, Color[] colors){
        int k = 0;

        for(int i = x; i < x + width; i++){
            for(int j = y; j < y + height; j++){
                img.SetPixel(i, j, colors[k]);
                k++;
            }
        }
    }

    private void setTexture(Texture2D tex){
        // Function to set the texture of the image and show it.

        img.texture = tex;
        tex.Apply();
    }
}
