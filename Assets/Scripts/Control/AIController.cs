﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
  public class AIController : MonoBehaviour
  {
    // 追逐距离
    [SerializeField] private float chaseDistance = 5f;

    // 怀疑时间
    [SerializeField] private float suspicionTime = 3f;

    // 导航点停留时间
    [SerializeField] private float waypointDwellTime = 3f;

    [SerializeField] private float aggrevateCooldownTime = 5f;

    [SerializeField] private float shoutDistance = 5f;

    // 巡逻路径
    [SerializeField] private PatrolPath patrolPath = null;

    // 导航点公差
    [SerializeField] private float waypointTolerance = 1f;

    // 巡逻速度分数比例
    [SerializeField] private float patrolSpeedFraction = 0.2f;

    private GameObject player;
    private Fighter fighter;
    private Health health;
    private Mover mover;

    private ActionScheduler actionScheduler;

    private LazyValue<Vector3> guardPosition;

    private float timeSinceLastSawPlayer = Mathf.Infinity;
    private float timeSinceArrivedWaypoint = Mathf.Infinity;
    private float timeSinceAggrevated = Mathf.Infinity;

    private int currentWaypointIndex = 0;

    private void Awake() 
    {
      player = GameObject.FindWithTag("Player");
      fighter = GetComponent<Fighter>();
      health = GetComponent<Health>();
      mover = GetComponent<Mover>();
      actionScheduler = GetComponent<ActionScheduler>();

      guardPosition = new LazyValue<Vector3>(GetInitGuardPosition);
    }

    private void Start()
    {
      guardPosition.ForceInit();
    }
    // Update is called once per frame
    private void Update()
    {
      if (health.IsDead()) return;

      if (IsAggrevate() && fighter.CanAttack(player))
      {
        AttackBehavior();
      }
      else if (timeSinceLastSawPlayer < suspicionTime)
      {
        SuspicionBehavior();
      }
      else
      {
        PatrolBehavior();
      }
      UpdateTimer();
    }

    private Vector3 GetInitGuardPosition()
    {
      return transform.position;
    }

    private void AttackBehavior()
    {
      timeSinceLastSawPlayer = 0;

      fighter.Attack(player);

      AggrevateNearByEnemies();
    }

    private void AggrevateNearByEnemies ()
    {
      RaycastHit[] hits =  Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

      foreach(RaycastHit hit in hits)
      {
        AIController controller = hit.collider.gameObject.GetComponent<AIController>();
        if (controller != null)
        {
          controller.Aggrevate();
        }
      }
    }

    private void SuspicionBehavior()
    {
      actionScheduler.CancelCurrentAction();

    }

    private void PatrolBehavior()
    {
      Vector3 nextPosition = guardPosition.value;

      if (patrolPath != null)
      {
        if (AtWaypoint())
        {
          timeSinceArrivedWaypoint = 0;

          CycleWayPoint();
        }
        nextPosition = GetCurrentWayPosition();
      }
      if (timeSinceArrivedWaypoint > waypointDwellTime)
      {

        mover.StartMoveAction(nextPosition, patrolSpeedFraction);
      }

    }

    private void UpdateTimer()
    {
      timeSinceLastSawPlayer += Time.deltaTime;
      timeSinceArrivedWaypoint += Time.deltaTime;
      timeSinceAggrevated += Time.deltaTime;
    }

    private bool AtWaypoint()
    {
      float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWayPosition());
      return distanceToWaypoint < waypointTolerance;
    }

    private void CycleWayPoint()
    {
      currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
    }

    private Vector3 GetCurrentWayPosition()
    {
      return patrolPath.GetWaypoint(currentWaypointIndex);
    }

    public void Aggrevate()
    {
      timeSinceAggrevated = 0;
    }

    private bool IsAggrevate()
    {
      float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
      return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggrevateCooldownTime;
    }

    // Call by unity
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
  }

}
