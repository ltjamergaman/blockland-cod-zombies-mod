$CoDZ::MaxBarriers = 50; //default, best if not changed unless by CoDZ default functionality
$CoDZ::MaxPartsPerBarrier = 6; //default, best if not changed unless by CoDZ default functionality
$CoDZ::MaxBricksPerPart = 10; //default, best if not changed unless by CoDZ default functionality

$CoDZ::CountDownTime = 30;
$CoDZ::Points["Barrier"] = 15;
$CoDZ::Points["Hit"] = 15;
$CoDZ::Points["Kill"] = 105;
$CoDZ::Color["Orange"] = "EEAA55"; //orange
$CoDZ::Color["Green"] = "55EEAA"; //green
$CoDZ::Color["White"] = "EEEEEE"; //white
$CoDZ::Color["Blue"] = "55AAEE"; //blue
$CoDZ::Color["Red"] = "BB5555"; //red
$CoDZ::Color["Yellow"] = "CCCC55"; //yellow
$CoDZ::Color["Gray"] = "AAAAAA"; //gray
$CoDZ::Color["Purple"] = "AAAAEE"; //purple
$CoDZ::ColorCount = 9;
$CoDZ::Colors = "Orange Green White Blue Red Yellow Gray Purple Rando";
$CoDZ::MaxMembers = 4; //default, best if not changed unless by CoDZ default functionality
$CoDZ::MaxBarriersInZone = 10;

$CoDZ::Perk[1] = "Quick Revive"; 		 //easy
$CoDZ::Perk[2] = "Double Tap Root Beer"; //easy
$CoDZ::Perk[3] = "Jugger-Nog"; 			 //easy
$CoDZ::Perk[4] = "Speed Cola";			 //not easy
$CoDZ::Perk[5] = "Mule Kick";			 //easy
$CoDZ::Perk[6] = "Stamin-Up";			 //easy
$CoDZ::Perk[7] = "Electric Cherry";		 //not easy
$CoDZ::Perk[8] = "Tombstone Soda";		 //easy
$CoDZ::Perk[9] = "Who's Who";			 //kinda easy
$CoDZ::Perk[10] = "Vulture-Aid";		 //don't know
$CoDZ::Perk[11] = "Widow's Wine";		 //don't know

$CoDZ::GameStatus[0] = "Not Active";
$CoDZ::GameStatus[1] = "Active";
$CoDZ::GameStatus[2] = "Intermission";
$CoDZ::GameStatus[3] = "Starting";
$CoDZ::GameStatus[4] = "Playing";
$CoDZ::GameStatus[5] = "Ending";
$CoDZ::GameStatus[6] = "Paused";
$CoDZ::GameStatus[7] = "Debugging";
if($CoDZ::GameStatus $= "")
	$CoDZ::GameStatus = "Not Active";

exec("./minigame_support.cs");

function CoDZ_Game_SetStatus(%status)
{
	if(%status $= "Not Active" | %status $= "Active" | %status $= "Intermission" | %status $= "Starting" | %status $= "Playing"
		| %status $= "Ending" | %status $= "Paused" | %status $= "Debugging")
	{
		if($CoDZ::GameStatus $= %status)
		{
			echo("CoDZ Minigame -+ Game Status is already set to " @ %status);
			return;
		}
		
		$CoDZ::GameStatus = %status;
	}
	else if(isInteger(%status) && %status >= 0 && %status < 8)
	{
		if($CoDZ::GameStatus $= $CoDZ::GameStatus[%status])
		{
			echo("CoDZ Minigame -+ Game Status is already set to " @ $CoDZ::GameStatus[%status]);
			return;
		}
		
		$CoDZ::GameStatus = $CoDZ::GameStatus[%status];
	}
	else if(%status $= "" || %status !$= "")
	{
		error("Incorrect Status Input");
		warn("Function Call \'CoDZ_Game_SetStatus(input)\' -+ usage (val or string)");
		warn("Function Call \'CoDZ_Game_SetStatus(input\' -+ val (0 - 7) :: string (Not Active, Active, etc.)");
		return;
	}
	
	warn("CoDZ Minigame -+ Game Status set to " @ $CoDZ::GameStatus);
	talk("Game status set to \c3" @ $CoDZ::GameStatus @ "\c6.");
}

