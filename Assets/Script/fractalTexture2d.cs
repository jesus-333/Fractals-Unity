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
    public double x_min = -1, x_max = 1, y_min = -1, y_max = 1;
    public GameObject Color_UI, Zoom_UI;

    [Range(1, 255)]
    public int max_iteration = 255;

    bool inside_image = false, color_inside, keep_aspect_ratio, zoom_click;
    double percentage_r, percentage_g, percentage_b, escape_radius;
    Vector2 start_click, end_click, start_coord_for_calculus, end_coord_for_calculus;

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    void Start()
    {
        percentage_r = percentage_g = percentage_b = 0.5f;
        color_inside = keep_aspect_ratio = true;
        zoom_click = false;
        escape_radius = 4;
        x_min = y_min = -1; x_max = y_max = 1;

        fractal = createFractal();
        fractal_copy = new Texture2D(width, height);
        Graphics.CopyTexture(fractal, fractal_copy);
        setTexture(fractal);

        start_coord_for_calculus = new Vector2(0f, 0f);
        end_coord_for_calculus = new Vector2(0f, 0f);
    }

    void Update(){
        // Activate/Deactivate the Color UI when C is pressed
        if(Input.GetKeyDown(KeyCode.C)){
            Color_UI.SetActive(!Color_UI.activeSelf);
        }

        // Activate/Deactivate the Zoom UI when Z is pressed
        if(Input.GetKeyDown(KeyCode.Z)){
            Zoom_UI.SetActive(!Zoom_UI.activeSelf);
        }

        // Reset to normal scale if press R
        if(Input.GetKeyDown(KeyCode.R)){
            x_min = y_min = -1;
            x_max = y_max = 1;

            drawnFractalThroughFunction();
        }
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Create Fractal functions

    Texture2D createFractal(){
        Texture2D fractal = new Texture2D(width, height);

        double[] x_points = linspace(x_min, x_max, width), y_points = linspace(y_min, y_max, height);
        Complex z, c;
        double count =0, color_inside_value = 1f, r, g ,b;

        for (int i = 0; i < width; i ++){
            for(int j = 0; j < height; j++){
                z = new Complex(0, 0);
                c = new Complex(x_points[i], y_points[j]);
                count = 0;

                for(int k = 0; k < max_iteration; k++){
                    z =  z * z + c;
                    // z =  (z + c) * Complex.Tan(z + c) * Complex.Sin(z + c);

                    if(Complex.Abs(z) > escape_radius) break;

                    count++;
                }

                if(color_inside) color_inside_value = (double)Complex.Abs(z);
                else color_inside_value = 1f;

                r = count/(double)max_iteration * color_inside_value * percentage_r;
                g = count/(double)max_iteration * color_inside_value * percentage_g;
                b = count/(double)max_iteration * color_inside_value * percentage_b;
                fractal.SetPixel(i, j, new Color((float)r, (float)g, (float)b));

            }
        }

        return fractal;
    }


    private double[] linspace(double start, double end, int n){
        double[] return_vet = new double[n];
        double step = (end - start) / (double) n, tmp_val = start;

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
        percentage_r = (double)tmp_percentage_r;
    }

    public void setPercentageGreen(float tmp_percentage_g){
        percentage_g = (double)tmp_percentage_g;
    }

    public void setPercentageBlue(float tmp_percentage_b){
        percentage_b = (double)tmp_percentage_b;
    }

    public void setEscapeRadius(float tmp_escape_radius){
        escape_radius = (double)tmp_escape_radius;

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
            double tmp_x_min, tmp_x_max, tmp_y_min, tmp_y_max;

            // Rescale x values
            tmp_x_min = (start_coord_for_calculus.x / width) * (x_max - x_min) + x_min;
            tmp_x_max = (end_coord_for_calculus.x / width) * (x_max - x_min) + x_min;

            // Rescale y values
            tmp_y_min = start_coord_for_calculus.y / (height) * (y_max - y_min) + y_min;
            tmp_y_max = end_coord_for_calculus.y / (height) * (y_max - y_min) + y_min;

            // Modify the lower y to mantain the aspect ratio of the zoomed image
            double tmp_aspect_ratio = (double)(x_max - x_min) / (double)(y_max - y_min);
            if(keep_aspect_ratio){

                tmp_y_min = tmp_y_max - (double)(tmp_x_max - tmp_x_min)/(double)(tmp_aspect_ratio);
            }

            // Assign rescale values
            x_min = tmp_x_min;
            x_max = tmp_x_max;
            y_min = tmp_y_min;
            y_max = tmp_y_max;

            print("x_min = " + x_min + "\t x_max = " + x_max);
            print("y_min = " + y_min + "\t y_max = " + y_max);
            // print("aspect ration = " + ((double)(x_max - x_min) / (double)(y_max - y_min)) + "\t OLD aspect_ratio = " + tmp_aspect_ratio);
            print("delta x = " + (x_max - x_min) + "\t delta y = " + (y_max - y_min));

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

        double tmp_min, tmp_max;

        // correct x values
        tmp_min = Mathf.Min(start_coord.x, end_coord.x);
        tmp_max = Mathf.Max(start_coord.x, end_coord.x);

        start_coord_for_calculus.x = (int)(((double)tmp_min / (double)Screen.width) * (double)width);
        end_coord_for_calculus.x   = (int)(((double)tmp_max / (double)Screen.width) * (double)width);

        // correct y values
        tmp_min = Mathf.Min(start_coord.y, end_coord.y);
        tmp_max = Mathf.Max(start_coord.y, end_coord.y);

        start_coord_for_calculus.y = (int)(((double)tmp_min / (double)Screen.height) * (double)height);
        end_coord_for_calculus.y   = (int)(((double)tmp_max / (double)Screen.height) * (double)height);
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
