using System;
using UnityEngine;
using System.Threading;

namespace Night
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Chunk  {

	    /// Holds the error associated with this chunk.
	    private float error;
	    private Chunk[] children;
	    /// Flag whether this node will never be shown.
	    private bool invisible;
	    private bool isRoot = false;
	    /// To attach this node to.
	    //private Node node;
	    private ChunkTreeSharedData shared;
	    //private static ThreadPoolExecutor executor = new ThreadPoolExecutor(8, 10000, 100, TimeUnit.SECONDS, new ArrayBlockingQueue(1000));
	    private ChunkRequest lastChunkRequest = null;
	
		private Mesh mesh = null;
		
		private GameObject gameObj;
		
	    public Chunk() {
	    }
	
	    public void load(Vector3 from, Vector3 to, int level, ChunkParameters parameter) {
	        isRoot = true;
	        // Don't recreate the shared parameters on update.
	        if (parameter.updateFrom.Equals(Vector3.zero) && parameter.updateTo.Equals(Vector3.zero)) {
	            shared = new ChunkTreeSharedData();
	            shared.maxScreenSpaceError = parameter.maxScreenSpaceError;
	            shared.scale = parameter.scale;
	            
	            
	        }else{
	            parameter.updateRadius = Vector3.Distance(parameter.updateFrom, parameter.updateTo)/2;
	            parameter.updateCenter = parameter.updateFrom + ((parameter.updateTo - parameter.updateFrom) / 2);
	        }
	
	        doLoad(from, to, from, to, level, level, parameter);
	    }
	
	    private void doLoad(Vector3 from, Vector3 to, Vector3 totalFrom, Vector3 totalTo,
	            int level, int maxLevels, ChunkParameters parameter) {
	        // Handle the situation where we update an existing tree
	        if (parameter.updateFrom.Equals(Vector3.zero) == false || parameter.updateTo.Equals(Vector3.zero) == false) {
	            // Early out if an update of a part of the tree volume is going on and this chunk is outside of the area.
	            // Free memory from old mesh version
	            
	            float radius = Vector3.Distance(from,to)/2;
                Vector3 center = new Vector3(from.x + (to.x-from.x)/2,
                                from.y + (to.y-from.y)/2,
                                from.z + (to.z-from.z)/2);
	            
	            gameObj.transform.position = (to-from) / 2;
				
	            if(Vector3.Distance(center,parameter.updateCenter) > radius + parameter.updateRadius)
	            {
	                return;
	            }
	        }else{
	            //setMaterial(parameter.material);
				gameObj = new GameObject("Chunk");
				
				MeshRenderer meshRenderer = gameObj.GetComponent<MeshRenderer>();
				if( meshRenderer == null )
					meshRenderer = gameObj.AddComponent<MeshRenderer>();
			
					
				MeshCollider meshCollider = gameObj.GetComponent<MeshCollider>();
				if( meshCollider == null )
					meshCollider = gameObj.AddComponent<MeshCollider>();
			
				MeshFilter meshFilter = gameObj.GetComponent<MeshFilter>();
				if( meshFilter == null )
					meshFilter = gameObj.AddComponent<MeshFilter>();
					
				
				gameObj.transform.position = (to-from) / 2;//new Vector3(from.x + (to.x-from.x)/2,
                        //from.y + (to.y-from.y)/2,
                       // from.z + (to.z-from.z)/2);
	        }
	
	        
			
	        //if(mesh == null)
	        //    mesh = new Mesh();
	        //node = parent;
	        //parent.attachChild(this);
	        // Set to invisible for now.
	        invisible = true;
	
	        if (!contributesToVolumeMesh(from, to, parameter.source)) {
	            return;
	        }
	
	        loadChunk(from, to, totalFrom, totalTo, level, maxLevels, parameter);
	
	        loadChildren(from, to, totalFrom, totalTo, level, maxLevels, parameter);
	
	    }
	
	    private void loadChunk(Vector3 from, Vector3 to, Vector3 totalFrom, Vector3 totalTo,
	            int level, int maxLevels, ChunkParameters parameter) {
	        //node.attachChild(this);
	
	        if (parameter.createGeometryFromLevel == 0 || level <= parameter.createGeometryFromLevel) {
	            //new Request()
	
	            ChunkRequest req = new ChunkRequest();
	            req.totalFrom = totalFrom;
	            req.totalTo = totalTo;
	            req.parameter = parameter;
	            req.level = level;
	            req.maxLevels = maxLevels;
	            req.isUpdate = parameter.updateFrom.Equals(Vector3.zero) == false || parameter.updateTo.Equals(Vector3.zero) == false;
	
	            req.origin = this;
	            req.root = new OctreeNode(from, to);
	            req.mb = new MeshBuilder();
	            req.dualGridGenerator = new DualGridGenerator();
	
	            lastChunkRequest = req;
	
	            //executor.execute(req);
				req.run ();
				//ThreadPool.QueueUserWorkItem(new WaitCallback(req.run));
	
	        } else {
	            invisible = false;
	        }
	    }
	
	    private void loadChildren(Vector3 from, Vector3 to, Vector3 totalFrom, Vector3 totalTo,
	            int level, int maxLevels, ChunkParameters parameter) {
	        // Now recursively create the more detailed children
	        if (level > 2) {
	
	            //OctreeNode.getChildrenDimensions(from, to, newCenter, xWidth, yWidth, zWidth);
	            if (children == null) {
	
	                children = new Chunk[8];
	                for (int i = 0; i < 8; i++) {
	                    children[i] = new Chunk();
	                    children[i].shared = shared;
	                }
	            }
	            
	            float x  = (to.x - from.x) / 2.0f;
	            float y = (to.y - from.y) / 2.0f;
	            float z  = (to.z - from.z) / 2.0f;
	            Vector3 newCenter = new Vector3(x,y,z);
	            newCenter += from;
	            
	           /* Vector3 newCenter = new Vector3();
	            Vector3 xWidth = new Vector3();
	            Vector3 yWidth = new Vector3();
	            Vector3 zWidth = new Vector3();
	
	            OctreeNode.getChildrenDimensions(from, to, newCenter, xWidth, yWidth, zWidth);*/
	            
	            children[0].doLoad(from, newCenter, totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[1].doLoad(new Vector3(from.x+x,from.y,from.z), new Vector3(newCenter.x+x,newCenter.y,newCenter.z), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[2].doLoad(new Vector3(from.x+x,from.y,from.z+z), new Vector3(newCenter.x+x,newCenter.y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[3].doLoad(new Vector3(from.x,from.y,from.z+z), new Vector3(newCenter.x,newCenter.y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[4].doLoad(new Vector3(from.x,from.y+y,from.z), new Vector3(newCenter.x,newCenter.y+y,newCenter.z), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[5].doLoad(new Vector3(from.x+x,from.y+y,from.z), new Vector3(newCenter.x+x,newCenter.y+y,newCenter.z), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[6].doLoad(new Vector3(from.x+x,from.y+y,from.z+z), new Vector3(newCenter.x+x,newCenter.y+y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[7].doLoad(new Vector3(from.x,from.y+y,from.z+z), new Vector3(newCenter.x,newCenter.y+y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);
	
	            /*children[0].doLoad(parent, from, newCenter, totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[1].doLoad(parent, from.add(xWidth), newCenter.add(xWidth), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[2].doLoad(parent, from.add(xWidth).add(zWidth), newCenter.add(xWidth).add(zWidth), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[3].doLoad(parent, from.add(zWidth), newCenter.add(zWidth), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[4].doLoad(parent, from.add(yWidth), newCenter.add(yWidth), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[5].doLoad(parent, from.add(yWidth.add(xWidth)), newCenter.add(yWidth).add(xWidth), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[6].doLoad(parent, from.add(yWidth).add(xWidth).add(zWidth), newCenter.add(yWidth).add(xWidth).add(zWidth), totalFrom, totalTo, level - 1, maxLevels, parameter);
	            children[7].doLoad(parent, from.add(yWidth).add(zWidth), newCenter.add(yWidth).add(zWidth), totalFrom, totalTo, level - 1, maxLevels, parameter);*/
	            
	        }// Just load one child of the same size as the parent for the leafes because they actually don't need to be subdivided as they
	        // are all rendered anywa
	        else if (level > 1) {
	            if (children == null) {
	                children = new Chunk[1];
	                children[0] = new Chunk();
	                children[0].shared = shared;
	            }
	            children[0].doLoad(from, to, totalFrom, totalTo, level - 1, maxLevels, parameter);
	        }
	    }
	
	    private void prepareGeometry(ChunkRequest cr) {
	        OctreeNodeSplitPolicy policy = new OctreeNodeSplitPolicy(cr.parameter.source, cr.parameter.errorMultiplicator * cr.parameter.baseError);
	        error = (float) cr.level * cr.parameter.errorMultiplicator * cr.parameter.baseError;
	        cr.root.split(policy, cr.parameter.source, error);
	        float maxMSDistance = (float) cr.level * cr.parameter.errorMultiplicator * cr.parameter.baseError * cr.parameter.skirtFactor;
	        IsoSurface iso = new IsoSurface(cr.parameter.source);
	        cr.dualGridGenerator.generateDualGrid(cr.root, iso, cr.mb, maxMSDistance,
	                cr.totalFrom, cr.totalTo, false);   //<-----DualGridVisualization = false
	        
	        cr.mesh = cr.mb.generateMesh();
	        this.mesh = cr.mesh;
	        cr.isFinished = true;
	        //  loadGeometry(cr);
	    }
	
	    private void loadGeometry(ChunkRequest chunkRequest) {
	      //  invisible = (chunkRequest.mb.countVertices() == 0);
	        invisible = (mesh.triangles.Length <= 0);
	
	        // chunkRequest.origin.box = chunkRequest.mb.getBoundingBox();
	
	        /*if (!invisible) {
	            if (chunkRequest.isUpdate) {
	                node.detachChild(this);
	            }
	            //node.attachChild(this);
	         }*/
	
	        //node.detachChild(this); //<-------------
	
	       // setMesh(chunkRequest.mb.generateMesh());
	       setMesh(chunkRequest.mesh);
	        
	       // updateModelBound();
			
			
			
			//if( renderer.material != m_Material )
				//renderer.material = m_Material;
			
	    }
		
		private void setMesh(Mesh mesh) 
		{
			MeshRenderer meshRenderer = gameObj.GetComponent<MeshRenderer>();
			MeshCollider meshCollider = gameObj.GetComponent<MeshCollider>();
			MeshFilter meshFilter = gameObj.GetComponent<MeshFilter>();
				
			this.mesh = mesh;
			meshRenderer.material = new Material( Shader.Find("Nature/Terrain/Diffuse") );
			
			meshRenderer.material.SetTexture("MainTexture", TerrainManager.meshTexture);
			meshFilter.mesh = null;
			meshFilter.mesh = mesh;
			meshCollider.sharedMesh = mesh;
			
		}
		private void updateModelBound() {
			
		}
		
	    private class ChunkTreeSharedData {
	        /// The maximum accepted screen space error.
	
	        public float maxScreenSpaceError;
	        public float scale;
	        // bool octreeVisible;
	        /// Another visibility flag to be user setable.
	        public bool volumeVisible=true;
	        
	
	    }
	
	    private class ChunkRequest  {
	        /// The back lower left corner of the world.
	
	        public Vector3 totalFrom;
	        /// The front upper rightcorner of the world.
	        public Vector3 totalTo;
	        /// The parameters to use while loading.
	        public ChunkParameters parameter;
	        /// The current LOD level.
	        public int level;
	        /// The maximum amount of levels.
	        public int maxLevels;
	        /// The MeshBuilder to use.
	        public MeshBuilder mb;
	        /// The DualGridGenerator to use.
	        public DualGridGenerator dualGridGenerator;
	        /// The octree node to use.
	        public OctreeNode root;
	        /// The chunk which created this request.
	        public Chunk origin;
	        /// Whether this is an update of an existing tree
	        public bool isUpdate;
	        public bool isFinished = false;
	        
	        public Mesh mesh;
	
	        public void run() {
	            origin.prepareGeometry(this);
	            //executor.remove(this);
	        }
	    }
	
	    private bool contributesToVolumeMesh(Vector3 from, Vector3 to, VolumeSource src) {
	        Vector3 v3f = new Vector3(
	                (to.x - from.x) / 2.0f + from.x,
	                (to.y - from.y) / 2.0f + from.y,
	                (to.z - from.z) / 2.0f + from.z);
	
	        float centralValue = src.getValue(v3f);
			Vector3 diff = to-from;
	        return Math.Abs(centralValue) <= (diff.magnitude * 1.5f);
	    }
	
	    public void waitForGeometry() {
	        if (lastChunkRequest != null) {
	            if (lastChunkRequest.isFinished) {
	                loadGeometry(lastChunkRequest);
	                lastChunkRequest = null;
	            }
	        }
	
	        if (children != null) {
	            for (int i = 0; i < children.Length; i++) {
	                children[i].waitForGeometry();
	            }
	
	        }
	    }
	
	    public void frameStarted(Camera camera) {
	
	        if (invisible) {
	            return;
	        }
	
			if( !isRoot && this.mesh == null ) return;
			
	        // This might be a chunk on a lower LOD level without geometry, so lets just proceed here.
	        if (children != null) {
	            for (int i = 0; i < children.Length; i++) {
	                children[i].frameStarted(camera);
	            }
	            return;
	        }
	
	        if (camera == null) {
	            setChunkVisible(true, false);
	            return;
	        }
	
	
	         float viewportHeight = camera.GetScreenHeight();//.getHeight();
	        
	        float k = camera.nearClipPlane/(2.0f*camera.rect.y)*viewportHeight;
	
	        Vector3 camPos = camera.transform.position;//.getLocation();
	        float d = Vector3.Distance(camPos,mesh.bounds.center * shared.scale);
	        if (d < 1.0)
	        {
	            d = 1.0f;
	        }
	        
	        //setChunkVisible(false, true);
			//return;
	        float screenSpaceError = error / d * k;
	
	        if (screenSpaceError <= shared.maxScreenSpaceError / shared.scale) {
	            setChunkVisible(true, false);
	            if (children != null) {
	                for (int i = 0; i < children.Length; i++) {
	                    children[i].setChunkVisible(false, true);
	                }
	            }
	        } else {
	            setChunkVisible(false, false);
	
	            if (children != null)
	            {
	                for (int i = 0; i < children.Length; i++) {
	                    children[i].frameStarted(camera);
	                }
	
	            } else {
	                setChunkVisible(true, false);
	            }
	        }
	    }
		
	
	
	    private void setChunkVisible(bool visible, bool applyToChildren) {
	        if (invisible) {
	            return;
	        }
	        if (shared.volumeVisible) {
	            if (visible) {
					gameObj.renderer.enabled = true;
	                //setCullHint(CullHint.Never);
	               // node.attachChild(this);
	            } else {
	                //setCullHint(CullHint.Always);
	                gameObj.renderer.enabled = false;
	              //  node.detachChild(this);
	            }
	            //this.visible = visible;
	        }
	        /* if (octree != null) Debug Octree
	         {
	         octree.setVisible(shared.octreeVisible && visible);
	         }*/
	        /*  if (dualGrid != null)  Debug DualGrid
	         {
	         dualGrid.setVisible(shared.dualGridVisible && visible);
	         }*/
	        if (applyToChildren && children != null) {
	            for (int i = 0; i < children.Length; i++) {
	                children[i].setChunkVisible(visible, applyToChildren);
	            }
	        }
	    }
	    
	    
	    public static void closeExecutorThread()
	    {
	        //executor.shutdown();
	    }
	}
}

