using System;
using UnityEngine;
using System.Collections.Generic;

namespace Night
{
	public class DualGridGenerator {
	    
	    private OctreeNode root;
	    private LinkedList<DualCell> dualCells = new LinkedList<DualCell>();
	    
	    private IsoSurface iso;
	    private MeshBuilder mb;
	    
	    private bool saveDualCells;
	    
	    /// The maximum distance where to generate the skirts.
	    private float maxMSDistance;
	     
	    private Vector3 totalFrom;
	    private Vector3 totalTo;
	    
	    
	    public class DualCell
	    {
	        Vector3[] c;
	        
	        public DualCell(Vector3 c0, Vector3 c1, Vector3 c2, Vector3 c3, 
	            Vector3 c4, Vector3 c5, Vector3 c6, Vector3 c7)
	        {
	            c = new Vector3[8];
	            c[0] = c0;
	            c[1] = c1;
	            c[2] = c2;
	            c[3] = c3;
	            c[4] = c4;
	            c[5] = c5;
	            c[6] = c6;
	            c[7] = c7;
	        }
	    }
	     
	    
	    public void nodeProc(OctreeNode n)
	    {
	        if(n.isSubdivided())
	        {   
	            for(int i=0;i<8;i++)
	            {
	                nodeProc(n.getChild(i));
	            }
	            
	            faceProcXY(n.getChild(0),n.getChild(3));
	            faceProcXY(n.getChild(1),n.getChild(2));
	            faceProcXY(n.getChild(4),n.getChild(7));
	            faceProcXY(n.getChild(5),n.getChild(6));
	              
	            faceProcZY(n.getChild(0),n.getChild(1));
	            faceProcZY(n.getChild(3),n.getChild(2));
	            faceProcZY(n.getChild(4),n.getChild(5));
	            faceProcZY(n.getChild(7),n.getChild(6));
	            
	            faceProcXZ(n.getChild(4),n.getChild(0));
	            faceProcXZ(n.getChild(5),n.getChild(1));
	            faceProcXZ(n.getChild(7),n.getChild(3));
	            faceProcXZ(n.getChild(6),n.getChild(2));
	            
	            edgeProcX(n.getChild(0), n.getChild(3), n.getChild(7), n.getChild(4));
	            edgeProcX(n.getChild(1), n.getChild(2), n.getChild(6), n.getChild(5));
	
	            edgeProcY(n.getChild(0), n.getChild(1), n.getChild(2), n.getChild(3));
	            edgeProcY(n.getChild(4), n.getChild(5), n.getChild(6), n.getChild(7));
	            
	            edgeProcZ(n.getChild(7), n.getChild(6), n.getChild(2), n.getChild(3));
	            edgeProcZ(n.getChild(4), n.getChild(5), n.getChild(1), n.getChild(0));
	
	            vertProc(n.getChild(0), n.getChild(1), n.getChild(2), n.getChild(3),
	                n.getChild(4), n.getChild(5), n.getChild(6), n.getChild(7));
	        }
	    }
	    
	    private void faceProcXY(OctreeNode n0, OctreeNode n1)
	    {
	        if (n0.isSubdivided() || n1.isSubdivided())
	        {
	            OctreeNode c0 = n0.isSubdivided() ? n0.getChild(3) : n0;
	            OctreeNode c1 = n0.isSubdivided() ? n0.getChild(2) : n0;
	            OctreeNode c2 = n1.isSubdivided() ? n1.getChild(1) : n1;
	            OctreeNode c3 = n1.isSubdivided() ? n1.getChild(0) : n1;
	
	            OctreeNode c4 = n0.isSubdivided() ? n0.getChild(7) : n0;
	            OctreeNode c5 = n0.isSubdivided() ? n0.getChild(6) : n0;
	            OctreeNode c6 = n1.isSubdivided() ? n1.getChild(5) : n1;
	            OctreeNode c7 = n1.isSubdivided() ? n1.getChild(4) : n1;
	                
	            faceProcXY(c0, c3);
	            faceProcXY(c1, c2);
	            faceProcXY(c4, c7);
	            faceProcXY(c5, c6);
	    
	            edgeProcX(c0, c3, c7, c4);
	            edgeProcX(c1, c2, c6, c5);
	            edgeProcY(c0, c1, c2, c3);
	            edgeProcY(c4, c5, c6, c7);
	             
	            vertProc(c0, c1, c2, c3, c4, c5, c6, c7);
	        }
	    }
	    
