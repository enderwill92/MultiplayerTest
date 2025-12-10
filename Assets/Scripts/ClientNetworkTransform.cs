using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
