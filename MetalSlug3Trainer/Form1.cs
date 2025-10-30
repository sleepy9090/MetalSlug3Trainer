using System;
using System.Collections;
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

        private const int BASE_ADDRESS_1 = 0x000FEC38;
        private const int BASE_ADDRESS_2 = 0x000FEC3C;

        private const int OFFSET_LEVEL_TIMER = 0xED79;
        private const int OFFSET_CONTINUE_TIMER = 0x01F9;
        private const int OFFSET_MISSION_COMPLETE = 0xED7C;
        private const int OFFSET_LEVEL_SELECT_1 = 0xED7A;
        private const int OFFSET_LEVEL_SELECT_2 = 0xED7B;
        private const int OFFSET_DEBUG_1 = 0xF000;
        private const int OFFSET_DEBUG_2 = 0xF001;

        // Player 1
        private const int OFFSET_P1_CREDITS_COUNT = 0x1003C;
        private const int OFFSET_P1_LIVES_COUNT = 0x02BA;
        private const int OFFSET_P1_INVINCIBILITY_TIMER = 0x0580;
        private const int OFFSET_P1_STATUS = 0x058E;
        private const int OFFSET_P1_SCORE = 0xED18;
        private const int OFFSET_P1_POWS_RESCUED = 0xED04;
        private const int OFFSET_P1_VEHICLE_HEALTH_COUNT = 0xEC30;
        private const int OFFSET_P1_VEHICLE_AMMO_CANON_COUNT = 0xEC38;
        private const int OFFSET_P1_WEAPON_TYPE = 0xEAF2;
        private const int OFFSET_P1_BOMB_COUNT = 0xEAF4;
        private const int OFFSET_P1_BOMB_TYPE = 0xEAF5;
        private const int OFFSET_P1_AMMO_COUNT = 0xEAFA;

        // Player 2
        private const int OFFSET_P2_CREDITS_COUNT = 0x1003D;
        private const int OFFSET_P2_LIVES_COUNT = 0x036A;
        private const int OFFSET_P2_INVINCIBILITY_TIMER = 0x0630;
        private const int OFFSET_P2_STATUS = 0x063E;
        private const int OFFSET_P2_SCORE = 0xED20;
        private const int OFFSET_P2_POWS_RESCUED = 0xED0C;
        private const int OFFSET_P2_VEHICLE_HEALTH_COUNT = 0xEC5C;
        private const int OFFSET_P2_VEHICLE_AMMO_CANON_COUNT = 0xEC64;
        private const int OFFSET_P2_WEAPON_TYPE = 0xEB00;
        private const int OFFSET_P2_BOMB_COUNT = 0xEB02;
        private const int OFFSET_P2_BOMB_TYPE = 0xEB03;
        private const int OFFSET_P2_AMMO_COUNT = 0xEB08;

        #endregion

        #region Fields

        private Process game;

        private IntPtr hProc = IntPtr.Zero;
        private IntPtr levelTimerAddressGlobal = IntPtr.Zero;
        private IntPtr continueTimerAddressGlobal = IntPtr.Zero;
        private IntPtr missionCompleteAddressGlobal = IntPtr.Zero;
        private IntPtr levelSelect1AddressGlobal = IntPtr.Zero;
        private IntPtr levelSelect2AddressGlobal = IntPtr.Zero;
        private IntPtr livesCountP1AddressGlobal = IntPtr.Zero;
        private IntPtr livesCountP2AddressGlobal = IntPtr.Zero;
        private IntPtr debug1AddressGlobal = IntPtr.Zero;
        private IntPtr debug2AddressGlobal = IntPtr.Zero;
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
        private IntPtr currentP1Status = IntPtr.Zero;
        private IntPtr currentP2Status = IntPtr.Zero;
        private IntPtr scoreP1AddressGlobal = IntPtr.Zero;
        private IntPtr scoreP2AddressGlobal = IntPtr.Zero;

        private Timer timer = new Timer();

        private string previousLogEntry = "";

        #endregion


        #region Methods

        public Form1()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.CenterScreen;

            Text = "Metal Slug 3 (GOG Version) Trainer by sLeEpY9090";
            textBoxLog.Text = "Metal Slug 3 (GOG Version) Trainer by sLeEpY9090" + Environment.NewLine;

            PopulateLevels();
            PopulateStatusTypes();
            PopulateWeaponTypes();
            PopulateBombTypes();
            PopulateScore();
            SetTextBoxMaxLength();
            SetDefaultTextBoxValues();

            // Keep trainer updated about the game process and game memory.
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        #region Form Setup Methods

        private void PopulateLevels()
        {
            Dictionary<int, string> levelsDictionary = new Dictionary<int, string>
            {
                {  0, "Stage 1: 1-1 Couples Love Land"                         },
                {  1, "Stage 1: 2-1 Marine Diver"                              },
                {  2, "Stage 1: 1-1 Return from Marine Diver"                  },
                {  3, "Stage 1: 3-1 The Ship Asia"                             },
                {  4, "Stage 1: 1-1 Return from the Ship Asia"                 },
                {  5, "Stage 1: 1-1 DEBUG: Before the Boat"                    },
                {  6, "Stage 1: 2-1 DEBUG: Marine Diver Midpoint"              },
                {  7, "Stage 2: 1-1 The Midnight Wandering"                    },
                {  8, "Stage 2: 2-1 Death Valley"                              },
                {  9, "Stage 2: 1-1 Return from the Valley"                    },
                { 10, "Stage 2: 3-1 Devil's Snow Cave"                         },
                { 11, "Stage 2: 1-1 Return from the Snow Cave"                 },
                { 12, "Stage 2: DEBUG: Mounting/Turnaround Point in Snow Cave" },
                { 13, "Stage 2: DEBUG: Helicopter Regiment"                    },
                { 14, "Stage 2: DEBUG: Boss 2"                                 },
                { 15, "Stage 3: 1-1 Eyes over the Tides"                       },
                { 16, "Stage 3: 1-2 The Blue Sea"                              },
                { 17, "Stage 3: 1-3 Secret Factory"                            },
                { 18, "Stage 3: 1-4 Meet the Boss"                             },
                { 19, "Stage 3: 2-1 Undersea Cave"                             },
                { 20, "Stage 3: 2-2 As Hard As Expected"                       },
                { 21, "Stage 3: 3-1 Great Jumping Ostriches"                   },
                { 22, "Stage 4: 1-1 Desert Loop"                               },
                { 23, "Stage 4: 1-2 Climbing the Pyramid"                      },
                { 24, "Stage 4: 1-3 Meet the Boss (Via Falling)"               },
                { 25, "Stage 4: 2-1 Carpet Shop"                               },
                { 26, "Stage 4: 2-2 Wine Storage and Father Inside"            },
                { 27, "Stage 4: 2-3 Japanese Soldiers Entrance"                },
                { 28, "Stage 4: 2-4 Japanese Soldiers"                         },
                { 29, "Stage 4: 3-1 Underground"                               },
                { 30, "Stage 4: 3-1 Underground Midpoint"                      },
                { 31, "Stage 4: 3-1 Underground Midpoint 2"                    },
                { 32, "Stage 4: 4-1 Maneater Den"                              },
                { 33, "Stage 4: 4-2 Ruins Corridor"                            },
                { 34, "Stage 4: 4-3 Small Room"                                },
                { 35, "Stage 4: 4-4 Quicksand"                                 },
                { 36, "Stage 4: 4-5 Suspended Ceiling"                         },
                { 37, "Stage 4: 4-6 Ruins Elevator"                            },
                { 38, "Stage 4: 1-3 Meet the Boss (Via Climbing)"              },
                { 39, "Stage 4: 4-7 Underground Warehouse (Intro)"             },
                { 40, "Stage 5: 1-0 Into the Sky"                              },
                { 41, "Stage 5: 1-1 The Deep Blue Sea of Clouds"               },
                { 42, "Stage 5: 1-2 The Morden Army's Space Base"              },
                { 43, "Stage 5: 1-3 Stratosphere"                              },
                { 44, "Stage 5: 1-4 The Cosmos"                                },
                { 45, "Stage 5: 1-5 Enemy Mothership Shaft"                    },
                { 46, "Stage 5: 1-6 Mothership Hallway"                        },
                { 47, "Stage 5: 1-7 The Prison"                                },
                { 48, "Stage 5: 1-8 Power Reactor Hallway"                     },
                { 49, "Stage 5: 1-9 Fake Last Boss"                            },
                { 50, "Stage 5: 1-A Bio-sector"                                },
                { 51, "Stage 5: 1-B Clone Room"                                },
                { 52, "Stage 5: 1-C Airlock Hallway"                           },
                { 53, "Stage 5: 1-D Escape!!"                                  },
                { 54, "Stage 5: 1-D Rootmars Battle"                           },
                { 55, "MISSION ALL OVER"                                       },
                { 56, "On the Sea 1"                                           },
                { 57, "On the Sea 2"                                           },
                { 58, "Under the Sea (Credits)"                                },
                { 59, "Game Over (Best Tank Busters)"                          },
                { 60, "Demo 1-1"                                               },
                { 61, "Demo 1-2"                                               },
                { 62, "Demo 1 End"                                             },
                { 63, "Demo 2-1"                                               },
                { 64, "Demo 2-2"                                               },
                { 65, "Demo 2 End"                                             },
                { 66, "How to Play"                                            },
                { 67, "Test Stage - Andy the Wizard"                           },
                { 68, "Test Stage - Taguchi"                                   },
                { 69, "Test Stage - Nishino"                                   },
                { 70, "Test Stage - Fujisawa Kankoku"                          },
                { 71, "Test Stage - Tyler Yamamoto"                            },
                { 72, "Test Stage - Mee Her"                                   },
                { 73, "Test Stage - Arita Secret Factory"                      },
                { 74, "Test Stage - Midnight Directions"                       },
                { 75, "Test Stage - It's barely there"                         },
                { 76, "Test Stage - Anmira"                                    }
            };

            comboBoxLevelSelect.Items.Clear();
            comboBoxLevelSelect.DataSource = new BindingSource(levelsDictionary, null);
            comboBoxLevelSelect.DisplayMember = "Value";
            comboBoxLevelSelect.ValueMember = "Key";
            comboBoxLevelSelect.SelectedIndex = 0;

        }

        private void PopulateWeaponTypes()
        {
            Dictionary<int, string> weaponTypesDictionary = new Dictionary<int, string>
            {
                {  0, " 0 - Hand Gun"                },
                {  1, " 1 - Cannon"                  },
                {  2, " 2 - Shotgun"                 },
                {  3, " 3 - Rocket Launcher"         },
                {  4, " 4 - Flame Shot"              },
                {  5, " 5 - Heavy Machine Gun"       },
                {  6, " 6 - Laser Gun"               },
                {  7, " 7 - Super Shotgun"           },
                {  8, " 8 - Super Rocket Launcher"   },
                {  9, " 9 - Super Flame Shot"        },
                { 10, "10 - Super Heavy Machine Gun" },
                { 11, "11 - Super Laser Gun"         },
                { 12, "12 - Enemy Chaser"            },
                { 13, "13 - Iron Lizard"             },
                { 14, "14 - Dropshot"                },
                { 15, "15 - Super Cannon"            }
            };

            comboBoxP1WeaponTypeRead.Items.Clear();
            comboBoxP1WeaponTypeRead.DataSource = new BindingSource(weaponTypesDictionary, null);
            comboBoxP1WeaponTypeRead.DisplayMember = "Value";
            comboBoxP1WeaponTypeRead.ValueMember = "Key";
            comboBoxP1WeaponTypeRead.SelectedIndex = 0;

            comboBoxP2WeaponTypeRead.Items.Clear();
            comboBoxP2WeaponTypeRead.DataSource = new BindingSource(weaponTypesDictionary, null);
            comboBoxP2WeaponTypeRead.DisplayMember = "Value";
            comboBoxP2WeaponTypeRead.ValueMember = "Key";
            comboBoxP2WeaponTypeRead.SelectedIndex = 0;

            comboBoxP1WeaponTypeWrite.Items.Clear();
            comboBoxP1WeaponTypeWrite.DataSource = new BindingSource(weaponTypesDictionary, null);
            comboBoxP1WeaponTypeWrite.DisplayMember = "Value";
            comboBoxP1WeaponTypeWrite.ValueMember = "Key";
            comboBoxP1WeaponTypeWrite.SelectedIndex = 0;

            comboBoxP2WeaponTypeWrite.Items.Clear();
            comboBoxP2WeaponTypeWrite.DataSource = new BindingSource(weaponTypesDictionary, null);
            comboBoxP2WeaponTypeWrite.DisplayMember = "Value";
            comboBoxP2WeaponTypeWrite.ValueMember = "Key";
            comboBoxP2WeaponTypeWrite.SelectedIndex = 0;
        }

        private void PopulateStatusTypes()
        {
            Dictionary<int, string> statusTypesDictionary = new Dictionary<int, string>
            {
                {  0, " 0 - Normal"                    },
                {  1, " 1 - Special Weapon"            },
                {  2, " 2 - Fat"                       },
                {  3, " 3 - Mummy"                     },
                {  4, " 4 - Scuba Gear Normal"         },
                {  5, " 5 - Scuba Gear Special Weapon" },
                {  6, " 6 - Snowman"                   },
                {  7, " 7 - Zombie"                    },
                {  8, " 8 - Flying (final stage)"      },
                {  9, " 9 - Flying (final stage)"      },
                { 10, "10 - Spaceship (final stage)"   }
            };

            comboBoxP1StatusRead.Items.Clear();
            comboBoxP1StatusRead.DataSource = new BindingSource(statusTypesDictionary, null);
            comboBoxP1StatusRead.DisplayMember = "Value";
            comboBoxP1StatusRead.ValueMember = "Key";
            comboBoxP1StatusRead.SelectedIndex = 0;

            comboBoxP1StatusWrite.Items.Clear();
            comboBoxP1StatusWrite.DataSource = new BindingSource(statusTypesDictionary, null);
            comboBoxP1StatusWrite.DisplayMember = "Value";
            comboBoxP1StatusWrite.ValueMember = "Key";
            comboBoxP1StatusWrite.SelectedIndex = 0;

            comboBoxP2StatusRead.Items.Clear();
            comboBoxP2StatusRead.DataSource = new BindingSource(statusTypesDictionary, null);
            comboBoxP2StatusRead.DisplayMember = "Value";
            comboBoxP2StatusRead.ValueMember = "Key";
            comboBoxP2StatusRead.SelectedIndex = 0;

            comboBoxP2StatusWrite.Items.Clear();
            comboBoxP2StatusWrite.DataSource = new BindingSource(statusTypesDictionary, null);
            comboBoxP2StatusWrite.DisplayMember = "Value";
            comboBoxP2StatusWrite.ValueMember = "Key";
            comboBoxP2StatusWrite.SelectedIndex = 0;
        }

        private void PopulateBombTypes()
        {
            Dictionary<int, string> bombTypesDictionary = new Dictionary<int, string>
            {
                { 0, "0 - None"      },
                { 1, "1 - Grenade"   },
                { 2, "2 - Fire Bomb" },
                { 3, "3 - Stone"     },
                { 4, "4 - Cannon"    },
                { 5, "5 - Cannon"    },
                { 6, "6 - Missile"   },
                { 7, "7 - Bomb"      },
                { 8, "8 - Missile"   },
                { 9, "9 - Cannon"    },
                { 10, "10 - Cannon"  },
                { 11, "11 - Missile" },
                { 12, "12 - Fire"    },
                { 13, "13 - Laser"   },
            };

            comboBoxP1BombTypeRead.Items.Clear();
            comboBoxP1BombTypeRead.DataSource = new BindingSource(bombTypesDictionary, null);
            comboBoxP1BombTypeRead.DisplayMember = "Value";
            comboBoxP1BombTypeRead.ValueMember = "Key";
            comboBoxP1BombTypeRead.SelectedIndex = 0;

            comboBoxP2BombTypeRead.Items.Clear();
            comboBoxP2BombTypeRead.DataSource = new BindingSource(bombTypesDictionary, null);
            comboBoxP2BombTypeRead.DisplayMember = "Value";
            comboBoxP2BombTypeRead.ValueMember = "Key";
            comboBoxP2BombTypeRead.SelectedIndex = 0;

            comboBoxP1BombTypeWrite.Items.Clear();
            comboBoxP1BombTypeWrite.DataSource = new BindingSource(bombTypesDictionary, null);
            comboBoxP1BombTypeWrite.DisplayMember = "Value";
            comboBoxP1BombTypeWrite.ValueMember = "Key";
            comboBoxP1BombTypeWrite.SelectedIndex = 0;

            comboBoxP2BombTypeWrite.Items.Clear();
            comboBoxP2BombTypeWrite.DataSource = new BindingSource(bombTypesDictionary, null);
            comboBoxP2BombTypeWrite.DisplayMember = "Value";
            comboBoxP2BombTypeWrite.ValueMember = "Key";
            comboBoxP2BombTypeWrite.SelectedIndex = 0;
        }

        private void PopulateScore()
        {
            for (int i = 0; i < 256; i++)
            {
                comboBoxP1Score1.Items.Add(i.ToString("X"));
                comboBoxP1Score2.Items.Add(i.ToString("X"));
                comboBoxP1Score3.Items.Add(i.ToString("X"));
                comboBoxP1Score4.Items.Add(i.ToString("X"));

                comboBoxP2Score1.Items.Add(i.ToString("X"));
                comboBoxP2Score2.Items.Add(i.ToString("X"));
                comboBoxP2Score3.Items.Add(i.ToString("X"));
                comboBoxP2Score4.Items.Add(i.ToString("X"));
            }

            comboBoxP1Score1.SelectedIndex = 255;
            comboBoxP1Score2.SelectedIndex = 255;
            comboBoxP1Score3.SelectedIndex = 255;
            comboBoxP1Score4.SelectedIndex = 255;

            comboBoxP2Score1.SelectedIndex = 255;
            comboBoxP2Score2.SelectedIndex = 255;
            comboBoxP2Score3.SelectedIndex = 255;
            comboBoxP2Score4.SelectedIndex = 255;
        }

        public void SetDefaultTextBoxValues()
        {
            textBoxLevelTimerWrite.Text = "159"; // anything higher kills the player
            textBoxContinueTimerWrite.Text = "255";

            textBoxP1LivesCountWrite.Text = "255";
            textBoxP1InvincibilityTimerWrite.Text = "60"; // Invincible
            textBoxP1POWsRescuedWrite.Text = "255";
            textBoxP1BombCountWrite.Text = "255";
            textBoxP1AmmoCountWrite.Text = "999"; // "65535";
            textBoxP1VehicleAmmoCountWrite.Text = "255";
            textBoxP1VehicleHealthCountWrite.Text = "48"; // Max Vehicle Health
            textBoxP1CreditsCountWrite.Text = "255";

            textBoxP2LivesCountWrite.Text = "255";
            textBoxP2InvincibilityTimerWrite.Text = "60"; // Invincible
            textBoxP2POWsRescuedWrite.Text = "255";
            textBoxP2BombCountWrite.Text = "255";
            textBoxP2AmmoCountWrite.Text = "999"; // "65535";
            textBoxP2VehicleAmmoCountWrite.Text = "255";
            textBoxP2VehicleHealthCountWrite.Text = "48"; // Max Vehicle Health
            textBoxP2CreditsCountWrite.Text = "255";

            textBoxLevelTimerRead.Enabled = false;
            textBoxContinueTimerRead.Enabled = false;

            textBoxP1LivesCountRead.Enabled = false;
            textBoxP1InvincibilityTimerRead.Enabled = false;
            textBoxP1POWsRescuedRead.Enabled = false;
            textBoxP1BombCountRead.Enabled = false;
            textBoxP1AmmoCountRead.Enabled = false;
            textBoxP1VehicleAmmoCountRead.Enabled = false;
            textBoxP1VehicleHealthCountRead.Enabled = false;
            textBoxP1CreditsCountRead.Enabled = false;
            textBoxP1ScoreRead.Enabled = false;
            comboBoxP1BombTypeRead.Enabled = false;
            comboBoxP1WeaponTypeRead.Enabled = false;
            comboBoxP1StatusRead.Enabled = false;

            textBoxP2LivesCountRead.Enabled = false;
            textBoxP2InvincibilityTimerRead.Enabled = false;
            textBoxP2POWsRescuedRead.Enabled = false;
            textBoxP2BombCountRead.Enabled = false;
            textBoxP2AmmoCountRead.Enabled = false;
            textBoxP2VehicleAmmoCountRead.Enabled = false;
            textBoxP2VehicleHealthCountRead.Enabled = false;
            textBoxP2CreditsCountRead.Enabled = false;
            textBoxP2ScoreRead.Enabled = false;
            comboBoxP2BombTypeRead.Enabled = false;
            comboBoxP2WeaponTypeRead.Enabled = false;
            comboBoxP2StatusRead.Enabled = false;

            textBoxModuleName.Text = MODULE_NAME;
            textBoxProcessName.Text = PROCESS_NAME;
        }

        private void SetTextBoxMaxLength()
        {
            textBoxLevelTimerWrite.MaxLength = 3;
            textBoxContinueTimerWrite.MaxLength = 3;

            textBoxP1LivesCountWrite.MaxLength = 3;
            textBoxP1InvincibilityTimerWrite.MaxLength = 3;
            textBoxP1POWsRescuedWrite.MaxLength = 3;
            textBoxP1BombCountWrite.MaxLength = 3;
            textBoxP1AmmoCountWrite.MaxLength = 5;
            textBoxP1VehicleAmmoCountWrite.MaxLength = 3;
            textBoxP1VehicleHealthCountWrite.MaxLength = 3;
            textBoxP1CreditsCountWrite.MaxLength = 3;

            textBoxP2LivesCountWrite.MaxLength = 3;
            textBoxP2InvincibilityTimerWrite.MaxLength = 3;
            textBoxP2POWsRescuedWrite.MaxLength = 3;
            textBoxP2BombCountWrite.MaxLength = 3;
            textBoxP2AmmoCountWrite.MaxLength = 5;
            textBoxP2VehicleAmmoCountWrite.MaxLength = 3;
            textBoxP2VehicleHealthCountWrite.MaxLength = 3;
            textBoxP2CreditsCountWrite.MaxLength = 3;
        }

        #endregion

        // Gets game connection or returns null if not found
        private Process GameConnect()
        {
            string processName = PROCESS_NAME;
            if (checkBoxProcessName.Checked)
            {
                processName = textBoxProcessName.Text;
            }

            // Game process: mslugx
            game = Process.GetProcessesByName(processName).FirstOrDefault();
            if (game == null)
            {
                hProc = IntPtr.Zero;
            }
            else
            {
                // Open handle with full permission to game process
                hProc = OpenProcess((int)ProcessAccessRights.PROCESS_ALL_ACCESS, false, game.Id);
            }

            return game;
        }

        private IntPtr GetBaseAddress()
        {
            string moduleName = MODULE_NAME;
            if (checkBoxModuleName.Checked)
            {
                moduleName = textBoxModuleName.Text;
            }

            IntPtr baseAddress = IntPtr.Zero;
            foreach (ProcessModule module in game.Modules)
            {
                if (module.ModuleName.ToLower() == moduleName)
                {
                    baseAddress = module.BaseAddress;
                    break;
                }
            }

            return baseAddress;
        }

        private IntPtr GetOffsetAddress(IntPtr hProc, int address, int offset)
        {
            IntPtr baseAddress = GetBaseAddress();
            IntPtr offsetAddress = IntPtr.Zero;
            if (baseAddress != IntPtr.Zero)
            {
                offsetAddress = IntPtr.Add(baseAddress, address);
                offsetAddress = ReadPointerUInt32(hProc, offsetAddress, offset);
            }
            return offsetAddress;
        }

        #region Display Value Methods

        private string DisplayByteValue(IntPtr hProc, IntPtr addressGlobal, int baseAddress, int valueOffset)
        {
            if (addressGlobal == IntPtr.Zero || ((int)addressGlobal) == valueOffset)
            {
                addressGlobal = GetOffsetAddress(hProc, baseAddress, valueOffset);
            }
            int tempValue = ReadByte(hProc, addressGlobal);
            return tempValue.ToString();
        }

        private string DisplayUInt16Value(IntPtr hProc, IntPtr addressGlobal, int baseAddress, int valueOffset)
        {
            if (addressGlobal == IntPtr.Zero || ((int)addressGlobal) == valueOffset)
            {
                addressGlobal = GetOffsetAddress(hProc, baseAddress, valueOffset);
            }
            uint tempValue = ReadUInt16(hProc, addressGlobal);
            return tempValue.ToString();
        }

        private string DisplayUInt32Value(IntPtr hProc, IntPtr addressGlobal, int baseAddress, int valueOffset)
        {
            if (addressGlobal == IntPtr.Zero || ((int)addressGlobal) == valueOffset)
            {
                addressGlobal = GetOffsetAddress(hProc, baseAddress, valueOffset);
            }
            uint tempValue = ReadUInt32(hProc, addressGlobal);
            return tempValue.ToString();
        }

        #endregion

        #region Get Pointer Methods

        private IntPtr ReadPointerInt64(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToInt64(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerUInt64(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToUInt64(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerInt32(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerUInt32(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToUInt32(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerInt16(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToInt16(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerUInt16(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToUInt16(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        #endregion

        #region Get/Set Memory Value Methods

        private float ReadFloat(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4]; // FLOAT = 4 byte
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }

        private void WriteFloat(IntPtr hProcess, IntPtr address, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        private int ReadInt(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4]; // INT = 4 byte
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        private void WriteInt(IntPtr hProcess, IntPtr address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        private int ReadByte(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[1];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return buffer[0];
        }

        private void WriteByte(IntPtr hProcess, IntPtr address, byte value)
        {
            // Since bitconverter getbytes with a byte value handles it as a short (no overload for byte), we get 2 bytes back as a short, but we only want the first
            // https://learn.microsoft.com/en-us/dotnet/api/system.bitconverter.getbytes?view=net-9.0#system-bitconverter-getbytes(system-int16)
            byte[] buffer = BitConverter.GetBytes(value);
            //WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length - 1, out int bytesWritten);
        }

        private uint ReadUInt16(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }

        private void WriteUInt16(IntPtr hProcess, IntPtr address, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        private uint ReadUInt32(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private void WriteUInt32(IntPtr hProcess, IntPtr address, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        #endregion

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((GameConnect() == null) || (GetBaseAddress() == IntPtr.Zero))
            {
                ForeColor = Color.Red;
                string logEntry = "Metal Slug 3 Process Not Found.";
                logEntry += Environment.NewLine;
                if (logEntry != previousLogEntry)
                {
                    textBoxLog.AppendText(logEntry + Environment.NewLine);
                    previousLogEntry = logEntry;
                }
            }
            else
            {
                ForeColor = Color.Green;
                string logEntry = $"[Metal Slug 3 Process {game.ProcessName} found in {game} with PID: {game.Id}]";
                logEntry += Environment.NewLine;
                logEntry += $"Start Time: {game.StartTime}";

                if (logEntry != previousLogEntry)
                {
                    previousLogEntry = logEntry;

                    textBoxLog.AppendText(logEntry + Environment.NewLine);
                    textBoxLog.AppendText($"Total Processor Time: {game.TotalProcessorTime}"
                        + Environment.NewLine);
                    textBoxLog.AppendText($"Physical Memory Usage (MB): {game.WorkingSet64 / (1024 * 1024)}"
                        + Environment.NewLine
                        + "---------------------------------------------------"
                        + Environment.NewLine);
                }

                levelTimerAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_LEVEL_TIMER);
                continueTimerAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_CONTINUE_TIMER);
                missionCompleteAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_MISSION_COMPLETE);
                levelSelect1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_LEVEL_SELECT_1);
                levelSelect2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_LEVEL_SELECT_2);
                debug1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_DEBUG_1);
                debug2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_DEBUG_2);

                livesCountP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_LIVES_COUNT);
                invincibilityTimerP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_INVINCIBILITY_TIMER);
                powsRescuedP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_POWS_RESCUED);
                weaponTypeP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_WEAPON_TYPE);
                bombTypeP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_BOMB_TYPE);
                bombCountP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_BOMB_COUNT);
                ammoCountP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_AMMO_COUNT);
                vehicleAmmoCanonCountP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_VEHICLE_AMMO_CANON_COUNT);
                vehicleHealthCountP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_VEHICLE_HEALTH_COUNT);
                scoreP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_SCORE);
                currentP1Status = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_STATUS);
                creditsCountP1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_CREDITS_COUNT);

                livesCountP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_LIVES_COUNT);
                invincibilityTimerP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_INVINCIBILITY_TIMER);
                powsRescuedP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_POWS_RESCUED);
                weaponTypeP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_WEAPON_TYPE);
                bombTypeP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_BOMB_TYPE);
                bombCountP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_BOMB_COUNT);
                ammoCountP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_AMMO_COUNT);
                vehicleAmmoCanonCountP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_VEHICLE_AMMO_CANON_COUNT);
                vehicleHealthCountP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_VEHICLE_HEALTH_COUNT);
                scoreP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_SCORE);
                currentP2Status = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P2_STATUS);
                creditsCountP2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P2_CREDITS_COUNT);

                #region Level Timer

                if (checkBoxLevelTimer.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxLevelTimerWrite.Text, out int levelTimer)
                            && (levelTimer >= 0 && levelTimer <= 255))
                        {
                            WriteByte(hProc, levelTimerAddressGlobal, Convert.ToByte(levelTimer));
                        }
                        else
                        {
                            textBoxLog.AppendText("Level Timer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Level Timer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxLevelTimerRead.Text = DisplayByteValue(hProc, levelTimerAddressGlobal, BASE_ADDRESS_1, OFFSET_LEVEL_TIMER);

                #endregion

                #region Continue Timer

                if (checkBoxContinueTimer.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxContinueTimerWrite.Text, out int continueTimer)
                            && (continueTimer >= 0 && continueTimer <= 255))
                        {
                            WriteByte(hProc, continueTimerAddressGlobal, Convert.ToByte(continueTimer));
                        }
                        else
                        {
                            textBoxLog.AppendText("Continue Timer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Continue Timer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxContinueTimerRead.Text = DisplayByteValue(hProc, continueTimerAddressGlobal, BASE_ADDRESS_1, OFFSET_CONTINUE_TIMER);

                #endregion

                #region Lives Count

                if (checkBoxP1LivesCount.Checked)
                {
                    try
                    {
                        // 0 - 127 Normal Lives
                        // 128 - 255 - Infinite Lives
                        if (int.TryParse(textBoxP1LivesCountWrite.Text, out int livesCount)
                            && (livesCount >= 0 && livesCount <= 255))
                        {
                            // Write P1 number of lives
                            WriteByte(hProc, livesCountP1AddressGlobal, Convert.ToByte(livesCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Lives Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Lives Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1LivesCountRead.Text = DisplayByteValue(hProc, livesCountP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_LIVES_COUNT);

                if (checkBoxP2LivesCount.Checked)
                {
                    try
                    {
                        // 0 - 127 Normal Lives
                        // 128 - 255 - Infinite Lives
                        if (int.TryParse(textBoxP2LivesCountWrite.Text, out int livesCount)
                            && (livesCount >= 0 && livesCount <= 255))
                        {
                            // Write P2 number of lives
                            WriteByte(hProc, livesCountP2AddressGlobal, Convert.ToByte(livesCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Lives Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Lives Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2LivesCountRead.Text = DisplayByteValue(hProc, livesCountP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_LIVES_COUNT);

                #endregion

                #region Invincibility Timer

                if (checkBoxP1InvincibilityTimer.Checked)
                {
                    try
                    {
                        // Freeze at 60 - Invincible
                        if (int.TryParse(textBoxP1InvincibilityTimerWrite.Text, out int invincibilityTimer)
                            && (invincibilityTimer >= 0 && invincibilityTimer <= 255))
                        {
                            WriteByte(hProc, invincibilityTimerP1AddressGlobal, Convert.ToByte(invincibilityTimer));
                        }
                        else
                        {
                            textBoxLog.AppendText("Invincibility Timer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Invincibility Timer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1InvincibilityTimerRead.Text = DisplayByteValue(hProc, invincibilityTimerP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_INVINCIBILITY_TIMER);

                if (checkBoxP2InvincibilityTimer.Checked)
                {
                    try
                    {
                        // Freeze at 60 - Invincible
                        if (int.TryParse(textBoxP2InvincibilityTimerWrite.Text, out int invincibilityTimer)
                            && (invincibilityTimer >= 0 && invincibilityTimer <= 255))
                        {
                            WriteByte(hProc, invincibilityTimerP2AddressGlobal, Convert.ToByte(invincibilityTimer));
                        }
                        else
                        {
                            textBoxLog.AppendText("Invincibility Timer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Invincibility Timer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2InvincibilityTimerRead.Text = DisplayByteValue(hProc, invincibilityTimerP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_INVINCIBILITY_TIMER);

                #endregion

                #region POW Count

                if (checkBoxP1POWsRescued.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1POWsRescuedWrite.Text, out int powsRescued)
                            && (powsRescued >= 0 && powsRescued <= 255))
                        {
                            WriteByte(hProc, powsRescuedP1AddressGlobal, Convert.ToByte(powsRescued));
                        }
                        else
                        {
                            textBoxLog.AppendText("POW's Rescued value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("POW's Rescued value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1POWsRescuedRead.Text = DisplayByteValue(hProc, powsRescuedP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_POWS_RESCUED);

                if (checkBoxP2POWsRescued.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP2POWsRescuedWrite.Text, out int powsRescued)
                            && (powsRescued >= 0 && powsRescued <= 255))
                        {
                            WriteByte(hProc, powsRescuedP2AddressGlobal, Convert.ToByte(powsRescued));
                        }
                        else
                        {
                            textBoxLog.AppendText("POW's Rescued value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("POW's Rescued value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2POWsRescuedRead.Text = DisplayByteValue(hProc, powsRescuedP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_POWS_RESCUED);

                #endregion

                #region Bomb Count

                if (checkBoxP1BombCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1BombCountWrite.Text, out int bombCount)
                            && (bombCount >= 0 && bombCount <= 255))
                        {
                            WriteByte(hProc, bombCountP1AddressGlobal, Convert.ToByte(bombCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Bomb Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Bomb value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1BombCountRead.Text = DisplayByteValue(hProc, bombCountP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_BOMB_COUNT);

                if (checkBoxP2BombCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP2BombCountWrite.Text, out int bombCount)
                            && (bombCount >= 0 && bombCount <= 255))
                        {
                            WriteByte(hProc, bombCountP2AddressGlobal, Convert.ToByte(bombCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Bomb Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Bomb Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2BombCountRead.Text = DisplayByteValue(hProc, bombCountP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_BOMB_COUNT);

                #endregion

                #region Vehicle Ammo Count

                if (checkBoxP1VehicleAmmoCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1VehicleAmmoCountWrite.Text, out int vehicleAmmoCount)
                            && (vehicleAmmoCount >= 0 && vehicleAmmoCount <= 255))
                        {
                            WriteByte(hProc, vehicleAmmoCanonCountP1AddressGlobal, Convert.ToByte(vehicleAmmoCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Vehicle Ammo Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Vehicle Ammo Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1VehicleAmmoCountRead.Text = DisplayByteValue(hProc, vehicleAmmoCanonCountP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_VEHICLE_AMMO_CANON_COUNT);

                if (checkBoxP2VehicleAmmoCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP2VehicleAmmoCountWrite.Text, out int vehicleAmmoCount)
                            && (vehicleAmmoCount >= 0 && vehicleAmmoCount <= 255))
                        {
                            WriteByte(hProc, vehicleAmmoCanonCountP2AddressGlobal, Convert.ToByte(vehicleAmmoCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Vehicle Ammo Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Vehicle Ammo Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2VehicleAmmoCountRead.Text = DisplayByteValue(hProc, vehicleAmmoCanonCountP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_VEHICLE_AMMO_CANON_COUNT);

                #endregion

                #region Vehicle Health Count

                if (checkBoxP1VehicleHealthCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1VehicleHealthCountWrite.Text, out int vehicleHealthCount)
                            && (vehicleHealthCount >= 0 && vehicleHealthCount <= 255))
                        {
                            WriteByte(hProc, vehicleHealthCountP1AddressGlobal, Convert.ToByte(vehicleHealthCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Vehicle Health Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Vehicle Health Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1VehicleHealthCountRead.Text = DisplayByteValue(hProc, vehicleHealthCountP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_VEHICLE_HEALTH_COUNT);

                if (checkBoxP2VehicleHealthCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP2VehicleHealthCountWrite.Text, out int vehicleHealthCount)
                            && (vehicleHealthCount >= 0 && vehicleHealthCount <= 255))
                        {
                            WriteByte(hProc, vehicleHealthCountP2AddressGlobal, Convert.ToByte(vehicleHealthCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Vehicle Health Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Vehicle Health Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2VehicleHealthCountRead.Text = DisplayByteValue(hProc, vehicleHealthCountP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_VEHICLE_HEALTH_COUNT);

                #endregion

                #region Credits Count

                if (checkBoxP1CreditsCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1CreditsCountWrite.Text, out int creditsCount)
                            && (creditsCount >= 0 && creditsCount <= 255))
                        {
                            WriteByte(hProc, creditsCountP1AddressGlobal, Convert.ToByte(creditsCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Credits Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Credits Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1CreditsCountRead.Text = DisplayByteValue(hProc, creditsCountP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_CREDITS_COUNT);

                if (checkBoxP2CreditsCount.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP2CreditsCountWrite.Text, out int creditsCount)
                            && (creditsCount >= 0 && creditsCount <= 255))
                        {
                            WriteByte(hProc, creditsCountP2AddressGlobal, Convert.ToByte(creditsCount));
                        }
                        else
                        {
                            textBoxLog.AppendText("Credits Count value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Credits Count value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2CreditsCountRead.Text = DisplayByteValue(hProc, creditsCountP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_CREDITS_COUNT);

                #endregion

                #region Ammo Count

                // Ammo is 2 bytes
                if (checkBoxP1AmmoCount.Checked)
                {
                    try
                    {
                        if (ushort.TryParse(textBoxP1AmmoCountWrite.Text, out ushort ammoCount)
                            && (ammoCount >= 0 && ammoCount <= 65535))
                        {
                            WriteUInt16(hProc, ammoCountP1AddressGlobal, ammoCount);
                        }
                        else
                        {
                            textBoxLog.AppendText("Ammo Count value must be between 0 and 65535." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Ammo value must be between 0 and 65535."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1AmmoCountRead.Text = DisplayUInt16Value(hProc, ammoCountP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_AMMO_COUNT);

                if (checkBoxP2AmmoCount.Checked)
                {
                    try
                    {

                        if (ushort.TryParse(textBoxP2AmmoCountWrite.Text, out ushort ammoCount)
                            && (ammoCount >= 0 && ammoCount <= 65535))
                        {
                            WriteUInt16(hProc, ammoCountP2AddressGlobal, ammoCount);
                        }
                        else
                        {
                            textBoxLog.AppendText("Ammo Count value must be between 0 and 65535." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Ammo value must be between 0 and 65535."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP2AmmoCountRead.Text = DisplayUInt16Value(hProc, ammoCountP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_AMMO_COUNT);

                #endregion

                #region Score

                // Score is 4 bytes
                if (checkBoxP1Score.Checked)
                {
                    // 12345678
                    // | ED19 | ED18 | ED1B | ED1A |
                    // | 12   | 34   | 56   | 78   |
                    int eD19 = comboBoxP1Score1.SelectedIndex;
                    int eD18 = comboBoxP1Score2.SelectedIndex;
                    int eD1B = comboBoxP1Score3.SelectedIndex;
                    int eD1A = comboBoxP1Score4.SelectedIndex;

                    WriteByte(hProc, scoreP1AddressGlobal, (byte)eD18);
                    WriteByte(hProc, scoreP1AddressGlobal + 1, (byte)eD19);
                    WriteByte(hProc, scoreP1AddressGlobal + 2, (byte)eD1A);
                    WriteByte(hProc, scoreP1AddressGlobal + 3, (byte)eD1B);
                }

                int tempValue2 = ReadByte(hProc, scoreP1AddressGlobal + 1);
                textBoxP1ScoreRead.Text = tempValue2.ToString("X").PadLeft(2, '0');
                int tempValue = ReadByte(hProc, scoreP1AddressGlobal);
                textBoxP1ScoreRead.Text += tempValue.ToString("X").PadLeft(2, '0');
                int tempValue4 = ReadByte(hProc, scoreP1AddressGlobal + 3);
                textBoxP1ScoreRead.Text += tempValue4.ToString("X").PadLeft(2, '0');
                int tempValue3 = ReadByte(hProc, scoreP1AddressGlobal + 2);
                textBoxP1ScoreRead.Text += tempValue3.ToString("X").PadLeft(2, '0');


                if (checkBoxP2Score.Checked)
                {

                    int eD21 = comboBoxP2Score1.SelectedIndex;
                    int eD20 = comboBoxP2Score2.SelectedIndex;
                    int eD23 = comboBoxP2Score3.SelectedIndex;
                    int eD22 = comboBoxP2Score4.SelectedIndex;

                    WriteByte(hProc, scoreP2AddressGlobal, (byte)eD20);
                    WriteByte(hProc, scoreP2AddressGlobal + 1, (byte)eD21);
                    WriteByte(hProc, scoreP2AddressGlobal + 2, (byte)eD22);
                    WriteByte(hProc, scoreP2AddressGlobal + 3, (byte)eD23);
                }

                tempValue2 = ReadByte(hProc, scoreP2AddressGlobal + 1);
                textBoxP2ScoreRead.Text = tempValue2.ToString("X").PadLeft(2, '0');
                tempValue = ReadByte(hProc, scoreP2AddressGlobal);
                textBoxP2ScoreRead.Text += tempValue.ToString("X").PadLeft(2, '0');
                tempValue4 = ReadByte(hProc, scoreP2AddressGlobal + 3);
                textBoxP2ScoreRead.Text += tempValue4.ToString("X").PadLeft(2, '0');
                tempValue3 = ReadByte(hProc, scoreP2AddressGlobal + 2);
                textBoxP2ScoreRead.Text += tempValue3.ToString("X").PadLeft(2, '0');

                #endregion

                #region Weapon Type

                if (checkBoxP1WeaponType.Checked)
                {
                    
                    try
                    {
                        //string value = ((KeyValuePair<int, string>)comboBoxP1WeaponTypeWrite.SelectedItem).Value;

                        //// if P1 Weapon Type is 0 (pistol), p1 status should be 0, or the pistol bullets will come out of the special weapon, seems ok so might not matter
                        //// if p1 weapon type is >0, p1 status must be 1 (holding special weapon or it won't allow firing laser weapon, other weapons fire but act slightly different)
                        //byte key = Convert.ToByte(((KeyValuePair<int, string>)comboBoxP1WeaponTypeWrite.SelectedItem).Key);
                        //if (key == 0)
                        //{
                        //    // write status of holding pistol
                        //    WriteByte(hProc, currentP1Status, 0);
                        //}
                        //else
                        //{
                        //    // write status of holding special weapon
                        //    WriteByte(hProc, currentP1Status, 1);
                        //}

                        // write weapon type
                        WriteByte(hProc, weaponTypeP1AddressGlobal, Convert.ToByte(((KeyValuePair<int, string>)comboBoxP1WeaponTypeWrite.SelectedItem).Key));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 1 Weapon Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                if (int.TryParse(DisplayByteValue(hProc, weaponTypeP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_WEAPON_TYPE), out int weaponTypeP1))
                {
                    try
                    {
                        comboBoxP1WeaponTypeRead.SelectedIndex = weaponTypeP1;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 1 Weapon Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                if (checkBoxP2WeaponType.Checked)
                {
                    try
                    {
                        // if P2 Weapon Type is 0, p2 status needs to be 0
                        // if p2 weapon type is >0, p2 status must be 1 (holding special weapon or it won't allow firing weapon)
                        //byte key = Convert.ToByte(((KeyValuePair<int, string>)comboBoxP2WeaponTypeWrite.SelectedItem).Key);
                        //if (key == 0)
                        //{
                        //    // write status of holding pistol
                        //    WriteByte(hProc, currentP2Status, 0);
                        //}
                        //else
                        //{
                        //    // write status of holding special weapon
                        //    WriteByte(hProc, currentP2Status, 1);
                        //}

                        // Write new P2 weapon type
                        WriteByte(hProc, weaponTypeP2AddressGlobal, Convert.ToByte(((KeyValuePair<int, string>)comboBoxP2WeaponTypeWrite.SelectedItem).Key));                        
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 2 Weapon Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                if (int.TryParse(DisplayByteValue(hProc, weaponTypeP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_WEAPON_TYPE), out int weaponTypeP2))
                {
                    try
                    {
                        comboBoxP2WeaponTypeRead.SelectedIndex = weaponTypeP2;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 2 Weapon Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                #endregion

                #region Bomb Type

                if (checkBoxP1BombType.Checked)
                {
                    try
                    {
                        WriteByte(hProc, bombTypeP1AddressGlobal, Convert.ToByte(((KeyValuePair<int, string>)comboBoxP1BombTypeWrite.SelectedItem).Key));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 1 Bomb Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                if (int.TryParse(DisplayByteValue(hProc, bombTypeP1AddressGlobal, BASE_ADDRESS_1, OFFSET_P1_BOMB_TYPE), out int bombTypeP1))
                {
                    try
                    {
                        comboBoxP1BombTypeRead.SelectedIndex = bombTypeP1;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 1 Bomb Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                if (checkBoxP2BombType.Checked)
                {
                    try
                    {
                        WriteByte(hProc, bombTypeP2AddressGlobal, Convert.ToByte(((KeyValuePair<int, string>)comboBoxP2BombTypeWrite.SelectedItem).Key));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 2 Bomb Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                if (int.TryParse(DisplayByteValue(hProc, bombTypeP2AddressGlobal, BASE_ADDRESS_2, OFFSET_P2_BOMB_TYPE), out int bombTypeP2))
                {
                    try
                    {
                        comboBoxP2BombTypeRead.SelectedIndex = bombTypeP2;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 2 Bomb Type."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                #endregion

                #region Status

                if (checkBoxP1Status.Checked)
                {
                    try
                    {
                        WriteByte(hProc, currentP1Status, Convert.ToByte(((KeyValuePair<int, string>)comboBoxP1StatusWrite.SelectedItem).Key));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 1 Status."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                if (int.TryParse(DisplayByteValue(hProc, currentP1Status, BASE_ADDRESS_1, OFFSET_P1_STATUS), out int currentP1StatusInt))
                {
                    try
                    {
                        comboBoxP1StatusRead.SelectedIndex = currentP1StatusInt;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 1 Status."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                if (checkBoxP2Status.Checked)
                {
                    try
                    {
                        WriteByte(hProc, currentP2Status, Convert.ToByte(((KeyValuePair<int, string>)comboBoxP2StatusWrite.SelectedItem).Key));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 2 Status."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                if (int.TryParse(DisplayByteValue(hProc, currentP2Status, BASE_ADDRESS_2, OFFSET_P2_STATUS), out int currentP2StatusInt))
                {
                    try
                    {
                        comboBoxP2StatusRead.SelectedIndex = currentP2StatusInt;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 2 Status."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                #endregion

            }
        }

        #region Mission Complete

        private void ButtonMissionCommplete_Click(object sender, EventArgs e)
        {
            try
            {
                WriteByte(hProc, missionCompleteAddressGlobal, 255);
            }
            catch (Exception ex)
            {
                textBoxLog.AppendText("An error occurred setting Mission Complete."
                    + Environment.NewLine
                    + "Exception: "
                    + ex);
            }
        }


        #endregion

        #region Level Select

        private void ButtonLevelSelect_Click(object sender, EventArgs e)
        {
            byte eD7A;
            byte eD7B;

            switch (comboBoxLevelSelect.SelectedIndex)
            {
                case 0:
                    eD7A = 0;
                    eD7B = 0;
                    break;
                case 1:
                    eD7A = 1;
                    eD7B = 0;
                    break;
                case 2:
                    eD7A = 2;
                    eD7B = 0;
                    break;
                case 3:
                    eD7A = 3;
                    eD7B = 0;
                    break;
                case 4:
                    eD7A = 4;
                    eD7B = 0;
                    break;
                case 5:
                    eD7A = 5;
                    eD7B = 0;
                    break;
                case 6:
                    eD7A = 6;
                    eD7B = 0;
                    break;
                case 7:
                    eD7A = 0;
                    eD7B = 1;
                    break;
                case 8:
                    eD7A = 1;
                    eD7B = 1;
                    break;
                case 9:
                    eD7A = 2;
                    eD7B = 1;
                    break;
                case 10:
                    eD7A = 3;
                    eD7B = 1;
                    break;
                case 11:
                    eD7A = 4;
                    eD7B = 1;
                    break;
                case 12:
                    eD7A = 5;
                    eD7B = 1;
                    break;
                case 13:
                    eD7A = 6;
                    eD7B = 1;
                    break;
                case 14:
                    eD7A = 7;
                    eD7B = 1;
                    break;
                case 15:
                    eD7A = 0;
                    eD7B = 2;
                    break;
                case 16:
                    eD7A = 1;
                    eD7B = 2;
                    break;
                case 17:
                    eD7A = 2;
                    eD7B = 2;
                    break;
                case 18:
                    eD7A = 3;
                    eD7B = 2;
                    break;
                case 19:
                    eD7A = 4;
                    eD7B = 2;
                    break;
                case 20:
                    eD7A = 5;
                    eD7B = 2;
                    break;
                case 21:
                    eD7A = 6;
                    eD7B = 2;
                    break;
                case 22:
                    eD7A = 0;
                    eD7B = 3;
                    break;
                case 23:
                    eD7A = 1;
                    eD7B = 3;
                    break;
                case 24:
                    eD7A = 2;
                    eD7B = 3;
                    break;
                case 25:
                    eD7A = 3;
                    eD7B = 3;
                    break;
                case 26:
                    eD7A = 4;
                    eD7B = 3;
                    break;
                case 27:
                    eD7A = 5;
                    eD7B = 3;
                    break;
                case 28:
                    eD7A = 6;
                    eD7B = 3;
                    break;
                case 29:
                    eD7A = 7;
                    eD7B = 3;
                    break;
                case 30:
                    eD7A = 8;
                    eD7B = 3;
                    break;
                case 31:
                    eD7A = 9;
                    eD7B = 3;
                    break;
                case 32:
                    eD7A = 10;
                    eD7B = 3;
                    break;
                case 33:
                    eD7A = 11;
                    eD7B = 3;
                    break;
                case 34:
                    eD7A = 12;
                    eD7B = 3;
                    break;
                case 35:
                    eD7A = 13;
                    eD7B = 3;
                    break;
                case 36:
                    eD7A = 14;
                    eD7B = 3;
                    break;
                case 37:
                    eD7A = 15;
                    eD7B = 3;
                    break;
                case 38:
                    eD7A = 16;
                    eD7B = 3;
                    break;
                case 39:
                    eD7A = 17;
                    eD7B = 3;
                    break;
                case 40:
                    eD7A = 0;
                    eD7B = 4;
                    break;
                case 41:
                    eD7A = 1;
                    eD7B = 4;
                    break;
                case 42:
                    eD7A = 2;
                    eD7B = 4;
                    break;
                case 43:
                    eD7A = 3;
                    eD7B = 4;
                    break;
                case 44:
                    eD7A = 4;
                    eD7B = 4;
                    break;
                case 45:
                    eD7A = 5;
                    eD7B = 4;
                    break;
                case 46:
                    eD7A = 6;
                    eD7B = 4;
                    break;
                case 47:
                    eD7A = 7;
                    eD7B = 4;
                    break;
                case 48:
                    eD7A = 8;
                    eD7B = 4;
                    break;
                case 49:
                    eD7A = 9;
                    eD7B = 4;
                    break;
                case 50:
                    eD7A = 10;
                    eD7B = 4;
                    break;
                case 51:
                    eD7A = 11;
                    eD7B = 4;
                    break;
                case 52:
                    eD7A = 12;
                    eD7B = 4;
                    break;
                case 53:
                    eD7A = 13;
                    eD7B = 4;
                    break;
                case 54:
                    eD7A = 14;
                    eD7B = 4;
                    break;
                case 55:
                    eD7A = 0;
                    eD7B = 5;
                    break;
                case 56:
                    eD7A = 1;
                    eD7B = 5;
                    break;
                case 57:
                    eD7A = 2;
                    eD7B = 5;
                    break;
                case 58:
                    eD7A = 3;
                    eD7B = 5;
                    break;
                case 59:
                    eD7A = 4;
                    eD7B = 5;
                    break;
                case 60:
                    eD7A = 0;
                    eD7B = 6;
                    break;
                case 61:
                    eD7A = 1;
                    eD7B = 6;
                    break;
                case 62:
                    eD7A = 2;
                    eD7B = 6;
                    break;
                case 63:
                    eD7A = 0;
                    eD7B = 7;
                    break;
                case 64:
                    eD7A = 1;
                    eD7B = 7;
                    break;
                case 65:
                    eD7A = 2;
                    eD7B = 7;
                    break;
                case 66:
                    eD7A = 0;
                    eD7B = 8;
                    break;
                case 67:
                    eD7A = 0;
                    eD7B = 9;
                    break;
                case 68:
                    eD7A = 0;
                    eD7B = 10;
                    break;
                case 69:
                    eD7A = 0;
                    eD7B = 11;
                    break;
                case 70:
                    eD7A = 0;
                    eD7B = 12;
                    break;
                case 71:
                    eD7A = 0;
                    eD7B = 13;
                    break;
                case 72:
                    eD7A = 0;
                    eD7B = 14;
                    break;
                case 73:
                    eD7A = 0;
                    eD7B = 15;
                    break;
                case 74:
                    eD7A = 0;
                    eD7B = 16;
                    break;
                case 75:
                    eD7A = 0;
                    eD7B = 17;
                    break;
                case 76:
                    eD7A = 0;
                    eD7B = 18;
                    break;
                default:
                    eD7A = 0;
                    eD7B = 0;
                    break;
            }

            try
            {
                WriteByte(hProc, levelSelect1AddressGlobal, eD7A);
                WriteByte(hProc, levelSelect2AddressGlobal, eD7B);
            }
            catch (Exception ex)
            {
                textBoxLog.AppendText("An error occurred setting Level Select."
                    + Environment.NewLine
                    + "Exception: "
                    + ex);
            }
        }

        #endregion

        #region Debug Flags

        public void SetDebug1Flags(object sender, EventArgs e)
        {
            BitArray bitArray = new BitArray(8);
            byte[] debug1Bytes = new byte[1];

            bitArray[0] = true ? checkBoxBit0.Checked : false;
            bitArray[1] = true ? checkBoxBit1.Checked : false;
            bitArray[2] = true ? checkBoxBit2.Checked : false;
            bitArray[3] = true ? checkBoxBit3.Checked : false;
            bitArray[4] = true ? checkBoxBit4.Checked : false;
            bitArray[5] = true ? checkBoxBit5.Checked : false;
            bitArray[6] = true ? checkBoxBit6.Checked : false;
            bitArray[7] = true ? checkBoxBit7.Checked : false;

            bitArray.CopyTo(debug1Bytes, 0);

            try
            {
                WriteByte(hProc, debug1AddressGlobal, debug1Bytes[0]);
            }
            catch (Exception ex)
            {
                textBoxLog.AppendText("An error occurred setting Debug 1."
                    + Environment.NewLine
                    + "Exception: "
                    + ex);
            }
        }

        public void SetDebug2Flags(object sender, EventArgs e)
        {
            BitArray bitArray = new BitArray(8);
            byte[] debug2Bytes = new byte[1];

            bitArray[0] = true ? checkBoxBit8.Checked : false;
            bitArray[1] = true ? checkBoxBit9.Checked : false;
            bitArray[2] = true ? checkBoxBit10.Checked : false;
            bitArray[3] = true ? checkBoxBit11.Checked : false;
            bitArray[4] = true ? checkBoxBit12.Checked : false;
            bitArray[5] = true ? checkBoxBit13.Checked : false;
            bitArray[6] = true ? checkBoxBit14.Checked : false;
            bitArray[7] = true ? checkBoxBit15.Checked : false;

            bitArray.CopyTo(debug2Bytes, 0);

            try
            {
                WriteByte(hProc, debug2AddressGlobal, debug2Bytes[0]);
            }
            catch (Exception ex)
            {
                textBoxLog.AppendText("An error occurred setting Debug 2."
                    + Environment.NewLine
                    + "Exception: "
                    + ex);
            }
        }

        #endregion

        #endregion
    }
}