	    private void faceProcZY(OctreeNode n0, OctreeNode n1)
	    {
	        if (n0.isSubdivided() || n1.isSubdivided())
	        {
	            OctreeNode c0 = n0.isSubdivided() ? n0.getChild(1) : n0;
	            OctreeNode c1 = n1.isSubdivided() ? n1.getChild(0) : n1;
	            OctreeNode c2 = n1.isSubdivided() ? n1.getChild(3) : n1;
	            OctreeNode c3 = n0.isSubdivided() ? n0.getChild(2) : n0;
	
	            OctreeNode c4 = n0.isSubdivided() ? n0.getChild(5) : n0;
	            OctreeNode c5 = n1.isSubdivided() ? n1.getChild(4) : n1;
	            OctreeNode c6 = n1.isSubdivided() ? n1.getChild(7) : n1;
	            OctreeNode c7 = n0.isSubdivided() ? n0.getChild(6) : n0;
	            
	            faceProcZY(c0, c1);
	            faceProcZY(c3, c2);
	            faceProcZY(c4, c5);
	            faceProcZY(c7, c6);
	            
	            edgeProcY(c0, c1, c2, c3);
	            edgeProcY(c4, c5, c6, c7);
	            edgeProcZ(c7, c6, c2, c3);
	            edgeProcZ(c4, c5, c1, c0);
	            
	    
	            vertProc(c0, c1, c2, c3, c4, c5, c6, c7);
	        }
	    }
	    
	    private void faceProcXZ(OctreeNode n0, OctreeNode n1)
	    {
	        if (n0.isSubdivided() || n1.isSubdivided())
	        {
	            OctreeNode c0 = n1.isSubdivided() ? n1.getChild(4) : n1;
	            OctreeNode c1 = n1.isSubdivided() ? n1.getChild(5) : n1;
	            OctreeNode c2 = n1.isSubdivided() ? n1.getChild(6) : n1;
	            OctreeNode c3 = n1.isSubdivided() ? n1.getChild(7) : n1;
	
	            OctreeNode c4 = n0.isSubdivided() ? n0.getChild(0) : n0;
	            OctreeNode c5 = n0.isSubdivided() ? n0.getChild(1) : n0;
	            OctreeNode c6 = n0.isSubdivided() ? n0.getChild(2) : n0;
	            OctreeNode c7 = n0.isSubdivided() ? n0.getChild(3) : n0;
	        
	            faceProcXZ(c4, c0);
	            faceProcXZ(c5, c1);
	            faceProcXZ(c7, c3);
	            faceProcXZ(c6, c2);
	        
	            edgeProcX(c0, c3, c7, c4);
	            edgeProcX(c1, c2, c6, c5);
	            edgeProcZ(c7, c6, c2, c3);
	            edgeProcZ(c4, c5, c1, c0);
	           
	    
	            vertProc(c0, c1, c2, c3, c4, c5, c6, c7);
	        }
	    }
	    
	    private void edgeProcX(OctreeNode n0, OctreeNode n1, OctreeNode n2, OctreeNode n3)
	    {
	        if (n0.isSubdivided() || n1.isSubdivided() || n2.isSubdivided() || n3.isSubdivided())
	        {
	            OctreeNode c0 = n0.isSubdivided() ? n0.getChild(7) : n0;
	            OctreeNode c1 = n0.isSubdivided() ? n0.getChild(6) : n0;
	            OctreeNode c2 = n1.isSubdivided() ? n1.getChild(5) : n1;
	            OctreeNode c3 = n1.isSubdivided() ? n1.getChild(4) : n1;
	            OctreeNode c4 = n3.isSubdivided() ? n3.getChild(3) : n3;
	            OctreeNode c5 = n3.isSubdivided() ? n3.getChild(2) : n3;
	            OctreeNode c6 = n2.isSubdivided() ? n2.getChild(1) : n2;
	            OctreeNode c7 = n2.isSubdivided() ? n2.getChild(0) : n2;
	
	            edgeProcX(c0, c3, c7, c4);
	            edgeProcX(c1, c2, c6, c5);
	    
	            vertProc(c0, c1, c2, c3, c4, c5, c6, c7);
	        }
	    }
	    
