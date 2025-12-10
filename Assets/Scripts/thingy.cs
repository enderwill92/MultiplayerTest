using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class thingy : MonoBehaviour
{
    public string code;
    public TMP_InputField tmproInputField;
    public TMP_Text tmproTextField;
    public GameObject panel;
    async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("JOIN CODE: " + joinCode);
            tmproTextField.text = "HOSTING \nJOIN CODE: " + joinCode;

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.ServerEndpoints[0].Host,
                (ushort)allocation.ServerEndpoints[0].Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            panel.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            tmproTextField.text =""+ e;
        }
    }

    public async void JoinRelay()
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.ServerEndpoints[0].Host,
                (ushort)allocation.ServerEndpoints[0].Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            panel.SetActive( false );
            tmproTextField.text = "CLIENT \nJOIN CODE: " + code;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            tmproTextField.text = "" + e;
        }
    }

    public void codeUpdate()
    {
        code = tmproInputField.text;
    }
}
