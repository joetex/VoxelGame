/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame;

import com.jme3.app.SimpleApplication;
import com.jme3.light.AmbientLight;
import com.jme3.light.DirectionalLight;
import com.jme3.material.Material;
import com.jme3.math.ColorRGBA;
import com.jme3.math.Vector3f;
import com.jme3.niftygui.NiftyJmeDisplay;
import com.jme3.scene.Geometry;
import com.jme3.scene.Mesh;
import com.jme3.scene.shape.Box;
import com.jme3.texture.Texture;
import com.jme3.util.SkyFactory;
import de.lessvoid.nifty.Nifty;
import de.lessvoid.nifty.builder.ScreenBuilder;
import de.lessvoid.nifty.builder.LayerBuilder;
import de.lessvoid.nifty.builder.PanelBuilder;
import de.lessvoid.nifty.controls.CheckBox;
import de.lessvoid.nifty.controls.DropDown;
import de.lessvoid.nifty.controls.TextField;
import de.lessvoid.nifty.controls.button.builder.ButtonBuilder;
import de.lessvoid.nifty.controls.checkbox.builder.CheckboxBuilder;
import de.lessvoid.nifty.controls.dropdown.builder.DropDownBuilder;
import de.lessvoid.nifty.controls.label.builder.LabelBuilder;
import de.lessvoid.nifty.controls.textfield.builder.TextFieldBuilder;
import de.lessvoid.nifty.screen.DefaultScreenController;
import de.lessvoid.nifty.screen.Screen;
import de.lessvoid.nifty.screen.ScreenController;
import mygame.dualmarchingcubes.source.FloatGridSource;
import mygame.sourcegeneration.Asteroid;
import mygame.sourcegeneration.Donut;
import mygame.sourcegeneration.GoursatSurface;
import mygame.sourcegeneration.Heart;
import mygame.sourcegeneration.SourceGenerator;
import mygame.sourcegeneration.Sphere;
import mygame.sourcegeneration.Terrain;

/**
 * @author iamcreasy
 */
public class NiftyTest extends SimpleApplication implements ScreenController {

    private static NiftyTest controller;
    
    private TextField textCubeSize;
    private TextField textGeometricError;
    private TextField textMaxCellSize;
    private TextField textMinSplitDistanceDiagonalFactor;
    private TextField textMaxMSDistance;
    private TextField textMaxClampedAbsoluteDensity;
    
    private DropDown dropSource;
    
    private CheckBox boxOctree;
    private CheckBox boxDualGrid;
    
    
    private TestGeometry testGeometry;
    private Geometry Octree;
    private Geometry DualGrid;
    
    private boolean dualMarchingCubes=false;

    public static void main(String[] args) {
        controller = new NiftyTest();
        controller.start();
    }

