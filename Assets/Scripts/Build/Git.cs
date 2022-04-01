/* MIT License
Copyright (c) 2016 RedBlueGames
Code written by Doug Cox
*/

// https://blog.redbluegames.com/version-numbering-for-games-in-unity-and-git-1d05fca83022

using System;
using UnityEngine;

/// <summary>
/// GitException includes the error output from a Git.Run() command as well as the
/// ExitCode it returned.
/// </summary>
public class GitException : InvalidOperationException
{
    public GitException(int exitCode, string errors) : base(errors) =>
        this.ExitCode = exitCode;

    /// <summary>
    /// The exit code returned when running the Git command.
    /// </summary>
    public readonly int ExitCode;
}

public static class Git
{
    /* Properties ============================================================================================================= */

    /// <summary>
    /// Retrieves the build version from git based on the most recent matching tag and
    /// commit history. This returns the version as: {major.minor.build} where 'build'
    /// represents the nth commit after the tagged commit.
    /// Note: The initial 'v' and the commit hash code are removed.
    /// </summary>
    public static string BuildVersion
    {
        get
        {
            string version = Run(@"describe --tags --match '[0–9]*'");
            // Remove ending git commit hash.
            version = version.Replace('-', '.');
            version = version.Substring(0, version.LastIndexOf('.') - 1);
            return version;
        }
    }

    /// <summary>
    /// The currently active branch.
    /// </summary>
    public static string Branch => Run(@"rev-parse --abbrev-ref HEAD");

    /// <summary>
    /// Returns a listing of all uncommitted or untracked (added) files.
    /// </summary>
    public static string Status => Run(@"status --porcelain");


    /* Methods ================================================================================================================ */

    /// <summary>
    /// Runs git.exe with the specified arguments and returns the output.
    /// </summary>
    public static string Run(string arguments)
    {
        using (var process = new System.Diagnostics.Process())
        {
            var exitCode = process.Run(@"git", arguments, Application.dataPath,
                out var output, out var errors);
            if (exitCode == 0)
            {
                return output;
            }
            else
            {
                throw new GitException(exitCode, errors);
            }
        }
    }
}
