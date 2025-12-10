using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private TopDownPlayerController m_TopDownPlayerController;

    private void Awake()
    {
        m_TopDownPlayerController.enabled = false;
        m_PlayerInput.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            m_TopDownPlayerController.enabled = true;
            m_PlayerInput.enabled = true;
        }

    }
}
