/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes.source;

import com.jme3.math.Vector3f;
import mygame.Noise;
import mygame.dualmarchingcubes.VolumeSource;

/**
 *
 * @author Karsten
 */
public class NoiseSource extends VolumeSource {
    private Noise noise = new Noise();

    public float getValue(Vector3f pos) {
        return (float)noise.noise(pos.x*0.01, pos.y*0.01, pos.z*0.01);
    }

    public Vector3f getGradient(Vector3f p) {
        Vector3f pos = p.mult(0.02f);
        
        Vector3f rfNormal = new Vector3f();
        rfNormal.x = (float)(noise.noise(pos.x-0.01, pos.y, pos.z) - noise.noise(pos.x+0.01, pos.y, pos.z));
        rfNormal.y = (float)(noise.noise(pos.x, pos.y-0.01, pos.z) - noise.noise(pos.x, pos.y+0.01, pos.z));
        rfNormal.z = (float)(noise.noise(pos.x, pos.y, pos.z-0.01) - noise.noise(pos.x, pos.y, pos.z+0.01));
        rfNormal.normalizeLocal();
        return rfNormal;
    }

    public float getValue(float x, float y, float z) {
        return (float)noise.noise(x*0.01, y*0.01, z*0.01);
    }

    public Vector3f getGradient(float x, float y, float z) {
        x *= 0.02;
        y *= 0.02;
        z *= 0.02;
        
        Vector3f rfNormal = new Vector3f();
        rfNormal.x = (float)(noise.noise(x-0.01, y, z) - noise.noise(x+0.01, y, z));
        rfNormal.y = (float)(noise.noise(x, y-0.01, z) - noise.noise(x, y+0.01, z));
        rfNormal.z = (float)(noise.noise(x, y, z-0.01) - noise.noise(x, y, z+0.01));
        rfNormal.normalizeLocal();
        return rfNormal;
    }
    
    
}
