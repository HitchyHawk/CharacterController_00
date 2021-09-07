using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PhysicsModes{
    STANDING, WALKING, RUNNING, CROUCHING, VAULTING, SLIDING,
    AIRBORNE,
    CLIMBING,
    RAGDOLL
}
public class TraversalFacade {
    Vector3 currentVelocity = Vector3.zero;
    Transform body;
    Vector3[] capsuleOffsets = {Vector3.zero, Vector3.up};
    Vector3 sphereOffset = Vector3.up;

    PhysicsActor currentPhysics = new GroundTraversal(); //This way every traversal type can have a start state, which we dont have to reset every time.
    PhysicsModes currentPhysicsMode = PhysicsModes.STANDING;
    PhysicsActor[] movementSelection = {new GroundTraversal(), new AirTraversal(), new ClimbingTraversal()};
    internal enum key{
        GROUND_TRAVERSAL = 0,
        AIR_TRAVERSAL,
        CLIMBING_TRAVERSAL
    }
    PhysicsSettings settings;

    public void refresh(Vector3 influence, bool jump, bool sprint, bool crouch){
        //Debug.Log("Hello?");
        capsuleOffsets[0] = Vector3.up * (settings.stepUpMax + settings.collisionRadius);
        capsuleOffsets[1] = capsuleOffsets[0] + Vector3.up*settings.collisionHeight;
        currentVelocity = currentPhysics.getVelocity();

        Vector3 workingInfluence = influence;

        switch (currentPhysicsMode){
            case PhysicsModes.STANDING: case PhysicsModes.WALKING: case PhysicsModes.RUNNING:{
                Debug.Log("GROUNDED!!");
                if (!currentPhysics.inValidState()){
                    //if on a slidable terrain.

                    currentPhysics = movementSelection[(int)key.AIR_TRAVERSAL];
                    currentPhysicsMode = PhysicsModes.AIRBORNE;
                    currentPhysics.reset();
                    currentPhysics.setVelocity(currentVelocity);
                    
                }

                if (jump){
                    //if infront of climable that fits a vaulatable, and enough speed, vault it
                    //else if infront of climbable and moving towards it, step up it.
                    

                    currentPhysics = movementSelection[(int)key.AIR_TRAVERSAL]; //AIR TRAVERSAL
                    currentPhysicsMode = PhysicsModes.AIRBORNE;
                    currentPhysics.reset();
                    currentPhysics.setVelocity(currentVelocity);
                    workingInfluence.y = 1;
                }                
            }
            break;

            case PhysicsModes.CROUCHING: case PhysicsModes.SLIDING: {
                
            }
            break;

            case PhysicsModes.CLIMBING:{
                
            }
            break;

            case PhysicsModes.AIRBORNE: default: {
                Debug.Log("AIRBORNE!!");
                if (!currentPhysics.inValidState()){
                    Debug.Log("in-valid");
                    currentPhysicsMode = PhysicsModes.STANDING;
                    currentPhysics = movementSelection[(int)key.GROUND_TRAVERSAL];
                    currentPhysics.setVelocity(currentVelocity);
                }

            }
            break;
        }

        
        currentPhysics.refresh(workingInfluence);
    }

    public void setUp(){
        capsuleOffsets[0] = Vector3.up * (settings.collisionRadius);
        capsuleOffsets[1] = capsuleOffsets[0] + Vector3.up * settings.collisionHeight;
        sphereOffset = Vector3.up * (settings.collisionRadius);

        foreach (PhysicsActor actor in movementSelection){
            actor.setPhysicsObject(body);
            actor.setSettings(settings);
            try{
                actor.setCapsules(capsuleOffsets); //done last as colliders generally need settings in order to calculate/ generate properly
                actor.setSphere(sphereOffset);
            } catch (System.NotImplementedException){}
        }

        //need to set this up too. 
        currentPhysics.setPhysicsObject(body);
        currentPhysics.setSettings(settings);
        try{
            currentPhysics.setCapsules(capsuleOffsets);
            currentPhysics.setSphere(sphereOffset);
        } catch (System.NotImplementedException){} 
    }

    public Vector3 getVelocity() => currentVelocity;
    public PhysicsModes getCurrentMode() => currentPhysicsMode;
    public void setPhysicsBody(Transform newBody){
        body = newBody;
    } 
    public void setPhysicsSettings(PhysicsSettings newSettings){
        settings = newSettings;
    }
}
