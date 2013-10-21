
using UnityEngine;

namespace Night
{
	public class OldNoise {

	    private Random rand_;
	    float roughness_;
	    private float[][] grid_;
	
	    public OldNoise(Random rand, float roughness, int width, int height) {
	        this.roughness_ = (roughness / width);
	        this.grid_ = new float[width][];
			for( int i=0; i<width;i++)
				this.grid_[i] = new float[height];
			
	        this.rand_ = (rand == null ? new Random() : rand);
	        initialise();
	    }
	
	    public void initialise() {
	        int xh = this.grid_.Length - 1;
	        int yh = this.grid_[0].Length - 1;
	
	        this.grid_[0][0] = (Random.value - 0.5F);
	        this.grid_[0][yh] = (Random.value - 0.5F);
	        this.grid_[xh][0] = (Random.value - 0.5F);
	        this.grid_[xh][yh] = (Random.value - 0.5F);
			Random.seed = 15234;
	        generate(0, 0, xh, yh);
	    }
	
	    private void generate(int xl, int yl, int xh, int yh) {
	        int xm = (xl + xh) / 2;
	        int ym = (yl + yh) / 2;
	        if ((xl == xm) && (yl == ym)) {
	            return;
	        }
	        this.grid_[xm][yl] = (0.5F * (this.grid_[xl][yl] + this.grid_[xh][yl]));
	        this.grid_[xm][yh] = (0.5F * (this.grid_[xl][yh] + this.grid_[xh][yh]));
	        this.grid_[xl][ym] = (0.5F * (this.grid_[xl][yl] + this.grid_[xl][yh]));
	        this.grid_[xh][ym] = (0.5F * (this.grid_[xh][yl] + this.grid_[xh][yh]));
	        float v = roughen(0.5F * (this.grid_[xm][yl] + this.grid_[xm][yh]), xl + yl, yh + xh);
	        this.grid_[xm][ym] = v;
	        this.grid_[xm][yl] = roughen(this.grid_[xm][yl], xl, xh);
	        this.grid_[xm][yh] = roughen(this.grid_[xm][yh], xl, xh);
	        this.grid_[xl][ym] = roughen(this.grid_[xl][ym], yl, yh);
	        this.grid_[xh][ym] = roughen(this.grid_[xh][ym], yl, yh);
	        generate(xl, yl, xm, ym);
	        generate(xm, yl, xh, ym);
	        generate(xl, ym, xm, yh);
	        generate(xm, ym, xh, yh);
	    }
	
	    private float roughen(float v, int l, int h) {
	        return (float) (v + this.roughness_ * (Random.value * (h - l)));
	    }
	
	    public void printAsCSV() {
	        for (int i = 0; i < this.grid_.Length; i++) {
	            for (int j = 0; j < this.grid_[0].Length; j++) {
	                Debug.Log(this.grid_[i][j]);
	                Debug.Log(",");
	            }
	            Debug.Log("\n");
	        }
	    }
	
	    public float[][] getGrid() {
	        return this.grid_;
	    }
	
	    public float getGridValue(int x, int y) {
	        return this.grid_[x][y];
	    }
	
	    public bool[][] tobools() /*     */ {
	        int w = this.grid_.Length;
	        int h = this.grid_[0].Length;
	        bool[][] ret = new bool[w][];
	        for (int i = 0; i < w; i++) {
	            for (int j = 0; j < h; j++) {
	                ret[i][j] = (this.grid_[i][j] < 0.0F ? true : false);
	            }
	        }
	        return ret;
	    }
	
	    public float getMinimum() {
	        float minimum = (float)(3.4028235E+38);
	        for (int i = 0; i < this.grid_.Length; i++) {
	            float[] row = this.grid_[i];
	            for (int r = 0; r < row.Length; r++) {
	                if (row[r] < minimum) {
	                    minimum = row[r];
	                }
	            }
	        }
	        return minimum;
	    }
	
	    public float getMaximum() {
	        float maximum = (float)(1.4E-45);
	        for (int i = 0; i < this.grid_.Length; i++) {
	            float[] row = this.grid_[i];
	            for (int r = 0; r < row.Length; r++) {
	                if (row[r] > maximum) {
	                    maximum = row[r];
	                }
	            }
	        }
	        return maximum;
	    }
	}
}

