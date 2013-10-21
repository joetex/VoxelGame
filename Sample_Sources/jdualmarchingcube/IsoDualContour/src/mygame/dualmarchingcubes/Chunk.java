/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes;

import com.jme3.math.FastMath;
import com.jme3.math.Vector3f;
import com.jme3.renderer.Camera;
import com.jme3.scene.Geometry;
import com.jme3.scene.Mesh;
import com.jme3.scene.Node;
import java.util.concurrent.ArrayBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

/**
 *
 * @author Karsten
 */
public class Chunk extends Geometry {

    /// Holds the error associated with this chunk.
    private float error;
    private Chunk children[];
    /// Flag whether this node will never be shown.
    private boolean invisible;
    private boolean isRoot = false;
    /// To attach this node to.
    private Node node;
    private ChunkTreeSharedData shared;
    private static ThreadPoolExecutor executor = new ThreadPoolExecutor(8, 10000, 100, TimeUnit.SECONDS, new ArrayBlockingQueue(1000));
    private ChunkRequest lastChunkRequest = null;

    public Chunk() {
    }

    public void load(Node parent, Vector3f from, Vector3f to, int level, ChunkParameters parameter) {
        isRoot = true;
        // Don't recreate the shared parameters on update.
        if (parameter.updateFrom.equals(Vector3f.ZERO) && parameter.updateTo.equals(Vector3f.ZERO)) {
            shared = new ChunkTreeSharedData();
            shared.maxScreenSpaceError = parameter.maxScreenSpaceError;
            shared.scale = parameter.scale;
            
            
        }else{
            parameter.updateRadius = parameter.updateFrom.distance(parameter.updateTo)/2;
            parameter.updateCenter =  parameter.updateFrom.add((parameter.updateTo.subtract(parameter.updateFrom)).divide(2));
        }

        doLoad(parent, from, to, from, to, level, level, parameter);
    }

    private void doLoad(Node parent, Vector3f from, Vector3f to, Vector3f totalFrom, Vector3f totalTo,
            int level, int maxLevels, ChunkParameters parameter) {
        // Handle the situation where we update an existing tree
        if (parameter.updateFrom.equals(Vector3f.ZERO) == false || parameter.updateTo.equals(Vector3f.ZERO) == false) {
            // Early out if an update of a part of the tree volume is going on and this chunk is outside of the area.
            // Free memory from old mesh version
            
            float radius = from.distance(to)/2;
                        Vector3f center = new Vector3f(from.x + (to.x-from.x)/2,
                                        from.y + (to.y-from.y)/2,
                                        from.z + (to.z-from.z)/2);
            
            
            if(center.distance(parameter.updateCenter) > radius + parameter.updateRadius)
            {
                return;
            }
        }else{
            setMaterial(parameter.material);
        }

        
        if(mesh == null)
            mesh = new Mesh();
        node = parent;
        parent.attachChild(this);
        // Set to invisible for now.
        invisible = true;

        if (!contributesToVolumeMesh(from, to, parameter.source)) {
            return;
        }

        loadChunk(parent, from, to, totalFrom, totalTo, level, maxLevels, parameter);

        loadChildren(parent, from, to, totalFrom, totalTo, level, maxLevels, parameter);

    }

