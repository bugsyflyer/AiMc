using Unity.VisualScripting;
using UnityEngine;

public class BlockWrapper : MonoBehaviour
{
    //public Block blockInstance;

    public float getBlockX()
    {
        return transform.position.x;
    }
    public float getBlockY()
    {
        return transform.position.y;
    }
    public float getBlockZ()
    {
        return transform.position.z;
    }

    public void Break()
    {
        Destroy(gameObject);
    }
}