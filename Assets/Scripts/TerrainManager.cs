using UnityEngine;
using System.Collections.Generic;
using Night;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainManager : MonoBehaviour {
	
	public static Texture meshTexture;
	public Texture MeshTexture{
		get{return meshTexture;}
		set{meshTexture=value;}
	}
	
	List<GameObject> m_GameObjects = new List<GameObject>();
	Chunk rootChunk;
	
	// Use this for initialization
	void Start () {
		SimplexNoise.Initialize();
		FloatGridSource source = new FloatGridSource(new Vector3(1f, 1f, 1f), 32, 16, 32, 1);
        LoadNoise.loadNoise(0, 0, 0, 32, 16, 32, 1f, (FloatGridSource)source);

        //rootNode.attachChild(terrainNode);
        //NoiseSource source = new NoiseSource(new Vector3(0.5f, 0.5f, 0.5f), 256, 128, 256, 0);
		
        //Mesh mesh = new Mesh();
        // System.out.println(mesh.getTriangleCount());
        
        // return;
        /*
         * Stuff not working yet
         * */        
        
         ChunkParameters parameter = new ChunkParameters();
         
         parameter.baseError=1.8f;
         parameter.createGeometryFromLevel = 1;
         parameter.errorMultiplicator=0.9f;
         parameter.maxScreenSpaceError = 30;
         parameter.scale = 1;
         parameter.skirtFactor = 0.7f;
         parameter.updateFrom = Vector3.zero;
         parameter.updateTo = Vector3.zero;
         parameter.source = source;
         //parameter.material = MeshTexture;
         parameter.updateRadius = -1;
         
         rootChunk = new Chunk();
         rootChunk.load(Vector3.zero, new Vector3(source.getWidth(),source.getHeight(),source.getDepth()), 1, parameter);
         //stateManager.attach(new ChunkAppState(chunk));
	}
	
	// Update is called once per frame
	void Update () {
		
		if( rootChunk != null ) {
			rootChunk.frameStarted(Camera.main);
			rootChunk.waitForGeometry();
		}
	}
}
