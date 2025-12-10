using Unity.Netcode;
using UnityEngine;

public class HostAndClientButtons : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
