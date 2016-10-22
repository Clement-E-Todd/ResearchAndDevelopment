using UnityEngine;

public abstract class HexTerrainElement : MonoBehaviour
{
    protected bool doNotRecreateOnDestroy = false;

    public abstract HexTerrain GetTerrain();
    
    public void DestoryWithoutRecreating()
    {
        doNotRecreateOnDestroy = true;
        DestroyImmediate(gameObject);
    }
}