package	CoDZ_MiniGame
{
	function CoDZMiniGame::setPlayerColor(%this,%client,%color)
	{
		if(!isObject(%client) || %client.getClassName() !$= "GameConnection" || !%this.isMember(%client))
			return error("");
		
		if(%color !$= "")
		{
			if(!isInteger(%color))
			{
				for(%i = 0; %i < $CoDZ::ColorCount - 1; %i++)
				{
					if(getWord($CoDZ::Colors,%i) $= %color)
					{
						if(CoDZ_CheckColor(%color))
							return error("CoDZ_CheckColor -+ " @ %color @ " is in use");
					}
				}
			}
		}
		else
		{
			%color = CoDZ_PickRandomColor();
		
			while(CoDZ_CheckColor(%color) && getWordCount($CoDZ::UsedColors) < 9)
			{
				//warn("Color in use");
				%color = CoDZ_PickRandomColor();
			}
		}
		
		if(%color $= "Rando")
			$CoDZ::Color[%color] = CodZ_MakeRandomColor();
		
		$CoDZ::Player[%client.getBLID(),"Color"] = %color;
		%client.player.setShapeNameColor(HexToRGB($CoDZ::Color[%color]));
		$CoDZ::UsedColors = trim($CoDZ::UsedColors SPC %color);
		talk("\c3" @ %client.getPlayerName() @ " \c6is now playing as <color:" @ $CoDZ::Color[%color] @ ">" @ %color @ "\c6.");
	}
	
	function serverCmdCoDZ_SendPlayerInfo(%client,%val)
	{
		//echo("Sending...");
		if(!isObject(getCoDZGame()))
			return error("err");
		
		if(%val > getCoDZGame().numMembers)
			return error("err");
		
		%player = getCoDZGame().member[%val];
		%name = %player.getPlayerName();
		%score = $CoDZ::Player[%player.getBLID(),"Score"];
		%kills = $CoDZ::Player[%player.getBLID(),"Kills"];
		
		%info = %name TAB %score TAB %kills;
		
		commandToClient(%client,'CoDZ_ReceivePlayerInfo',%info);
		//echo("sent...");
	}

	function miniGameCanUse(%obja,%objb)
	{
		//if(isObject(nameToID("CoDZMinigame")) == true)
		//	return 1;
		//else
			return isObject(getCoDZGame());
	}
	
	function miniGameCanDamage(%obja,%objb)
	{
		//if(isObject(nameToID("CoDZMinigame")) == true)
		//	return 1;
		//else
			return isObject(getCoDZGame());
	}
	
	function CoDZMiniGame::spawnZombies(%this,%amount,%spawnNum)
	{
		if(!$CoDZ::Debug)
			return;
		//if(%spawnNum > 50 || %spawnNum < 1 || %spawnNum $= "" || !$CoDZ::SpawnZombies || !%this.gameStarted)// || isInteger(%spawnNum) == false)
		//	return;
	
		%name = "_CoDZ_botSpawn" @ %spawnNum;
		%target = nameToID("_CoDZ_barrier" @ %spawnNum @ "_Base");
		%areas = %this.getOccupiedAreas();
	
		//Does it %amount amount of times.
		for(%i = 0; %i < %amount; %i++)
		{
			//schedule(3000,0,"CoDZ_createBot",%name,%spawnNum);
			//Creates the necessary spawn brick. Otherwise bot kablooey-s.
			%brick = nameToID(%name);
			%brick.isBotHole = true;
			if(isObject(%brick) == false)
				return error();

			//Creating the bot itself.
			%bot = new AiPlayer()
			{
				spawnTime = $Sim::Time;
				spawnBrick = %brick;
				dataBlock = ZombieHoleBot;
				position = getWord(%brick.getPosition(),0) SPC getWord(%brick.getPosition(),1) SPC 1;

				//Springtime for Hitler
				Name = ZombieHoleBot.hName;
				hType = ZombieHoleBot.hType;
				hSearchRadius = ZombieHoleBot.hSearchRadius;
				hSearch = ZombieHoleBot.hSearch;
				hSight = ZombieHoleBot.hSight;
				hWander = ZombieHoleBot.hWander;
				hGridWander = ZombieHoleBot.hGridWander;
				hReturnToSpawn = ZombieHoleBot.hReturnToSpawn;
				hSpawnDist = ZombieHoleBot.hSpawnDist;
				hMelee = ZombieHoleBot.hMelee;
				hAttackDamage = ZombieHoleBot.hAttackDamage;
				hSpazJump = ZombieHoleBot.hSpazJump;
				hSearchFOV = ZombieHoleBot.hSearchFOV;
				hFOVRadius = ZombieHoleBot.hFOVRadius;
				hTooCloseRange = ZombieHoleBot.hTooCloseRange;
				hAvoidCloseRange = ZombieHoleBot.hAvoidCloseRange;
				hShoot = ZombieHoleBot.hShoot;
				hMaxShootRange = ZombieHoleBot.hMaxShootRange;
				hStrafe = ZombieHoleBot.hStrafe;
				hAlertOtherBots = ZombieHoleBot.hAlertOtherBots;
				hIdleAnimation = ZombieHoleBot.hIdleAnimation;
				hSpasticLook = ZombieHoleBot.hSpasticLook;
				hAvoidObstacles = ZombieHoleBot.hAvoidObstacles;
				hIdleLookAtOthers = ZombieHoleBot.hIdleLookAtOthers;
				hIdleSpam = ZombieHoleBot.hIdleSpam;
				hAFKOmeter = ZombieHoleBot.hAFKOmeter + getRandom( 0, 2 );
				hHearing = ZombieHoleBot.hHearing;
				hIdle = ZombieHoleBot.hIdle;
				hSmoothWander = ZombieHoleBot.hSmoothWander;
				hEmote = ZombieHoleBot.hEmote;
				hSuperStacker = ZombieHoleBot.hSuperStacker;
				hNeutralAttackChance = ZombieHoleBot.hNeutralAttackChance;
				hFOVRange = ZombieHoleBot.hFOVRange;
				hMoveSlowdown = ZombieHoleBot.hMoveSlowdown;
				hMaxMoveSpeed = 1.0;
				hActivateDirection = ZombieHoleBot.hActivateDirection;
				isHoleBot = 1;
			};
			missionCleanup.add(%bot);
			//onCoDZ_ZombieSpawn(%bot);
			//%isoccupied = CoDZ_checkBarrier(%spawnNum);
			//if(%isoccupied)
			//{
			//	%bot.delete();
			//	return;
			//}
		
			%this.onZombieSpawn(%bot,1,%target);
			talk("CoDZMiniGame::spawnZombies - check 1");
		}
	}
};
activatePackage("CoDZ_MiniGame");

