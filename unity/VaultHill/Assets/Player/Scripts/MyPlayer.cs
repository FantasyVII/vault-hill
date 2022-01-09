using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : MonoBehaviour
{
    [SerializeField] Text usernameText;
    [SerializeField] float speed;

    NetworkComponent networkComponent;

    void Start()
    {
        networkComponent = GetComponent<NetworkComponent>();
    }

    void Update()
    {
        if (networkComponent.IsMine)
        {
            if (Input.GetKey(KeyCode.W))
                transform.position += Vector3.forward * speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.S))
                transform.position += Vector3.back * speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.D))
                transform.position += Vector3.right * speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.A))
                transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    public void SetUsername(string username)
    {
        usernameText.text = username;
    }
}