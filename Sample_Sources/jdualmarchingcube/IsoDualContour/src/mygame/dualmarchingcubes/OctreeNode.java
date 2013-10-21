/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes;

import com.jme3.math.Vector3f;

/**
 *
 * @author Karsten
 */
public class OctreeNode {

    /// The children of this node.
    private OctreeNode[] children;
    /// The back lower left corner of the cell.
    private Vector3f from;
    /// The front upper right corner of the cell.
    private Vector3f to;
    /// Value and gradient of the center.
    private Vector3f centerGradient;
    private float centerValue;
    private static float NEAR_FACTOR=2.0f;
    
    private Vector3f center;
    
    private boolean subdivided=false;

    public OctreeNode(Vector3f from, Vector3f to) {
        this.from = from;
        this.to = to;
        this.center = (from.add(to)).divide(2);
        
        centerGradient = Vector3f.ZERO;
        centerValue = 0;
    }

    public void split(OctreeNodeSplitPolicy splitPolicy, VolumeSource source, float geometricError) {
        if (splitPolicy.doSplit(this, geometricError)) {
            children = new OctreeNode[8];

            subdivided=true;
            
            float x  = (to.x - from.x) / 2.0f;
            float y = (to.y - from.y) / 2.0f;
            float z  = (to.z - from.z) / 2.0f;
            Vector3f newCenter = new Vector3f(x,y,z);
            newCenter.addLocal(from);
            
           // Vector3f xWidth = new Vector3f();
           // Vector3f yWidth = new Vector3f();
           // Vector3f zWidth = new Vector3f();

            //getChildrenDimensions(from, to, newCenter, xWidth, yWidth, zWidth);
            
            children[0] = new OctreeNode(from, newCenter);
            children[1] = new OctreeNode(new Vector3f(from.x+x,from.y,from.z), new Vector3f(newCenter.x+x,newCenter.y,newCenter.z));
            children[2] = new OctreeNode(new Vector3f(from.x+x,from.y,from.z+z), new Vector3f(newCenter.x+x,newCenter.y,newCenter.z+z));
            children[3] = new OctreeNode(new Vector3f(from.x,from.y,from.z+z), new Vector3f(newCenter.x,newCenter.y,newCenter.z+z));
            children[4] = new OctreeNode(new Vector3f(from.x,from.y+y,from.z), new Vector3f(newCenter.x,newCenter.y+y,newCenter.z));
            children[5] = new OctreeNode(new Vector3f(from.x+x,from.y+y,from.z), new Vector3f(newCenter.x+x,newCenter.y+y,newCenter.z));
            children[6] = new OctreeNode(new Vector3f(from.x+x,from.y+y,from.z+z), new Vector3f(newCenter.x+x,newCenter.y+y,newCenter.z+z));
            children[7] = new OctreeNode(new Vector3f(from.x,from.y+y,from.z+z), new Vector3f(newCenter.x,newCenter.y+y,newCenter.z+z));

            
           /* children[1] = new OctreeNode(from.add(xWidth), newCenter.add(xWidth));
            children[2] = new OctreeNode(from.add(xWidth).add(zWidth), newCenter.add(xWidth).add(zWidth));
            children[3] = new OctreeNode(from.add(zWidth), newCenter.add(zWidth));
            children[4] = new OctreeNode(from.add(yWidth), newCenter.add(yWidth));
            children[5] = new OctreeNode(from.add(yWidth).add(xWidth), newCenter.add(yWidth).add(xWidth));
            children[6] = new OctreeNode(from.add(yWidth).add(xWidth).add(zWidth), newCenter.add(yWidth).add(xWidth).add(zWidth));
            children[7] = new OctreeNode(from.add(yWidth).add(zWidth), newCenter.add(yWidth).add(zWidth));*/

            for (int i = 0; i < 8; i++) {
                children[i].split(splitPolicy, source, geometricError);
            }

             
        } else {
            if (centerValue == 0 && centerGradient.equals(Vector3f.ZERO)) {
                centerGradient = source.getGradient(getCenter());
                centerValue = source.getValue(getCenter());
            }
        }

    }

    /*private void getChildrenDimensions(Vector3f center, Vector3f width, Vector3f height, Vector3f depth) {
        center.x = (to.x - from.x) / 2.0f;
        center.y = (to.y - from.y) / 2.0f;
        center.z = (to.z - from.z) / 2.0f;

        width.x = center.x;
        width.y = 0.0f;
        width.z = 0.0f;
        height.x = 0.0f;
        height.y = center.y;
        height.z = 0.0f;
        depth.x = 0.0f;
        depth.y = 0.0f;
        depth.z = center.z;
        center.addLocal(from);
    }*/
    
    
     public static void getChildrenDimensions(Vector3f from, Vector3f to, Vector3f center, Vector3f width, Vector3f height, Vector3f depth)
        {
            center.x = (to.x - from.x) / 2.0f;
            center.y = (to.y - from.y) / 2.0f;
            center.z = (to.z - from.z) / 2.0f;
        
            width.x = center.x;
            width.y = 0.0f;
            width.z = 0.0f;
            height.x = 0.0f;
            height.y = center.y;
            height.z = 0.0f;
            depth.x = 0.0f;
            depth.y = 0.0f;
            depth.z = center.z;
            center.addLocal(from);
        }
    

