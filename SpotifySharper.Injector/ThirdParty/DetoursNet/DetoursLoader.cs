﻿using System;
using System.Runtime.InteropServices;

namespace SpotifySharper.Injector.ThirdParty.DetoursNet
{
    public static class DetoursLoader
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpModuleName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("DetoursDll.dll")]
        public static extern long DetourAttach(ref IntPtr a, IntPtr b);

        [DllImport("DetoursDll.dll")]
        public static extern long DetourUpdateThread(IntPtr a);

        [DllImport("DetoursDll.dll")]
        public static extern long DetourTransactionBegin();

        [DllImport("DetoursDll.dll")]
        public static extern long DetourTransactionCommit();

        [DllImport("DetoursDll.dll")]
        public static extern bool DetoursPatchIAT(IntPtr hModule, IntPtr import, IntPtr real);

        [DllImport("DetoursNetCLR.dll", CharSet = CharSet.Ansi)]
        public static extern void DetoursCLRSetGetProcAddressCache(IntPtr hModule, string procName, IntPtr real);

        public static void DetourUpdateThread()
        {
            DetourUpdateThread(GetCurrentThread());
        }

        ///// <summary>
        ///// Find all static method with custom attribute type
        ///// </summary>
        ///// <param name="assembly">Assembly object</param>
        ///// <param name="attributeType">type of custom attribute</param>
        ///// <returns>All method infos</returns>
        //public static MethodInfo[] FindAttribute(this Assembly assembly, Type attributeType)
        //{
        //    return assembly.GetTypes()
        //        .SelectMany(t => t.GetMethods())
        //        .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0)
        //        .ToArray();
        //}

        ///// <summary>
        ///// Main entry point of loader
        ///// </summary>
        //public static int Start(string arguments)
        //{
        //    string assemblyName = System.Environment.GetEnvironmentVariable("DETOURSNET_ASSEMBLY_PLUGIN");

        //    Assembly assembly = Assembly.LoadFrom(assemblyName);

        //    foreach (var method in assembly.FindAttribute(typeof(OnInitAttribute)))
        //    {
        //        method.Invoke(null, null);
        //    }

        //    foreach (var method in assembly.FindAttribute(typeof(DetoursAttribute)))
        //    {
        //        var attribute = (DetoursAttribute)method.GetCustomAttributes(typeof(DetoursAttribute), false)[0];

        //        DelegateStore.Mine[method] = Delegate.CreateDelegate(attribute.DelegateType, method);

        //        IntPtr module = LoadLibrary(attribute.Module);
        //        if (module == IntPtr.Zero)
        //        {
        //            continue;
        //        }

        //        IntPtr real = GetProcAddress(module, method.Name);
        //        if (real == IntPtr.Zero)
        //        {
        //            continue;
        //        }

        //        // record pointer
        //        IntPtr import = real;

        //        DetourTransactionBegin();
        //        DetourUpdateThread(GetCurrentThread());
        //        DetourAttach(ref real, Marshal.GetFunctionPointerForDelegate(DelegateStore.Mine[method]));
        //        DetourTransactionCommit();

        //        // Add function to pinvoke cache
        //        DetoursCLRSetGetProcAddressCache(module, method.Name, real);

        //        // and so on patch IAT of clr module
        //        DetoursPatchIAT(GetModuleHandle("clr.dll"), import, real);

        //        DelegateStore.Real[method] = Marshal.GetDelegateForFunctionPointer(real, attribute.DelegateType);
        //    }

        //    return 0;
        //}
    }
}