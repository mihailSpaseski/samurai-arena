using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int DamageDealt { get; private set; }
    public int Kills { get; private set; }

    public void AddDamage(int amount)
    {
        DamageDealt += amount;
    }

    public void AddKill()
    {
        Kills++;
    }
}