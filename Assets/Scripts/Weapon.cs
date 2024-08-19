using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour {
    [SerializeField] protected float shootingForce = 50;
    [SerializeField] protected Transform bulletSpawn;
    [SerializeField] private float recoilForce;
    [SerializeField] private float damage;

    [SerializeField] private AudioClip[] audios;
    private AudioSource controlAudio;

    private Rigidbody rigidBody;
    private XRGrabInteractable interactableWeapon;
    private AudioManagerEasy audioManagerEasy;

    protected virtual void Awake() {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        SetupInteractableWeaponEvents();
        controlAudio = GetComponent<AudioSource>();
    }

    private void SetupInteractableWeaponEvents() {
        interactableWeapon.onSelectEntered.AddListener(PickUpWeapon);
        interactableWeapon.onSelectExited.AddListener(DropWeapon);
        interactableWeapon.onActivate.AddListener(StartShooting);
        interactableWeapon.onDeactivate.AddListener(StopShooting);
    }

    private void SeleccionAudio(int indice, float volumen) {
        controlAudio.PlayOneShot(audios[indice], volumen);
    }

    private void PickUpWeapon(XRBaseInteractor interactor) {
        interactor.GetComponent<MeshHidder>().Hide();
    }

    private void DropWeapon(XRBaseInteractor interactor) {
        interactor.GetComponent<MeshHidder>().Show();
    }

    protected virtual void StartShooting(XRBaseInteractor interactor)
    {
    }

    protected virtual void StopShooting(XRBaseInteractor interactor)
    {
    }

    protected virtual void Shoot() {
        ApplyRecoil();
        SeleccionAudio(0, 0.2f);
        Invoke("SeleccionAudio(1, 0.2f)", 0.5f);
        //Player.SendBTMessage("F");
    }

    private void ApplyRecoil() {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce() {
        return shootingForce;
    }

    public float GetDamage() {
        return damage;
    }
}