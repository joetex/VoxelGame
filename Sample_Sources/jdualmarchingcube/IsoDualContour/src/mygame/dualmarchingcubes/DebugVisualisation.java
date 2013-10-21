/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes;

import com.jme3.asset.AssetManager;
import com.jme3.material.Material;
import com.jme3.material.RenderState;
import com.jme3.math.ColorRGBA;
import com.jme3.math.Vector3f;
import com.jme3.scene.Geometry;
import com.jme3.scene.Mesh;
import com.jme3.scene.VertexBuffer;
import com.jme3.util.BufferUtils;
import java.util.LinkedList;
import java.util.List;

/**
 *
 * @author Karsten
 */
public class DebugVisualisation {

    public static Geometry visualizeDualGrid(DualGridGenerator dualGridGenerator,AssetManager assetManager) {
        LinkedList<Vector3f> vertices = new LinkedList();
        LinkedList<Integer> indices = new LinkedList();
        int baseIndex = 0;

        for (DualGridGenerator.DualCell d : dualGridGenerator.getDualCells()) {

            for (Vector3f pos : d.c) {
                vertices.add(pos);
            }

            addIndices(indices, baseIndex);

            baseIndex += 8;
        }

        Geometry g = new Geometry("Box");

        Mesh mesh = new Mesh();

        mesh.setBuffer(VertexBuffer.Type.Position, 3, BufferUtils.createFloatBuffer(vertices.toArray(new Vector3f[0])));
        mesh.setBuffer(VertexBuffer.Type.Index, 3, BufferUtils.createIntBuffer(toIntArray(indices)));

        mesh.updateBound();
        g.setMesh(mesh);
        g.setMaterial(getMaterial(assetManager, ColorRGBA.Blue));

        
        return g;
    }

    public static Geometry visualizeOctree(OctreeNode treeNode, AssetManager assetManager) {
        LinkedList<Vector3f> vertices = new LinkedList();
        LinkedList<Integer> indices = new LinkedList();

        visualizeOctree(treeNode, vertices, indices, 0);

        Geometry g = new Geometry("Box");

        Mesh mesh = new Mesh();

        mesh.setBuffer(VertexBuffer.Type.Position, 3, BufferUtils.createFloatBuffer(vertices.toArray(new Vector3f[0])));
        mesh.setBuffer(VertexBuffer.Type.Index, 3, BufferUtils.createIntBuffer(toIntArray(indices)));

        mesh.updateBound();
        g.setMesh(mesh);
        g.setMaterial(getMaterial(assetManager,ColorRGBA.Red));

        return g;
    }

    private static int visualizeOctree(OctreeNode treeNode,
       LinkedList<Vector3f> vertices, LinkedList<Integer> indices, int baseIndex) {

        Vector3f xWidth = new Vector3f(treeNode.getTo().x - treeNode.getFrom().x, 0, 0);
        Vector3f yWidth = new Vector3f(0, treeNode.getTo().y - treeNode.getFrom().y, 0);
        Vector3f zWidth = new Vector3f(0, 0, treeNode.getTo().z - treeNode.getFrom().z);

        int newindex = baseIndex;
        
        if (!treeNode.isSubdivided()) {

            vertices.add(treeNode.getFrom());
            vertices.add(treeNode.getFrom().add(xWidth));
            vertices.add(treeNode.getFrom().add(xWidth).add(zWidth));
            vertices.add(treeNode.getFrom().add(zWidth));
            vertices.add(treeNode.getFrom().add(yWidth));
            vertices.add(treeNode.getFrom().add(yWidth).add(xWidth));
            vertices.add(treeNode.getFrom().add(yWidth).add(xWidth).add(zWidth));
            vertices.add(treeNode.getFrom().add(yWidth).add(zWidth));

            /*
             vertices.add(treeNode.getFrom());
             vertices.add(treeNode.getCorner3());
             vertices.add(treeNode.getCorner4());
             vertices.add(treeNode.getCorner7());
             vertices.add(treeNode.getCorner1());
             vertices.add(treeNode.getCorner2());
             vertices.add(treeNode.getCorner5());
             vertices.add(treeNode.getTo());*/

            addIndices(indices, baseIndex);

            newindex += 8;

        } else {
            for (int i = 0; i < 8; i++) {
                newindex = visualizeOctree(treeNode.getChild(i), vertices, indices, newindex);
            }
        }

        return newindex;

    }
    
    
   private static void addIndices(LinkedList<Integer> indices, int baseIndex) {
        indices.add(baseIndex + 0);
        indices.add(baseIndex + 1);
        indices.add(baseIndex + 1);
        indices.add(baseIndex + 2);
        indices.add(baseIndex + 2);
        indices.add(baseIndex + 3);
        indices.add(baseIndex + 3);
        indices.add(baseIndex + 0);

       
        indices.add(baseIndex + 4);
        indices.add(baseIndex + 5);
        indices.add(baseIndex + 5);
        indices.add(baseIndex + 6);
        indices.add(baseIndex + 6);
        indices.add(baseIndex + 7);
        indices.add(baseIndex + 7);
        indices.add(baseIndex + 4);

        indices.add(baseIndex + 0);
        indices.add(baseIndex + 4);
        indices.add(baseIndex + 1);
        indices.add(baseIndex + 5);
        indices.add(baseIndex + 2);
        indices.add(baseIndex + 6);
        indices.add(baseIndex + 3);
        indices.add(baseIndex + 7);
    }
        

    private static int[] toIntArray(List<Integer> list) {
        int[] ret = new int[list.size()];
        int i = 0;
        for (Integer e : list) {
            ret[i++] = e.intValue();
        }
        return ret;
    }
    
    
    private static Material getMaterial(AssetManager assetManager, ColorRGBA color)
    {
        Material mat = new Material(assetManager, "Common/MatDefs/Misc/Unshaded.j3md");
        mat.getAdditionalRenderState().setWireframe(true);
        mat.setColor("Color", color);
        mat.getAdditionalRenderState().setFaceCullMode(RenderState.FaceCullMode.Off);
        
        return mat;
    }
    
}
