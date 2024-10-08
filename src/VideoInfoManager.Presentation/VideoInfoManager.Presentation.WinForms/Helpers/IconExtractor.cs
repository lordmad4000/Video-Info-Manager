﻿using System.Runtime.InteropServices;

namespace VideoInfoManager.Presentation.WinForms.Helpers;

public class IconExtractor
{
    public static Icon Extract(string file, int number, bool largeIcon)
    {
        IntPtr large;
        IntPtr small;
        ExtractIconEx(file, number, out large, out small, 1);
        try
        {
            return Icon.FromHandle(largeIcon ? large : small);
        }
        catch
        {
            ExtractIconEx(file, 1, out large, out small, 1);
            return Icon.FromHandle(largeIcon ? large : small);
        }

    }
    [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

}
