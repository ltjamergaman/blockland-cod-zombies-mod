function CoDZ_MakeRandomColor()
{
	%charList = "0123456789ABCDEF";
	%charListLen = strLen(%charList);
	%i = 0;
	
	while(%i < 6)
	{
		%char[%i] = getSubStr(%charList,getRandom(0,%charListLen - 1),1);
		%color = trim(%color @ %char[%i]);
		%i++;
	}
	
	return %color;
}

function CoDZ_PickRandomColor()
{
	%color = getWord($CoDZ::Colors,getRandom(0,$CoDZ::ColorCount - 1));
	return %color;
}

function CoDZ_CheckColor(%color)
{
	if(!isObject(getCoDZGame()))
	{
		error("");
		CoDZ_RefreshColors();
		return 0;
	}
	
	%check = strPos($CoDZ::UsedColors,%color);
	if(%check != -1)
		return 1;
	else
		return 0;
}

function CoDZ_RefreshColors()
{
	if($CoDZ::GameState $= "Playing")
		return error("Cannot refresh colors because the game is currently in progress...");
	$CoDZ::UsedColors = "";
	deleteVariables("$CoDZ::Color[Rando]");
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
		$CoDZ::Player[ClientGroup.getObject(%i).getBLID(),"Color"] = "";
}

package CoDZ_MiniGameSupport_PlayerSearch
{
	function Player::CoDZ_StartSearch(%this)
	{
		cancel($CoDZ::Player[%this.client.getBLID(),"SearchLoop"]);
		%time = 250; //time to tick
		%this.CoDZ_LoopSearch(%time);
	}
	
	function Player::CoDZ_LoopSearch(%this,%time)
	{
		cancel($CoDZ::Player[%this.client.getBLID(),"SearchLoop"]);
		$CoDZ::Player[%this.client.getBLID(),"ActiveActor"] = %this.CoDZ_Search();
		%this.schedule(%time,"CoDZ_LoopSearch",%time);
	}
	
	function Player::CoDZ_Search(%this)
	{
		
		if(!isObject(%this))
			return;
		
		%position = getWords(%this.getTransform(),0,2); //Get his position
		%radius = 1.25; //Specify a radius to search
		%mask = $Typemasks::fxBrickObjectType; //Use a typemask
		initContainerRadiusSearch(%position,%radius,%mask);
		
		while(%obj = containerSearchNext())
		{
			if(%obj $= "" | !isObject(%obj))
				return;
			if(%obj.getDatablock().CoDZDataType $= "BarrierBase")
			{
				if(isObject(%partsBrick = %obj.partsBrick))
					return %partsBrick;
			}
			if(%obj.getDatablock().CoDZDataType $= "Perk")
				return %obj;
			if(%obj.getDatablock().CoDZDataType $= "Door")
				return %obj;
			if(%obj.getDatablock().CoDZDataType $= "Trap")
				return %obj;
			if(%obj.getDatablock().CoDZDataType $= "Teleporter")
				return %obj;
			if(%obj.getDatablock().CoDZDataType $= "MysteryBox")
				return %obj;
		}
	}
};
activatePackage("CoDZ_MiniGameSupport_PlayerSearch");

function GameConnection::CoDZ_UpdateHUD(%this)
{
	cancel($CoDZ::Player[%this.getBLID(),"HUDLoop"]);
	if(isObject(getCoDZGame()))
	{
		%playerCount = getCoDZGame().numMembers;
		%time = 1.1;
		
		if(%playerCount == 1)
		{
			%player = getCoDZGame().member[0];
			%playerColor = $CoDZ::Player[%player.getBLID(),"Color"];
			%playerScore = $CoDZ::Player[%player.getBLID(),"Score"];
			%msg = "<just:right><font:impact:22><color:" @ $CoDZ::Color[%playerColor] @ ">" @ %playerScore;
		}
		else if(%playerCount > 1)
		{
			for(%i = 0; %i < %playerCount; %i++)
			{
				%player = getCoDZGame().member[%i];
				%playerColor = $CoDZ::Player[%player.getBLID(),"Color"];
				%playerScore = $CoDZ::Player[%player.getBLID(),"Score"];
				
				if(%msg $= "")
					%msg = "<just:right><font:impact:22><color:" @ $CoDZ::Color[%playerColor] @ ">" @ %playerScore;
				else
					%msg = trim(%msg @ "<br><color:" @ $CoDZ::Color[%playerColor] @ ">" @ %playerScore);
			}
		}
		
		if(%msg $= "")
		{
			%msg = "<font:impact:40>NO CURRENT PLAYERS";
			%time = 0.5;
		}
		
		%this.centerPrint(%msg,%time);
			
		if(!getCoDZGame().isMember(%this))
			%this.bottomPrint("<font:impact:26>SPECTATING",2,1);
		
		$CoDZ::Player[%this.getBLID(),"HUDLoop"] = %this.schedule(1000,"CoDZ_UpdateHUD");
	}
	else
		warn("GameConnection::CoDZ_UpdateHUD -+ There is no minigame");
}

