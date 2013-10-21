/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mygame.dualmarchingcubes;

import com.jme3.app.Application;
import com.jme3.app.state.AbstractAppState;
import com.jme3.app.state.AppStateManager;
import com.jme3.renderer.Camera;

/**
 *
 * @author Karsten
 */
public class ChunkAppState extends AbstractAppState {

    private Chunk rootChunk;
    private Camera camera;

    public ChunkAppState(Chunk chunk) {
        rootChunk= chunk;
    }

    @Override
    public void initialize(AppStateManager stateManager, Application app) {
        super.initialize(stateManager, app);
        
        camera = app.getCamera();
    }

    @Override
    public void update(float tpf) {
       
        rootChunk.waitForGeometry();
        rootChunk.frameStarted(camera);
      //  rootChunk.frameStarted(null);
    }
    
    public void cleanup() {
        super.cleanup();
        Chunk.closeExecutorThread();
    }
}
