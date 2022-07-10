using System;
using System.IO;

namespace SharpXOR
{
    class Program
    {
        public static byte[] XOR(byte [] payload, string XORKey)
        {
           byte[] xorStuff = new byte[payload.Length] ;
           char[] bXORKey = XORKey.ToCharArray();
           for(int i = 0; i< payload.Length;i++)
            {
                xorStuff[i] = (byte)(payload[i] ^ bXORKey[i % bXORKey.Length]);
            }
            return xorStuff;
        }
        static void Main(string[] args)
        {
            if (args[0] == "-h" || args[0] == "--help" || args == null || args.Length < 1)
            {
                Console.WriteLine("Usage: SharpXOR.exe <INPUT FILE> <XOR KEY PASS> <OUTPUT FILE>");
                Environment.Exit(0);
            }
            String inputPath = args[0];
            String key = args[1];
            String outputPath = args[2];
            byte[] payload = File.ReadAllBytes(inputPath);
            byte[] stuff = XOR(payload, key);
            File.WriteAllBytes(outputPath,stuff);
            Console.WriteLine("successfully XOR'd {0}! \n Data written to {1}",inputPath,outputPath);

        }
    }
}
