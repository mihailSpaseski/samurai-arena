using UnityEngine;
using Photon.Pun;

public class NetworkedHealth : MonoBehaviourPun
{
    [SerializeField] private PlayerHealth playerHealth;

    public void TakeDamage(int damage, int attackerViewID)
    {
        photonView.RPC("RPC_TakeDamage", photonView.Owner, damage, attackerViewID);
    }

    [PunRPC]
    private void RPC_TakeDamage(int damage, int attackerViewID)
    {
        int damageDone = Mathf.Min(damage, playerHealth.CurrentHealth);

        playerHealth.TakeDamage(damage);

        PhotonView attackerView = PhotonView.Find(attackerViewID);
        if (attackerView != null)
        {
            PlayerStats attackerStats = attackerView.GetComponent<PlayerStats>();
            attackerStats?.AddDamage(damageDone);

            if (playerHealth.CurrentHealth <= 0)
                attackerStats?.AddKill();
        }

        // only sync health bar if player is still alive
        if (playerHealth.CurrentHealth > 0)
        {
            photonView.RPC("RPC_SyncHealthBar", RpcTarget.All,
                playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
    }

    [PunRPC]
    private void RPC_SyncHealthBar(int current, int max)
    {
        playerHealth.UpdateHealthBar(current, max);
    }

    [PunRPC]
    public void RPC_Die()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}