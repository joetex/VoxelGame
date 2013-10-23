using System;
using UnityEngine;

namespace Night
{

	public class OctreeNode {

	    /// The children of this node.
	    private OctreeNode[] children;
	    /// The back lower left corner of the cell.
	    private Vector3 from;
	    /// The front upper right corner of the cell.
	    private Vector3 to;
	    /// Value and gradient of the center.
	    private Vector3 centerGradient;
	    private float centerValue;
	    private static float NEAR_FACTOR=2.0f;
	    
	    private Vector3 center;
	    
	    private bool subdivided=false;
	
		private float length;
		
		private Vector3 vBack, vFront, vLeft, vRight, vTop, vBottom;
		private Vector3 vBackTop, vBackBottom, vFrontTop, vFrontBottom;
		private Vector3 vLeftTop, vLeftBottom, vRightTop, vRightBottom;
		private Vector3 vBackLeft, vBackRight, vFrontLeft, vFrontRight;
		
		
		
	    public OctreeNode(Vector3 from, Vector3 to) {
	        this.from = from;
	        this.to = to;
	        this.center = (from + to) / 2;
	        this.length = Vector3.Distance(from,to);
	        centerGradient = Vector3.zero;
	        centerValue = 0;
			 
			
			float vCenterX = this.center.x;//from.x + (to.x - from.x) / 2.0f;
			float vCenterY = this.center.y;//from.y + (to.y - from.y) / 2.0f;
			float vCenterZ = this.center.z;//from.z + (to.z - from.z) / 2.0f;
			
			this.vBack = new Vector3(vCenterX, vCenterY, from.z);
			this.vFront = new Vector3(vCenterX, vCenterY, to.z);
			this.vLeft = new Vector3(from.x, vCenterY, vCenterZ);
			this.vRight = new Vector3(to.x, vCenterY, vCenterZ);
			this.vTop = new Vector3(vCenterX, to.y, vCenterZ);
			this.vBottom = new Vector3(vCenterX, from.y, vCenterZ);
			this.vBackTop = new Vector3(vCenterX, to.y, from.z);
			this.vBackBottom = new Vector3(vCenterX, from.y, from.z);
			this.vFrontTop = new Vector3(vCenterX, to.y, to.z);
			this.vFrontBottom = new Vector3(vCenterX, from.y, to.z);
			this.vLeftTop = new Vector3(from.x, to.y, vCenterZ);
			this.vLeftBottom = new Vector3(from.x, from.y, vCenterZ);
			this.vRightTop = new Vector3(to.x, to.y, vCenterZ);
			this.vRightBottom = new Vector3(to.x, from.y, vCenterZ);
			this.vBackLeft = new Vector3(from.x, vCenterY, from.z);
			this.vFrontLeft = new Vector3(from.x, vCenterY, to.z);
			this.vBackRight = new Vector3(to.x, vCenterY, from.z);
			this.vFrontRight = new Vector3(to.x, vCenterY, to.z);
	  
			
	    }
	
	    public void split(OctreeNodeSplitPolicy splitPolicy, VolumeSource source, float geometricError) {
	        if (splitPolicy.doSplit(this, geometricError)) {
	            children = new OctreeNode[8];
	
	            subdivided=true;
	            
	            float x  = (to.x - from.x) / 2.0f;
	            float y = (to.y - from.y) / 2.0f;
	            float z  = (to.z - from.z) / 2.0f;
	            Vector3 newCenter = new Vector3(x,y,z);
	            newCenter += from;
	            
	           // Vector3 xWidth = new Vector3();
	           // Vector3 yWidth = new Vector3();
	           // Vector3 zWidth = new Vector3();
	
	            //getChildrenDimensions(from, to, newCenter, xWidth, yWidth, zWidth);
	            
	            children[0] = new OctreeNode(from, newCenter);
	            children[1] = new OctreeNode(new Vector3(from.x+x,from.y,from.z), new Vector3(newCenter.x+x,newCenter.y,newCenter.z));
	            children[2] = new OctreeNode(new Vector3(from.x+x,from.y,from.z+z), new Vector3(newCenter.x+x,newCenter.y,newCenter.z+z));
	            children[3] = new OctreeNode(new Vector3(from.x,from.y,from.z+z), new Vector3(newCenter.x,newCenter.y,newCenter.z+z));
	            children[4] = new OctreeNode(new Vector3(from.x,from.y+y,from.z), new Vector3(newCenter.x,newCenter.y+y,newCenter.z));
	            children[5] = new OctreeNode(new Vector3(from.x+x,from.y+y,from.z), new Vector3(newCenter.x+x,newCenter.y+y,newCenter.z));
	            children[6] = new OctreeNode(new Vector3(from.x+x,from.y+y,from.z+z), new Vector3(newCenter.x+x,newCenter.y+y,newCenter.z+z));
	            children[7] = new OctreeNode(new Vector3(from.x,from.y+y,from.z+z), new Vector3(newCenter.x,newCenter.y+y,newCenter.z+z));
	
	            
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
	            if (centerValue == 0 && centerGradient.Equals(Vector3.zero)) {
	                centerGradient = source.getGradient(getCenter());
	                centerValue = source.getValue(getCenter());
	            }
	        }
	
	    }
	
