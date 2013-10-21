/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes.source;

import com.jme3.math.Vector3f;
import mygame.dualmarchingcubes.VolumeSource;

/**
 *
 * @author Karsten
 */
public class SphereSource extends VolumeSource {

    private Vector3f center = new Vector3f(50,50,50);
    private float size=45;
    
    public float getValue(Vector3f pos) {
        return size-pos.distance(center);
    }

    public Vector3f getGradient(Vector3f pos) {
        
        Vector3f rfNormal = pos.subtract(center);
        
        rfNormal.normalizeLocal();
        return rfNormal;
    }

    public float getValue(float x, float y, float z) {
        return size-center.distance(new Vector3f(x,y,z));
    }

    public Vector3f getGradient(float x, float y, float z) {

        Vector3f pos = new Vector3f(x,y,z);
        
        Vector3f rfNormal = pos.subtract(center);

        rfNormal.normalizeLocal();
        return rfNormal;
    }
}
