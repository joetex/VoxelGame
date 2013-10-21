using System;
using UnityEngine;

namespace Night
{
	public class OctreeNodeSplitPolicy {
    
	    private float maxCellSize;
	    private VolumeSource source;
	    
	    private float minSplitDistanceDiagonalFactor ;//= 1.5f;
	    
	    private Vector3[] const_position = {
	            new Vector3(0.5f,0.0f, 0.0f),
	            new Vector3(0.0f,0.0f, 0.5f),
	            new Vector3(0.5f,0.0f, 0.5f),
	            new Vector3(1.0f,0.0f, 0.5f),
	            new Vector3(0.5f,0.0f, 1.0f),
	            
	            new Vector3(0.0f,0.5f, 0.0f),
	            new Vector3(0.5f,0.5f, 0.0f),
	            new Vector3(1.0f,0.5f, 0.0f),
	            new Vector3(0.0f,0.5f, 0.5f),
	            new Vector3(0.5f,0.5f, 0.5f),
	            new Vector3(1.0f,0.5f, 0.5f),
	            new Vector3(0.0f,0.5f, 1.0f),
	            new Vector3(0.5f,0.5f, 1.0f),
	            new Vector3(1.0f,0.5f, 1.0f),
	            
	            new Vector3(0.5f,1.0f, 0.0f),
	            new Vector3(0.0f,1.0f, 0.5f),
	            new Vector3(0.5f,1.0f, 0.5f),
	            new Vector3(1.0f,1.0f, 0.5f),
	            new Vector3(0.5f,1.0f, 1.0f),
	    };//new Vector3[19];
	    
	    public OctreeNodeSplitPolicy(VolumeSource source, float maxCellSize) : this(source,maxCellSize,1.5f)
	    {
	        
	    }
	    
	    public OctreeNodeSplitPolicy(VolumeSource source, float maxCellSize, float minSplitDistanceDiagonalFactor)
	    {
	        this.source=source;
	        this.maxCellSize=maxCellSize;
	        this.minSplitDistanceDiagonalFactor=minSplitDistanceDiagonalFactor;
	        
	        //int next=0;
	        
	        /* Automatic
	        for(int x=0; x<=2; x++)
	        {
	            for(int y=0; y<=2; y++)
	            {
	                for(int z=0; z<=2; z++)
	                {
	                    if(x%2 != 0 || y%2 != 0 || z%2 != 0)
	                    {
	                        const_position[next] = new Vector3(x*0.5f,y*0.5f,z*0.5f);
	                        next++;
	                    }
	                }
	            }
	        }*/
	    }
	    
	    public bool doSplit(OctreeNode node, float geometricError)
	    {
	         // We have a highest resolution.
	        Vector3 from = node.getFrom();
	        Vector3 to = node.getTo();
	        
	        if(to.x - from.x <= maxCellSize)
	        {
	            return false;
	        }
	        
	        // Don't split if nothing is inside.
	        float centerValue = source.getValue(node.getCenter());
	        Vector3 centerGradient = source.getGradient(node.getCenter());
	        if (Math.Abs(centerValue) > Vector3.Distance (to,from) * minSplitDistanceDiagonalFactor)
	        {
	            node.setCenterGradient(centerGradient);
	            node.setCenterValue(centerValue);
	            return false;
	        }
	        
	        // Error metric of http://www.andrew.cmu.edu/user/jessicaz/publication/meshing/
	        float f000 = source.getValue(from);
	        float f001 = source.getValue(node.getCorner3());
	        float f010 = source.getValue(node.getCorner4());
	        float f011 = source.getValue(node.getCorner7());
	        float f100 = source.getValue(node.getCorner1());
	        float f101 = source.getValue(node.getCorner2());
	        float f110 = source.getValue(node.getCorner5());
	        float f111 = source.getValue(to);
	        
	       // Vector3[] gradients = new Vector3[19];
	      //  gradients[9] = centerGradient;
	        
	        Vector3[] position = {
	            node.getCenterBackBottom(),
	            node.getCenterLeftBottom(),
	            node.getCenterBottom(),
	            node.getCenterRightBottom(),
	            node.getCenterFrontBottom(),
	            
	            node.getCenterBackLeft(),
	            node.getCenterBack(),
	            node.getCenterBackRight(),
	            node.getCenterLeft(),
	            node.getCenter(),
	            node.getCenterRight(),
	            node.getCenterFrontLeft(),
	            node.getCenterFront(),
	            node.getCenterFrontRight(),
	            
	            node.getCenterBackTop(),
	            node.getCenterLeftTop(),
	            node.getCenterTop(),
	            node.getCenterRightTop(),
	            node.getCenterFrontTop()};
	                
	          
	        //Auto generation
	        //createPosition(from, to);
	        
	        float error = 0;
	        
	        for(int i=0;i<19;++i)
	        {
	            float  value = source.getValue(position[i]);
	            Vector3 gradient = source.getGradient(position[i]);
	            
	            float interpolated = interpolate(f000, f001, f010, f011, f100, f101, f110, f111, const_position[i]);
	            float gradientMagnitude = gradient.magnitude;
	            
	           if (gradientMagnitude < 1.192092896e-07F)
	            {
	                gradientMagnitude = 1.0f;
	            }
	            error += Math.Abs(value - interpolated) / gradientMagnitude;
	            
	           // System.out.println("Error: " + error);
	            if (error >= geometricError)
	            {
	                return true;
	            }        
	        }
	        
	        node.setCenterGradient(centerGradient);
	        node.setCenterValue(centerValue);
	        return false;
	    }
	
