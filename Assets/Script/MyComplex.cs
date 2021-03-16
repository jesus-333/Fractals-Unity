using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyComplex
{
    private float real, imag;

    public MyComplex(float real, float imag){
        this.real = real;
        this.imag = imag;
    }

    public string toString(){
        return real + "+ j" + imag;
    }

    public float Abs(){
        return (float)System.Math.Sqrt(real * real + imag * imag);
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Operator overload

    public static MyComplex operator +(MyComplex z)
        => z;

    public static MyComplex operator -(MyComplex z)
        => new MyComplex(-z.real, -z.imag);

    public static MyComplex operator +(MyComplex z1, MyComplex z2)
        => new MyComplex(z1.real + z2.real, z1.imag + z2.imag);

    public static MyComplex operator -(MyComplex z1, MyComplex z2)
        => z1 + (-z2);

    public static MyComplex operator *(MyComplex z1, MyComplex z2)
        => new MyComplex(((z1.real * z2.real) - (z1.imag * z2.imag)), ((z1.real * z2.imag) + (z1.imag * z2.real)));


    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    public static float Abs(MyComplex z){
        return z.Abs();
    }

}