	    private void edgeProcY(OctreeNode n0, OctreeNode n1, OctreeNode n2, OctreeNode n3)
	    {
	        if (n0.isSubdivided() || n1.isSubdivided() || n2.isSubdivided() || n3.isSubdivided())
	        {
	            OctreeNode c0 = n0.isSubdivided() ? n0.getChild(2) : n0;
	            OctreeNode c1 = n1.isSubdivided() ? n1.getChild(3) : n1;
	            OctreeNode c2 = n2.isSubdivided() ? n2.getChild(0) : n2;
	            OctreeNode c3 = n3.isSubdivided() ? n3.getChild(1) : n3;
	            OctreeNode c4 = n0.isSubdivided() ? n0.getChild(6) : n0;
	            OctreeNode c5 = n1.isSubdivided() ? n1.getChild(7) : n1;
	            OctreeNode c6 = n2.isSubdivided() ? n2.getChild(4) : n2;
	            OctreeNode c7 = n3.isSubdivided() ? n3.getChild(5) : n3;
	
	            edgeProcY(c0, c1, c2, c3);
	            edgeProcY(c4, c5, c6, c7);
	    
	            vertProc(c0, c1, c2, c3, c4, c5, c6, c7);
	        }
	    }
	    
	    private void edgeProcZ(OctreeNode n0, OctreeNode n1, OctreeNode n2, OctreeNode n3)
	    {
	        if (n0.isSubdivided() || n1.isSubdivided() || n2.isSubdivided() || n3.isSubdivided())
	        {
	            OctreeNode c0 = n3.isSubdivided() ? n3.getChild(5) : n3;
	            OctreeNode c1 = n2.isSubdivided() ? n2.getChild(4) : n2;
	            OctreeNode c2 = n2.isSubdivided() ? n2.getChild(7) : n2;
	            OctreeNode c3 = n3.isSubdivided() ? n3.getChild(6) : n3;
	            OctreeNode c4 = n0.isSubdivided() ? n0.getChild(1) : n0;
	            OctreeNode c5 = n1.isSubdivided() ? n1.getChild(0) : n1;
	            OctreeNode c6 = n1.isSubdivided() ? n1.getChild(3) : n1;
	            OctreeNode c7 = n0.isSubdivided() ? n0.getChild(2) : n0;
	
	            edgeProcZ(c7, c6, c2, c3);
	            edgeProcZ(c4, c5, c1, c0);
	    
	            vertProc(c0, c1, c2, c3, c4, c5, c6, c7);
	        }
	    }
	    