function getCoDZGame()
{
	if(isObject(%obj = nameToID("CoDZMiniGame")))
	{
		%obj.owner = getCoDZOwner();
		if(%obj.owner == -1)
		{
			createCoDZOwner();
			%obj.owner = getCoDZOwner();
		}
		
		return %obj;
	}
	else
		return -1;
}

function createCoDZGame()
{
	if(isObject(%obj = nameToID("CoDZMiniGame")))
	{
		%obj.owner = getCoDZOwner();
		if(%obj.owner == -1)
		{
			createCoDZOwner();
			%obj.owner = getCoDZOwner();
		}
		
		return %obj;
	}
	
	%owner = getCoDZOwner();  //Owner object of the minigame object
	if(%owner == -1)
	{
		createCoDZOwner();
		%owner = getCoDZOwner();
	}
	
	%obj = new ScriptObject("CoDZMiniGame")
	{
		class = "MiniGameSO";			  //Class
		isCoDZMinigame = true;		      //CoDZ Minigame check
		title = "Zombies";     			  //Name
		owner = %owner;                   //Client Object
		
		botDamage = true;                 //bots can be damaged or not
		botRespawnTime = 5000;            //bot respawn time (MS)
		brickDamage = false;              //bricks can be damaged or not
		brickRespawnTime = 30000;         //brick respawn time (MS)
		colorIdx = 0;                     //minigame color id
		downedTimer = 30000;              //amount of time (in ms) before a downed player dies
		enableBuilding = false;           //the ability of a player to build or not
		enablePainting = false;           //the ability of a player to paint or not
		enableWand = false;               //the ability of a player to use wand or not
		fallingDamage = false;            //player can be damaged from falling or not
		incBots = 10;                     //amount of bots to increase for the next round
		inviteOnly = true;                //the ability of a minigame to be joinable or not
		maxBegBots = 10;                  //max amount of beginning bots
		maxBots = 10000;                  //max amount of bots to be used
		maxRenderedBots = 50;             //max amount of bots rendered at once
		numMembers = 0;                   //amount of members
		playerDataBlock = PlayerCoDZDefault;     //minigame overall player datablock to be used
		playersUseOwnBricks = false;      //the ability of a minigame to differentiate brickgroups
		points_BreakBrick = 0;            //amount of points to give for breaking bricks
		points_Die = 300;                 //amount of points to deduct for dying
		points_HitBot = 15; 			  //amount of points to give for hitting a bot
		points_KillBot = 100;             //amount of points to give for killing a bot
		points_KillPlayer = 0;            //amount of points to give for killing a player
		points_KillSelf = 300;            //amount of points to deduct for killing yourself
		points_PlantBrick = 0;            //amount of points to deduct for planting a brick
		points_buildBarrier = 15;         //amount of points to give for building a barrier (per part)
		resetSchedule = 0;                //minigame reset schedule
		respawnTime = 1;                  //overall player respawn time (MS)
		round = 0;                        //Starting round
		selfDamage = true;                //the ability of a player to damage themself
		startBall = 0;                    //starting sports ball (0 for none)
		startEquip0 = HammerItem.getID(); //1st ItemData that players in the minigame are equipped with by default
		startEquip1 = WrenchItem.getID(); //2nd ItemData
		startEquip2 = PrintGun.getID();   //3rd ItemData
		startEquip3 = GunItem.getID();    //4th ItemData
		startEquip4 = -1;                 //5th ItemData
		timeLimit = 0;                    //time limit
		timeLimitSchedule = 0;            //ScheduleID for the time limiter. This changes every time a message of how much time remaining is sent.
		useAllPlayersBricks = true;       //the ability of a minigame to use everyone's bricks
		useSpawnBricks = false;           //the ability of a minigame to use spawn bricks for spawning
		vehicleDamage = true;             //vehicles can be damaged or not
		vehicleReSpawnTime = -1;          //vehicle respawn time (MS)
		weaponDamage = true;              //weapons can damage or not
	};
	
	%owner.miniGame = %obj;
	miniGameGroup.add(%obj);
	%obj.maxMembers = $CoDZ::MaxMembers;
	
	//%count = clientGroup.getCount();
	
	//for(%i = 0; %i < %count; %i++)
	//{
	//	%cl = clientGroup.getObject(%i);
		
	//	if(isObject(%cl.miniGame))
	//	{
	//		if(%obj.numMembers == %obj.maxMembers)
	//		{
	//			break;
	//			return;
	//		}
	//		if(%cl.miniGame !$= %obj)
	//		{
	//			%cl.miniGame.endGame();
	//			%cl.miniGame.delete();
	//		}
	//		else
	//		{
	//			continue;
	//		}
	//	}
		
	//	%obj.addMember(%cl);
	//}
	
	$DefaultMinigame = %obj;
	echo("\c2Call of Duty Zombies Minigame created.");
	return %obj;
}

