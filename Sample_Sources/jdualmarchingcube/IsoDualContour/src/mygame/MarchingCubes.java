/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame;

import com.jme3.math.Vector3f;
import com.jme3.scene.Mesh;
import mygame.dualmarchingcubes.IsoSurface;
import mygame.dualmarchingcubes.MeshBuilder;
import mygame.dualmarchingcubes.VolumeSource;

/**
 *
 * @author Karsten
 */
public class MarchingCubes {
    
        /*
         * This is the normal marching cubes algorithm NOT dual marching cubes
         * 
         */
        public static Mesh createMeshMarchingCubes(Vector3f loc, VolumeSource source, Vector3f start, Vector3f end, Vector3f cubeSize) {

        MeshBuilder meshBuilder = new MeshBuilder();
        IsoSurface isoSurface = new IsoSurface(source);

        for (float i = start.x; i < end.x; i += cubeSize.x) {
            for (float j = start.y; j < end.y; j += cubeSize.y) {
                for (float k = 1; k < end.z; k += cubeSize.z) {

                    float[] val = {source.getValue(i, j, k),
                        source.getValue(i + cubeSize.x, j, k),
                        source.getValue(i + cubeSize.x, j, k + cubeSize.z),
                        source.getValue(i, j, k + cubeSize.z),
                        source.getValue(i, j + cubeSize.y, k),
                        source.getValue(i + cubeSize.x, j + cubeSize.y, k),
                        source.getValue(i + cubeSize.x, j + cubeSize.y, k + cubeSize.z),
                        source.getValue(i, j + cubeSize.y, k + cubeSize.z),};

                    Vector3f[] loc1 = {
                        new Vector3f(loc.x + i + 0, loc.y + j + 0, loc.z + k + 0),
                        new Vector3f(loc.x + i + cubeSize.x, loc.y  + j + 0, loc.z + k + 0),
                        new Vector3f(loc.x + i + cubeSize.x, loc.y  + j + 0, loc.z + k + cubeSize.z),
                        new Vector3f(loc.x + i + 0, loc.y  + j + 0, loc.z + k + cubeSize.z),
                        new Vector3f(loc.x + i + 0, loc.y  + j + cubeSize.y, loc.z + k + 0),
                        new Vector3f(loc.x + i + cubeSize.x, loc.y  + j + cubeSize.y, loc.z + k + 0),
                        new Vector3f(loc.x + i + cubeSize.x, loc.y  + j + cubeSize.y, loc.z + k + cubeSize.z),
                        new Vector3f(loc.x + i + 0, loc.y  + j + cubeSize.y, loc.z + k + cubeSize.z)
                    };


                    isoSurface.addMarchingCubesTriangles(loc1, val, null, meshBuilder);
                }
            }
        }

        return meshBuilder.generateMesh();
    }
}
