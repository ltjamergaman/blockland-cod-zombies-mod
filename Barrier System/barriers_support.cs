//CODE IS PROBABLY NOT USED SO IDK BUT ITS HERE ANYWAYS
//SO WHO GIVES A FUCK
package CoDZBarrierPosCheckPackage
{
	function CoDZ_CorrectPosFromAngleIDs(%newAngle,%oldAngle,%pos)
	{
		switch(%newAngle)
		{
			case 3:
				return CoDZ_FindNorthPos(%oldAngle,%pos);
			
			case 2:
				return CoDZ_FindWestPos(%oldAngle,%pos);
				
			case 1:
				return CoDZ_FindSouthPos(%oldAngle,%pos);
				
			case 0:
				return CoDZ_FindEastPos(%oldAngle,%pos);
		}
	}
	//3 = north (0 0 -1 1.57) transform last 4 args
	//2 = west  (0 0 1 0)
	//1 = south (0 0 1 1.57)
	//0 = east  (0 0 1 3.14)
	
	function CoDZ_FindNorthPos(%angleID,%pos)
	{
		%posX = getWord(%pos,0);
		%posY = getWord(%pos,1);
		%posZ = getWord(%pos,2);
		
		switch(%angleID)
		{
			//pos = 0.75 0 0.6
			//returns 0.75 0 0.6
			case 3:
				return %pos;
			
			//pos = 0.75 0 0.6
			//returns 0 0.75 0.6
			case 2:
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
				
			//pos = -0.75 0 0.6
			//returns 0.75 0 0.6
			//pos = 0.75 0 0.6
			//returns -0.75 0 0.6
			case 1:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
			
			//pos = 0.75 0 0.6
			//returns 0 -0.75 0.6
			//pos = -0.75 0 0.6
			//returns 0 0.75 0.6
			case 0:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
		}
		
		%pos = %posX SPC %posY SPC %posZ;
		return %pos;
	}
	
	function CoDZ_FindSouthPos(%angleID,%pos)
	{
		%posX = getWord(%pos,0);
		%posY = getWord(%pos,1);
		%posZ = getWord(%pos,2);
		
		switch(%angleID)
		{
			case 3:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
			
			case 2:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
				
			case 1:
				return %pos;
			
			case 0:
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
		}
		
		%pos = %posX SPC %posY SPC %posZ;
		return %pos;
	}
	
	function CoDZ_FindWestPos(%angleID,%pos)
	{
		%posX = getWord(%pos,0);
		%posY = getWord(%pos,1);
		%posZ = getWord(%pos,2);
		
		switch(%angleID)
		{
			case 3:
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
			
			case 2:
				return %pos;
				
			case 1:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
			
			case 0:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
		}
				
		%pos = %posX SPC %posY SPC %posZ;
		return %pos;
	}
	
	function CoDZ_FindEastPos(%angleID,%pos)
	{
		%posX = getWord(%pos,0);
		%posY = getWord(%pos,1);
		%posZ = getWord(%pos,2);
		
		switch(%angleID)
		{
			case 3:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
			
			case 2:
				if(%posX < 0)
					%posX = mAbs(%posX);
				else if(%posX > 0)
					%posX = %posX * -1;
				
			case 1:
				%newPosX = %posY;
				%posY = %posX;
				%posX = %newPosX;
			
			case 0:
				return %pos;
		}
		
		%pos = %posX SPC %posY SPC %posZ;
		return %pos;
	}
	
	//pos1 = 0 0 0
	//pos2 = 0 0.5 0
	//pos3 = 0.5 0 0
	//pos4 = 0.75 0 0
	//pos5 = 0 0.75 0
	//input1 CoDZ_BarrierCompXYVal(pos1x,pos2x)
	//pos2x + 0.5 = 1 != 0
	//pos2x - 0.5 = 0 == 0
	//pos2x != 0
	//returns 1
	//input2 CoDZ_BarrierCompXYVal(pos1y,pos2y)
	//pos2y + 0.5 = 0.5 != 0
	//pos2y - 0.5 = -0.5 != 0
	//pos2y == 0
	//returns 1
	//input3 CoDZ_BarrierCompXYVal(pos3x,pos4x)
	//pos4x + 0.5 = 1.25 != 0.5
	//pos4x - 0.5 = 0.25 != 0.5
	//pos4x != 0.5
	//returns 0
	//input4 CoDZ_BarrierCompXYVal(pos3y,pos4y)
	//pos4y + 0.5 = 0.5 != 0
	//pos4y - 0.5 = -0.5 != 0
	//pos4y == 0
	//returns 1
	//input5 CoDZ_BarrierCompXYVal(pos5x,pos1x)
	//pos1x + 0.5 = 0.5 != 0
	//pos1x - 0.5 = -0.5 != 0
	//pos1x == 0
	//returns 1
	//input6 CoDZ_BarrierCompXYVal(pos5y,pos1y)
	//pos1y + 0.5 = 0.5 != 0.75
	//pos1y - 0.5 = -0.5 != 0.75
	//pos1y != 0.75
	//returns 0
	function CoDZ_BarrierCompXYVal(%val0,%val1)
	{
		%r = 0;
		if(%val0 $= %val1)
			%r = 1;
		if(%val0 $= %val1 + 0.5)
			%r = 1;
		if(%val0 $= %val1 - 0.5)
			%r = 1;
		//if(%val0 == %val1 || %val0 == (%val1 + 0.5) || %val0 == (%val1 - 0.5))
		//	return 1;
		
		return %r;
	}
	
	function CoDZ_BarrierCompZVal(%val0,%val1)
	{
		if(%val1 > %val0)
			return 1;
		
		return 0;
	}
};
activatePackage("CoDZBarrierPosCheckPackage");

