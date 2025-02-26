using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Connexion à Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecté au serveur Photon !");
        PhotonNetwork.JoinLobby(); // Rejoint automatiquement un lobby
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connecté au lobby !");
        PhotonNetwork.JoinOrCreateRoom("Salle1", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joueur rejoint la salle !");
        Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), 0, 0);
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}
public class PlayerControllerPun : MonoBehaviourPun
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Désactiver le contrôle si ce n'est pas notre joueur
        if (!photonView.IsMine)
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    void Update()
    {
        // Seul le joueur local peut bouger
        if (!photonView.IsMine) return;

        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1); // Flip sprite
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            photonView.RPC("SyncJumpAnimation", RpcTarget.All);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    [PunRPC]
    void SyncJumpAnimation()
    {
        anim.SetTrigger("Jump");
    }
}