function CoDZ_clearVariables()
	{
		deleteVariables("$CoDZ*");
	}
	
	function Player::togSprint(%player,%oR)
	{
		//%oR = override
		if(%player.sprinting == true || %oR == false && %oR !$= "")
		{
			%player.sprinting = false;
			if(%player.getDatablock() == PlayerCoDZStaminUp.getID() && %player.client.CoDZPerk["StaminUp"] || %player.getDatablock() == PlayerCoDZSUMK.getID() && %player.client.CoDZPerk["StaminUp"])
				%player.setMaxForwardSpeed($CoDZ::Datablock::Player::MinFS + 2);
			else
				%player.setMaxForwardSpeed($CoDZ::Datablock::Player::MinFS);
		}
		else if(%player.sprinting == false || %oR == true && %oR !$= "")
		{
			if(%player.isCrouched())
			{
				%player.sprinting = false;
				if(%player.getDatablock() == PlayerCoDZStaminUp.getID() && %player.client.CoDZPerk["StaminUp"] || %player.getDatablock() == PlayerCoDZSUMK.getID() && %player.client.CoDZPerk["StaminUp"])
					%player.setMaxForwardSpeed($CoDZ::Datablock::Player::MinFS + 2);
				else
					%player.setMaxForwardSpeed($CoDZ::Datablock::Player::MinFS);
				
				return;
			}

			%player.sprinting = true;
			if(%player.getDatablock() == PlayerCoDZStaminUp.getID() && %player.client.CoDZPerk["StaminUp"] || %player.getDatablock() == PlayerCoDZSUMK.getID() && %player.client.CoDZPerk["StaminUp"])
				%player.setMaxForwardSpeed($CoDZ::Datablock::Player::MaxFS + 2);
			else
				%player.setMaxForwardSpeed($CoDZ::Datablock::Player::MaxFS);
		}
	}
	
	function Player::CoDZ_regenSprintEnergy(%this)
	{
		cancel(%this.CoDZ_SprintRegenTick);
		cancel(%this.CoDZ_SprintDegenTick);
		if(%this.sprinting)
			return;
		//%energy = %this.sprintEnergy;
		if(%this.sprintEnergy < %this.getDatablock().sprintEnergy)
			%this.sprintEnergy += 2.5;
		if(%this.sprintEnergy >= %this.getDatablock().sprintEnergy)
			return;
		%this.CoDZ_SprintRegenTick = %this.schedule(500,"CoDZ_regenSprintEnergy");
	}
	
	function Player::CoDZ_degenSprintEnergy(%this)
	{
		cancel(%this.CoDZ_SprintDegenTick);
		cancel(%this.CoDZ_SprintRegenTick);
		if(!%this.sprinting)
			return;
		//%energy = %this.sprintEnergy;
		if(%this.sprintEnergy > 0)// && (%energy-2.5) > 0)
			%this.sprintEnergy -= 2.5;
		if(%this.sprintEnergy < 2.5)
			return %this.togSprint(0);
		%this.CoDZ_SprintDegenTick = %this.schedule(500,"CoDZ_degenSprintEnergy");
	}
	
	function GameConnection::showSprintStuff(%this)
	{
		cancel(%this.CoDZ_sprintstufftick);
		%this.bottomPrint(%this.player.sprintEnergy SPC %this.player.getDatablock().sprintEnergy,1,1);
		%this.CoDZ_sprintstufftick = %this.schedule(100,"showSprintStuff");
	}