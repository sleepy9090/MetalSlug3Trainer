using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetalSlug3Trainer
{
    public partial class Form1 : Form
    {

        #region Imports

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

        #endregion

        #region Enums

        [Flags]
        private enum ProcessAccessRights
        {
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_TERMINATE = 0x0001,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            SYNCHRONIZE = 0x00100000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            STANDARD_RIGHTS_REQUIRED = 0x000f0000,
            PROCESS_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF)
        }

        #endregion


        #region Constants

        private const string MODULE_NAME = "mslug3.exe";
        private const string PROCESS_NAME = "mslug3";

        private const int BASE_ADDRESS_1 = 0x000FEC38; // Used for Player 1 and everything else
        private const int BASE_ADDRESS_2 = 0x000FEC3C; // Used for Player 2 and everything else (alt)
        private const int BASE_ADDRESS_3 = 0x00000000; // Unused // TODO

        // 10A4A6A9<-- actual
        // 10A3C6A8<-- offset points to
        // E001     <-- difference
        // E001 + D78 = ED79 <--correct offset
        private const int OFFSET_LEVEL_TIMER = 0xED79;
        private const int OFFSET_CONTINUE_TIMER = 0x01F9; 

        // Player 1
        private const int OFFSET_P1_CREDITS_COUNT = 0x1003C;
        private const int OFFSET_P1_LIVES_COUNT = 0x02BA; // Screen updates after losing a life
        private const int OFFSET_P1_LIVES_COUNT_COMPLETE = 0x0000; // Set this offset value to 0 after setting lives.  // TODO
        private const int OFFSET_P1_INVINCIBILITY_TIMER = 0x0580;
        // 0 = Normal
        // 1 = Machine Gun
        // 2 = Fat 
        // 3 = Mummy
        // 4 = Scuba Gear Normal
        // 5 = Scuba Gear Machine Gun
        // 6 = Snowman
        // 7 = Zombie
        // 8 = Flying(final stage)
        // 9 = Flying(final stage)
        // 10 = Spaceship(final stage)
        // 11 = 
        // 12 + Game Crash
        // Other statuses(dropping guns, helmets, water splash, etc)
        // "mslug3.exe"+000FEC38 58D
        // "mslug3.exe"+000FEC38 58F
        private const int OFFSET_P1_STATUS = 0x058E;
        //        example score: 34127856
        //0EF36C80 = 12h
        //0EF36C81 = 34h
        //0EF36C82 = 56h
        //0EF36C83 = 78h

        //Base Address Offset
        //"mslug3.exe"+000FEC38 ED18
        //"mslug3.exe"+000FEC38 ED19
        //"mslug3.exe"+000FEC38 ED1A
        //"mslug3.exe"+000FEC38 ED1B
        private const int OFFSET_P1_SCORE = 0xED18;
        private const int OFFSET_P1_POWS_RESCUED = 0xED04;
        private const int OFFSET_P1_VEHICLE_HEALTH_COUNT = 0xEC30; // Max 48
        private const int OFFSET_P1_VEHICLE_AMMO_CANON_COUNT = 0xEC38;
        //    0 = Hand Gun
        //    1 = Cannon
        //    2 = Shotgun
        //    3 = Rocket Launcher
        //    4 = Flame Shot
        //    5 = Heavy Machine Gun
        //    6 = Laser Gun
        //    7 = Super Shotgun
        //    8 = Super Rocket Launcher
        //    9 = Super Flame Shot
        //    10 = Super Heavy Machine Gun
        //    11 = Super Laser Gun
        //    12 = Enemy Chaser
        //    13 = Iron Lizard
        //    14 = Dropshot
        //    15 = Super Cannon
        private const int OFFSET_P1_WEAPON_TYPE = 0xEAF2;
        private const int OFFSET_P1_BOMB_COUNT = 0xEAF4;
        private const int OFFSET_P1_BOMB_TYPE = 0x0000; // TODO
        private const int OFFSET_P1_AMMO_COUNT = 0xEAFA; // 2 bytes, 0-65535(65535 = infinite)
        private const int OFFSET_P1_CURRENT_CHARACTER = 0x0000; // TODO

        // Player 2
        private const int OFFSET_P2_CREDITS_COUNT = 0x1003D;
        private const int OFFSET_P2_LIVES_COUNT = 0x036A; // Screen updates after losing a life
        private const int OFFSET_P2_LIVES_COUNT_COMPLETE = 0x0000; // Set this offset value to 0 after setting lives. // TODO
        private const int OFFSET_P2_INVINCIBILITY_TIMER = 0x0630;
        private const int OFFSET_P2_STATUS = 0x063E;
        //        example score: 34127856
        //0EF36C80 = 12h
        //0EF36C81 = 34h
        //0EF36C82 = 56h
        //0EF36C83 = 78h

        //Base Address Offset
        //"mslug3.exe"+000FEC38 ED20
        //"mslug3.exe"+000FEC38 ED21
        //"mslug3.exe"+000FEC38 ED22
        //"mslug3.exe"+000FEC38 ED23
        private const int OFFSET_P2_SCORE = 0xED20;
        private const int OFFSET_P2_POWS_RESCUED = 0xED0C;
        private const int OFFSET_P2_VEHICLE_HEALTH_COUNT = 0xEC5C; // Max 48
        private const int OFFSET_P2_VEHICLE_AMMO_CANON_COUNT = 0xEC64;
        private const int OFFSET_P2_WEAPON_TYPE = 0x0000; // TODO
        private const int OFFSET_P2_BOMB_COUNT = 0xEB02;
        private const int OFFSET_P2_BOMB_TYPE = 0x0000; // TODO
        private const int OFFSET_P2_AMMO_COUNT = 0xEB08;
        private const int OFFSET_P2_CURRENT_CHARACTER = 0x0000; // TODO

        #endregion

        #region Fields

        private Process game;

        private IntPtr hProc = IntPtr.Zero;
        private IntPtr levelTimerAddressGlobal = IntPtr.Zero;
        private IntPtr continueTimerAddressGlobal = IntPtr.Zero;
        private IntPtr livesCountP1AddressGlobal = IntPtr.Zero;
        private IntPtr livesCountP1CompleteAddressGlobal = IntPtr.Zero;
        private IntPtr livesCountP2AddressGlobal = IntPtr.Zero;
        private IntPtr livesCountP2CompleteAddressGlobal = IntPtr.Zero;
        private IntPtr invincibilityTimerP1AddressGlobal = IntPtr.Zero;
        private IntPtr invincibilityTimerP2AddressGlobal = IntPtr.Zero;
        private IntPtr powsRescuedP1AddressGlobal = IntPtr.Zero;
        private IntPtr powsRescuedP2AddressGlobal = IntPtr.Zero;
        private IntPtr weaponTypeP1AddressGlobal = IntPtr.Zero;
        private IntPtr weaponTypeP2AddressGlobal = IntPtr.Zero;
        private IntPtr bombTypeP1AddressGlobal = IntPtr.Zero;
        private IntPtr bombTypeP2AddressGlobal = IntPtr.Zero;
        private IntPtr bombCountP1AddressGlobal = IntPtr.Zero;
        private IntPtr bombCountP2AddressGlobal = IntPtr.Zero;
        private IntPtr ammoCountP1AddressGlobal = IntPtr.Zero;
        private IntPtr ammoCountP2AddressGlobal = IntPtr.Zero;
        private IntPtr vehicleAmmoCanonCountP1AddressGlobal = IntPtr.Zero;
        private IntPtr vehicleAmmoCanonCountP2AddressGlobal = IntPtr.Zero;
        private IntPtr vehicleHealthCountP1AddressGlobal = IntPtr.Zero;
        private IntPtr vehicleHealthCountP2AddressGlobal = IntPtr.Zero;
        private IntPtr creditsCountP1AddressGlobal = IntPtr.Zero;
        private IntPtr creditsCountP2AddressGlobal = IntPtr.Zero;
        private IntPtr currentCharacterP1AddressGlobal = IntPtr.Zero;
        private IntPtr currentCharacterP2AddressGlobal = IntPtr.Zero;
        private IntPtr scoreP1AddressGlobal = IntPtr.Zero;
        private IntPtr scoreP2AddressGlobal = IntPtr.Zero;

        private Timer timer = new Timer();

        private string previousLogEntry = "";

        #endregion

        public Form1()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.CenterScreen;


        }
    }
}
