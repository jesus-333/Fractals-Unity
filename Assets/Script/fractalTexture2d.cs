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
    int max_iteration = 255;

    bool inside_image = false, reduce_size_selection = false;
    float selection_area;
    Vector2 start_click, end_click, start_coord_for_calculus, end_coord_for_calculus;

    // Start is called before the first frame update
    void Start()
    {
        fractal = new Texture2D(width, height);
        fractal_copy = new Texture2D(width, height);
        img.texture = fractal;

        createFractal();
        Graphics.CopyTexture(fractal, fractal_copy);

        start_coord_for_calculus = new Vector2(0f, 0f);
        end_coord_for_calculus = new Vector2(0f, 0f);

        selection_area = 0;
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Create Fractal functions

    void createFractal(){
        float[] x_points = linspace(x_min, x_max, width), y_points = linspace(y_min, y_max, height);
        Complex z, c;
        float count =0;

        for (int i = 0; i < width; i ++){
            for(int j = 0; j < height; j++){
                z = new Complex(0, 0);
                c = new Complex(x_points[i], y_points[j]);
                count = 0;

                // if(i < 10 && j < 10) print(c);

                for(int k = 0; k < max_iteration; k++){
                    z = z * z + c;

                    if(Complex.Abs(z) > 4) {
                        //print(count);
                        break;
                    }

                    count++;
                }
                //print(count);
                // fractal.SetPixel(i, j, new Color(count/255f, 0f, 0f));

                fractal.SetPixel(i, j, new Color(count/255f * (float)Complex.Abs(z), count/255f * (float)Complex.Abs(z), count/255f * (float)Complex.Abs(z)));
                // fractal.SetPixel(i, j, new Color(count/255f * (float)Complex.Abs(z), (float)Complex.Abs(z), (float)Complex.Abs(z)));
            }
        }

        fractal.Apply();
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
    // Manage Click Functions

    public void OnDrag(){
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
            // img.texture = fractal_copy;

            print("Point = " + start_click);
            Vector2 tmp_traslato = new Vector2(0,0);
            tmp_traslato.x = (int)(((float)start_click.x / (float)Screen.width) * (float)width);
            tmp_traslato.y = (int)(((float)start_click.y / (float)Screen.height) * (float)height);
            print("Point = " + tmp_traslato);
        }
    }

    public void OnClickUp(){
        if(inside_image){
            Vector3 mousePos = Input.mousePosition;
            end_click = new Vector2(mousePos.x, mousePos.y);
            correctCordinates(start_click, end_click);

            // Clean the selection when I stop to click the mouse button
            Graphics.CopyTexture(fractal, fractal_copy);
            setTexture(fractal_copy);
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
        // print(Screen.width);

        // correct y values
        tmp_min = Mathf.Min(start_coord.y, end_coord.y);
        tmp_max = Mathf.Max(start_coord.y, end_coord.y);

        start_coord_for_calculus.y = (int)(((float)tmp_min / (float)Screen.height) * (float)height);
        end_coord_for_calculus.y   = (int)(((float)tmp_max / (float)Screen.height) * (float)height);

        // Evaluate the area of the selection
        // float tmp_new_area = (end_coord_for_calculus.x - start_coord_for_calculus.x) * (end_coord_for_calculus.y - start_coord_for_calculus.y);
        // if(tmp_new_area > selection_area) reduce_size_selection = false;
        // else reduce_size_selection = true;
        // // print("NEW AREA = " + tmp_new_area + "   OLD AREA = " + selection_area + "   change = " + reduce_size_selection);
        // selection_area = tmp_new_area;

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
        // Function to change the color of the pixels

        for(int i = 0; i < colors.Length; i++){
            // colors[i] = new Color(1f - colors[i].r, 1f, 1f - colors[i].b);

            colors[i].r = 1 - colors[i].r;
            colors[i].g = 0f;
            colors[i].b = 0f;
            // colors[i].a = 0f;
        }

        return colors;
    }

    private Color[] changeColorsVector2(Color[] colors){
        // Function to change the color of the pixels

        for(int i = 0; i < colors.Length; i++){
            // colors[i] = new Color(1f - colors[i].r, 1f, 1f - colors[i].b);

            colors[i].r = 1 - colors[i].r;
            colors[i].g = 0.5f;
            colors[i].b = 0.5f;
            // colors[i].a = 0f;
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
        img.texture = tex;
        tex.Apply();
    }
}
