using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
  public class Mover : MonoBehaviour, IAction, ISaveable
  {
    [SerializeField] private Transform target;
    [SerializeField] private float maxSpeed = 6f;

    [SerializeField] private float maxNavPathLength = 40f;

    private Ray lastRay;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private ActionScheduler actionScheduler;
    private Health health;
    // Start is called before the first frame update

    private void Awake()
    {
      navMeshAgent = GetComponent<NavMeshAgent>();
      animator = GetComponent<Animator>();

      actionScheduler = GetComponent<ActionScheduler>();
      health = GetComponent<Health>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      navMeshAgent.enabled = !health.IsDead();
      UpdateAnimator();

    }

    public void StartMoveAction(Vector3 destination, float speedFraction)
    {
      actionScheduler.StartAction(this);
      MoveTo(destination, speedFraction);
    }

    public bool CanMoveTo(Vector3 destination)
    {

      NavMeshPath navMeshPath = new NavMeshPath();
      bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, navMeshPath);
      if (!hasPath) return false;
      if (navMeshPath.status != NavMeshPathStatus.PathComplete) return false;
      if (GetPathLength(navMeshPath) > maxNavPathLength) return false;
      return true;
    }


    public void MoveTo(Vector3 destination, float speedFraction)
    {
      navMeshAgent.destination = destination;
      navMeshAgent.speed = maxSpeed * speedFraction;
      navMeshAgent.isStopped = false;

    }

    private float GetPathLength(NavMeshPath navMeshPath)
    {
      float pathLength = 0;

      if (navMeshPath.corners.Length < 2) return pathLength;

      for (int i = 1; i < navMeshPath.corners.Length; i++)
      {
        pathLength += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i - 1]);
      }

      return pathLength;
    }

    public void Cancel()
    {
      navMeshAgent.isStopped = true;
    }

    private void UpdateAnimator()
    {
      Vector3 velocity = navMeshAgent.velocity;
      Vector3 localVelocity = transform.InverseTransformDirection(velocity);
      float speed = localVelocity.z;
      animator.SetFloat("forwardSpeed", speed);
    }

    public object CaptureState()
    {
      return new SerializeVector3(transform.position);
    }

    public void RestoreState(object state)
    {
      SerializeVector3 serializeVector3 = (SerializeVector3)state;
      navMeshAgent.enabled = false;
      transform.position = serializeVector3.ToVector();
      navMeshAgent.enabled = true;
      actionScheduler.CancelCurrentAction();
    }
  }

}
