/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.sourcegeneration;


import mygame.Noise;

/**
 *
 * @author Karsten
 */
public class Donut implements GeneratorData{
        public float[][] getDim() {

        float[][] res = 
            {{-2, 2, 0.02f},
            {-2, 2, 0.02f},
            {-1, 1, 0.02f}};

        return res;
    }

    public float getValue(float x, float y, float z) {
        return (float)(Math.pow(1.0 - Math.sqrt(x*x + y*y), 2) + z*z - 0.25);
    }
}
