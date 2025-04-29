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
            // 1. ��װ��Ҫ�� NuGet ��
            //Install - Package BouncyCastle.Cryptography  # �����㷨֧��
            //Install-Package NBitcoin                   # BIP39/BIP44 ʵ��
            //Install-Package TronNet                   # TRON ��ַ����

            //2.�������Ǵʣ�BIP39��
            // ʾ�������� 12 �����ʵ����Ǵ�
            //Mnemonic mnemonic = MnemonicGenerator.GenerateMnemonic(12);
            //string strMnemonic = string.Join(" ", mnemonic.Words);
            //Console.WriteLine("���Ǵ�: " + strMnemonic);

            Mnemonic mnemonic = new Mnemonic("still museum oil nose laugh cheap meat step cage artefact already bubble");

            //3.�����Ǵ��������ӣ�BIP39��
            // ��������
            byte[] seed = MnemonicGenerator.GenerateSeedFromMnemonic(mnemonic,""); // Haich20250327Hao
            Console.WriteLine("���� (Hex): " + BitConverter.ToString(seed).Replace("-", "").ToLower());
            // ���ʾ����b5d3e6f2a1c8d7e0...

            // (1) ������˽Կ
            ExtKey masterKey = ExtKey.CreateFromSeed(seed);
            string masterPrivateKey = masterKey.PrivateKey.ToHex(); // ��ʵ��˽Կ
            //string address = TronKeyDerivation.GetTronAddressFromPrivateKey(masterPrivateKey);

            // ��˽Կ�����������ɵ�Ψһ��Կ������˽Կ����ͨ����˽Կ������·�����ɵĶ����Կ��
            // ͬһ�����Ǵʺ��������ɵ�����ֻ�ܶ�Ӧһ����˽Կ��������������������˽Կ��

            //var purpose = masterKey.Derive(44, true);    // m/44'
            //var coinType = purpose.Derive(195, true);    // m/44'/195'
            //var account = coinType.Derive(0, true);      // m/44'/195'/0'
            //var change = account.Derive(0);             // m/44'/195'/0'/0
            // (2) ���� TRON ��չ��Կ
            //var tronPath = new KeyPath("m/44'/195'/0'"); // BIP44 TRON ·��
            //var tronMasterKey = masterKey.Derive(tronPath); // TRON ����չ��Կ
            // (3) ���������ַ˽Կ
            //var addressKey = tronMasterKey.Derive(0).Derive(0); // m/44'/195'/0'/0/0
            //string privateKey = addressKey.PrivateKey.ToHex();

            //4.���� TRON ˽Կ��BIP44��
            // ʾ��������ǰ 5 �� TRON ��ַ
            // ���� BIP44 ·�������� TRON ����չ��Կ�����ں������������ַ��˽Կ�������� BIP44 ·�� m/44'/195'/0' ��������Կ������ TRON ��ַ���ɡ�
            ExtKey deriveMasterKey = TronKeyDerivation.DeriveTronMasterKey(seed);
            for (uint i = 0; i < 3; i++)
            {
                // ����˽Կ
                string privateKey = TronKeyDerivation.DeriveTronPrivateKey(deriveMasterKey, i);
                // ���ɵ�ַ
                string address = TronKeyDerivation.GetTronAddressFromPrivateKey(privateKey);
                Console.WriteLine($"��ַ {i}: {address}");
                Console.WriteLine($"˽Կ {i}: {privateKey}\n");
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
