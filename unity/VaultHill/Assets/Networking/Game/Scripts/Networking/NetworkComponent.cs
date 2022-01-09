using UnityEngine;

public class NetworkComponent : MonoBehaviour
{
    GameNetwork gameNetwork;
    string ownerID;

    public bool IsMine
    {
        get
        {
            if (gameNetwork.player.ID == ownerID)
                return true;

            return false;
        }
    }

    void Awake()
    {
        gameNetwork = FindObjectOfType<GameNetwork>();
        ownerID = "";
    }

    public void SetOwnerID(string ownerID)
    {
        this.ownerID = ownerID;
    }
}