    public boolean isSubdivided() {
        return subdivided;
    }

    public OctreeNode getChild(int i) {
        return children[i];
    }

    public Vector3f getFrom() {
        return from;
    }

    public Vector3f getTo() {
        return to;
    }

    public Vector3f getCenterGradient() {
        return centerGradient;
    }

    public float getCenterValue() {
        return centerValue;
    }

    public Vector3f getCenter() {
        return center;
    }

    public void setCenterValue(float value) {
        centerValue = value;
    }

    public void setCenterGradient(Vector3f gradient) {
        centerGradient = gradient;
    }

    public Vector3f getCorner1() {
        return new Vector3f(to.x, from.y, from.z);
    }

    public Vector3f getCorner2() {
        return new Vector3f(to.x, from.y, to.z);
    }

    public Vector3f getCorner3() {
        return new Vector3f(from.x, from.y, to.z);
    }

    public Vector3f getCorner4() {
        return new Vector3f(from.x, to.y, from.z);
    }

    public Vector3f getCorner5() {
        return new Vector3f(to.x, to.y, from.z);
    }

    public Vector3f getCorner7() {
        return new Vector3f(from.x, to.y, to.z);
    }

    public boolean isIsoSurfaceNear() {
        if (centerValue == 0.0) {
            return true;
        }
        
      /*  float diffX = to.x-from.x;
        float diffY = to.y-from.y;
        float diffZ = to.z-from.z;
        return Math.abs(centerValue) < Math.sqrt(diffX*diffX +diffY*diffY + diffZ*diffZ) * NEAR_FACTOR;*/
        
        return Math.abs(centerValue) < (from.subtract(to)).length() * NEAR_FACTOR;
    }

    boolean isBorderLeft(OctreeNode root) {
        return from.x == root.from.x;
    }
    
    boolean isBorderRight(OctreeNode root) {
        return to.x == root.to.x;
    }
    
    boolean isBorderBottom(OctreeNode root) {
        return from.y == root.from.y;
    }
    
    boolean isBorderTop(OctreeNode root) {
        return to.y == root.to.y;
    }
    
    boolean isBorderBack(OctreeNode root) {
        return from.z == root.from.z;
    }
    
    boolean isBorderFront(OctreeNode root) {
        return to.z == root.to.z;
    }
    
    
    
    
    
    
    
    
    
    
     public Vector3f getCenterBack() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, from.y + (to.y - from.y) / 2.0f, from.z);
     }

     public Vector3f getCenterFront() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, from.y + (to.y - from.y) / 2.0f, to.z);
     }

     public Vector3f getCenterLeft() {
     return new Vector3f(from.x, from.y + (to.y - from.y) / 2.0f, from.z + (to.z - from.z) / 2.0f);
     }

     public Vector3f getCenterRight() {
     return new Vector3f(to.x, from.y + (to.y - from.y) / 2.0f, from.z + (to.z - from.z) / 2.0f);
     }

     public Vector3f getCenterTop() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, to.y, from.z + (to.z - from.z) / 2.0f);
     }

     public Vector3f getCenterBottom() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, from.y, from.z + (to.z - from.z) / 2.0f);
     }

     public Vector3f getCenterBackTop() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, to.y, from.z);
     }

     public Vector3f getCenterBackBottom() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, from.y, from.z);
     }

     public Vector3f getCenterFrontTop() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, to.y, to.z);
     }

     public Vector3f getCenterFrontBottom() {
     return new Vector3f(from.x + (to.x - from.x) / 2.0f, from.y, to.z);
     }
     
      public Vector3f getCenterLeftTop() {
     return new Vector3f(from.x, to.y, from.z + (to.z - from.z) / 2.0f);
     }
      
     public Vector3f getCenterLeftBottom() {
     return new Vector3f(from.x, from.y, from.z + (to.z - from.z) / 2.0f);
     }
     
     public Vector3f getCenterRightTop() {
     return new Vector3f(to.x, to.y, from.z + (to.z - from.z) / 2.0f);
     }
     
     public Vector3f getCenterRightBottom() {
     return new Vector3f(to.x, from.y, from.z + (to.z - from.z) / 2.0f);
     }
     
    public Vector3f getCenterBackLeft() {
     return new Vector3f(from.x, from.y + (to.y - from.y) / 2.0f, from.z);
     }
    
     public Vector3f getCenterFrontLeft() {
     return new Vector3f(from.x, from.y + (to.y - from.y) / 2.0f, to.z);
     }
     
    public Vector3f getCenterBackRight() {
     return new Vector3f(to.x, from.y + (to.y - from.y) / 2.0f, from.z);
     }
    
    public Vector3f getCenterFrontRight() {
     return new Vector3f(to.x, from.y + (to.y - from.y) / 2.0f, to.z);
     }
}
