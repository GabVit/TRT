using UnityEngine;

public class CivilFootsteps : MonoBehaviour {
    private AudioManager audioManager;

    private void Start() {
        audioManager = AudioManager.instance;
    }

    public void PlayFootstepSound() {
        audioManager.Play("CivilFootstepSound");
    }
}