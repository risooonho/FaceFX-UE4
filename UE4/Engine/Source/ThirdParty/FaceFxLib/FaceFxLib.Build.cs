﻿using UnrealBuildTool;
using System.IO;

public class FaceFxLib : ModuleRules
{
    //used to show warning only once.
    static bool debugLibsWarningDisplayed = false;

    public FaceFxLib(TargetInfo Target)
    {
        Type = ModuleType.External;

        string FaceFxLib;
        string FaceFxDir;
        string FaceFxDirLib;
        if (GetLibs(Target, out FaceFxDir, out FaceFxDirLib, out FaceFxLib))
        {
            Definitions.Add("Y_WITH_FACEFX=1");

            PublicIncludePaths.Add(Path.Combine(new[] { FaceFxDir, "common", "src" }));
            PublicLibraryPaths.Add(FaceFxDirLib);
            PublicAdditionalLibraries.Add(FaceFxLib);
        }
        else
        {
            Definitions.Add("Y_WITH_FACEFX=0");
        }
    }


    /// <summary>
    /// Gets the libs for Silverlink
    /// </summary>
    /// <param name="Target">The targetinfo to get the libs for</param>
    /// <param name="FaceFxDir">The result facefx directory</param>
    /// <param name="FaceFxDirLib">The result facefx directory for libraries</param>
    /// <param name="FaceFxLib">The actual lib filename</param>
    /// <returns>True if all libs were found, else false</returns>
    private static bool GetLibs(TargetInfo Target, out string FaceFxDir, out string FaceFxDirLib, out string FaceFxLib)
    {
        FaceFxDir = UEBuildConfiguration.UEThirdPartySourceDirectory + Path.Combine("FaceFxLib", "FaceFx-0.9.10");
        FaceFxDirLib = string.Empty;
        FaceFxLib = string.Empty;

        if (!Directory.Exists(FaceFxDir))
        {
            System.Console.WriteLine(System.String.Format("FaceFX: Cannot find FaceFX Folder '{0}'", FaceFxDir));
            return false;
        }

        //default static lib file
        FaceFxLib = "libfacefx.lib";

        string CompilerFolder = WindowsPlatform.Compiler == WindowsCompiler.VisualStudio2013 ? "vs12" : "vs11";

        string PlatformFolder = string.Empty;

        switch (Target.Platform)
        {
            case UnrealTargetPlatform.Win32:
                PlatformFolder = Path.Combine(new[] { "windows", CompilerFolder, "Win32" });
                break;
            case UnrealTargetPlatform.Win64:
                PlatformFolder = Path.Combine(new[] { "windows", CompilerFolder, "x64" });
                break;
            case UnrealTargetPlatform.XboxOne:
                PlatformFolder = Path.Combine(new[] { "xboxone", "vs11" });
                break;
            case UnrealTargetPlatform.PS4:
                FaceFxLib = "facefx";
                PlatformFolder = Path.Combine(new[] { "ps4", CompilerFolder });
                break;
            default:
                System.Console.WriteLine(System.String.Format("FaceFX disabled. Unknown target platform: {0}", Target.Platform));
                return false;
        }

        string ConfigFolder = "Release";

        switch (Target.Configuration)
        {
            case UnrealTargetConfiguration.Debug:
                // change bDebugBuildsActuallyUseDebugCRT to true in BuildConfiguration.cs to actually link debug binaries
                if (BuildConfiguration.bDebugBuildsActuallyUseDebugCRT)
                {
                    ConfigFolder = "Debug";
                    if (debugLibsWarningDisplayed == false)
                    {
                        System.Console.WriteLine("Using debug libs for FaceFX");
                        debugLibsWarningDisplayed = true;
                    }
                }
                break;
        }

        FaceFxDirLib = System.IO.Path.Combine(new[] { FaceFxDir, "bin", PlatformFolder, ConfigFolder });

        if (!Directory.Exists(FaceFxDirLib))
        {
            System.Console.WriteLine(System.String.Format("FaceFX: Cannot find FaceFX lib Folder '{0}'", FaceFxDirLib));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the indicator if FaceFX can be used on the given target
    /// </summary>
    /// <param name="Target">The target info</param>
    /// <returns>True if can be used, else false</returns>
    public static bool IsSupported(TargetInfo Target)
    {
        string FaceFxDir;
        string FaceFxDirLib;
        string FaceFxLib;
        return GetLibs(Target, out FaceFxDir, out FaceFxDirLib, out FaceFxLib);
    }
}