	    private Vector3[] createPosition(Vector3 from, Vector3 to)
	    {
	        Vector3[] position = new Vector3[19];
	        int next=0;
	        
	        for(int x=0; x<=2; x++)
	        {
	            for(int y=0; y<=2; y++)
	            {
	                for(int z=0; z<=2; z++)
	                {
	                    if(x%2 != 0 || y%2 != 0 || z%2 != 0)
	                    {
	                         Vector3 v3f = new Vector3();
	                        
	                        switch(x){
	                            case 0 : v3f.x = from.x; break;
	                            case 1 : v3f.x = from.x + (to.x - from.x) / 2.0f; break;
	                            case 2 : v3f.x = to.x; break;
	                        }
	                        
	                        switch(y){
	                            case 0 : v3f.y = from.y; break;
	                            case 1 : v3f.y = from.y + (to.y - from.y) / 2.0f; break;
	                            case 2 : v3f.y = to.y; break;
	                        }
	                        
	                        switch(z){
	                            case 0 : v3f.z = from.z; break;
	                            case 1 : v3f.z = from.z + (to.z - from.z) / 2.0f; break;
	                            case 2 : v3f.z = to.z; break;
	                        }
	                            
	                        position[next] = v3f;
	                        next++;
	                    }
	                }
	            }
	        }
	        
	        return position;
	    }
	    
	    
	    
	    /** Trilinear interpolation of a relative point.
	        @param f000
	            Value of the lower back left corner.
	        @param f001
	            Value of the lower front right corner.
	        @param f010
	            Value of the upper back left corner.
	        @param f011
	            Value of the upper front left corner.
	        @param f100
	            Value of the lower back right corner.
	        @param f101
	            Value of the lower back right corner.
	        @param f110
	            Value of the upper front right corner.
	        @param f111
	            Value of the upper front right corner.
	        @param position
	            The relative (0-1) position to interpolate.
	        @return
	            The interpolated value.
	        */
	    private float interpolate(float  f000, float  f001, float  f010,  float  f011,  
	            float  f100, float f101, float  f110, float  f111, Vector3 position)
	    {
	            float oneMinX = 1.0f - position.x;
	            float oneMinY = 1.0f - position.y;
	            float oneMinZ = 1.0f - position.z;
	            float oneMinXoneMinY = oneMinX * oneMinY;
	            float xOneMinY = position.x * oneMinY;
	            return oneMinZ * (f000 * oneMinXoneMinY
	                + f100 * xOneMinY
	                + f010 * oneMinX * position.y)
	                + position.z * (f001 * oneMinXoneMinY
	                + f101 * xOneMinY
	                + f011 * oneMinX * position.y)
	                + position.x * position.y * (f110 * oneMinZ
	                + f111 * position.z);    
	    }
	
	}
}

