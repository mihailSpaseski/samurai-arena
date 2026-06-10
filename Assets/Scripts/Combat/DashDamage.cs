using System.Collections.Generic;
using UnityEngine;

public class DashDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private GameObject bloodSplatterPrefab;

    public int Damage => damage;
    public GameObject BloodSplatterPrefab => bloodSplatterPrefab;

    private readonly HashSet<GameObject> hitTargets = new();

    public void ResetHits() => hitTargets.Clear();

    public bool AlreadyHit(GameObject target)
    {
        return hitTargets.Contains(target);
    }

    public void RegisterHit(GameObject target)
    {
        hitTargets.Add(target);
    }
}