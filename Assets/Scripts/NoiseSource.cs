using System;
using UnityEngine;

namespace Night
{
	public class NoiseSource : VolumeSource {
	    //private SimplexNoise noise = new SimplexNoise();
		/// The texture width.
		private int width;
		/// The texture height.
		private int height;
		
		private int depth;
		private float maxClampedAbsoluteDensity = 5;
		/// The scale of the position based on the world width(x)/height(y)/depth(z).
		private Vector3 scale;
		/// The texture depth.
		
		
		public NoiseSource(Vector3 scale, int width, int height, int depth, float maxClampedAbsoluteDensity)
		{
		    this.maxClampedAbsoluteDensity=maxClampedAbsoluteDensity;
		    this.scale=scale;
		    
		    this.width=width;
		    this.height=height;
		    this.depth=depth;
		}
		
		
	    public override float getValue(Vector3 pos) {
	        return (float)SimplexNoise.noise(pos.x*0.01, pos.y*0.01, pos.z*0.01);
	    }
	
	    public override Vector3 getGradient(Vector3 p) {
	        Vector3 pos = p *= (0.02f);
	        
	        Vector3 rfNormal = new Vector3();
	        rfNormal.x = (float)(SimplexNoise.noise(pos.x-0.01, pos.y, pos.z) - SimplexNoise.noise(pos.x+0.01, pos.y, pos.z));
	        rfNormal.y = (float)(SimplexNoise.noise(pos.x, pos.y-0.01, pos.z) - SimplexNoise.noise(pos.x, pos.y+0.01, pos.z));
	        rfNormal.z = (float)(SimplexNoise.noise(pos.x, pos.y, pos.z-0.01) - SimplexNoise.noise(pos.x, pos.y, pos.z+0.01));
	        rfNormal.Normalize();
	        return rfNormal;
	    }
	
	    public override float getValue(float x, float y, float z) {
	        return (float)SimplexNoise.noise(x*0.01, y*0.01, z*0.01);
	    }
	
	    public override Vector3 getGradient(float x, float y, float z) {
	        x *= (float)0.02;
	        y *= (float)0.02;
	        z *= (float)0.02;
	        
	        Vector3 rfNormal = new Vector3();
	        rfNormal.x = (float)(SimplexNoise.noise(x-0.01, y, z) - SimplexNoise.noise(x+0.01, y, z));
	        rfNormal.y = (float)(SimplexNoise.noise(x, y-0.01, z) - SimplexNoise.noise(x, y+0.01, z));
	        rfNormal.z = (float)(SimplexNoise.noise(x, y, z-0.01) - SimplexNoise.noise(x, y, z+0.01));
	        rfNormal.Normalize();
	        return rfNormal;
	    }
	    		/**
		 * @return the width
		 */
		public int getWidth() {
		    return width;
		}
		
		/**
		 * @return the height
		 */
		public int getHeight() {
		    return height;
		}
		
		/**
		 * @return the depth
		 */
		public int getDepth() {
		    return depth;
		}
	    
	}
}

