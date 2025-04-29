using Haich.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;

namespace Haich.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger<DemoController> _logger;
        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Test")]
        public string Test()
        {
            // 1. 安装必要的 NuGet 包
            //Install - Package BouncyCastle.Cryptography  # 加密算法支持
            //Install-Package NBitcoin                   # BIP39/BIP44 实现
            //Install-Package TronNet                   # TRON 地址编码

            //2.生成助记词（BIP39）
            // 示例：生成 12 个单词的助记词
            //Mnemonic mnemonic = MnemonicGenerator.GenerateMnemonic(12);
            //string strMnemonic = string.Join(" ", mnemonic.Words);
            //Console.WriteLine("助记词: " + strMnemonic);

            Mnemonic mnemonic = new Mnemonic("still museum oil nose laugh cheap meat step cage artefact already bubble");

            //3.从助记词生成种子（BIP39）
            // 生成种子
            byte[] seed = MnemonicGenerator.GenerateSeedFromMnemonic(mnemonic,""); // Haich20250327Hao
            Console.WriteLine("种子 (Hex): " + BitConverter.ToString(seed).Replace("-", "").ToLower());
            // 输出示例：b5d3e6f2a1c8d7e0...

            // (1) 生成主私钥
            ExtKey masterKey = ExtKey.CreateFromSeed(seed);
            string masterPrivateKey = masterKey.PrivateKey.ToHex(); // 真实主私钥
            //string address = TronKeyDerivation.GetTronAddressFromPrivateKey(masterPrivateKey);

            // 主私钥是由种子生成的唯一密钥，而子私钥则是通过主私钥和派生路径生成的多个密钥。
            // 同一个助记词和密码生成的种子只能对应一个主私钥，但可以生成无数个子私钥。

            //var purpose = masterKey.Derive(44, true);    // m/44'
            //var coinType = purpose.Derive(195, true);    // m/44'/195'
            //var account = coinType.Derive(0, true);      // m/44'/195'/0'
            //var change = account.Derive(0);             // m/44'/195'/0'/0
            // (2) 派生 TRON 扩展密钥
            //var tronPath = new KeyPath("m/44'/195'/0'"); // BIP44 TRON 路径
            //var tronMasterKey = masterKey.Derive(tronPath); // TRON 主扩展密钥
            // (3) 派生具体地址私钥
            //var addressKey = tronMasterKey.Derive(0).Derive(0); // m/44'/195'/0'/0/0
            //string privateKey = addressKey.PrivateKey.ToHex();

            //4.派生 TRON 私钥（BIP44）
            // 示例：派生前 5 个 TRON 地址
            // 根据 BIP44 路径派生的 TRON 主扩展密钥（用于后续派生具体地址的私钥）。基于 BIP44 路径 m/44'/195'/0' 派生的密钥，用于 TRON 地址生成。
            ExtKey deriveMasterKey = TronKeyDerivation.DeriveTronMasterKey(seed);
            for (uint i = 0; i < 3; i++)
            {
                // 派生私钥
                string privateKey = TronKeyDerivation.DeriveTronPrivateKey(deriveMasterKey, i);
                // 生成地址
                string address = TronKeyDerivation.GetTronAddressFromPrivateKey(privateKey);
                Console.WriteLine($"地址 {i}: {address}");
                Console.WriteLine($"私钥 {i}: {privateKey}\n");
            }

            return "";
        }

        public static ExtKey DeriveCustomPath(byte[] seed, string path = "m/44'/195'/0'/0/0")
        {
            ExtKey extKey = ExtKey.CreateFromSeed(seed);
            var segments = path.Split('/');
            foreach (var segment in segments.Skip(1))
            {
                bool hardened = segment.EndsWith("'");
                uint index = hardened ?
                    uint.Parse(segment.TrimEnd('\'')) | 0x80000000 :
                    uint.Parse(segment);
                extKey = extKey.Derive(index);
            }
            return extKey;
        }
    }
}
