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
public class BoxSource extends VolumeSource {

    Vector3f center = new Vector3f(50, 50, 50);

    public float getValue(float x, float y, float z) {
        if (Math.abs(center.x - x) > 40) {
            return -1;
        }

        if (Math.abs(center.y - y) > 40) {
            return -1;
        }

        if (Math.abs(center.z - z) > 40) {
            return -1;
        }

        return 1;
    }

    public Vector3f getGradient(float x, float y, float z) {
        return Vector3f.UNIT_X;
    }

    public float getValue(Vector3f pos) {
        return getValue(pos.x, pos.y, pos.z);
    }

    public Vector3f getGradient(Vector3f pos) {
        return Vector3f.UNIT_X;
    }
}
