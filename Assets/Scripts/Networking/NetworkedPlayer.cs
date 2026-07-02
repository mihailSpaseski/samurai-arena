using UnityEngine;
using Photon.Pun;

public class NetworkedPlayer : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Animator animator;

    private float networkSpeed;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float lag;

    private void Awake()
    {
        // disable control scripts until we know if this is our local player
        playerController.enabled = false;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerController.enabled = true;
            playerController.InitialiseLocalPlayer();

            // find and assign camera to this player
            FindFirstObjectByType<CameraFollow>()?.SetTarget(transform);

            GetComponentInChildren<Camera>()?.gameObject.SetActive(true);

            photonView.RPC("RPC_SetSkin", RpcTarget.AllBuffered, UserProfile.SelectedSkinID);

        }
        else
        {
            playerController.enabled = false;
            GetComponentInChildren<Camera>()?.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void RPC_SetSkin(int skinID)
    {
        GetComponent<PlayerModelSwitcher>()?.ApplySkin(skinID);
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            float distance = Vector3.Distance(transform.position, networkPosition);
            float speed = distance > 2f ? 50f : 15f;

            // use CharacterController.Move instead of transform directly
            // so the physics collider actually updates
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null && cc.enabled)
            {
                Vector3 delta = networkPosition - transform.position;
                cc.Move(delta * Time.deltaTime * speed);
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    networkPosition,
                    Time.deltaTime * speed
                );
            }

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                networkRotation,
                Time.deltaTime * 360f
            );

            if (animator != null)
                animator.SetFloat("Speed", networkSpeed);
        }
    }

    // called automatically by Photon to send/receive data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // we own this player — send position and rotation
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));
        }
        else
        {
            // remote player — receive and store
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkSpeed = (float)stream.ReceiveNext();

            // account for network lag
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (networkPosition - transform.position) * lag;
        }
    }
}