using System.Collections.Generic;
using UnityEngine;

public class DashDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private HashSet<Collider> hitTargets = new();

    private void OnEnable()
    {
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root)
        {
            return;
        }
        if (hitTargets.Contains(other))
            return;

        hitTargets.Add(other);

        DummyHealth dummy =
            other.GetComponentInParent<DummyHealth>();

        if (dummy != null)
        {
            Debug.Log("Dummy hit!");

            dummy.TakeDamage(damage);
        }
    }

    private void OnDisable()
    {
        hitTargets.Clear();
    }

     public void ResetHits()
    {
        hitTargets.Clear();
    }
}