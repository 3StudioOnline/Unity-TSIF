namespace ThreeStudio.IPFS.Tests.Shared
{
    public static class TestIpfsConstants
    {
        public static IpfsHttpGatewayConfig DefaultIpfsHttpGatewayConfig = Ipfs.GetIpfsHttpGatewayConfig(IpfsHttpGateway.W3SLink);

        public static readonly IpfsPinningServiceConfig DefaultIpfsPinningServiceConfig =
            Ipfs.GetIpfsPinningServiceConfig(IpfsPinningService.Web3Storage);

        public static readonly string BearerToken_Web3Storage =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkaWQ6ZXRocjoweEQxQWMyMTQwNTdDMTI2OTMyZjQ3YWVCZEY1MjM0OTRiZmE5MzYyQTAiLCJpc3MiOiJ3ZWIzLXN0b3JhZ2UiLCJpYXQiOjE2NzU5NjAwMDAwMTQsIm5hbWUiOiJUU0lGIERlbW8gUHJvamVjdCBCZWFyZXIgVG9rZW4ifQ.Am_B1Vbc_hTIsmfqnOyPi8kJkiJ2vdiCPT3Uqhv7i_o";

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