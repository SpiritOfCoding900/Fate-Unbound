using UnityEngine;

public class Crate : MonoBehaviour, ISmashable
{
    public void SmashThisObject()
    {
        Destroy(gameObject);
    }
}