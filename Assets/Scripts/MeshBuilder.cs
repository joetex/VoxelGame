using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace Night
{
	public class MeshBuilder {
	
	    private Dictionary<Vector3, int> indexMap;
	    private List<Vector3> verticesPosition;
	    private List<Vector3> verticesNormal;
	    private List<int> indices;
	
	    public MeshBuilder() {
	        indexMap = new Dictionary<Vector3, int>();
	        verticesPosition = new List<Vector3>();
	        verticesNormal = new List<Vector3>();
	        indices = new List<int>();
	    }
	
	    public void addVertex(Vector3 position, Vector3 normal) {
	        int i = 0;
	        //int index = indexMap.get(position);
			//int index = indexMap.ContainsValue(position);
	        if (!indexMap.ContainsKey(position)) {
	            i = verticesPosition.Count;
	            indexMap.Add(position, i);
	            verticesPosition.Add(position);
	            verticesNormal.Add(normal);
	
	            // Update bounding box
	        } else {
				indexMap.TryGetValue(position, out i);
	            //i = index;
	        }
	
	        indices.Add(i);
	    }
	
	    public Mesh generateMesh() {
			
	        Mesh mesh = new Mesh();
	
	        //mesh.setBuffer(VertexBuffer.Type.Position, 3, BufferUtils.createFloatBuffer(verticesPosition.toArray(new Vector3[0])));
	        //mesh.setBuffer(VertexBuffer.Type.Normal, 3, BufferUtils.createFloatBuffer(verticesNormal.toArray(new Vector3[0])));
	        // mesh.setBuffer(VertexBuffer.Type.TexCoord, 2, BufferUtils.createFloatBuffer(texcoords.toArray(new Vector2f[0])));
	
	       // mesh.setBuffer(VertexBuffer.Type.Index, 3, BufferUtils.createIntBuffer(toIntArray(indices)));
	       // mesh.setBuffer(VertexBuffer.Type.Index, 3, listToBuffer(indices));

			int[] triangles = new int[verticesPosition.Count];
			Vector3[] vertices = new Vector3[verticesPosition.Count];
			Color[] colors = new Color[verticesPosition.Count];
			Vector2[] uvCoords = new Vector2[verticesPosition.Count];
			float UVScaleFactor = (1.0f / 64);//(float)(verts.Count-1));
			
			vertices = verticesPosition.ToArray();
			triangles = indices.ToArray();
			
			//Debug.Log ( "Vertices : " + verts.Count);
			for( int i=0; i<verticesPosition.Count; i++ )
			{
				colors[i] = new Color(1,0,0,1);
				uvCoords[i] = new Vector3(vertices[i].x*UVScaleFactor, vertices[i].z*UVScaleFactor);
			}
	       
	        mesh.Clear ();
			mesh.vertices = vertices;//VoxelManager.m_Vertices;
			mesh.triangles = triangles;
			mesh.normals = verticesNormal.ToArray ();
			mesh.colors = colors;
			mesh.uv = uvCoords;
			mesh.name = "Voxel Mesh";
			//m_Mesh.renderer.material = this.m_Material;
			
			
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();
			
	        //Clear All
	        indexMap = new Dictionary<Vector3,int>();
	        verticesPosition = new List<Vector3>();
	        verticesNormal = new List<Vector3>();
	        indices = new List<int>();
	        
	
	        return mesh;
	        
	    }
	    /*
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
	    */
	    public int countVertices()
	    {
	        return verticesPosition.Count;
	    }
	}
}

