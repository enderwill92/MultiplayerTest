using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private TopDownPlayerController m_TopDownPlayerController;

    public SpriteRenderer sr;

    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        default(Color),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
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
        if (IsServer)
        {
            // Assign random color on spawn
            playerColor.Value = Random.ColorHSV();
        }

        // Apply color any time it changes
        playerColor.OnValueChanged += (oldCol, newCol) =>
        {
            sr.color = newCol;
        };

        // Apply immediately on spawn
        sr.color = playerColor.Value;

    }
}
