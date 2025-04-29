using NBitcoin;
using System;

namespace Haich.Web.Utils
{
    public class MnemonicGenerator
    {
        public static Mnemonic GenerateMnemonic(int wordCount = 12)
        {
            // 支持 12（128位熵）或 24（256位熵）个单词
            var mn = new Mnemonic(Wordlist.English, (WordCount)wordCount);
            return mn;
        }

        public static byte[] GenerateSeedFromMnemonic(Mnemonic mnemonic, string passphrase = "")
        {
            // 通过助记词和密码生成种子（符合 BIP39 标准）
            return mnemonic.DeriveSeed(passphrase);
        }
    }
}