	    private void vertProc(OctreeNode n0, OctreeNode n1, OctreeNode n2, OctreeNode n3, 
	                OctreeNode n4, OctreeNode n5, OctreeNode n6, OctreeNode n7)
	    {
	
	        if (n0.isSubdivided() || n1.isSubdivided() || n2.isSubdivided() || n3.isSubdivided() ||
	            n4.isSubdivided() || n5.isSubdivided() || n6.isSubdivided() || n7.isSubdivided())
	        {
	            OctreeNode c0 = n0.isSubdivided() ? n0.getChild(6) : n0;
	            OctreeNode c1 = n1.isSubdivided() ? n1.getChild(7) : n1;
	            OctreeNode c2 = n2.isSubdivided() ? n2.getChild(4) : n2;
	            OctreeNode c3 = n3.isSubdivided() ? n3.getChild(5) : n3;
	            OctreeNode c4 = n4.isSubdivided() ? n4.getChild(2) : n4;
	            OctreeNode c5 = n5.isSubdivided() ? n5.getChild(3) : n5;
	            OctreeNode c6 = n6.isSubdivided() ? n6.getChild(0) : n6;
	            OctreeNode c7 = n7.isSubdivided() ? n7.getChild(1) : n7;
	        
	            vertProc(c0, c1, c2, c3, c4, c5, c6, c7);
	        }
	        else
	        {
	
	            if (!n0.isIsoSurfaceNear() && !n1.isIsoSurfaceNear() && !n2.isIsoSurfaceNear() && !n3.isIsoSurfaceNear() &&
	                !n4.isIsoSurfaceNear() && !n5.isIsoSurfaceNear() && !n6.isIsoSurfaceNear() && !n7.isIsoSurfaceNear())
	            {
	                return;
	            }
	
	            float[] values = new float[8];
	            values[0] = n0.getCenterValue();
	            values[1] = n1.getCenterValue();
	            values[2] = n2.getCenterValue();
	            values[3] = n3.getCenterValue();
	            values[4] = n4.getCenterValue();
	            values[5] = n5.getCenterValue();
	            values[6] = n6.getCenterValue();
	            values[7] = n7.getCenterValue();
	            
	            Vector3[] gradients = new Vector3[8];
	            gradients[0] = n0.getCenterGradient();
	            gradients[1] = n1.getCenterGradient();
	            gradients[2] = n2.getCenterGradient();
	            gradients[3] = n3.getCenterGradient();
	            gradients[4] = n4.getCenterGradient();
	            gradients[5] = n5.getCenterGradient();
	            gradients[6] = n6.getCenterGradient();
	            gradients[7] = n7.getCenterGradient();
	            
	            addDualCell(n0.getCenter(), n1.getCenter(), n2.getCenter(), n3.getCenter(),
	                n4.getCenter(), n5.getCenter(), n6.getCenter(), n7.getCenter(), values, gradients);
	            createBorderCells(n0, n1, n2, n3, n4, n5, n6, n7);
	        }
	    }
	    
