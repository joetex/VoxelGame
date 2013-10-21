using System;
using UnityEngine;

namespace Night
{
	public class LoadNoise {
    
	    public static void loadNoise(int locX, int locY, int locZ, int sizeX, int sizeY, int sizeZ, float roughness, FloatGridSource source) { 
			/*
	        OldNoise noise = new OldNoise(null, roughness, sizeX, sizeZ);
	        noise.initialise();
	        double gridMinimum = noise.getMinimum();
	        double gridLargestDifference = noise.getMaximum() - gridMinimum;
	        float[][] grid = noise.getGrid();
	
	        float[][] newGrid = new float[grid.Length][];
	
	        for (int x = 0; x < grid.Length; x++) {
				newGrid[x] = new float[grid[0].Length];
	            float[] row = grid[x];
	            for (int z = 0; z < row.Length; z++) {
	                float blockHeight = (float)(((row[z] - gridMinimum) / gridLargestDifference * sizeY) + 1);
	                
	                
	                newGrid[x][z] = blockHeight;
	            }
	        }
	*/
			SimplexNoise.Initialize();
			
			int xx,yy,zz;
	        for (int x = 0; x < sizeX; x++) {
	            //float[] row = grid[x];
	            for (int z = 0; z < sizeZ; z++) {
	                //int blockHeight = (int)newGrid[x][z];
	                
	               // for (int y = 0; y < blockHeight-2; y++) {
	                for (int y = 0; y < sizeY; y++) {
	                    xx = locX + x;
						yy = locY + y;
						zz = locZ + z;
	                    source.setVolumeGridValue(xx, yy, zz, SimplexNoise.octave_noise_3d(3,0.05f,0.025f,xx,yy,zz));//locX(float)blockHeight - (float)y));
	                    
	                }
	                
	            }
	        }
	    }
	}
}

