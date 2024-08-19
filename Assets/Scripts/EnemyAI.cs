using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, ITakeDamage {
    private EnemyAudioScript enemyAudioScript = new EnemyAudioScript();

    private enum AnimatorState {
        Run, Crouch, Shoot
    }

    [SerializeField] private float startingHealth;
    [SerializeField] private float minTimeUnderCover;
    [SerializeField] private float maxTimeUnderCover;
    [SerializeField] private int minShotsToTake;
    [SerializeField] private int maxShotsToTake;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float damage;
    [Range(0, 100)] [SerializeField] private float shootingAccuracy;

    [SerializeField] private Transform shootingPosition;
    [SerializeField] private ParticleSystem bloodSplatterFX;

    [Header("Haptic Vest Events")] [SerializeField]
    private UnityEvent hapticEvent1;

    [SerializeField] private UnityEvent hapticEvent2;

    [Header("Scene Transition")] [SerializeField]
    private string nextSceneName; // Nombre de la siguiente escena

    private int remainingEnemies;
    private bool isShooting;
    private int currentShotsTaken;
    private int currentMaxShotsToTake;
    private NavMeshAgent agent;
    private Player player;
    private Transform occupiedCoverSpot;
    private Animator animator;


    [SerializeField] private AudioClip[] audios;

    private AudioSource controlAudio;

    private float health;

    private float Health {
        get => health;
        set => health = Mathf.Clamp(value, 0, startingHealth);
    }

    private void Awake() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        animator.SetTrigger(AnimatorState.Run.ToString());
        health = startingHealth;
        remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        controlAudio = GetComponent<AudioSource>();
    }

    public void Init(Player player, Transform coverSpot) {
        occupiedCoverSpot = coverSpot;
        this.player = player;
        GetToCover();
    }

    private void GetToCover() {
        agent.isStopped = false;
        agent.SetDestination(occupiedCoverSpot.position);
        SelectAudio(1, 0.2f);
    }

    private void Update() {
        if (agent.isStopped == false && (transform.position - occupiedCoverSpot.position).sqrMagnitude <= 0.1f) {
            agent.isStopped = true;
            controlAudio.Stop();
            StartCoroutine(InitializeShootingCo());
        }

        if (isShooting) {
            RotateTowardsPlayer();
        }
    }

    private IEnumerator InitializeShootingCo() {
        HideBehindCover();
        yield return new WaitForSeconds(Random.Range(minTimeUnderCover, maxTimeUnderCover));
        StartShooting();
    }

    private void HideBehindCover() {
        animator.SetTrigger(AnimatorState.Crouch.ToString());
    }

    private void StartShooting() {
        isShooting = true;
        currentMaxShotsToTake = Random.Range(minShotsToTake, maxShotsToTake);
        currentShotsTaken = 0;
        animator.SetTrigger(AnimatorState.Shoot.ToString());
    }

    private void SelectAudio(int index, float volume) {
        controlAudio.PlayOneShot(audios[index], volume);
    }

    [UsedImplicitly] /* Animator calls */
    public void Shoot() {
        RaycastHit hit;
        var direction = player.GetHeadPosition() - shootingPosition.position;
        if (Physics.Raycast(shootingPosition.position, direction, out hit)) {
            enemyAudioScript.PlayShootSound();

            Debug.DrawRay(shootingPosition.position, direction, Color.green, 2.0f);
            var player = hit.collider.GetComponentInParent<Player>();


            if (player) {
                SelectAudio(0, 0.5f);
                if (Random.Range(0, 100) < shootingAccuracy) {
                    player.TakeDamage(damage);
                }
            } else {
                Debug.LogWarning("Ray hit something, but it's not the player.");
            }
        } else {
            Debug.DrawRay(shootingPosition.position, direction, Color.red, 2.0f);
            Debug.LogWarning("Ray did not hit anything.");
        }


        currentShotsTaken++;
        if (currentShotsTaken >= currentMaxShotsToTake) {
            StartCoroutine(InitializeShootingCo());
        }
    }

    private void RotateTowardsPlayer() {
        var direction = player.GetHeadPosition() - transform.position;
        direction.y = 0;
        var rotation = Quaternion.LookRotation(direction);
        rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }

    public void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint) {
        Health -= weapon.GetDamage();
        if (Health <= 0) {
            Destroy(gameObject);
            remainingEnemies--;
            if (remainingEnemies <= 0) {
                Invoke("LoadNextScene", 20f);
            }
        }

        var effect = Instantiate(
            bloodSplatterFX,
            contactPoint,
            Quaternion.LookRotation(weapon.transform.position - contactPoint)
        );
        effect.Stop();
        effect.Play();
    }


    private void LoadNextScene() {
        if (string.IsNullOrEmpty(nextSceneName)) {
            Debug.LogWarning("El nombre de la siguiente escena no está configurado en el inspector.");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}