using System;

namespace ThreeStudio.IPFS.Tests.Shared
{
    public static class TestIpfsConstants
    {
        public static IpfsHttpGatewayConfig DefaultIpfsHttpGatewayConfig = Ipfs.GetIpfsHttpGatewayConfig(IpfsHttpGateway.W3SLink);

        public static readonly IpfsPinningServiceConfig DefaultIpfsPinningServiceConfig =
            Ipfs.GetIpfsPinningServiceConfig(IpfsPinningService.Web3Storage);

        public static readonly string BearerToken_Web3Storage = Environment.GetEnvironmentVariable("TSIF_BEARER_TOKEN_WEB3STORAGE");

        public static readonly IpfsAddress AddressTestImage = new IpfsAddress("bafkreihutddv4nrs3fj246puy72mexgbnf5bvmqug3o2sjgjubia7ka64i");

        public static readonly IpfsAddress AddressTestString = new IpfsAddress("bafkreih77yzma2itvkuw3xnfaarqhi4wcihkd5fg2ihq2kmiwignuaafsm");

        public static readonly IpfsAddress AddressTestImageWithPath = new IpfsAddress(
            "bafybeidqlo4cju5m3rgw3vpkukfk5yobzgs7xxpuaga4leqfmpwui4fmuu",
            "3S Studio.png");

        public static readonly IpfsAddress AddressTestStringWithPath = new IpfsAddress(
            "bafybeifemj2ipsvbzxts6lj4xo3y5cl44ekqqdfgn5tdydqmywyain2jye",
            "3S Hello.txt");
    }
}