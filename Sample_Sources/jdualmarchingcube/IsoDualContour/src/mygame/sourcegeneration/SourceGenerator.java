/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.sourcegeneration;

import com.jme3.math.Vector3f;
import mygame.Noise;
import mygame.dualmarchingcubes.source.FloatGridSource;

/**
 *
 * @author Karsten
 */
public class SourceGenerator {
    
    public static FloatGridSource getSource(GeneratorData data, float maxClampedAbsoluteDensity)
    {       
         float[][] dims = data.getDim();
         
         int[] res = new int[3];
         for(int i=0; i<3; ++i) {
            res[i] = (int)(2 + Math.ceil((dims[i][1] - dims[i][0]) / dims[i][2]));
            System.out.println(res[i]);
        } 
         
        FloatGridSource source = new FloatGridSource(new Vector3f(1,1,1),res[0],res[1],res[2],maxClampedAbsoluteDensity);

        float z=dims[2][0]-dims[2][2];
        
        for(int k=0; k<res[2]; ++k, z+=dims[2][2]){
            float y=dims[1][0]-dims[1][2];
            for(int j=0; j<res[1]; ++j, y+=dims[1][2]){
                float x = x=dims[0][0]-dims[0][2];
                for(int i=0 ; i<res[0]; ++i, x+=dims[0][2]) {
                    
                   // System.out.println(x + " " + y + " " + z + " " + value);
                    source.setVolumeGridValue(i, j, k, (float)-data.getValue(x, y, z));
                }
            }
        }
         return source;
    }

    

}
