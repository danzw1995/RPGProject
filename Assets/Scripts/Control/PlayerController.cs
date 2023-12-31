using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
  public class PlayerController : MonoBehaviour
  {

    private Camera mainCamera;

    private Mover mover;
    private Fighter fighter;
    private Health health;

    [System.Serializable]
    private struct CursorMapping
    {
      public CursorType type;
      public Texture2D texture;
      public Vector2 hotspot;
    }

    [SerializeField] private CursorMapping[] cursorMappings = null;

    [SerializeField] private float maxNavMeshProjectionDistance = 1f;

    [SerializeField] private float raycastRadius = 0.3f;

    private void Awake()
    {
      mainCamera = Camera.main;
      mover = GetComponent<Mover>();
      fighter = GetComponent<Fighter>();
      health = GetComponent<Health>();
    }

    private void Update()
    {
      if (InteractWithUI())
      {
        return;
      }
      if (health.IsDead())
      {
        SetCursor(CursorType.None);
        return;
      }

      if (InteractWithComponent()) return;


      // if (InteractWithCombat()) return;
      if (InteractWithMovement()) return;

      SetCursor(CursorType.None);


    }

    private bool InteractWithUI()
    {
      if (EventSystem.current.IsPointerOverGameObject())
      {
        SetCursor(CursorType.UI);
        return true;
      }
      return false;
    }

    private bool InteractWithComponent()
    {
      RaycastHit[] hits = GetRaycastAllSorted();
      foreach (RaycastHit hit in hits)
      {
        IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
        foreach (IRaycastable raycastable in raycastables)
        {
          if (raycastable.HandleRaycast(this))
          {
            SetCursor(raycastable.GetCursorType());
            return true;
          }
        }
      }
      return false;

    }

    private RaycastHit[] GetRaycastAllSorted()
    {
      RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

      float[] distances = new float[hits.Length];
      for (int i = 0; i < hits.Length; i++)
      {
        distances[i] = hits[i].distance;
      }

      Array.Sort(distances, hits);

      return hits;

    }

    private bool InteractWithCombat()
    {
      RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
      foreach (RaycastHit hit in hits)
      {
        CombatTarget target = hit.transform.GetComponent<CombatTarget>();
        if (target == null)
        {
          continue;
        }
        // 判断目标能否被攻击
        if (!fighter.CanAttack(target.gameObject))
        {
          continue;
        }
        if (Input.GetMouseButton(0))
        {
          GetComponent<Fighter>().Attack(target.gameObject);
        }

        SetCursor(CursorType.Combat);
        return true;
      }


      return false;
    }

    private bool InteractWithMovement()
    {
      Vector3 target;
      if (RaycastNavMesh(out target))
      {
        if (Input.GetMouseButton(0))
        {
          mover.StartMoveAction(target, 1f);
        }
        SetCursor(CursorType.Movement);
        return true;
      }
      return false;
    }

    private bool RaycastNavMesh(out Vector3 target)
    {
      target = new Vector3();
      RaycastHit hit;

      bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
      if (!hasHit) return false;

      NavMeshHit navMeshHit;

      bool hasRaycastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

      if (!hasRaycastToNavMesh) return false;

      target = navMeshHit.position;

      if (!mover.CanMoveTo(target)) return false;


      return true;
    }


    private void SetCursor(CursorType type)
    {
      CursorMapping mapping = GetCusorMapping(type);
      Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
    }

    private CursorMapping GetCusorMapping(CursorType type)
    {
      foreach (CursorMapping mapping in cursorMappings)
      {
        if (mapping.type == type)
        {
          return mapping;
        }
      }
      return cursorMappings[0];
    }

    private Ray GetMouseRay()
    {
      return mainCamera.ScreenPointToRay(Input.mousePosition);
    }
  }
}
