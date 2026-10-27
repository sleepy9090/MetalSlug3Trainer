# A game trainer for Metal Slug 3 (Gog Version, setup_metal_slug_3_gog-5_(12274))

<img width="1368" height="800" alt="image" src="https://github.com/user-attachments/assets/368f913f-cf5f-46b7-9a2a-9b1058bc59ed" />

<img width="1368" height="800" alt="image" src="https://github.com/user-attachments/assets/230d9f74-c6b7-46ff-acb1-308aa2564783" />

## Misc notes dump on reversing and offsets

Metal Slug 3
setup_metal_slug_3_gog-5_(12274)

Everything initializes when the player appears on screen.

Level Timer 10A4A6A9
hex, byte
151d = 97 displayed
Base Address             Offset
"mslug3.exe"+000FEC38    D78 <-- ED79
"mslug3.exe"+000FEC3C    D78 <-- ED79

10A4A6A9 <-- actual
10A3C6A8 <-- offset points to
E001     <-- difference
E001 + D78 = ED79 <-- correct offset

P1 Bomb Count 10A4A424
byte
Base Address             Offset
"mslug3.exe"+000FEC38    EAF4
"mslug3.exe"+000FEC3C    EAF4

10A4A6A9
10A4A424
285
ED79 - 285 = EAF4

P2 Bomb Count
byte
Base Address             Offset
"mslug3.exe"+000FEC38    EB02
"mslug3.exe"+000FEC3C    EB02

Continue Timer 10A3BB29
byte
Base Address             Offset
"mslug3.exe"+000FEC38    1F9
"mslug3.exe"+000FEC3C    1F9

10A4A424
10A3BB29
E8FB
EAF4 - E8FB = 1F9

P1 Lives 10A3BBEA
byte, screen updates after losing a life
Base Address             Offset
"mslug3.exe"+000FEC38    2BA
"mslug3.exe"+000FEC3C    2BA

10A3BBEA
10A3BB29
C1
1F9 + C1 = 2BA

P2 Lives
byte, screen updates after losing a life
Base Address             Offset
"mslug3.exe"+000FEC38    36A
"mslug3.exe"+000FEC3C    36A

10A751F2 (p2 lives)
10A75142 (p1 lives)
B0

P1 Status 10A3BEBE
byte
Base Address             Offset
"mslug3.exe"+000FEC38    58E
"mslug3.exe"+000FEC3C    58E
0 = Normal
1 = Machine Gun
2 = Fat 
3 = Mummy
4 = Scuba Gear Normal
5 = Scuba Gear Machine Gun
6 = Snowman
7 = Zombie
8 = Flying (final stage)
9 = Flying (final stage)
10 = Spaceship (final stage)
11 = 
12 + Game Crash

Other statuses (dropping guns, helmets, water splash, etc)
"mslug3.exe"+000FEC38 58D
"mslug3.exe"+000FEC38 58F


10A3BEBE
10A3BBEA
2D4
2D4 + 2BA = 58E

P2 Status
byte
Base Address             Offset
"mslug3.exe"+000FEC38    63E
"mslug3.exe"+000FEC3C    63E
0 = Normal
1 = Machine Gun
2 = Fat Transition to Normal
3 = Mummy
4 = Scuba Gear Normal
5 = Scuba Gear Machine Gun
6 = Snowman
7 = Zombie
8 = Flying (final stage)
9 = Flying (final stage)
10 = Spaceship (final stage)
11 = 
12 + Game Crash




P1 POWs Rescued 0EF36C6C
byte
Base Address             Offset
"mslug3.exe"+000FEC38    ED04
"mslug3.exe"+000FEC3C    ED04

0EF36CE1 (level timer)
0EF36C6C (POWs)
75
ED79 (level timer offset) - 75 = ED04

P2 POWs Rescued
byte
Base Address             Offset
"mslug3.exe"+000FEC38    ED0C
"mslug3.exe"+000FEC3C    ED0C

P1 Ammo 0EF36A62
2 bytes, 0-65535(65535 = infinite)
Base Address             Offset
"mslug3.exe"+000FEC38    EAFA
"mslug3.exe"+000FEC3C    EAFA

0EF36C6C (POWs)
0EF36A62 (Ammo)
20A
ED04 - 20A = EAFA

P2 Ammo
2 bytes, 0-65535(65535 = infinite)
Base Address             Offset
"mslug3.exe"+000FEC38    EB08
"mslug3.exe"+000FEC3C    EB08

P1 Weapon Type 0EF36A5A
byte
(0 - 15):
0 = Hand Gun
1 = Cannon
2 = Shotgun
3 = Rocket Launcher
4 = Flame Shot
5 = Heavy Machine Gun
6 = Laser Gun
7 = Super Shotgun
8 = Super Rocket Launcher
9 = Super Flame Shot
10 = Super Heavy Machine Gun
11 = Super Laser Gun
12 = Enemy Chaser
13 = Iron Lizard
14 = Dropshot
15 = Super Cannon

Base Address             Offset
"mslug3.exe"+000FEC38    EAF2
"mslug3.exe"+000FEC3C    EAF2