	    private void createBorderCells(OctreeNode n0, OctreeNode n1, OctreeNode n2, OctreeNode n3, 
	            OctreeNode n4, OctreeNode n5, OctreeNode n6, OctreeNode n7)
	    {
	        if (n0.isBorderBack(root) && n1.isBorderBack(root) && n4.isBorderBack(root) && n5.isBorderBack(root))
	        {
	            addDualCell(n0.getCenterBack(), n1.getCenterBack(), n1.getCenter(), n0.getCenter(),
	                n4.getCenterBack(), n5.getCenterBack(), n5.getCenter(), n4.getCenter());
	            // Generate back edge border cells
	            if (n4.isBorderTop(root) && n5.isBorderTop(root))
	            {
	                addDualCell(n4.getCenterBack(), n5.getCenterBack(), n5.getCenter(), n4.getCenter(),
	                    n4.getCenterBackTop(), n5.getCenterBackTop(), n5.getCenterTop(), n4.getCenterTop());
	                // Generate back top corner cells
	                if (n4.isBorderLeft(root))
	                {
	                    addDualCell(n4.getCenterBackLeft(), n4.getCenterBack(), n4.getCenter(), n4.getCenterLeft(),
	                        n4.getCorner4(), n4.getCenterBackTop(), n4.getCenterTop(), n4.getCenterLeftTop());
	                }
	                if (n5.isBorderRight(root))
	                {
	                    addDualCell(n5.getCenterBack(), n5.getCenterBackRight(), n5.getCenterRight(), n5.getCenter(),
	                        n5.getCenterBackTop(), n5.getCorner5(), n5.getCenterRightTop(), n5.getCenterTop());
	                }
	            }
	            if (n0.isBorderBottom(root) && n1.isBorderBottom(root))
	            {
	                addDualCell(n0.getCenterBackBottom(), n1.getCenterBackBottom(), n1.getCenterBottom(), n0.getCenterBottom(),
	                    n0.getCenterBack(), n1.getCenterBack(), n1.getCenter(), n0.getCenter());
	                // Generate back bottom corner cells
	                if (n0.isBorderLeft(root))
	                {
	                    addDualCell(n0.getFrom(), n0.getCenterBackBottom(), n0.getCenterBottom(), n0.getCenterLeftBottom(),
	                        n0.getCenterBackLeft(), n0.getCenterBack(), n0.getCenter(), n0.getCenterLeft());
	                }
	                if (n1.isBorderRight(root))
	                {
	                    addDualCell(n1.getCenterBackBottom(), n1.getCorner1(), n1.getCenterRightBottom(), n1.getCenterBottom(),
	                        n1.getCenterBack(), n1.getCenterBackRight(), n1.getCenterRight(), n1.getCenter());
	                }
	            }
	        }
	        if (n2.isBorderFront(root) && n3.isBorderFront(root) && n6.isBorderFront(root) && n7.isBorderFront(root))
	        {
	            addDualCell(n3.getCenter(), n2.getCenter(), n2.getCenterFront(), n3.getCenterFront(),
	                n7.getCenter(), n6.getCenter(), n6.getCenterFront(), n7.getCenterFront());
	            // Generate front edge border cells
	            if (n6.isBorderTop(root) && n7.isBorderTop(root))
	            {
	                addDualCell(n7.getCenter(), n6.getCenter(), n6.getCenterFront(), n7.getCenterFront(),
	                    n7.getCenterTop(), n6.getCenterTop(), n6.getCenterFrontTop(), n7.getCenterFrontTop());
	                // Generate back bottom corner cells
	                if (n7.isBorderLeft(root))
	                {
	                    addDualCell(n7.getCenterLeft(), n7.getCenter(), n7.getCenterFront(), n7.getCenterFrontLeft(),
	                        n7.getCenterLeftTop(), n7.getCenterTop(), n7.getCenterFrontTop(), n7.getCorner7());
	                }
	                if (n6.isBorderRight(root))
	                {
	                    addDualCell(n6.getCenter(), n6.getCenterRight(), n6.getCenterFrontRight(), n6.getCenterFront(),
	                        n6.getCenterTop(), n6.getCenterRightTop(), n6.getTo(), n6.getCenterFrontTop());
	                }
	            }
	            if (n3.isBorderBottom(root) && n2.isBorderBottom(root))
	            {
	                addDualCell(n3.getCenterBottom(), n2.getCenterBottom(), n2.getCenterFrontBottom(), n3.getCenterFrontBottom(), 
	                    n3.getCenter(), n2.getCenter(), n2.getCenterFront(), n3.getCenterFront());
	                // Generate back bottom corner cells
	                if (n3.isBorderLeft(root))
	                {
	                    addDualCell(n3.getCenterLeftBottom(), n3.getCenterBottom(), n3.getCenterFrontBottom(), n3.getCorner3(),
	                        n3.getCenterLeft(), n3.getCenter(), n3.getCenterFront(), n3.getCenterFrontLeft());
	                }
	                if (n2.isBorderRight(root))
	                {
	                    addDualCell(n2.getCenterBottom(), n2.getCenterRightBottom(), n2.getCorner2(), n2.getCenterFrontBottom(),
	                        n2.getCenter(), n2.getCenterRight(), n2.getCenterFrontRight(), n2.getCenterFront());
	                }
	            }
	        }
	        if (n0.isBorderLeft(root) && n3.isBorderLeft(root) && n4.isBorderLeft(root) && n7.isBorderLeft(root))
	        {
	            addDualCell(n0.getCenterLeft(), n0.getCenter(), n3.getCenter(), n3.getCenterLeft(),
	                n4.getCenterLeft(), n4.getCenter(), n7.getCenter(), n7.getCenterLeft());
	            // Generate left edge border cells
	            if (n4.isBorderTop(root) && n7.isBorderTop(root))
	            {
	                addDualCell(n4.getCenterLeft(), n4.getCenter(), n7.getCenter(), n7.getCenterLeft(),
	                    n4.getCenterLeftTop(), n4.getCenterTop(), n7.getCenterTop(), n7.getCenterLeftTop());
	            }
	            if (n0.isBorderBottom(root) && n3.isBorderBottom(root))
	            {
	                addDualCell(n0.getCenterLeftBottom(), n0.getCenterBottom(), n3.getCenterBottom(), n3.getCenterLeftBottom(),
	                    n0.getCenterLeft(), n0.getCenter(), n3.getCenter(), n3.getCenterLeft());
	            }
	            if (n0.isBorderBack(root) && n4.isBorderBack(root))
	            {
	                addDualCell(n0.getCenterBackLeft(), n0.getCenterBack(), n0.getCenter(), n0.getCenterLeft(),
	                    n4.getCenterBackLeft(), n4.getCenterBack(), n4.getCenter(), n4.getCenterLeft());
	            }
	            if (n3.isBorderFront(root) && n7.isBorderFront(root))
	            {
	                addDualCell(n3.getCenterLeft(), n3.getCenter(), n3.getCenterFront(), n3.getCenterFrontLeft(),
	                    n7.getCenterLeft(), n7.getCenter(), n7.getCenterFront(), n7.getCenterFrontLeft());
	            }
	        }
	        if (n1.isBorderRight(root) && n2.isBorderRight(root) && n5.isBorderRight(root) && n6.isBorderRight(root))
	        {
	            addDualCell(n1.getCenter(), n1.getCenterRight(), n2.getCenterRight(), n2.getCenter(),
	                n5.getCenter(), n5.getCenterRight(), n6.getCenterRight(), n6.getCenter());
	            // Generate right edge border cells
	            if (n5.isBorderTop(root) && n6.isBorderTop(root))
	            {
	                addDualCell(n5.getCenter(), n5.getCenterRight(), n6.getCenterRight(), n6.getCenter(),
	                    n5.getCenterTop(), n5.getCenterRightTop(), n6.getCenterRightTop(), n6.getCenterTop());
	            }
	            if (n1.isBorderBottom(root) && n2.isBorderBottom(root))
	            {
	                addDualCell(n1.getCenterBottom(), n1.getCenterRightBottom(), n2.getCenterRightBottom(), n2.getCenterBottom(),
	                    n1.getCenter(), n1.getCenterRight(), n2.getCenterRight(), n2.getCenter());
	            }
	            if (n1.isBorderBack(root) && n5.isBorderBack(root))
	            {
	                addDualCell(n1.getCenterBack(), n1.getCenterBackRight(), n1.getCenterRight(), n1.getCenter(),
	                    n5.getCenterBack(), n5.getCenterBackRight(), n5.getCenterRight(), n5.getCenter());
	            }
	            if (n2.isBorderFront(root) && n6.isBorderFront(root))
	            {
	                addDualCell(n2.getCenter(), n2.getCenterRight(), n2.getCenterFrontRight(), n2.getCenterFront(),
	                    n6.getCenter(), n6.getCenterRight(), n6.getCenterFrontRight(), n6.getCenterFront());
	            }
	        }
	        if (n4.isBorderTop(root) && n5.isBorderTop(root) && n6.isBorderTop(root) && n7.isBorderTop(root))
	        {
	            addDualCell(n4.getCenter(), n5.getCenter(), n6.getCenter(), n7.getCenter(),
	                n4.getCenterTop(), n5.getCenterTop(), n6.getCenterTop(), n7.getCenterTop());
	        }
	        if (n0.isBorderBottom(root) && n1.isBorderBottom(root) && n2.isBorderBottom(root) && n3.isBorderBottom(root))
	        {
	            addDualCell(n0.getCenterBottom(), n1.getCenterBottom(), n2.getCenterBottom(), n3.getCenterBottom(),
	                n0.getCenter(), n1.getCenter(), n2.getCenter(), n3.getCenter());
	        }
	    }
	    
	    
	
