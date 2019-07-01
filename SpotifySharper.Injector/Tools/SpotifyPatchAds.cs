using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static SpotifySharper.Injector.Tools.SpotifyFunctions;

namespace SpotifySharper.Injector.Tools
{
    using ThirdParty.DetoursNet;
    using Utils;

    public class SpotifyPatchAds
    {
        /*

                long CmdAddText_back = 0x116B01A;
                //        void __declspec(naked) CmdAddTextGAIA_stub(int a1, int a2, const char* msg, const char* dummy, int dummy1, int dummy2, int dummy3)
                //{
                //    __asm
                //	{
                //		push ebp
                //        mov ebp, esp
                //        push esi
                //        mov esi, [ebp + 8]
                //        lea eax, [ebp + 20]
                //        jmp CmdAddText_back
                //    }
                //}

        void CmdAddTextGAIA_hk(int a1, int a2, const char* fmt, const char* dummy, int dummy1, int dummy2, int dummy3)
        {
            //check if current track is an ad or not
            if (fmt[4] == char (116) && fmt[5] == char (114) && fmt[6] == char (97) && fmt[7] == char (99) && fmt[8] == char (107) && fmt[9] == char (95) && fmt[10] == char (117) && fmt[11] == char (114) && fmt[12] == char (105))
            {
                std::cout << "Track: " << dummy << std::endl;

                if (dummy[0] == char (115) && dummy[1] == char (112) && dummy[2] == char (111) && dummy[3] == char (116) && dummy[4] == char (105) && dummy[5] == char (102) && dummy[6] == char (121) && dummy[7] == char (58) && dummy[8] == char (97) && dummy[9] == char (100))
                {
                    std::cout << "Skipping AD: " << dummy << std::endl;
                    ___position = 100000; // just in case an ad is 100 sec long lol
                }
                else
                    ___position = 0;
            }

            CmdAddTextGAIA_stub(a1, a2, fmt, dummy, dummy1, dummy2, dummy3);
        }

        void __fastcall CloseTrack_hk(IntPtr _this, DWORD edx, int a2, int a3, int a4)
        {
            std::cout << "Closing track" << std::endl;
            _thiss = _this;
            _a2 = a2;
            _a3 = a3;
            _a4 = a4;

            CloseTrack(_thiss, _a2, _a3, _a4);
        }

        void __fastcall CreateTrackPlayer_hk(IntPtr _this, DWORD edx, int a2, int a3, double speed, int a5, int a6, int flag, int a8, int a9)
        {
            std::cout << "Creating track" << std::endl;

            __thiss = _this;
            __a2 = a2;
            __a3 = a3;
            __a4 = speed;
            __a5 = a5;
            __a6 = a6;
            __a7 = flag;
            __a8 = a8;
            __a9 = a9;

            CreateTrackPlayer(__thiss, __a2, __a3, __a4, __a5, __a6, __a7, __a8, __a9);
        }

        void __fastcall OpenTrack_hk(IntPtr _this, DWORD edx, int a2, int a3, long position, int a5, int a6)
        {
            std::cout << "Opening track\n" << std::endl;
            ___thiss = _this;
            ___a2 = a2;
            ___a3 = a3;
            position = ___position;
            ___a5 = a5;
            ___a6 = a6;

            OpenTrack(___thiss, ___a2, ___a3, position, ___a5, ___a6);
        }*/

        //CloseTrack
        private int CloseTrack_a2Field,
                    CloseTrack_a3Field,
                    CloseTrack_a4Field;

        private IntPtr CloseTrack_Ptr;

        //CreateTrack
        private int CreateTrack_a2Field,
                    CreateTrack_a3Field,
                    CreateTrack_a5Field,
                    CreateTrack_a6Field,
                    CreateTrack_a7Field,
                    CreateTrack_a8Field,
                    CreateTrack_a9Field;

        private double CreateTrack_a4Field;
        private IntPtr CreateTrack_Ptr;

        //OpenTrack
        private int OpenTrack_a2Field,
                    OpenTrack_a3Field,
                    OpenTrack_a5Field,
                    OpenTrack_a6Field;

        private static long trackPosition = 0;
        private IntPtr OpenTrack_Ptr;

        private long CmdAddText_back = 0x116B01A;

        // [Detours("ole32.dll", typeof(CmdAddTextGAIA_t))]
        private static void CmdAddTextGAIA_hk(
            int a1,
            int a2,
            [MarshalAs(UnmanagedType.LPStr)] string fmt,
            [MarshalAs(UnmanagedType.LPStr)] string dummy,
            int dummy1,
            int dummy2,
            int dummy3)

        {
            //check if current track is an ad or not
            // fmt[4] == char(116) && fmt[5] == char(114) && fmt[6] == char(97) && fmt[7] == char(99) && fmt[8] == char(107) && fmt[9] == char(95) && fmt[10] == char(117) && fmt[11] == char(114) && fmt[12] == char(105)

            if (fmt.Substring(4, 8) == "track_uri")
            {
                Main.SendMessage($"Track: {fmt}");
                MessageBox.Show($"Testing FMT: {fmt}");

                if (dummy.Substring(0, 9) == "spotify:ad")
                {
                    // dummy[0] == char(115) && dummy[1] == char(112) && dummy[2] == char(111) && dummy[3] == char(116) && dummy[4] == char(105) && dummy[5] == char(102) && dummy[6] == char(121) && dummy[7] == char(58) && dummy[8] == char(97) && dummy[9] == char(100)

                    Main.SendMessage($"Skipping AD: {dummy}");
                    trackPosition = 100000; // just in case an ad is 100 sec long lol
                }
                else
                    trackPosition = 0;
            }

            CmdAddTextGAIA_Stub(a1, a2, fmt, dummy, dummy1, dummy2, dummy3);
        }

        //detect ads
        private static void SpotifyTrackAds()
        {
            Main.SendMessage("Tracking ads!");

            using (var handleProvider = new GCHandleProvider(new CmdAddTextGAIA_t(CmdAddTextGAIA_hk)))
            {
                DetoursLoader.DetourTransactionBegin();
                DetoursLoader.DetourUpdateThread();
                DetoursLoader.DetourAttach(ref CmdAddTextGAIA, handleProvider.Pointer);
                DetoursLoader.DetourTransactionCommit();
            }
        }

        //open new song
        private static void SpotifyOpenTrack()
        {
            //DetoursLoader.DetourTransactionBegin();
            //DetoursLoader.DetourUpdateThread();
            //DetoursLoader.DetourAttach(ref OpenTrack, OpenTrack_hk);
            //DetoursLoader.DetourTransactionCommit();
        }

        //close song
        private static void SpotifyCloseTrack()
        {
            //DetoursLoader.DetourTransactionBegin();
            //DetoursLoader.DetourUpdateThread();
            //DetoursLoader.DetourAttach(ref CloseTrack, CloseTrack_hk);
            //DetoursLoader.DetourTransactionCommit();
        }

        //create new song
        private static void SpotifyCreateTrack()
        {
            //DetoursLoader.DetourTransactionBegin();
            //DetoursLoader.DetourUpdateThread();
            //DetoursLoader.DetourAttach(ref CreateTrackPlayer, CreateTrackPlayer_hk);
            //DetoursLoader.DetourTransactionCommit();
        }

        public static void PatchAds()
        {
            SpotifyTrackAds();
            SpotifyOpenTrack();
            SpotifyCloseTrack();
            SpotifyCreateTrack();
        }
    }
}