0EF36A5C (Bomb Count)
0EF36A5A (weapon type)
2
EAF4 - 2 = EAF2

P1 Score
byte, hex
example score: 34127856
0EF36C80 = 12h
0EF36C81 = 34h
0EF36C82 = 56h
0EF36C83 = 78h

Base Address             Offset
"mslug3.exe"+000FEC38    ED18
"mslug3.exe"+000FEC38    ED19
"mslug3.exe"+000FEC38    ED1A
"mslug3.exe"+000FEC38    ED1B

"mslug3.exe"+000FEC3C    ED18
"mslug3.exe"+000FEC3C    ED19
"mslug3.exe"+000FEC3C    ED1A
"mslug3.exe"+000FEC3C    ED1B

P2 Score
byte, hex
example score: 34127856
0EF36C80 = 12h
0EF36C81 = 34h
0EF36C82 = 56h
0EF36C83 = 78h

Base Address             Offset
"mslug3.exe"+000FEC38    ED20
"mslug3.exe"+000FEC38    ED21
"mslug3.exe"+000FEC38    ED22
"mslug3.exe"+000FEC38    ED23

"mslug3.exe"+000FEC3C    ED20
"mslug3.exe"+000FEC3C    ED21
"mslug3.exe"+000FEC3C    ED22
"mslug3.exe"+000FEC3C    ED23

P1 Credits
Base Address             Offset
"mslug3.exe"+000FEC38    1003C
"mslug3.exe"+000FEC3C    1003C

0EF37FA4 (Credits)
0EF36CE1 (Level Timer)
12C3
ED79 + 12C3 = 1003C

P2 Credits
Base Address             Offset
"mslug3.exe"+000FEC38    1003D
"mslug3.exe"+000FEC3C    1003D


P1 Vehicle Health 0F525408
Base Address             Offset
"mslug3.exe"+000FEC38    EC30
"mslug3.exe"+000FEC3C    EC30


P1 Vehicle Missile Count 0F525410
Base Address             Offset
"mslug3.exe"+000FEC38    EC38
"mslug3.exe"+000FEC3C    EC38
0F525410
0F5252D2
13E
EAFA

P1 Invincibility 0EE40498
Set to zero so the P1 character indicator stops else it will not go away, then set to non-zero, since it fluctuates a little, 30 to 255 works fine, just set to 255, its a countdown timer
How to handle this between levels?
Base Address             Offset
"mslug3.exe"+000FEC38    580
"mslug3.exe"+000FEC3C    580

0EE40498 (invincible)
0EE401D2 (lives)
2C6
2BA + 2C6 = 580

P2 Invincibility 0EE40498
Set to zero so the P1 character indicator stops else it will not go away, then set to non-zero, since it fluctuates a little, 30 to 255 works fine, just set to 255, its a countdown timer
How to handle this between levels?
Base Address             Offset
"mslug3.exe"+000FEC38    630
"mslug3.exe"+000FEC3C    630

10A754B8 (p2 inv)
10A75408 (p1 inv)
B0
2BA + 2C6 = 580

P1 0EE4046E
Enemies don't attack at 140d, blinking at 0-64
ignore this one...
Base Address             Offset
"mslug3.exe"+000FEC38    2A4E6
"mslug3.exe"+000FEC3C    2A4E6






P2 Vehicle Missile Count
Base Address             Offset
"mslug3.exe"+000FEC38    EC64
"mslug3.exe"+000FEC3C    EC64


P2 Vehicle Health
Base Address             Offset
"mslug3.exe"+000FEC38    EC5C
"mslug3.exe"+000FEC3C    EC5C



0EC93560
0EC935FF




BASE_ADDRESS_1                     = 0x000FEC38;
BASE_ADDRESS_2                     = 0x000FEC3C;

OFFSET_CONTINUE_TIMER              = 0x01F9;
OFFSET_P1_LIVES_COUNT              = 0x02BA;
OFFSET_P2_LIVES_COUNT              = 0x036A;
OFFSET_P1_INVINCIBILITY_TIMER      = 0x0580;
OFFSET_P2_INVINCIBILITY_TIMER      = 0x0630;
OFFSET_P1_STATUS                   = 0x058E;
OFFSET_P2_STATUS                   = 0x063E;

OFFSET_P1_WEAPON_TYPE              = 0xEAF2;
OFFSET_P1_BOMB_COUNT               = 0xEAF4;
OFFSET_P1_AMMO_COUNT               = 0xEAFA-0xEAFB;
OFFSET_P2_WEAPON_TYPE              = 0xEB00;
OFFSET_P2_BOMB_COUNT               = 0xEB02;
OFFSET_P2_AMMO_COUNT               = 0xEB08-0xEB09;
OFFSET_P1_VEHICLE_HEALTH_COUNT     = 0xEC30;
OFFSET_P1_VEHICLE_AMMO_CANON_COUNT = 0xEC38;
OFFSET_P2_VEHICLE_HEALTH_COUNT     = 0xEC5C;
OFFSET_P2_VEHICLE_AMMO_CANON_COUNT = 0xEC64;
OFFSET_P1_POWS_RESCUED             = 0xED04;
OFFSET_P2_POWS_RESCUED             = 0xED0C;
OFFSET_P1_SCORE                    = 0xED18;
OFFSET_P2_SCORE                    = 0xED20;
OFFSET_LEVEL_TIMER                 = 0xED79;

