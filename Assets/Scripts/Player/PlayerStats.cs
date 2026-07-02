using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int Kills { get; private set; }
    public int DamageDealt { get; private set; }

    public void AddKill()
    {
        Kills++;
    }

    public void AddDamage(int damage)
    {
        DamageDealt += damage;
    }

    public void ResetStats()
    {
        Kills = 0;
        DamageDealt = 0;
    }
}