    private void loadChunk(Node parent, Vector3f from, Vector3f to, Vector3f totalFrom, Vector3f totalTo,
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
            req.isUpdate = parameter.updateFrom.equals(Vector3f.ZERO) == false || parameter.updateTo.equals(Vector3f.ZERO) == false;

            req.origin = this;
            req.root = new OctreeNode(from, to);
            req.mb = new MeshBuilder();
            req.dualGridGenerator = new DualGridGenerator();

            lastChunkRequest = req;

            executor.execute(req);

        } else {
            invisible = false;
        }
    }

    private void loadChildren(Node parent, Vector3f from, Vector3f to, Vector3f totalFrom, Vector3f totalTo,
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
            Vector3f newCenter = new Vector3f(x,y,z);
            newCenter.addLocal(from);
            
           /* Vector3f newCenter = new Vector3f();
            Vector3f xWidth = new Vector3f();
            Vector3f yWidth = new Vector3f();
            Vector3f zWidth = new Vector3f();

            OctreeNode.getChildrenDimensions(from, to, newCenter, xWidth, yWidth, zWidth);*/
            
            children[0].doLoad(parent,from, newCenter, totalFrom, totalTo, level - 1, maxLevels, parameter);
            children[1].doLoad(parent,new Vector3f(from.x+x,from.y,from.z), new Vector3f(newCenter.x+x,newCenter.y,newCenter.z), totalFrom, totalTo, level - 1, maxLevels, parameter);
            children[2].doLoad(parent,new Vector3f(from.x+x,from.y,from.z+z), new Vector3f(newCenter.x+x,newCenter.y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);
            children[3].doLoad(parent,new Vector3f(from.x,from.y,from.z+z), new Vector3f(newCenter.x,newCenter.y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);
            children[4].doLoad(parent,new Vector3f(from.x,from.y+y,from.z), new Vector3f(newCenter.x,newCenter.y+y,newCenter.z), totalFrom, totalTo, level - 1, maxLevels, parameter);
            children[5].doLoad(parent,new Vector3f(from.x+x,from.y+y,from.z), new Vector3f(newCenter.x+x,newCenter.y+y,newCenter.z), totalFrom, totalTo, level - 1, maxLevels, parameter);
            children[6].doLoad(parent,new Vector3f(from.x+x,from.y+y,from.z+z), new Vector3f(newCenter.x+x,newCenter.y+y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);
            children[7].doLoad(parent,new Vector3f(from.x,from.y+y,from.z+z), new Vector3f(newCenter.x,newCenter.y+y,newCenter.z+z), totalFrom, totalTo, level - 1, maxLevels, parameter);

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
            children[0].doLoad(node, from, to, totalFrom, totalTo, level - 1, maxLevels, parameter);
        }
    }

    private void prepareGeometry(ChunkRequest cr) {
        OctreeNodeSplitPolicy policy = new OctreeNodeSplitPolicy(cr.parameter.source, cr.parameter.errorMultiplicator * cr.parameter.baseError);
        error = (float) cr.level * cr.parameter.errorMultiplicator * cr.parameter.baseError;
        cr.root.split(policy, cr.parameter.source, error);
        float maxMSDistance = (float) cr.level * cr.parameter.errorMultiplicator * cr.parameter.baseError * cr.parameter.skirtFactor;
        IsoSurface is = new IsoSurface(cr.parameter.source);
        cr.dualGridGenerator.generateDualGrid(cr.root, is, cr.mb, maxMSDistance,
                cr.totalFrom, cr.totalTo, false);   //<-----DualGridVisualization = false
        
        cr.mesh = cr.mb.generateMesh();
        
        cr.isFinished = true;
        //  loadGeometry(cr);
    }

    private void loadGeometry(ChunkRequest chunkRequest) {
      //  invisible = (chunkRequest.mb.countVertices() == 0);
        invisible = (chunkRequest.mesh.getTriangleCount() <= 0);

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
        updateModelBound();
    }

    private class ChunkTreeSharedData {
        /// The maximum accepted screen space error.

        float maxScreenSpaceError;
        float scale;
        // boolean octreeVisible;
        /// Another visibility flag to be user setable.
        boolean volumeVisible=true;
        

    }

    private class ChunkRequest implements Runnable {
        /// The back lower left corner of the world.

        Vector3f totalFrom;
        /// The front upper rightcorner of the world.
        Vector3f totalTo;
        /// The parameters to use while loading.
        ChunkParameters parameter;
        /// The current LOD level.
        int level;
        /// The maximum amount of levels.
        int maxLevels;
        /// The MeshBuilder to use.
        MeshBuilder mb;
        /// The DualGridGenerator to use.
        DualGridGenerator dualGridGenerator;
        /// The octree node to use.
        OctreeNode root;
        /// The chunk which created this request.
        Chunk origin;
        /// Whether this is an update of an existing tree
        boolean isUpdate;
        boolean isFinished = false;
        
        Mesh mesh;

        public void run() {
            origin.prepareGeometry(this);
            executor.remove(this);
        }
    }

    private boolean contributesToVolumeMesh(Vector3f from, Vector3f to, VolumeSource src) {
        Vector3f v3f = new Vector3f(
                (to.x - from.x) / 2.0f + from.x,
                (to.y - from.y) / 2.0f + from.y,
                (to.z - from.z) / 2.0f + from.z);

        float centralValue = src.getValue(v3f);
        return Math.abs(centralValue) <= (to.subtract(from)).length() * 1.5f;
    }

    public void waitForGeometry() {
        if (lastChunkRequest != null) {
            if (lastChunkRequest.isFinished) {
                loadGeometry(lastChunkRequest);
                lastChunkRequest = null;
            }
        }

        if (children != null) {
            for (int i = 0; i < children.length; i++) {
                children[i].waitForGeometry();
            }

        }
    }

    public void frameStarted(Camera camera) {

        if (invisible) {
            return;
        }

        // This might be a chunk on a lower LOD level without geometry, so lets just proceed here.
        if (mesh.getTriangleCount() <= 0 && children != null) {
            for (int i = 0; i < children.length; i++) {
                children[i].frameStarted(camera);
            }
            return;
        }

        if (camera == null) {
            setChunkVisible(true, false);
            return;
        }


         float viewportHeight = camera.getHeight();
        
        float k = camera.getFrustumNear()/(2.0f*camera.getFrustumTop())*viewportHeight;

        Vector3f camPos = camera.getLocation();
        float d = camPos.distance(getModelBound().getCenter().mult(shared.scale));
        if (d < 1.0)
        {
            d = 1.0f;
        }
        
        
        float screenSpaceError = error / d * k;

        if (screenSpaceError <= shared.maxScreenSpaceError / shared.scale) {
            setChunkVisible(true, false);
            if (children != null) {
                for (int i = 0; i < children.length; i++) {
                    children[i].setChunkVisible(false, true);
                }
            }
        } else {
            setChunkVisible(false, false);

            if (children != null)
            {
                for (int i = 0; i < children.length; i++) {
                    children[i].frameStarted(camera);
                }

            } else {
                setChunkVisible(true, false);
            }
        }
    }

    private void setChunkVisible(boolean visible, boolean applyToChildren) {
        if (invisible) {
            return;
        }
        if (shared.volumeVisible) {
            if (visible) {
                setCullHint(CullHint.Never);
               // node.attachChild(this);
            } else {
                setCullHint(CullHint.Always);
                
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
            for (int i = 0; i < children.length; i++) {
                children[i].setChunkVisible(visible, applyToChildren);
            }
        }
    }
    
    
    public static void closeExecutorThread()
    {
        executor.shutdown();
    }
}
