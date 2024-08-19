using System.Collections;
using UnityEngine;

public class EnemyAudioScript : MonoBehaviour {
    public string shootAudioName;
    public string deathAudioName;

    public void PlayShootSound() {
        if (string.IsNullOrEmpty(shootAudioName)) return;
        AudioManager.instance.Play(shootAudioName);
    }

    public void PlayDeathSound() {
        if (string.IsNullOrEmpty(deathAudioName)) return;
        AudioManager.instance.Play(deathAudioName);
    }
}