	   public void addDualCell(Vector3 c0, Vector3 c1, Vector3 c2, Vector3 c3, 
	            Vector3 c4, Vector3 c5, Vector3 c6, Vector3 c7)
	        {
	            addDualCell(c0, c1, c2, c3, c4, c5, c6, c7, null, null);
	        }
	    
	    public void addDualCell(Vector3 c0, Vector3 c1, Vector3 c2, Vector3 c3, 
	            Vector3 c4, Vector3 c5, Vector3 c6, Vector3 c7, float[] values, Vector3[] gradients)
	        {
	
	            if (saveDualCells)
	            {
	                dualCells.AddLast(new DualCell(c0, c1, c2, c3, c4, c5, c6, c7));
	            }
	
	            Vector3[] corners = new Vector3[8];
	            corners[0] = c0;
	            corners[1] = c1;
	            corners[2] = c2;
	            corners[3] = c3;
	            corners[4] = c4;
	            corners[5] = c5;
	            corners[6] = c6;
	            corners[7] = c7;
	            iso.addMarchingCubesTriangles(corners, values,gradients, mb);
	            
	            
	            Vector3 from = root.getFrom();
	            Vector3 to = root.getTo();
	            if (corners[0].z == from.z && corners[0].z != totalFrom.z)
	            {
	                iso.addMarchingSquaresTriangles(corners, values, gradients, IsoSurface.MS_CORNERS_BACK, maxMSDistance, mb);
	            }
	            if (corners[2].z == to.z && corners[2].z != totalTo.z)
	            {
	                iso.addMarchingSquaresTriangles(corners, values,gradients, IsoSurface.MS_CORNERS_FRONT, maxMSDistance, mb);
	            }
	            if (corners[0].x == from.x && corners[0].x != totalFrom.x)
	            {
	                iso.addMarchingSquaresTriangles(corners, values,gradients, IsoSurface.MS_CORNERS_LEFT, maxMSDistance, mb);
	            }
	            if (corners[1].x == to.x && corners[1].x != totalTo.x)
	            {
	                iso.addMarchingSquaresTriangles(corners, values,gradients, IsoSurface.MS_CORNERS_RIGHT, maxMSDistance, mb);
	            }
	            if (corners[5].y == to.y && corners[5].y != totalTo.y)
	            {
	                iso.addMarchingSquaresTriangles(corners, values,gradients, IsoSurface.MS_CORNERS_TOP, maxMSDistance, mb);
	            }
	            if (corners[0].y == from.y && corners[0].y != totalFrom.y)
	            {
	                iso.addMarchingSquaresTriangles(corners, values,gradients, IsoSurface.MS_CORNERS_BOTTOM, maxMSDistance, mb);
	            }
	        }
	    
	    
	    public void generateDualGrid(OctreeNode root, IsoSurface iso, MeshBuilder mb, float maxMSDistance, Vector3 totalFrom, Vector3 totalTo, bool saveDualCells)
	    {
	        this.root = root;
	        this.iso = iso;
	        this.mb = mb;
	        this.maxMSDistance = maxMSDistance;
	        this.totalFrom = totalFrom;
	        this.totalTo = totalTo;
	        this.saveDualCells = saveDualCells;
	
	        nodeProc(root);
	
	        // Build up a minimal dualgrid for octrees without children.
	        if (!root.isSubdivided())
	        {
	            addDualCell(root.getFrom(), root.getCenterBackBottom(), root.getCenterBottom(), root.getCenterLeftBottom(),
	                root.getCenterBackLeft(), root.getCenterBack(), root.getCenter(), root.getCenterLeft());
	            addDualCell(root.getCenterBackBottom(), root.getCorner1(), root.getCenterRightBottom(), root.getCenterBottom(),
	                root.getCenterBack(), root.getCenterBackRight(), root.getCenterRight(), root.getCenter());
	            addDualCell(root.getCenterBottom(), root.getCenterRightBottom(), root.getCorner2(), root.getCenterFrontBottom(),
	                root.getCenter(), root.getCenterRight(), root.getCenterFrontRight(), root.getCenterFront());
	            addDualCell(root.getCenterLeftBottom(), root.getCenterBottom(), root.getCenterFrontBottom(), root.getCorner3(),
	                root.getCenterLeft(), root.getCenter(), root.getCenterFront(), root.getCenterFrontLeft());
	
	            addDualCell(root.getCenterBackLeft(), root.getCenterBack(), root.getCenter(), root.getCenterLeft(),
	                root.getCorner4(), root.getCenterBackTop(), root.getCenterTop(), root.getCenterLeftTop());
	            addDualCell(root.getCenterBack(), root.getCenterBackRight(), root.getCenterRight(), root.getCenter(),
	                root.getCenterBackTop(), root.getCorner5(), root.getCenterRightTop(), root.getCenterTop());
	            addDualCell(root.getCenter(), root.getCenterRight(), root.getCenterFrontRight(), root.getCenterFront(),
	                root.getCenterTop(), root.getCenterRightTop(), root.getTo(), root.getCenterFrontTop());
	            addDualCell(root.getCenterLeft(), root.getCenter(), root.getCenterFront(), root.getCenterFrontLeft(),
	                root.getCenterLeftTop(), root.getCenterTop(), root.getCenterFrontTop(), root.getCorner7());
	        }
	    }
	    
	    public LinkedList<DualCell> getDualCells()
	    {
	        return dualCells;
	    }
	}
}

