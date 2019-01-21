using System.Threading.Tasks;

namespace VkNet.TokenMagic.Services.Token.Google
{
    public class ReceiptReceiver
    {
        private readonly MTalkTcpClient _mTalkTcpClient;
        private readonly AndroidHttpClient _androidHttpClient;
        private readonly GoogleSecurityHttpClient _googleSecurityHttpClient;

        public ReceiptReceiver(MTalkTcpClient mTalkTcpClient, AndroidHttpClient androidHttpClient, GoogleSecurityHttpClient googleSecurityHttpClient)
        {
            _mTalkTcpClient = mTalkTcpClient;
            _androidHttpClient = androidHttpClient;
            _googleSecurityHttpClient = googleSecurityHttpClient;
        }

        public async Task<string> GetReceipt()
        {
            var protobuf = await _androidHttpClient.CheckIn().ConfigureAwait(false);
            var googleCredentials = new ProtobufParser(protobuf).Parse();
            _mTalkTcpClient.SendRequest(googleCredentials);
            return await _googleSecurityHttpClient.GetReceipt(googleCredentials).ConfigureAwait(false);
        }
    }
}
