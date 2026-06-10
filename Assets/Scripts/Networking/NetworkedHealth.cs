using UnityEngine;
using Photon.Pun;

public class NetworkedHealth : MonoBehaviourPun
{
    [SerializeField] private PlayerHealth playerHealth;

    public void TakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", photonView.Owner, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        playerHealth.TakeDamage(damage);

        // sync health bar to all clients after taking damage
        photonView.RPC("RPC_SyncHealthBar", RpcTarget.All,
            playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    [PunRPC]
    private void RPC_SyncHealthBar(int current, int max)
    {
        playerHealth.UpdateHealthBar(current, max);
    }

    [PunRPC]
    private void RPC_Die()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}