using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
public class ClickSteerBall : NetworkBehaviour
{
    private Rigidbody2D rb;
    private LineRenderer line;
    private TrailRenderer trail;
    private SpriteRenderer spriteRenderer;

    public float slingshotForce = 15f;
    public float homingStrength = 0f;
    public Transform target;

    public AudioSource audioSource;
    public AudioClip slingshotSound;
    public AudioClip bounceSound;
    public AudioClip targetSound;

    private bool dragging = false;

    // Networked color
    private NetworkVariable<Color> ballColor = new NetworkVariable<Color>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
        trail = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        // Server assigns the color once
        if (IsServer)
            ballColor.Value = Random.ColorHSV();

        ApplyColor(ballColor.Value);

        ballColor.OnValueChanged += (oldCol, newCol) => ApplyColor(newCol);

        // Only owner sees the slingshot line
        line.enabled = false;
    }

    private void ApplyColor(Color c)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = c;

        if (trail != null)
            trail.startColor = c;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        HandleSlingshot();
        ApplyHoming();
    }

    private void HandleSlingshot()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float slingRadius = 1f;

        // Start dragging
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Vector2.Distance(mouseWorldPos, rb.position) <= slingRadius)
            {
                dragging = true;
                line.enabled = true;
                line.positionCount = 2;
            }
        }

        // Update line
        if (dragging)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, mouseWorldPos);
        }

        // Release
        if (Mouse.current.leftButton.wasReleasedThisFrame && dragging)
        {
            dragging = false;
            line.enabled = false;

            Vector2 launchDir = (Vector2)transform.position - mouseWorldPos;

            // Apply force immediately locally
            rb.AddForce(launchDir * slingshotForce, ForceMode2D.Impulse);

            // Optionally, sync with server for others to see proper physics
            SyncVelocity_ServerRpc(rb.linearVelocity);

            if (audioSource != null && slingshotSound != null)
                audioSource.PlayOneShot(slingshotSound);
        }
    }

    private void ApplyHoming()
    {
        if (target == null || homingStrength <= 0f)
            return;

        Vector2 toTarget = (target.position - transform.position).normalized;
        Vector2 vel = rb.linearVelocity;

        Vector2 newVel = Vector2.Lerp(vel, toTarget * vel.magnitude, homingStrength * Time.deltaTime);
        rb.linearVelocity = newVel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (audioSource != null && bounceSound != null)
            audioSource.PlayOneShot(bounceSound);
    }

    // Optional velocity sync: keeps other clients seeing correct motion
    [ServerRpc]
    private void SyncVelocity_ServerRpc(Vector2 newVelocity)
    {
        rb.linearVelocity = newVelocity;
    }
}
