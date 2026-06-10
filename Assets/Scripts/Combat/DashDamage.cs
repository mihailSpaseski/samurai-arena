using System.Collections.Generic;
using UnityEngine;

public class DashDamage : MonoBehaviour
{

    [SerializeField] private int damage = 1;
    [SerializeField] private GameObject bloodSplatterPrefab;

    public int Damage => damage;
    public GameObject BloodSplatterPrefab => bloodSplatterPrefab;

    private HashSet<Collider> hitTargets = new();

    public void ResetHits() => hitTargets.Clear();
    public bool AlreadyHit(Collider col) => hitTargets.Contains(col);
    public void RegisterHit(Collider col) => hitTargets.Add(col);

    // OnTriggerEnter no longer needed — handled by PlayerController
    // [SerializeField] private int damage = 1;
    // [SerializeField] private GameObject bloodSplatterPrefab;
    // private HashSet<Collider> hitTargets = new();

    // private void OnEnable()
    // {
    //     hitTargets.Clear();
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.transform.root == transform.root) return;
    //     if (hitTargets.Contains(other)) return;

    //     hitTargets.Add(other);

    //     // spawn blood at hit point
    //     if (bloodSplatterPrefab != null)
    //         Instantiate(bloodSplatterPrefab,
    //             other.ClosestPoint(transform.position),
    //             Quaternion.identity);

    //     // try networked player first
    //     NetworkedHealth networkedHealth =
    //         other.GetComponentInParent<NetworkedHealth>();

    //     if (networkedHealth != null)
    //     {
    //         networkedHealth.TakeDamage(damage);
    //         return;
    //     }

    //     // fallback for local dummies
    //     DummyHealth dummy = other.GetComponentInParent<DummyHealth>();
    //     if (dummy != null)
    //         dummy.TakeDamage(damage);
    // }

    // private void OnDisable()
    // {
    //     hitTargets.Clear();
    // }

    // public void ResetHits()
    // {
    //     hitTargets.Clear();
    // }
}