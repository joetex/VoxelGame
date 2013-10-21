/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes;

import com.jme3.math.Ray;
import com.jme3.math.Vector3f;

/**
 *
 * @author Karsten
 */
public abstract class VolumeSource {
    public abstract float getValue(float x, float y, float z);
    public abstract Vector3f getGradient(float x, float y, float z);
    
    public abstract float getValue(Vector3f pos);
    public abstract Vector3f getGradient(Vector3f pos);
    
    
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
    public Vector3f getFirstRayIntersection(Ray ray, float scale,int maxIterations, float maxDistance)
    {
        Ray scaledRay = new Ray(ray.getOrigin().divide(scale), ray.getDirection());
        Vector3f start = getIntersectionStart(scaledRay, maxDistance);
        Vector3f cur = start.clone();
        Vector3f end = getIntersectionEnd(scaledRay, maxDistance);
     
        float startVal = getValue(start);
        Vector3f scaleSampleGradient = getGradient(start);
        Vector3f scaleSampleEnd = start.add(scaleSampleGradient.normalize());
        float scaleSample = getValue(scaleSampleEnd);
       // float densityScale = 1.0f / Math.abs(scaleSample - startVal) * 2.0f;
        float densityScale=10f;
        
        float densityCur = getValue(cur);
        Vector3f dir = scaledRay.getDirection().normalize();

        int count = 0;
        Vector3f prev, prevPrev;
        prevPrev = new Vector3f();
        prev = new Vector3f();
        boolean atEnd = false;
        float totalLength = start.distance(end);
        while (Math.abs(densityCur) > 0.01f && !atEnd)
        {
            cur.addLocal(dir.mult( -(densityCur / densityScale)));
            //cur += dir * -1.0 * (densityCur / densityScale);
            
            // Increase the scaling a bit if we jump forth and back due to bad depth data.
            /*if (cur.distance(prevPrev) < 0.0001f)
            {
                densityScale *= 2.0f;
            }*/
            prevPrev = prev;
            prev = cur;

            densityCur = getValue(cur);

            // Check if we are out of range
            if (start.distance(cur) >= totalLength) {
                System.out.println("MaxLength");
                atEnd = true;
            }

            // We have a limit here...
            count++;
            if (count > maxIterations)
            {
                System.out.println("Break!" + densityScale);
                break;
            }
        }

        if (Math.abs(densityCur) <= 0.01f)
        {
            return cur;
        }
        return null;

    }
     
    private Vector3f getIntersectionStart(Ray ray, float maxDistance)
    {
        return ray.getOrigin();
    }


    private Vector3f getIntersectionEnd(Ray ray, float maxDistance)
    {
        Vector3f dir = ray.getDirection().normalize();
        dir.multLocal(maxDistance);
        return ray.getOrigin().add(dir);
    }
}
