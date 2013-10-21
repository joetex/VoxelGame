/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame;

import com.jme3.asset.AssetManager;
import com.jme3.math.Vector3f;
import com.jme3.scene.Geometry;
import com.jme3.scene.Mesh;
import mygame.dualmarchingcubes.DebugVisualisation;
import mygame.dualmarchingcubes.DualGridGenerator;
import mygame.dualmarchingcubes.IsoSurface;
import mygame.dualmarchingcubes.MeshBuilder;
import mygame.dualmarchingcubes.OctreeNode;
import mygame.dualmarchingcubes.OctreeNodeSplitPolicy;
import mygame.dualmarchingcubes.VolumeSource;
import mygame.dualmarchingcubes.source.FloatGridSource;

/**
 *
 * @author Karsten
 */
public class TestGeometry extends Geometry {

    private boolean dualMarchingCubes;
    
    private OctreeNode octNode;
    private DualGridGenerator dualGridGenerator;
    
    private Geometry debugOctree;
    private Geometry debugDualGrid;
    
    private AssetManager assetManager;
    
    public TestGeometry(AssetManager assetManager)
    {
        this.assetManager=assetManager;
    }
    
    public void marchingCubes(FloatGridSource source, float cubeSize) {
        
        dualMarchingCubes=false;
        
        setMesh(MarchingCubes.createMeshMarchingCubes(Vector3f.ZERO, source, Vector3f.ZERO,
                new Vector3f(source.getWidth(), source.getHeight(), source.getDepth()), new Vector3f(cubeSize, cubeSize, cubeSize)));
    }

    public void dualMarchingCubes(FloatGridSource source, float geometricError, float maxCellSize,
            float minSplitDistanceDiagonalFactor, float maxMSDistance) {
        
        dualMarchingCubes=true;
        
        float v = source.getWidth();
        if (v < source.getHeight()) {
            v = source.getHeight();
        }
        if (v < source.getDepth()) {
            v = source.getDepth();
        }

        octNode = new OctreeNode(Vector3f.ZERO, new Vector3f(v, v, v));
        octNode.split(new OctreeNodeSplitPolicy(source, maxCellSize, minSplitDistanceDiagonalFactor), source, geometricError);

        IsoSurface isoSurface = new IsoSurface(source);
        MeshBuilder meshBuilder = new MeshBuilder();
        dualGridGenerator = new DualGridGenerator();
        dualGridGenerator.generateDualGrid(octNode, isoSurface, meshBuilder, maxMSDistance, Vector3f.ZERO, new Vector3f(v, v, v), true);

        mesh = meshBuilder.generateMesh();
    }
    
    
    public Geometry getDebugDualGrid()
    {
        return DebugVisualisation.visualizeDualGrid(dualGridGenerator, assetManager);
    }
    
    public Geometry getDebugOctree()
    {
        return DebugVisualisation.visualizeOctree(octNode, assetManager);
    }
    
    
    
}
