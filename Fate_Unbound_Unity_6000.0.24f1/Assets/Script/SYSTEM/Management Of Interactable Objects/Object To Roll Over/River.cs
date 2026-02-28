using UnityEngine;

public class River : MonoBehaviour, IRollable
{
    public void RollOverObject()
    {
        // optional: splash VFX, sound, stamina cost, etc
        AudioManager.Instance.SFXSound(SoundID.Confirm);
        Debug.Log("Rolled over river!");
    }
}