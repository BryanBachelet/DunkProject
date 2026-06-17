using UnityEngine;

public interface IInteractibleObject  
{
    public abstract void CollisionInteraction(IInteractibleObject Obj);
    public abstract GameObject GetGameObject();

}
