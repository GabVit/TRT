using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class CivilAI : MonoBehaviour, ITakeDamage {
    private enum AnimatorState {
        Run,
        Crouch
    }

    [SerializeField] private float startingHealth;
    [SerializeField] private float minTimeUnderCover;
    [SerializeField] private float maxTimeUnderCover;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float damage;
    [Range(0, 100)] [SerializeField] private ParticleSystem bloodSplatterFX;


    private NavMeshAgent agent;
    private Player player;
    private Transform occupiedCoverSpot;
    private Animator animator;
    private float health;

    private static int woundedCount;
    private static int deadCount;

    [SerializeField] private AudioClip[] audios;
    private AudioSource controlAudio;

    private float Health {
        get => health;
        set => health = Mathf.Clamp(value, 0, startingHealth);
    }

    private void Awake() {
        Initialize();
        controlAudio = GetComponent<AudioSource>();
    }

    private void Initialize() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        health = startingHealth;
        animator.SetTrigger(AnimatorState.Run.ToString());
    }

    public void Init(Player player, Transform coverSpot) {
        occupiedCoverSpot = coverSpot;
        this.player = player;
        GetToCover();
    }

    private void GetToCover() {
        agent.isStopped = false;
        agent.SetDestination(occupiedCoverSpot.position);
        SeleccionAudio(0, 0.2f);
    }

    private void Update() {
        RotateTowardsPlayer();

        if (
            agent.isStopped ||
            (transform.position - occupiedCoverSpot.position).sqrMagnitude <= 0.1f
        ) return;
        agent.isStopped = true;
        controlAudio.Stop();
        HideBehindCover();
    }

    private void SeleccionAudio(int indice, float volumen) {
        controlAudio.PlayOneShot(audios[indice], volumen);
    }

    private void HideBehindCover() {
        animator.SetTrigger(AnimatorState.Crouch.ToString());
        StartCoroutine(StayUnderCover());
    }

    private IEnumerator StayUnderCover() {
        var timeUnderCover = Random.Range(minTimeUnderCover, maxTimeUnderCover);
        yield return new WaitForSeconds(timeUnderCover);
        GetToCover();
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
            Debug.Log("Civil destroyed!");
            deadCount++;
        } else {
            woundedCount++;
        }

        var effect = Instantiate(
            bloodSplatterFX,
            contactPoint,
            Quaternion.LookRotation(weapon.transform.position - contactPoint)
        );
        effect.Stop();
        effect.Play();
    }

    private void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20), "Wounded: " + woundedCount);
        GUI.Label(new Rect(10, 30, 100, 20), "Dead: " + deadCount);
    }
}