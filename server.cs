//((((((((((((((((((((((((()))))))))))))))))))))))))\\
//                                                  \\
//------------ A Zombie Survival Add-on ------------\\
//                        By                        \\
//             Lt. Jamergaman/tkepahama             \\
//                    BLID 23108                    \\
//--------------------------------------------------\\
//((((((((((((((((((((((((()))))))))))))))))))))))))\\
	
//exec("./AI_Funcs.cs");
//exec("./minigame.cs");
exec("./Barrier System/barriers_main.cs");
exec("./minigame_main.cs");
exec("./items.cs");
exec("./bricks.cs");
exec("./players.cs");
exec("./packages.cs");
exec("./resources.cs");
exec("./zones_main.cs");

function exCoDZ()
{
	exec("./server.cs");
}

//$botspawn exists
function CoDZ_createBot(%name,%spawnNum)
{
	if(!$CoDZ::Debug)
		return;
	//if(!isObject(nameToID("CoDZMiniGame")) || !nameToID("CoDZMiniGame").isDebugging && !nameToID("CoDZMiniGame").GameStarted)
	//	return error("Cannot spawn bot - (MINIGAME DOES NOT EXIST, DEBUGGING IS DISABLED BEFORE GAME START, OR THE GAME JUST HASN'T STARTED");

	//Creates the necessary spawn brick. Otherwise bot kablooey-s.
	%brick = nameToID(%name);
	if(!isObject(%brick))
		return error("There is no spawn brick! Place a bot spawn area brick first. DO NOT USE A BOT HOLE SPAWN BRICK");

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
	
	%isoccupied = CoDZ_checkBarrier(%spawnNum);
	if(%isoccupied)
	{
		%bot.delete();
		return;
	}
	
	%this.onZombieSpawn(%bot,1,%target);
	talk("CoDZMiniGame::spawnZombies - check 1");
}
function CoDZ_spawnBots(%amount,%spawnNum)
{
	//%spawnNum = brick spawn #
	if(%spawnNum > 50 || %spawnNum < 1 || %spawnNum $= "")// || isInteger(%spawnNum) == false)
		return error(%spawnNum @ "Inefficient amount of bots");
	
	%name = "_CoDZ_botSpawn" @ %spawnNum;
	//Does it %amount amount of times.
	for(%i = 0; %i < %amount; %i++)
		schedule(1000,0,"CoDZ_createBot",%name,%spawnNum);
}

//pointserimental stuff, trying to fix the problem.
//package xXhacksXx
//{
//	function hBrickClientCheck(%brickgroup)
//	{
		//talk("cl check");
//		return 1;
//	}
//};
//activatePackage(xXhacksXx);

package blah2
{
	function CoDZ_SpawnItem(%item,%pos,%rot,%rotCheck,%obj)
	{
		%newitem = new item()
		{
			datablock = %item.getID();
			canPickup = true;
			scale = "1 1 1";
			position = %pos;
			
			collideable = false;
			rotate = %rotCheck; //If true, the item will rotate around in clockwise circles.
			rotation = %rot;
			scale = "1 1 1";
			static = true; //If true, the item will float in the air and won't be affected by gravity or velocity.

			//bl_id = %client.getBLID();
			//minigame = %client.minigame;
			spawnBrick = -1;
		};
		talk(%newItem.getID());
		//%newitem.setVelocity("0 0 0");
		//%newitem.schedule(15000,"delete");
	}
};
activatePackage(blah2);