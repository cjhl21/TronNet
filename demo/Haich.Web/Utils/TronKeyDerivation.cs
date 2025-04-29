using NBitcoin;
using NBitcoin.DataEncoders;
using TronNet;
using TronNet.Crypto;

namespace Haich.Web.Utils
{
    public class TronKeyDerivation
    {
        public static ExtKey DeriveTronMasterKey(byte[] seed)
        {
            // 根据 BIP44 路径生成主密钥
            var masterKey = ExtKey.CreateFromSeed(seed);
            // TRON 的 BIP44 路径：m/44'/195'/0'/0/0
            var purpose = masterKey.Derive(44, true);  // 强化派生 m/44'
            var coinType = purpose.Derive(195, true);  // TRON 的 coin_type 是 195
            var account = coinType.Derive(0, true);    // 账户索引 m/44'/195'/0'
            var change = account.Derive(0);            // 外部链（0）或找零链（1）
            return change;
        }

        public static string DeriveTronPrivateKey(ExtKey changeChain, uint index)
        {
            // 派生指定索引的子私钥
            var childKey = changeChain.Derive(index);
            // 转换为 TRON 格式的私钥（64位十六进制）
            return childKey.PrivateKey.ToHex();
        }

        public static string GetTronAddressFromPrivateKey(string privateKey)
        {
            // 通过私钥生成 TRON 地址
            //var privateKeyBytes = Encoders.Hex.DecodeData(privateKeyHex);
            //var ecKey = new ECKey(privateKeyBytes, true);
            //return ecKey.GetPublicAddress();

            // 示例：通过已知私钥初始化 TronECKey
            var ecKey = new TronECKey(privateKey, TronNetwork.MainNet);
            string privateKey1 = ecKey.GetPrivateKey();
            return ecKey.GetPublicAddress();
        }
    }
}
