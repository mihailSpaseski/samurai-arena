using UnityEngine;
using Photon.Pun;

public class NetworkedPlayer : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;

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
        }
        else
        {
            playerController.enabled = false;
            GetComponentInChildren<Camera>()?.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            float distance = Vector3.Distance(transform.position, networkPosition);

            // if far away (dashing) snap faster
            float speed = distance > 2f ? 50f : 15f;

            transform.position = Vector3.MoveTowards(
                transform.position,
                networkPosition,
                Time.deltaTime * speed
            );
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                networkRotation,
                Time.deltaTime * 360f
            );
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
        }
        else
        {
            // remote player — receive and store
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            // account for network lag
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (networkPosition - transform.position) * lag;
        }
    }
}