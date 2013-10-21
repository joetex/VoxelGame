using System;

namespace Night
{
	public class Terrain {
        public float[][] getDim() {

        float[][] res = 
            new float[][]{new float[]{-1, 1, 0.01f},
            new float[]{-1, 1, 0.01f},
            new float[]{-1, 1, 0.01f}};
           /* {{-2, 2, 0.02f},
            {-2, 2, 0.02f},
            {-1, 1, 0.02f}};*/

        return res;
    }

    public float getValue(float x, float y, float z) {
     //   return (float)(Math.pow(1.0 - Math.sqrt(x*x + y*y), 2) + z*z - 0.25);
        return (float) (y + SimplexNoise.noise(x*2+5,y*2+3,z*2+0.6));
    }
}
}

