# Chisel-Strike

A .NET XOR encrypted cobalt strike aggressor implementation for chisel to utilize faster proxy and advanced socks5 capabilities.

## Why write this?

In my experience I found socks4/socks4a proxies quite slow in comparison to its socks5 counterparts and a lack of implementation of socks5 in most C2 frameworks. There is a C# wrapper around the go version of [chisel](https://github.com/jpillora/chisel) called [SharpChisel](https://github.com/shantanu561993/SharpChisel). This wrapper has a few [issues](https://github.com/shantanu561993/SharpChisel/issues/1) and isn't maintained to the latest version of [chisel](https://github.com/jpillora/chisel). It didn’t allow using shellcode with [donut](https://github.com/TheWover/donut), reflection methods or `execute-assembly`. I found a fix for this using the [SharpChisel-NG](https://github.com/latortuga71/SharpChisel-NG) project.

Since the [SharpChisel](https://github.com/shantanu561993/SharpChisel) assembly is around `16.7 MB`, `execute-assembly`(has a hidden size limitation of `1 MB`) and similar in memory methods wouldn’t work. To maintain most of the execution in memory I incorporated the [NetLoader](https://gist.github.com/Arno0x/2b223114a726be3c5e7a9cacd25053a2) project by [Flangvik](https://github.com/Flangvik) which is executed via `execute-assembly` to reflectively host and load a XOR encrypted version of `SharpChisel` with base64 arguments in memory.

As an alternative, it is also possible to implement similar C# proxies like [SharpSocks](https://github.com/nettitude/SharpSocks) by replacing the appropriate [chisel](https://github.com/jpillora/chisel) binaries in the project.

## Setup

_Note: If using a Windows teamserver skip steps 2 and 3._

1. Clone/download the repository: `git clone https://github.com/m3rcer/Chisel-Strike.git`

2. Make all binaries executable: 

  - `cd Chisel-Strike` 

  - `chmod +x -R chisel-modules` 
  
  - `chmod +x -R tools`

3. Install `Mingw-w64` and `mono`: 

  - `sudo apt-get install mingw-w64`
  
  - `sudo apt install mono-complete`

4. Import `ChiselStrike.cna` in cobalt strike using the `Script Manager`

_Recompile binaries from the `src` folder if needed._

## Usage

[chisel](https://github.com/jpillora/chisel) can be executed on both the teamserver (windows/linux) and the beacon. With either acting as the server/client. A normal execution flow would be to setup a [chisel](https://github.com/jpillora/chisel) server on the teamserver and create a client on the beacon connecting back to the teamserver.

### Commands

1. `chisel <client/server> <command>`: Run Chisel on a beacon

2. `chisel-tms <client/server> <command>`: Run Chisel on your teamserver

3. `chisel-enc`: XOR Encrypt `SharpChisel.exe` with a password of choice

4. `chisel-jobs`: List active chisel jobs on the teamserver and beacon

5. `chisel-kill`: Kill active chisel jobs on a beacon

6. `chisel-tms-kill`: Kill active chisel jobs on teamserver

### Example

![Alt Text](chiselstrike.gif)

## OPSEC

[NetLoader](https://gist.github.com/Arno0x/2b223114a726be3c5e7a9cacd25053a2) can easily be obfuscated and used to bypass defender using projects like [NimCrypt2](https://github.com/icyguider/Nimcrypt2) and the like.

Yet `SharpChisel.exe` drops a `dll` on disk due to the use of `Costura/Fody` packages at a location similar to: `C:\Users\m3rcer\AppData\Local\Temp\Costura\CB9433C24E75EC539BF34CD1AA12B236\64\main.dll` which is detected by defender. It is advised to obfuscate [chisel](https://github.com/jpillora/chisel) dll's using projects like [gobfuscate](https://github.com/unixpickle/gobfuscate) in the [SharpChisel-NG](https://github.com/latortuga71/SharpChisel-NG) project and re-build new [SharpChisel-NG](https://github.com/latortuga71/SharpChisel-NG) binaries as shown [here](https://medium.com/@shantanukhande/red-team-how-to-embed-golang-tools-in-c-e269bf33876a#:~:text=Step%202%3A%20Make%20a%20C%23,rename%20the%20XML%20to%20FodyWeavers.).

## TODO

- Figure a way to avoid `SharpChisel` dropping `main.dll` on disk / Create a new C# wrapper for [chisel](https://github.com/jpillora/chisel).

- Create a method to parse command output for the `chisel-tms` command.

## Credits

- [Flangvik](https://github.com/Flangvik) and [Arno0x](https://gist.github.com/Arno0x) for the [NetLoader](https://gist.github.com/Arno0x/2b223114a726be3c5e7a9cacd25053a2) project.

- [shantanu561993](https://github.com/shantanu561993) for the C# wrapper implementation of chisel: [SharpChisel](https://github.com/shantanu561993/SharpChisel)

- [latortuga71](https://github.com/latortuga71) for the `load-assembly` fix: [SharpChisel-NG](https://github.com/latortuga71/SharpChisel-NG)
