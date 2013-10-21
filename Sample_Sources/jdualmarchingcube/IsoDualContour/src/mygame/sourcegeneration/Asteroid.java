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
public class Asteroid implements GeneratorData {

    public float[][] getDim() {

        float[][] res = {{-1, 1, 0.01f},
            {-1, 1, 0.01f},
            {-1, 1, 0.01f}};

        return res;
    }

    public float getValue(float x, float y, float z) {
        return (float) ((x * x + y * y + z * z) - Noise.noise(x * 2, y * 2, z * 2));
    }
}
