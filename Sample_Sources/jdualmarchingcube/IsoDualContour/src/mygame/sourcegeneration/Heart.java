/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.sourcegeneration;

/**
 *
 * @author Karsten
 */
/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

import mygame.Noise;

/**
 *
 * @author Karsten
 */
public class Heart implements GeneratorData {

    public float[][] getDim() {

        float[][] res = {{-2, 2, 0.02f},
            {-2, 2, 0.02f},
            {-2, 2, 0.02f}};

        return res;
    }

    public float getValue(float x, float y, float z) {
        y *= 1.5;
        z *= 1.5;
        return (float)(Math.pow(2*x*x+y*y+2*z*z-1, 3) - 0.1 * z*z*y*y*y - y*y*y*x*x);
    }
}
