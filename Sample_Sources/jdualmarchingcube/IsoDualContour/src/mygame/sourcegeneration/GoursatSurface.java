/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.sourcegeneration;

import com.jme3.math.FastMath;

/**
 *
 * @author Karsten
 */
public class GoursatSurface implements GeneratorData{
        public float[][] getDim() {

        float[][] res = 
            {{-2, 2, 0.02f},
            {-2, 2, 0.02f},
            {-2, 2, 0.02f}};

        return res;
    }

    public float getValue(float x, float y, float z) {
        
        float x2 = x*x;
        float y2 = y*y;
        float z2 = z*z;
        
         return (float)(x2*x2 + y2*y2 + z2*z2 - 1.5 * (x2  + y2 + z2) + 1);
       // return (float)(FastMath.pow(x,4) + FastMath.pow(y,4) + FastMath.pow(z,4) - 1.5 * (x*x  + y*y + z*z) + 1);
    }
}
