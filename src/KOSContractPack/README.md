### To Compile and install the DLL:


In your C# build environment, load solution file KOSContractPack.sln.

In your C# build environment, include References to the following:

- (ksp home)/KSP_Data/Managed/Assembly-CSharp.dll
- (ksp home)/KSP_Data/Managed/UnityEngine.dll
- (ksp home)/GameData/ContractConfigurator/ContractConfigurator.dll

Build, targeting a .NET 3.5 class library "any platform" DLL.


Find the DLL file the build process created:

If in DEBUG mode, it's here:

- KOSContractPack/bin/Debug/KOSContractPack.dll

If in RELEASE mode, it's here:

- KOSContractPack/bin/Release/KOSContractPack.dll

In either case, copy that .dll file to:

GameData/ContractConfigurator/kOS-Contract-Pack/Plugins/


