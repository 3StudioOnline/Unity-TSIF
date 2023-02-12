using ThreeStudio.IPFS;
using UnityEngine;

namespace _00_Project.ThreeStudio.IPFS.Scripts
{
    public class ProjectDemo : MonoBehaviour
    {
        private async void Start()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.UploadFileOrData, Ipfs.DebugMode.DownloadFileOrGetData);

            IpfsAddress addressTestString = new IpfsAddress("bafkreih77yzma2itvkuw3xnfaarqhi4wcihkd5fg2ihq2kmiwignuaafsm");

            (bool success, string errorMessage, HttpResponse response, string dataString) result = await IpfsFunctionLibrary.GetDataAsStringAsync(
                Ipfs.GetIpfsHttpGatewayConfig(IpfsHttpGateway.GatewayPinataCloud),
                addressTestString);
            Debug.Log($"Data String (async): {result.dataString}");

            IpfsFunctionLibrary.GetDataAsString(
                Ipfs.GetIpfsHttpGatewayConfig(IpfsHttpGateway.GatewayPinataCloud),
                addressTestString,
                ResponseDelegate);
        }

        private void ResponseDelegate(bool success, string errorMessage, HttpResponse response, string dataString)
        {
            Debug.Log($"Data String: {dataString}");
        }
    }
}