function getCoDZOwner()
{
	if(!isObject(%obj = nameToID("CoDZMiniGame")))
	{
		return -1;
	}
	if(isObject(%obj2 = nameToID("CoDZOwner")))
	{
		return %obj2;
	}
	else
		return -1;
}

function createCoDZOwner()
{
	if(!isObject(%obj = nameToID("CoDZMiniGame")))
	{
		return;
	}
	if(isObject(%obj2 = nameToID("CoDZOwner")))
	{
		return %obj2;
	}
	
	%brickGroup = getCoDZBrickGroup();
	
	%obj3 = new gameConnection("CoDZOwner")
	{
		name = "Server";
		bl_id = 50;
		brickGroup = %brickGroup;
		isVirtualOwner = true;
	};
	
	%brickGroup.client = %obj3;
	return %obj3;
}

function getCoDZBrickGroup()
{
	if(isObject(%obj = nameToID("CoDZBrickGroup")))
	{
		return %obj;
	}
}

function createCoDZBrickGroup()
{
	if(isObject(%obj2 = nameToID("CoDZBrickGrou")))
	{
		return %obj2;
	}
	
	%obj = new simGroup("CoDZBrickGroup")
	{
		name = "Server";
		bl_id = 50;
		isVirtualBrickGroup = true;
	};
	
	mainBrickGroup.add(%obj);
	
	return %obj;
}

//zombie spawning algorithm??? idk
//%a = 5;
//for(%i = 1; %i < 41; %i++)
//{
	//if(%i != 1)
	//{
		//if(%i < 11)
		//{
			//%a = mceil(%a*1.15);
		//}
		//else
		//{
			//%a = mceil(%a*1.075);
		//}
	//}
	//echo(%i @ ":" SPC %a);
//}