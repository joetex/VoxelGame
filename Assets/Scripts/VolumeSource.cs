using System;
using UnityEngine;

namespace Night
{
	public abstract class VolumeSource {
	    public abstract float getValue(float x, float y, float z);
	    public abstract Vector3 getGradient(float x, float y, float z);
	    
	    public abstract float getValue(Vector3 pos);
	    public abstract Vector3 getGradient(Vector3 pos);
	    
	    
	      /** Gets the first intersection of a ray with the volume.
	        If you are using this together with the VolumeChunk:
	        Beware of the possible scaling or other transformations you did on the Chunk! Do the inverse first
	        on the ray origin. Example of a scaling with the factor 10: ray.setOrigin(ray.getOrigin() / (Real)10.0);
	        * 
	        * return null means no intersection
	        * 
	        @param ray
	            The ray.
	        @param scale
	            The scaling of the volume compared to the world.
	        @param maxIterations
	            The maximum amount of iterations on the ray before giving up.
	        @param maxDistance
	            The maximum distance of the intersection point.
	        */
	    public Vector3 getFirstRayIntersection(Ray ray, float scale,int maxIterations, float maxDistance)
	    {
	        Ray scaledRay = new Ray(ray.origin / scale, ray.direction);
	        Vector3 start = getIntersectionStart(scaledRay, maxDistance);
	        Vector3 cur = start;
	        Vector3 end = getIntersectionEnd(scaledRay, maxDistance);
	     
	        //float startVal = getValue(start);
	        Vector3 scaleSampleGradient = getGradient(start);
			scaleSampleGradient.Normalize();
	        //Vector3 scaleSampleEnd = start + scaleSampleGradient;
	        //float scaleSample = getValue(scaleSampleEnd);
	       // float densityScale = 1.0f / Math.abs(scaleSample - startVal) * 2.0f;
	        float densityScale=10f;
	        
	        float densityCur = getValue(cur);
	        Vector3 dir = scaledRay.direction;
			dir.Normalize();
	
	        int count = 0;
	        //Vector3 prev;//, prevPrev;
	        //prevPrev = new Vector3();
	        //prev = new Vector3();
	        bool atEnd = false;
	        float totalLength = Vector3.Distance(start, end);
	        while (Math.Abs(densityCur) > 0.01f && !atEnd)
	        {
	            cur = cur + (dir * ( -(densityCur / densityScale)));
	            //cur += dir * -1.0 * (densityCur / densityScale);
	            
	            // Increase the scaling a bit if we jump forth and back due to bad depth data.
	            /*if (cur.distance(prevPrev) < 0.0001f)
	            {
	                densityScale *= 2.0f;
	            }*/
	            //prevPrev = prev;
	            //prev = cur;
	
	            densityCur = getValue(cur);
	
	            // Check if we are out of range
	            if (Vector3.Distance(start,cur) >= totalLength) {
	                Debug.Log("MaxLength");
	                atEnd = true;
	            }
	
	            // We have a limit here...
	            count++;
	            if (count > maxIterations)
	            {
	                Debug.Log("Break!" + densityScale);
	                break;
	            }
	        }
	
	        if (Math.Abs(densityCur) <= 0.01f)
	        {
	            return cur;
	        }
	        return Vector3.zero;
	
	    }
	     
	    private Vector3 getIntersectionStart(Ray ray, float maxDistance)
	    {
	        return ray.origin;
	    }
	
	
	    private Vector3 getIntersectionEnd(Ray ray, float maxDistance)
	    {
	        Vector3 dir = ray.direction;
			dir.Normalize();
	        dir = dir * maxDistance;
	        return ray.origin + dir;
	    }
	}
}

