﻿using System;
using System.Runtime.InteropServices;

namespace SpotifySharper.Injector
{
    /*
     *
     * SpotifyFunctions.h: https://github.com/meik1710/Spotify1710/blob/master/HookingBase/SpotifyFunctions.h
       typedef void(__cdecl * CmdAddTextGAIA_t)(int a1, int a2, const char* msg, int dummy, int dummy1, int dummy2, int dummy3);
       extern CmdAddTextGAIA_t CmdAddTextGAIA;
       CmdAddTextGAIA_t CmdAddTextGAIA = (CmdAddTextGAIA_t)0x116B010;

       typedef void(__thiscall * CreateTrackPlayer_t)(void *a1, int a2 , int a3, double speed, int a5, int a6, int flag, int a8, int a9);
       extern CreateTrackPlayer_t CreateTrackPlayer;
       CreateTrackPlayer_t CreateTrackPlayer = (CreateTrackPlayer_t)0xD88A50;

       typedef void(__thiscall * OpenTrack_t)(void *_this, int a2, int a3, __int64 position, int a5, int a6);
       extern OpenTrack_t OpenTrack;
       OpenTrack_t OpenTrack = (OpenTrack_t)0xD8B4F0;

       typedef void(__thiscall * CloseTrack_t)(void *_this, int a2, int a3, int a4);
       extern CloseTrack_t CloseTrack;
       CloseTrack_t CloseTrack = (CloseTrack_t)0xD88390;
     * Equivalent: https://stackoverflow.com/questions/51991689/c-sharp-call-a-method-by-memory-address
     * =================================================================
       public delegate void _do(int i); // the delegate type

       var ptr = new IntPtr(0xAAAABEEF);
       var doActor = Marshal.GetDelegateForFunctionPointer<_do>(ptr);
       doActor(1);
     * ==================================================================
     *
     */

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CmdAddTextGAIA_t(int a1, int a2, string msg, int dummy, int dummy1, int dummy2, int dummy3);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void CreateTrackPlayer_t(IntPtr a1, int a2, int a3, double speed, int a5, int a6, int flag, int a8, int a9);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void OpenTrack_t(IntPtr _this, int a2, int a3, long position, int a5, int a6);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void CloseTrack_t(IntPtr _this, int a2, int a3, int a4);

    public static class SpotifyFunctions
    {
        public static CmdAddTextGAIA_t CmdAddTextGAIA
            => GetFunc<CmdAddTextGAIA_t>(0x116B010);

        public static CreateTrackPlayer_t CreateTrackPlayer
            => GetFunc<CreateTrackPlayer_t>(0xD88A50);

        public static OpenTrack_t OpenTrack
            => GetFunc<OpenTrack_t>(0xD8B4F0);

        public static CloseTrack_t CloseTrack
            => GetFunc<CloseTrack_t>(0xD88390);

        public static T GetFunc<T>(long address)
            where T : Delegate // C# 7.3 :D
        {
            var ptr = new IntPtr(address);
            return Marshal.GetDelegateForFunctionPointer<T>(ptr);
        }
    }
}