function CoDZ_FindTempClient(%obj)
{
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%id = ClientGroup.getObject(%i).player.tempBrick;
		%client = ClientGroup.getObject(%i);
		if(%id == %obj)
			return %client;
	}
	
	return 0;
}

function CoDZ_Load_AddTempBase(%brick)
{
	if(%brick.getDatablock().CoDZDataType $= "BarrierBase" && %brick.isPlanted)
	{
		if($tmpBaseCount < 1)
			$tmpBaseBrick[0] = %brick;
		else
			$tmpBaseBrick[$tmpBaseCount] = %brick;
		
		$tmpBaseCount++;
		
		warn("CoDZ Load -+ Adding base to list");
	}
}

function CoDZ_Load_CorrectTempBases()
{
	//Dont need it
	return;
	
	if(isObject(getWord($CoDZ::TempBases,0)))
	{
		%i = 0;
		while(%i < $CoDZ::TempBasesCount)
		{
			if(getWord($CoDZ::TempBases,%i) $= "")
				break;
			
			%brick = getWord($CoDZ::TempBases,%i);
			%down = %brick.getDownBrick(0);
			%up = %brick.getUpBrick(0);
			
			if(isObject(%down))
			{
				if(%down.getDatablock().CoDZDataType $= "BarrierBase")
				{
					$CoDZ::TempBases = strReplace($CoDZ::TempBases,%down,"");
					$CoDZ::TempBases = strReplace($CoDZ::TempBases,%brick,"");
					%firstBase = %down;
					%secondBase = %brick;
					//do something with %brick and %down like a temp barrier ???
				}
			}
			if(isObject(%up))
			{
				if(%up.getDatablock().CoDZDataType $= "BarrierBase")
				{
					$CoDZ::TempBases = strReplace($CoDZ::TempBases,%brick,"");
					$CoDZ::TempBases = strReplace($CoDZ::TempBases,%up,"");
					%firstBase = %brick;
					%secondBase = %up;
				}
			}
			else
				return error("Cannot correct any bases!");
			
			%i++;
			$CoDZ::TempFirstBases = trim($CoDZ::TempFirstBases SPC %firstBase);
			$CoDZ::TempSecondBases = trim($CoDZ::TempSecondBases SPC %secondBase);
		}
	}
}

function CoDZ_CreateTempBarrierFromList(%list)
{
}
