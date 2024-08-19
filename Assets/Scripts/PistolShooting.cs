//este codigo realiza el sonido del disparo del arma al estar agarrado por el mando del oculus quest 2 y se detecta el gatillo para disaprar

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class PlayerShooting : MonoBehaviour {
    // Referencia al AudioManager
    private AudioManager audioManager;
    private AudioSource audioSource;

    //funicon que al detectar el gatillo repoduzca el sonido del arma+
    private void Start() {
        // Obt�n la instancia del AudioManager
        audioManager = AudioManager.instance;
        audioSource = GetComponent<AudioSource>();
    }

    //detectamos gatillo de XRBaseInteractor
    public void Shoot(XRBaseInteractor interactor) {
        if (interactor is XRDirectInteractor) {
            // Reproduce el sonido
            audioManager.Play("PistolShotSound");
            audioSource.Play();
        }
    }
}