	    /*private void getChildrenDimensions(Vector3 center, Vector3 width, Vector3 height, Vector3 depth) {
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
	    
	    
	     public static void getChildrenDimensions(Vector3 from, Vector3 to, Vector3 center, Vector3 width, Vector3 height, Vector3 depth)
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
	            center += from;
	        }
	    
	
	    public bool isSubdivided() {
	        return subdivided;
	    }
	
	    public OctreeNode getChild(int i) {
	        return children[i];
	    }
	
	    public Vector3 getFrom() {
	        return from;
	    }
	
	    public Vector3 getTo() {
	        return to;
	    }
	
	    public Vector3 getCenterGradient() {
	        return centerGradient;
	    }
	
	    public float getCenterValue() {
	        return centerValue;
	    }
	
	    public Vector3 getCenter() {
	        return center;
	    }
	
	    public void setCenterValue(float value) {
	        centerValue = value;
	    }
	
	    public void setCenterGradient(Vector3 gradient) {
	        centerGradient = gradient;
	    }
	
	    public Vector3 getCorner1() {
	        return new Vector3(to.x, from.y, from.z);
	    }
	
	    public Vector3 getCorner2() {
	        return new Vector3(to.x, from.y, to.z);
	    }
	
	    public Vector3 getCorner3() {
	        return new Vector3(from.x, from.y, to.z);
	    }
	
	    public Vector3 getCorner4() {
	        return new Vector3(from.x, to.y, from.z);
	    }
	
	    public Vector3 getCorner5() {
	        return new Vector3(to.x, to.y, from.z);
	    }
	
	    public Vector3 getCorner7() {
	        return new Vector3(from.x, to.y, to.z);
	    }
		
	    public bool isIsoSurfaceNear() {
	        if (centerValue == 0.0) {
	            return true;
	        }
	        
	      /*  float diffX = to.x-from.x;
	        float diffY = to.y-from.y;
	        float diffZ = to.z-from.z;
	        return Math.abs(centerValue) < Math.sqrt(diffX*diffX +diffY*diffY + diffZ*diffZ) * NEAR_FACTOR;*/
	        
	        return Math.Abs(centerValue) < this.length * NEAR_FACTOR;
	    }
	
	    public bool isBorderLeft(OctreeNode root) {
	        return from.x == root.from.x;
	    }
	    
	    public bool isBorderRight(OctreeNode root) {
	        return to.x == root.to.x;
	    }
	    
	    public bool isBorderBottom(OctreeNode root) {
	        return from.y == root.from.y;
	    }
	    
	    public bool isBorderTop(OctreeNode root) {
	        return to.y == root.to.y;
	    }
	    
	    public bool isBorderBack(OctreeNode root) {
	        return from.z == root.from.z;
	    }
	    
	    public bool isBorderFront(OctreeNode root) {
	        return to.z == root.to.z;
	    }
	    
	    
		public Vector3 getCenterBack() {
			return vBack;
		}
		
		public Vector3 getCenterFront() {
			return vFront;
		}
		
		public Vector3 getCenterLeft() {
			return vLeft;
		}
		
		public Vector3 getCenterRight() {
			return vRight;
		}
		
		public Vector3 getCenterTop() {
			return vTop;
		}
		
		public Vector3 getCenterBottom() {
			return vBottom;
		}
		
		public Vector3 getCenterBackTop() {
			return vBackTop;
		}
		
		public Vector3 getCenterBackBottom() {
			return vBackBottom;
		}
		
		public Vector3 getCenterFrontTop() {
			return vFrontTop;
		}
		
		public Vector3 getCenterFrontBottom() {
			return vFrontBottom;
		}
		
		public Vector3 getCenterLeftTop() {
			return vLeftTop;
		}
		
		public Vector3 getCenterLeftBottom() {
			return vLeftBottom;
		}
		
		public Vector3 getCenterRightTop() {
			return vRightTop;
		}
		
		public Vector3 getCenterRightBottom() {
			return vRightBottom;
		}
		
		public Vector3 getCenterBackLeft() {
			return vBackLeft;
		}
		
		public Vector3 getCenterFrontLeft() {
			return vFrontLeft;
		}
		
		public Vector3 getCenterBackRight() {
			return vBackRight;
		}
		
		public Vector3 getCenterFrontRight() {
			return vFrontRight;
		}
	}

}

