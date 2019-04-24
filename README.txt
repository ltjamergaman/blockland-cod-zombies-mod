{| This add-on utilizes a replication of the zombies gamemode from Call of Duty |}

-:- What To Know
------------------------------
You need to make/load a build that works well with the game's default bot pathfinding.
After you have already done that, make sure to make barriers with these specified bricks:

- 2 1x6 bricks or 2 1x4 bricks
- 6 1x1 bricks
- 3 1x6f plates or 2 1x4f plates
- 2 1x1f plates

-:- Configuration of Barriers (1x6)
------------------------------
||| BUILDING - How to make simple barrier
------------------------------

Place:

- 2 1x6 bricks on top of each other

  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

Place:

- 2 1x1 bricks on the top 1x6 brick on the second-to-last-outside studs

     ---      ---
     | |      | |
     ---      ---
  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

Place:

- 1 1x6f plate on top of the 2 1x1 bricks

  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

Place:

- 2 1x1 bricks on top of the 1x6f plate on the second-to-last-outside studs

     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

Place:

- 1 1x6f plate on top of the 2 1x1 bricks

  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

Place:

- 2 1x1 bricks on top of the 1x6f plate on the second-to-last-outside studs

     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

Place:

- 1 1x6f plate on top of the 2 1x1 bricks

  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

Place:

- 2 1x1f plates on top of the 1x6f plate on the second-to-last-outside studs

     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
     ---      ---
     | |      | |
     ---      ---
  ------------------
  |                |
  ------------------
  ------------------
  |                |
  ------------------

----------------------------------------------------------------------------------------------------
1x4 configuration is just replacing the 1x6 bricks and 1x6f plates with 1x4 bricks and 1x4f plates
and using the outside studs for the rest of the parts for the barrier.
----------------------------------------------------------------------------------------------------

-------------------------------
||| NAMING
-------------------------------

- Name the top 1x6f plate to "MZ_bar1_prt1" without the quotations

- Name the middle 1x6f plate to "MZ_bar1_prt2" without the quotations

- Name the bottom 1x6f plate to "MZ_bar1_prt3" without the quotations

- Name the left set of 1x1 bricks and the left 1x1f plate to "MZ_bar1_prt4" without the quotations

- Name the right set of 1x1 bricks and the right 1x1f plate to "MZ_bar1_prt5" without the quotations

=============================
       (DISCLAIMER)
=============================

When you are naming the bricks, do not name every other brick in that barrier group with "MZ_bar1-", make
sure to keep count of each barrier you make. After the first barrier is complete, increment the barrier
number. (e.g. "MZ_bar2_prt1" or "MZ_bar3_prt1" without the quotations)

-------------------------------
||| EVENTING
-------------------------------

When you are finished building your barrier, put a 2x2f or 2x4f plate behind the barrier where the zombie
comes spawns from. Put the TakeDownBarrier event on it as demonstrated below.

{0} - OnBotTouch -> Bot -> TakeDownBarrier -> [(the barrier number)]

------------------------------------------------
-:- Creating The MiniGame
------------------------------------------------

There is an interface included with the gamemode add-on.

If you have not already set up the keybind for the interface, please go into Options and go to Controls,
then scroll down until you see "UZ Interface" without the quotations and double-click on the item that says
"Open/Close". Then press the key(s) you want keybinded to the control so you can open and close the interface.

Once that is done, open the interface and click on "Director" then click the button that says "Search for Bricks".
It will probably lag for a bit and/or lag a lot. Please make sure you have already gone through the
steps above before doing this. It will pop up a message saying "Search Complete" and will then say
how many bricks are registered in the text item "Registered Bricks" on the interface.

After you've done that, set the rest of the settings accordingly and then start the minigame by clicking
the button labled "Start Game".

You're all ready to go, have fun!
