using UnityEngine;

namespace RPG.Combat
{
  public class Health : MonoBehaviour {
    [SerializeField] private float healthPoints = 100f;
    private bool isDead = false;

    public void TakeDamage(float damage) {
      healthPoints = Mathf.Max(healthPoints - damage, 0);
      print(healthPoints);
      if (healthPoints == 0) {
        Die();
      }
    }

    public void Die() {
      if (isDead) return;
      isDead = true;
      GetComponent<Animator>().SetTrigger("die");
    }
  }
}
