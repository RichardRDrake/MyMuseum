using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AutoBuildNumber : IPreprocessBuildWithReport
{

    private static int PreviousMinor;

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        //A new build has happened so lets increase our version number

        string CurrentVersion = PlayerSettings.bundleVersion;
        string[] versionElements = CurrentVersion.Split('.');
        int[] intElements = new int[3];
        intElements[0] = int.Parse(versionElements[0]);
        intElements[1] = int.Parse(versionElements[1]);
        intElements[2] = int.Parse(versionElements[2]);
        intElements[2]++;
        //if (intElements[1] != PreviousMinor)
        //{
        //    intElements[2] = 0;
        //    PreviousMinor = intElements[1];

        //}
        PlayerSettings.bundleVersion = string.Format("{0}.{1}.{2}", intElements[0], intElements[1], intElements[2]);
    }
}

