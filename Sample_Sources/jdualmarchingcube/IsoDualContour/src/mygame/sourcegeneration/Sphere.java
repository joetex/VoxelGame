/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.sourcegeneration;

/**
 *
 * @author Karsten
 */
public class Sphere implements GeneratorData {

    public float[][] getDim() {

        float[][] res = {{-1, 1, 0.015f},
            {-1, 1, 0.015f},
            {-1, 1, 0.015f}};

        return res;
    }

    public float getValue(float x, float y, float z) {
        return (float)(x*x + y*y + z*z - 0.9);
    }
    
}
