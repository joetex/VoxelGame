/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes;

import com.jme3.math.Vector2f;
import com.jme3.math.Vector3f;
import com.jme3.scene.Mesh;
import com.jme3.scene.VertexBuffer;
import com.jme3.util.BufferUtils;
import java.nio.IntBuffer;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;

/**
 *
 * @author Karsten
 */
public class MeshBuilder {

    private HashMap<Vector3f, Integer> indexMap;
    private LinkedList<Vector3f> verticesPosition;
    private LinkedList<Vector3f> verticesNormal;
    private LinkedList<Integer> indices;

    public MeshBuilder() {
        indexMap = new HashMap();
        verticesPosition = new LinkedList();
        verticesNormal = new LinkedList();
        indices = new LinkedList();
    }

    public void addVertex(Vector3f position, Vector3f normal) {
        int i = 0;
        Integer index = indexMap.get(position);

        if (index == null) {
            i = verticesPosition.size();
            indexMap.put(position, i);
            verticesPosition.add(position);
            verticesNormal.add(normal);

            // Update bounding box
        } else {
            i = index;
        }

        indices.add(i);
    }

    public Mesh generateMesh() {
        Mesh mesh = new Mesh();

        mesh.setBuffer(VertexBuffer.Type.Position, 3, BufferUtils.createFloatBuffer(verticesPosition.toArray(new Vector3f[0])));
        mesh.setBuffer(VertexBuffer.Type.Normal, 3, BufferUtils.createFloatBuffer(verticesNormal.toArray(new Vector3f[0])));
        // mesh.setBuffer(VertexBuffer.Type.TexCoord, 2, BufferUtils.createFloatBuffer(texcoords.toArray(new Vector2f[0])));

       // mesh.setBuffer(VertexBuffer.Type.Index, 3, BufferUtils.createIntBuffer(toIntArray(indices)));
        mesh.setBuffer(VertexBuffer.Type.Index, 3, listToBuffer(indices));

        mesh.updateBound();
        
        //Clear All
        indexMap = new HashMap();
        verticesPosition = new LinkedList();
        verticesNormal = new LinkedList();
        indices = new LinkedList();
        

        return mesh;
    }
    
    public IntBuffer listToBuffer(List<Integer> list)
    {
        IntBuffer buff = BufferUtils.createIntBuffer(list.size());
        buff.clear();
        for (Integer e : list) {
            buff.put(e.intValue());
        }
        buff.flip();
        return buff;
    }

    int[] toIntArray(List<Integer> list) {
        int[] ret = new int[list.size()];
        int i = 0;
        for (Integer e : list) {
            ret[i++] = e.intValue();
        }
        return ret;
    }
    
    public int countVertices()
    {
        return verticesPosition.size();
    }
}
