using UnityEngine;

public class NetworkComponent : MonoBehaviour
{
    public GameNetwork gameNetwork { get; private set; }
    public string OwnerID { get; set; }

    public bool IsMine
    {
        get
        {
            if (gameNetwork.player.ID == OwnerID)
                return true;

            return false;
        }
    }

    void Awake()
    {
        gameNetwork = FindObjectOfType<GameNetwork>();
        gameNetwork.NetworkUpdate += OnNetworkUpdate;
        OwnerID = "";
    }

    public void Register(GameObject gameObject)
    {
        gameNetwork.registeredNetworkComponents.Add(this);
    }

    public void OnNetworkUpdate(GameNetwork gameNetwork)
    {
        if (IsMine)
        {
            gameNetwork.SendPosition(transform.position);
        }
    }
}