using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement {
  public class Mover : MonoBehaviour, IAction
{
    [SerializeField] private Transform target;
    private Ray lastRay;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private ActionScheduler actionScheduler;
    // Start is called before the first frame update

    private void Awake() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      animator = GetComponent<Animator>();

      actionScheduler = GetComponent<ActionScheduler>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
   
      UpdateAnimator();

    }

    public void StartMoveAction(Vector3 destination) {
      actionScheduler.StartAction(this);
      MoveTo(destination);
    }


    public void MoveTo(Vector3 destination) {
      navMeshAgent.destination = destination;
      navMeshAgent.isStopped = false;

    }

    public void Cancel() {
      navMeshAgent.isStopped = true;
    }

    private void UpdateAnimator() {
      Vector3 velocity = navMeshAgent.velocity;
      Vector3 localVelocity = transform.InverseTransformDirection(velocity);
      float speed = localVelocity.z;
      animator.SetFloat("forwardSpeed", speed);
    }
}

} 