OFFSET_P1_CREDITS_COUNT            = 0x0001003C;
OFFSET_P2_CREDITS_COUNT            = 0x0001003D;




Level Select
ED7A  ED7B  Results
0     0     Stage 1 1-1 Couples Love Land
1     0             2-1 Marine Diver
2     0             1-1 Return from Marine Diver
3     0             3-1 The Ship Asia
4     0             1-1 Return from the Ship Asia
5     0             1-1 DEBUG: Before the Boat
6     0             2-1 DEBUG: Marine Diver Midpoint

0     1     Stage 2 1-1 The Midnight Wandering
1     1             2-1 Death Valley
2     1             1-1 Return from the Valley
3     1             3-1 Devil's Snow Cave
4     1             1-1 Return from the Snow Cave
5     1             DEBUG: Mounting/Turnaround Point in Snow Cave
6     1             DEBUG: Helicopter Regiment
7     1             DEBUG: Boss 2

0     2     Stage 3 1-1 Eyes over the Tides
1     2             1-2 The Blue Sea
2     2             1-3 Secret Factory
3     2             1-4 Meet the Boss
4     2             2-1 Undersea Cave
5     2             2-2 As Hard As Expected
6     2             3-1 Great Jumping Ostriches


0     3     Stage 4 1-1 Desert Loop
1     3             1-2 Climbing the Pyramid
2     3             1-3 Meet the Boss (Via Falling)
3     3             2-1 Carpet Shop
4     3             2-2 Wine Storage and Father Inside
5     3             2-3 Japanese Soldiers Entrance
6     3             2-4 Japanese Soldiers
7     3             3-1 Underground
8     3             3-1 Underground Midpoint
9     3             3-1 Underground Midpoint 2
10    3             4-1 Maneater Den
11    3             4-2 Ruins Corridor
12    3             4-3 Small Room
13    3             4-4 Quicksand
14    3             4-5 Suspended Ceiling
15    3             4-6 Ruins Elevator
16    3             1-3 Meet the Boss (Via Climbing)
17    3             4-7 Underground Warehouse (Intro)

0     4     Stage 5 1-0 Into the Sky
1     4             1-1 The Deep Blue Sea of Clouds
2     4             1-2 The Morden Army's Space Base
3     4             1-3 Stratosphere
4     4             1-4 The Cosmos
5     4             1-5 Enemy Mothership Shaft
6     4             1-6 Mothership Hallway
7     4             1-7 The Prison
8     4             1-8 Power Reactor Hallway
9     4             1-9 Fake Last Boss
10    4             1-A Bio-sector
11    4             1-B Clone Room
12    4             1-C Airlock Hallway
13    4             1-D Escape!!
14    4             1-D Rootmars Battle

0     5     MISSION ALL OVER
1     5     On the Sea 1
2     5     On the Sea 2
3     5     Under the Sea (Credits)
4     5     Game Over (Best Tank Busters)

0     6     Demo 1-1
1     6     Demo 1-2
2     6     Demo 1 End

0     7     Demo 2-1
1     7     Demo 2-2
2     7     Demo 2 End

0     8     How to Play

0     9     Test Stage - Andy the Wizard
0    10     Test Stage - Taguchi
0    11     Test Stage - Nishino
0    12     Test Stage - Fujisawa Kankoku
0    13     Test Stage - Tyler Yamamoto
0    14     Test Stage - Mee Her
0    15     Test Stage - Arita Secret Factory
0    16     Test Stage - Midnight Directions
0    17     Test Stage - It's barely there
0    18     Test Stage - Anmira






Debug
F000
Binary
Bit 0 = 1  ???
Bit 1 = 2  VIEW PA Point (sprite showing respawn point)
Bit 2 = 4  Museigen Grenade (Infinite Grenades)
Bit 3 = 8  Museigen POW Gun (Infinite Ammo)
Bit 4 = 16 ???
Bit 5 = 32 ???
Bit 6 = 64 ???
Bit 7 = 128 VIEW PUSH RECT (View objects that block the players path)

Debug
F001
Binary
Bit 0 = 1  VIEW INFORMATION (View debug information)
Bit 1 = 2  ???
Bit 2 = 4  ???
Bit 3 = 8  VIEW GEO SCROLL B (Background)
Bit 4 = 16 VIEW GEO SCROLL A (Walkable ground)
Bit 5 = 32 VIEW ATTACK RECT (View attack hitbox)
Bit 6 = 64 VIEW BODY RECT (View body hitbox)
Bit 7 = 128 Muteki (Invincibility)

??? = PAUSE ALL THE TIME (Allows pause during score tally, cutscenes, and credits)
??? = D button, full attack (Press D to destroy all onscreen enemies)
??? = "PAUSE" display OFF (Disables the blinking paise message)
??? = BGM OFF (Turns off the BGM)
??? = WARN Palette Over