    @Override
    public void simpleInitApp() {

        testGeometry = new TestGeometry(assetManager);
        
        NiftyJmeDisplay niftyDisplay = new NiftyJmeDisplay(
                assetManager, inputManager, audioRenderer, guiViewPort);
        Nifty nifty = niftyDisplay.getNifty();
        guiViewPort.addProcessor(niftyDisplay);
        flyCam.setDragToRotate(true);

        nifty.loadStyleFile("nifty-default-styles.xml");
        nifty.loadControlFile("nifty-default-controls.xml");

        // <screen>
        nifty.addScreen("Screen_ID", new ScreenBuilder("Hello Nifty Screen") {
            {
                controller(controller); // Screen properties       

                // <layer>
                layer(new LayerBuilder("Layer_ID") {
                    {
                        childLayoutHorizontal(); // layer properties, add more...

                            panel(new PanelBuilder("Panel_ID") {
                            {
                                childLayoutVertical(); // panel properties, add more...               
                                width("80%");
                                backgroundColor("#0000");
                            }});
                        
                        // <panel>
                        panel(new PanelBuilder("Panel_ID") {
                            {
                                childLayoutVertical(); // panel properties, add more...               
                                width("20%");
                                backgroundColor("#999");
                                // GUI elements

                                /*panel(new PanelBuilder("Panel_ID") {{
                                 alignCenter();
                                 valignCenter();
                                 childLayoutVertical(); // panel properties, add more...               
                                 width("95%");
                                 height("20%");
                                 backgroundColor("#888");*/

                                control(new DropDownBuilder("dropdown") {
                                    {
                                        width("*");
                                    }
                                });

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("Marching Cubes");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("Cube Size:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new TextFieldBuilder("CubeSize") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        color("#000");
                                        text("4.0");
                                        height("3%");
                                        width("90%");
                                    }
                                });


                                control(new ButtonBuilder("Button_ID", "Marching Cubes") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        height("5%");
                                        width("90%");
                                        interactOnClick("generateMarchingCubes()");
                                    }
                                });

                                //}});

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("Dual Marching Cubes");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("Geometric Error:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new TextFieldBuilder("GeometricError") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        color("#000");
                                        text("1.8");
                                        height("3%");
                                        width("90%");
                                    }
                                });
                                
                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("MaxClampedAbsoluteDensity:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new TextFieldBuilder("MaxClampedAbsoluteDensity") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        color("#000");
                                        text("5.0");
                                        height("3%");
                                        width("90%");
                                    }
                                });

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("maxCellSize:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new TextFieldBuilder("MaxCellSize") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        color("#000");
                                        text("2.0");
                                        height("3%");
                                        width("90%");
                                    }
                                });

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("minSplitDistanceDiagonalFactor:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new TextFieldBuilder("MinSplitDistanceDiagonalFactor") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        color("#000");
                                        text("1.5");
                                        height("3%");
                                        width("90%");
                                    }
                                });

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("maxMSDistance:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new TextFieldBuilder("MaxMSDistance") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        color("#000");
                                        text("0.5");
                                        height("3%");
                                        width("90%");
                                    }
                                });

                                control(new ButtonBuilder("Button_ID", "Dual Marching Cubes") {
                                    {
                                        interactOnClick("generateDualMarchingCubes()");
                                        alignCenter();
                                        valignCenter();
                                        height("5%");
                                        width("90%");
                                    }
                                });


                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("Debug Octree:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new CheckboxBuilder("Octree") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        checked(false);
                                    }
                                });

                                control(new LabelBuilder() {
                                    {
                                        color("#000");
                                        text("Debug DualGrid:");
                                        height("5%");
                                        width("90%");
                                    }
                                });

                                control(new CheckboxBuilder("DualGrid") {
                                    {
                                        alignCenter();
                                        valignCenter();
                                        checked(false);
                                    }
                                });

                                //.. add more GUI elements here              

                            }
                        });
                        // </panel>
                    }
                });
                // </layer>
            }
        }.build(nifty));
        // </screen>


        nifty.gotoScreen("Screen_ID"); // start the screen

        dropSource = nifty.getCurrentScreen().findNiftyControl("dropdown", DropDown.class);
        dropSource.addItem("Donut");
        dropSource.addItem("Heart");
        dropSource.addItem("GoursatSurface");
        dropSource.addItem("Sphere");
        dropSource.addItem("Asteroid");


        textCubeSize = nifty.getCurrentScreen().findNiftyControl("CubeSize", TextField.class);
        textGeometricError = nifty.getCurrentScreen().findNiftyControl("GeometricError", TextField.class);
        textMaxCellSize = nifty.getCurrentScreen().findNiftyControl("MaxCellSize", TextField.class);
        textMinSplitDistanceDiagonalFactor = nifty.getCurrentScreen().findNiftyControl("MinSplitDistanceDiagonalFactor", TextField.class);
        textMaxMSDistance = nifty.getCurrentScreen().findNiftyControl("MaxMSDistance", TextField.class);
        textMaxClampedAbsoluteDensity = nifty.getCurrentScreen().findNiftyControl("MaxClampedAbsoluteDensity", TextField.class);
        
        boxOctree = nifty.getCurrentScreen().findNiftyControl("Octree", CheckBox.class);
        boxDualGrid = nifty.getCurrentScreen().findNiftyControl("DualGrid", CheckBox.class);
    
        generateMarchingCubes();
        testGeometry.setMaterial(getMaterial());
        rootNode.attachChild(testGeometry);
        
        makeLight();
        
    }

    public void generateMarchingCubes() {

        float size = Float.parseFloat( textCubeSize.getText());
        
        testGeometry.marchingCubes(getSource(0), size);
        
        if(Octree != null)
            rootNode.detachChild(Octree);
         if(DualGrid != null)
            rootNode.detachChild(DualGrid);
    }

    public void generateDualMarchingCubes() {

        float geometricError = Float.parseFloat( textGeometricError.getText());
        float maxCellSize = Float.parseFloat( textMaxCellSize.getText());
        float minSplitDistanceDiagonalFactor = Float.parseFloat( textMinSplitDistanceDiagonalFactor.getText());
        float maxMSDistance = Float.parseFloat( textMaxMSDistance.getText());
        float maxClampedAbsoluteDensity = Float.parseFloat( textMaxClampedAbsoluteDensity.getText());
        
        testGeometry.dualMarchingCubes(getSource(maxClampedAbsoluteDensity), geometricError,maxCellSize,
                minSplitDistanceDiagonalFactor,maxMSDistance  );
        

        if(Octree != null)
                rootNode.detachChild(Octree);
        
        if(DualGrid != null)
                rootNode.detachChild(DualGrid);
        
        if(boxOctree.isChecked())
        {
            System.out.println("OCTREE!!!");
            Octree = testGeometry.getDebugOctree();
            rootNode.attachChild(Octree);
        }
        
        if(boxDualGrid.isChecked())
        {
            DualGrid= testGeometry.getDebugDualGrid();
            rootNode.attachChild(DualGrid);
        }
    }
    

    
    public FloatGridSource getSource(float maxClampedAbsoluteDensity)
    {
        String name = (String)dropSource.getItems().get(dropSource.getSelectedIndex());
        System.out.println("----------->" + name);
        
        if(name.equals("Donut"))
        {
            return SourceGenerator.getSource(new Donut(),maxClampedAbsoluteDensity);
        }else if(name.equals("Heart"))
        {
            return SourceGenerator.getSource(new Heart(),maxClampedAbsoluteDensity);
        }else if(name.equals("GoursatSurface"))
        {
            return SourceGenerator.getSource(new GoursatSurface(),maxClampedAbsoluteDensity);
        }else if(name.equals("Sphere"))
        {
            return SourceGenerator.getSource(new Sphere(),maxClampedAbsoluteDensity);
        }
        
        return SourceGenerator.getSource(new Asteroid(),maxClampedAbsoluteDensity);
    }
    
    public void makeLight()
    {

        cam.setLocation(new Vector3f(120, 120, 120));
        cam.lookAt(Vector3f.ZERO, Vector3f.UNIT_Y);
        
        DirectionalLight sun = new DirectionalLight();
        sun.setColor(ColorRGBA.White);
        sun.setDirection(new Vector3f(-.45f, -.501f, -.60f).normalizeLocal());
        rootNode.addLight(sun);

        AmbientLight al = new AmbientLight();
        al.setColor(ColorRGBA.White.mult(1.3f));
        rootNode.addLight(al);

        flyCam.setMoveSpeed(50);
        
        flyCam.setDragToRotate(true);

        //for a nicer look ;)
        rootNode.attachChild(SkyFactory.createSky(
                assetManager, "Textures/Sky/Bright/BrightSky.dds", false));
    }

    public void bind(Nifty nifty, Screen screen) {
        //throw new UnsupportedOperationException("Not supported yet.");
    }

    public void onStartScreen() {
        // throw new UnsupportedOperationException("Not supported yet.");
    }

    public void onEndScreen() {
        //  throw new UnsupportedOperationException("Not supported yet.");
    }
    
    public Material getMaterial() {

        float grassScale = 64;
        float dirtScale = 16;
        float rockScale = 128;

        Material matTerrain = new Material(assetManager, "Materials/TerrainLighting.j3md");
        matTerrain.setBoolean("useTriPlanarMapping", true);
        matTerrain.setFloat("Shininess", 0.0f);

        //   ALPHA map (for splat textures)
        matTerrain.setTexture("AlphaMap", assetManager.loadTexture("Textures/Terrain/splat/alpha1.png"));
        //   matTerrain.setTexture("AlphaMap_1", assetManager.loadTexture("Textures/Terrain/splat/alpha2.png"));
        // this material also supports 'AlphaMap_2', so you can get up to 12 diffuse textures

        // HEIGHTMAP image (for the terrain heightmap)
        //  Texture heightMapImage = assetManager.loadTexture("Textures/Terrain/splat/mountains512.png");

        // DIRT texture, Diffuse textures 0 to 3 use the first AlphaMap
        Texture dirt = assetManager.loadTexture("Textures/Terrain/splat/dirt.jpg");
        dirt.setWrap(Texture.WrapMode.Repeat);
        matTerrain.setTexture("DiffuseMap", dirt);
        //  matTerrain.setFloat("DiffuseMap_0_scale", dirtScale);

        // GRASS texture
        Texture grass = assetManager.loadTexture("Textures/Terrain/splat/grass.jpg");
        grass.setWrap(Texture.WrapMode.Repeat);
        matTerrain.setTexture("DiffuseMap_1", grass);
        //   matTerrain.setFloat("DiffuseMap_1_scale", grassScale);

        // ROCK texture
        Texture rock = assetManager.loadTexture("Textures/Terrain/splat/road.jpg");
        rock.setWrap(Texture.WrapMode.Repeat);
        matTerrain.setTexture("DiffuseMap_2", rock);
        //  matTerrain.setFloat("DiffuseMap_2_scale", rockScale);


        matTerrain.setFloat("DiffuseMap_0_scale", 1f / (float) (128f / grassScale));
        matTerrain.setFloat("DiffuseMap_1_scale", 1f / (float) (128f / dirtScale));
        matTerrain.setFloat("DiffuseMap_2_scale", 1f / (float) (128f / rockScale));

        return matTerrain;
    }
}
