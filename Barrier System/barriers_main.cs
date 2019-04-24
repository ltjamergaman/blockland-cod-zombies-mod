$CoDZ::MaxBarriers = 50; //default, best if not changed unless by CoDZ default functionality
$CoDZ::MaxPartsPerBarrier = 6; //default, best if not changed unless by CoDZ default functionality
$CoDZ::MaxBricksPerPart = 10; //default, best if not changed unless by CoDZ default functionality
$CoDZ::MaxBarriersInZone = 10;

$CoDZ::RecordingFolder = "Add-ons/Gamemode_CoD_Zombies/barrierFiles/";

exec("./barriers_Support.cs");
exec("./barriers_Recording.cs");

//idk
if(!isObject(nameToID("CoDZ_BarrierGroup")))
{
	new SimGroup("CoDZ_BarrierGroup")
	{
		barrierCount = 0;
		//this var is for the "base brick" of each barrier
		//every barrier has to have a base brick, but if
		//there's more than one then the last one on the
		//top is the parts brick, otherwise the base brick
		//also serves as the parts brick
		barriers = "";
	};
}


//basic barrier structure
// |-| |-|   (left vertical stack is a part)
//- - - - -  (right vertical stack is a part)
// |-| |-|   (each horizontal brick [even stacked ones] is a/are part/s)
//- - - - -  (1 left vertical part)
// |-| |-|   (1 right vertical part)
//- - - - -  (4 horizontal parts)
// |-| |-|   (1 + 1 + 4 = 6 parts)
//- - - - -  (the parts and the information about the parts are stored in the parts brick)
// |-| |-|   (the amount of parts, bot occupancy, and status are stored in the base brick) 
//---------  (if there's only the base brick, it will also act as the parts brick)
//|PartsBr|  (the parts brick will include parts 0-5 and part rendering and collision 0-5)
//---------  (the base brick will include occupancy [0 - empty, 3 - full], partsCount [in this case 6], status [empty, buildable, full])
//---------  ([status] empty - no parts remain | can climb through/rebuild, buildable - some parts remain | can rebuild/takedown, full - can only takedown)
//|BaseBrk|  ([occupancy] empty - no bots occupying | no takedown/can occupy, 1-2 some bots occupying | takedown/can occupy, 3 full occupancy | takedown/cannot occupy)
//---------
package CoDZ_BarriersMainPackage
{
	function CoDZ_DeleteExcessBarriers()
	{
		%tempCount = $CoDZ::TempBarrierCount;
		%regCount = $CoDZ::RegisteredBarrierCount;
		
		if(%tempCount > $CoDZ::MaxBarriers)
		{
			%val = $CoDZ::MaxBarriers - %tempCount;
			for(%i = 0; %i < %val; %i++)
			{
				%baseList = $CoDZ::TempBarrier[%tempCount - (%i + 1),"Base"];
				%partsList = $CoDZ::TempBarrier[%tempCount - (%i + 1),"Parts"];
				
				for(%x = 0; %x < getWordCount(%baseList); %x++)
					getWord(%baseList,%x).delete();
				for(%x = 0; %x < getWordCount(%partsList); %x++)
					getWord(%partsList,%x).delete();
				
				$CoDZ::TempBarrier[%tempCount - (%i + 1),"Base"] = "";
				$CoDZ::TempBarrier[%tempCount - (%i + 1),"Parts"] = "";
			}
			
			$CoDZ::TempBarrierCount -= %val;
		}
		if(%regCount > $CoDZ::MaxBarriers && $CoDZ::BarriersAreRegistered)
		{
			%val = $CoDZ::MaxBarriers - %regCount;
			for(%i = 0; %i < %val; %i++)
			{
				%baseList = $CoDZ::RegisteredBarrier[%regCount - (%i + 1),"Base"];
				%partsList = $CoDZ::RegisteredBarrier[%regCount - (%i + 1),"Parts"];
				
				for(%x = 0; %x < getWordCount(%baseList); %x++)
					getWord(%baseList,%x).delete();
				for(%x = 0; %x < getWordCount(%partsList); %x++)
					getWord(%partsList,%x).delete();
				
				$CoDZ::RegisteredBarrier[%regCount - (%i + 1),"Base"] = "";
				$CoDZ::RegisteredBarrier[%regCount - (%i + 1),"Parts"] = "";
			}
			
			$CoDZ::RegisteredBarrierCount -= %val;
		}
		
		echo("Excess Barriers deleted.");
	}
	
	function CoDZ_CorrectTempBases()
	{
		if($tmpBaseCount $= "" || $tmpBaseCount < 1)
			return error("no temp bases");
		
		for(%i = 0; %i < $tmpBaseCount; %i++)
		{
			%brick = $tmpBaseBrick[%i];
			%data = %brick.getDatablock();
			%type = %data.CoDZDataType;
			
			%down = %brick.getDownBrick(0);
			if(!isObject(%down))
			{
				if(!hasItemOnList(%brick,%list))
					%list = trim(%list SPC %brick);
			}
			else if(%down.getDatablock().CoDZDataType !$= "BarrierBase")
			{
				if(!hasItemOnList(%brick,%list))
					%list = trim(%list SPC %brick);
			}
		}
		deleteVariables("$tmpBase*");
		return %list;
	}
	
	function CoDZ_CreateBarriersFromTemp()
	{
		%list = CoDZ_CorrectTempBases();
		if($CoDZ::TempBarrierCount $= "")
			$CoDZ::TempBarrierCount = 0;
		if(%list $= "" || getWordCount(%list) < 1)
			return error("list broken");
		
		for(%i = 0; %i < getWordCount(%list); %i++)
		{
			%brick = getWord(%list,%i);
			%data = %brick.getDatablock();
			%type = %data.CoDZDataType;
			%barList = CoDZ_SearchBaseForParts(%brick);
			%baseList = getWords(%barList,0,1);
			%partsList = getWords(%barList,2,getWordCount(%barList) - 1);
			CoDZ_SetAllPartsDifPos(getWord(%barList,0),%partsList);
			if(%barList $= "-1")
				return;
			
			getWord(%baseList,0).CoDZ_BarNum = $CoDZ::TempBarrierCount;
			getWord(%baseList,1).CoDZ_BarNum = $CoDZ::TempBarrierCount;
			$CoDZ::TempBarrier[$CoDZ::TempBarrierCount,"Base"] = %baseList;
			$CoDZ::TempBarrier[$CoDZ::TempBarrierCount,"Parts"] = %partsList;
			$CoDZ::TempBarrierCount++;
			talk("Temporary barrier created");
		}
	}
	
	function CoDZ_SearchBaseForParts(%brick)
	{
		%pos = %brick.getPosition();
		%posX = getWord(%pos,0);
		%posY = getWord(%pos,1);
		%posZ = getWord(%pos,2);
		%data = %brick.getDatablock();
		%type = %data.CoDZDataType;
		%radiusX = %data.brickSizeX / 4;
		%radiusY = %data.brickSizeY / 4;
		%radiusZ = %data.brickSizeZ / 4;
		%angleID = %brick.getAngleID();
		%newPosZ = %posZ + 2;
		//barriers can only be a max size of 7 bricks tall
		%pos = %posX SPC %posY SPC %newPosZ;
		
		switch(%angleID)
		{
			//east
			case 0:
				//sometingh
			//south
			case 1:
				%t = %radiusX;
				%radiusX = %radiusY;
				%radiusY = %t;
			//west
			case 2:
				//something
			//north
			case 3:
				%t = %radiusX;
				%radiusX = %radiusY;
				%radiusY = %t;
		}
		
		%radiusZ = 3.5; //7 bricks tall
		%radius = %radiusX SPC %radiusY SPC %radiusZ;
		%mask = $Typemasks::fxBrickObjectType;
		//containerBoxEmpty(%mask,%pos,%radiusX,%radiusY,%radiusZ);
		initContainerBoxSearch(%pos,%radius,%mask);
		%i = 0;
		while(%obj = containerSearchNext())
		{
			%objData = %obj.getDatablock();
			%objType = %objData.CoDZDataType;
			
			echo(%obj SPC %i);
			%obj.setColor(0);
			if(%objType $= "BarrierBase")
			{
				talk(%obj SPC "is a base");
				%baseList = trim(%baseList SPC %obj);
			}
			if(%objType $= "BarrierPartH" | %objType $= "BarrierPartV")
			{
				talk(%obj SPC "is a part");
				%partsList = trim(%partsList SPC %obj);
			}
			%i++;
		}
		
		%baseList = CoDZ_GetCorrectedBases(%baseList);
		talk(%baseList);
		if(%baseList $= "-1")
			return -1;
		
		%list = trim(%baseList) SPC trim(%partsList);
		talk(%list);
		return %list;
	}
	
	function CoDZ_GetCorrectedBases(%list)
	{
		talk(%list);
		if(getWordCount(%list) > 2)
			return -1;
		
		%firstBrick = getWord(%list,0);
		%secondBrick = getWord(%list,1);
		
		if(%firstBrick.getDownBrick(0) == %secondBrick)
			%list = %secondBrick SPC %firstBrick;
		
		return %list;
	}
	
	function CoDZ_SetAllPartsDifPos(%base,%partsList)
	{
		%count = getWordCount(%partsList);
		%basePos = %base.getPosition();
		for(%i = 0; %i < %count; %i++)
		{
			%brick = getWord(%partsList,%i);
			%pos = getWords(%brick.getTransform(),0,2);
			%differencePos = vectorSub(%pos,%basePos);
			%brick.difPosition = %differencePos;
		}
	}
	
	function CoDZ_barrierRebuild(%num)
	{
		if($CoDZ::BarriersAreRegisterd)
		{
			%baseCount = getWordCount($CoDZ::RegisteredBarrier[%num,"Base"]);
			%base = $CoDZ::RegisteredBarrier[%num,"Base"];
			%partsCount = $CoDZ::RegisteredBarrier[%num,"Parts","Count"];
			%parts = $CoDZ::RegisteredBarrier[%num,"Parts"];
		}
		else
		{
			%baseCount = getWordCount($CoDZ::TempBarrier[%num,"Base"]);
			%base = $CoDZ::TempBarrier[%num,"Base"];
			%partsCount = $CoDZ::TempBarrier[%num,"Parts","Count"];
			%parts = $CoDZ::TempBarrier[%num,"Parts"];
		}
		
		if(%baseCount > 1)
		{
			%baseBrick = getWord(%base,0);
			%partsBrick = getWord(%base,%baseCount - 1);
		}
			
		%occupancy = %baseBrick.barOccupancy;
		//%partsCount = %baseBrick.barPartsCount;
		%status = %baseBrick.barStatus;
			
		if(%occupancy $= "" || %partsCount $= "" || %status $= "")
			return error("Appropriate values need to be set");
			
		if(%status $= "2")
			echo("Full - cannot build part");
		if(%status $= "0" | %status $= "1")
		{
			//echo("Empty/Buildable - building");
			
			for(%i = 0; %i < %partsCount; %i++)
			{
				%part = %partsBrick.barPart[%i];
				%partStatus = %partsBrick.barPart[%i,"Status"];
					
				//only want to do this once so we break;
				if(!%partStatus)
				{
					for(%x = 0; %x < getWordCount(%part); %x++)
					{
						getWord(%part,%x).setColliding(1);
						getWord(%part,%x).setRendering(1);
					}
					%partsBrick.barPart[%i,"Status"] = true;
					break;
				}
				if(%partStatus && %i == %partsCount - 1)
					error("ERROR");
			}
		}
	}
	
	function CoDZ_barrierTakeDown(%num)
	{
		if($CoDZ::BarriersAreRegisterd)
		{
			%baseCount = getWordCount($CoDZ::RegisteredBarrier[%num,"Base"]);
			%base = $CoDZ::RegisteredBarrier[%num,"Base"];
			%partsCount = $CoDZ::RegisteredBarrier[%num,"Parts","Count"];
			%parts = $CoDZ::RegisteredBarrier[%num,"Parts"];
		}
		else
		{
			%baseCount = getWordCount($CoDZ::TempBarrier[%num,"Base"]);
			%base = $CoDZ::TempBarrier[%num,"Base"];
			%partsCount = $CoDZ::TempBarrier[%num,"Parts","Count"];
			%parts = $CoDZ::TempBarrier[%num,"Parts"];
		}
		
		if(%baseCount > 1)
		{
			%baseBrick = getWord(%base,0);
			%partsBrick = getWord(%base,%baseCount - 1);
		}
			
		%occupancy = %baseBrick.barOccupancy;
		//%partsCount = %baseBrick.barPartsCount;
		%status = %baseBrick.barStatus;
			
		if(%occupancy $= "" || %partsCount $= "" || %status $= "")
			return error("Appropriate values need to be set");
			
		if(%occupancy < 1)
			return error("No occupants on barrier!");
			
		if(%status $= "0")
			echo("Empty - cannot take down part");
		if(%status $= "1" | %status $= "2")
		{
			//echo("Buildable/full - taking part down");
			//have to use decrement because barriers are built
			//from the bottom up so vertical parts get build first
			for(%i = %partsCount - 1; %i > -1; %i--)
			{
				%part = %partsBrick.barPart[%i];					
				%partStatus = %partsBrick.barPart[%i,"Status"];
					
				//only want to do this once so we break;
				if(%partStatus)
				{
					for(%x = 0; %x < getWordCount(%part); %x++)
					{
						getWord(%part,%x).setColliding(0);
						getWord(%part,%x).setRendering(0);
					}
					
					%partsBrick.barPart[%i,"Status"] = false;
					break;
				}
				if(!%partStatus && !%i)
					error("ERROR");
			}
		}
	}
				
	function CoDZ_EraseRegisteredBarriers()
	{
		if($CoDZ::RegisteredBarrierCount > 0)
		{
			for(%i = 0; %i < $CoDZ::RegisteredBarrierCount; %i++)
			{
				$CoDZ::RegisteredBarrier[%i,"Base"] = "";
				$CoDZ::RegisteredBarrier[%i,"Parts"] = "";
				$CoDZ::RegisteredBarrier[%i] = 0;
			}
		}
		else
		{
			$CoDZ::RegisteredBarrierCount = 0;
			$CoDZ::RegisteredBarrier[%i,"Base"] = "";
			$CoDZ::RegisteredBarrier[%i,"Parts"] = "";
			$CoDZ::RegisteredBarrier[%i] = 0;
		}
		deleteVariables("$CoDZ::Reg*");
		
		messageAll('',"\c3Registered Barriers \c6erased.");
	}
	
	//USE ORIGINAL PART NOT PREV PART
	function CoDZ_RegisterBarriers()//,%id,%slot)
	{
		%count = $CoDZ::TempBarrierCount;
		
		if(%count < 1)
			return;
		
		CoDZ_EraseRegisteredBarriers();
		//$CoDZ::RegisteredBarrierCount = 0;
		
		for(%i = 0; %i < %count; %i++)
		{
			%check = CoDZ_CheckTempBarrier(%i);
			if(!%check)
			{
				$CoDZ::TempBarrier[%i,"Base"] = "";
				$CoDZ::TempBarrier[%i,"Parts"] = "";
				continue;
			}
			%partCount = 0;
			%rightSideCount = 0;
			%leftSideCount = 0;
			
			//talk("Initiate i" @ %i);
			for(%x = 0; %x < getWordCount($CoDZ::TempBarrier[%i,"Parts"]); %x++)
			{
				%brick = getWord($CoDZ::TempBarrier[%i,"Parts"],%x);
				%pos = %brick.difPosition;
				%type = %brick.getDatablock().CoDZDataType;
				if(%pos $= "")
					return error("erorr");
				
				%posX = getWord(%pos,0);
				%posY = getWord(%pos,1);
				%posZ = getWord(%pos,2);
				
				if(%type $= "BarrierPartV")
				{
					if(%posX > 0 | %posY > 0)
					{
						%right = true;
						%left = false;
					}
					else if(%posX < 0 | %posY < 0)
					{
						%right = false;
						%left = true;
					}
					
					if(%right)
					{
					//if(%partCount < 1)
					//	%partCount = 1;
					
						if(%rightSideCount < 1)
						{
							%partCount++;
							%partR = %partCount;
						}
						
						%rightSideCount++;
					
						%rightB[%rightSideCount - 1] = %brick;
					}
					else if(%left)
					{
						if(%leftSideCount < 1)
						{
							%partCount++;
							%partL = %partCount;
						}
						
						%leftSideCount++;
					
						%leftB[%leftSideCount - 1] = %brick;
					}
				}
				else if(%type $= "BarrierPartH")
				{
					%partCount++;
					if(%partCount < 1 | %partCount < 2 | %partCount < 3)
						$CoDZ::RegisteredBarrier[%i,"Part",2] = %brick;
					else if(%partCount > 2)
						$CoDZ::RegisteredBarrier[%i,"Part",%partCount - 1] = %brick;
				}
			}
			for(%x = 0; %x < %rightSideCount; %x++)
				$CoDZ::RegisteredBarrier[%i,"Part",0] = trim($CoDZ::RegisteredBarrier[%i,"Part",0] SPC %rightB[%x]);
			for(%x = 0; %x < %leftSideCount; %x++)
				$CoDZ::RegisteredBarrier[%i,"Part",1] = trim($CoDZ::RegisteredBarrier[%i,"Part",1] SPC %leftB[%x]);
			
			$CoDZ::RegisteredBarrier[%i,"Base"] = $CoDZ::TempBarrier[%i,"Base"];
			//$CoDZ::TempBarrier[%i,"Base"] = "";
			$CoDZ::RegisteredBarrier[%i,"Parts"] = $CoDZ::TempBarrier[%i,"Parts"];
			//$CoDZ::TempBarrier[%i,"Parts"] = "";
			//$CoDZ::TempBarrierCount--;
			$CoDZ::RegisteredBarrierCount++;
			talk("barrier registered");
			$CoDZ::RegisteredBarrier[$CoDZ::RegisteredBarrierCount - 1,"Parts","Count"] = CoDZ_GetBarrierPartsCount($CoDZ::RegisteredBarrierCount - 1);
			CoDZ_SetBarrierInfo($CoDZ::RegisteredBarrierCount - 1);
		}
	}
	
	function CoDZ_GetBarrierPartsCount(%val)
	{
		if(!isObject(getWord($CoDZ::RegisteredBarrier[%val,"Base"],0)))
			return;
		
		for(%i = 0; $CoDZ::RegisteredBarrier[%val,"Part",%i] $= ""; %i++)
		{
			//nothing
		}
		
		return %i;
	}
	
	///asfoifagong
	//asfasoifnasnfui5g5ui4ig
	//FINISH THIS FUCKING FUNCTION YOU FAT FUCK
	function CoDZ_SetBarrierInfo(%val)
	{
		if(!isObject(getWord($CoDZ::RegisteredBarrier[%val,"Base"],0)))
			return;
		
		%baseCount = getWordCount($CoDZ::RegisteredBarrier[%val,"Base"]);
		if(%baseCount > 2)
			return;
		%partsCount = getWordCount($CoDZ::RegisteredBarrier[%val,"Parts","Count"]);
	}
	
	function CoDZ_ClearTempBarriers()
	{
		if($CoDZ::TempBarrierCount > 0)
		{
			for(%i = 0; %i < $CoDZ::TempBarrierCount; %i++)
			{
				%baseCount = getWordCount($CoDZ::TempBarrier[%i,"Base"]);
				for(%x = 0; %x < %baseCount; %x++)
					getWord($CoDZ::TempBarrier[%i,"Base"],%x).delete();
				%partCount = getWordCount($CoDZ::TempBarrier[%i,"Parts"]);
				for(%x = 0; %x < %partCount; %x++)
					getWord($CoDZ::TempBarrier[%i,"Parts"],%x).delete();
				$CoDZ::TempBarrier[%i,"Base"] = "";
				$CoDZ::TempBarrier[%i,"Parts"] = "";
				$CoDZ::TempBarrier[%i] = 0;
			}
		}
		else
		{
			$CoDZ::TempBarrierCount = 0;
			$CoDZ::TempBarrier[%i,"Base"] = "";
			$CoDZ::TempBarrier[%i,"Parts"] = "";
			$CoDZ::TempBarrier[%i] = 0;
		}
		
		deleteVariables("$CoDZ::Temp*");
		
		messageAll('',"\c3Temporary Barriers \c6erased.");
	}
	
	function CoDZ_CheckTempBarrier(%num)
	{
		if($CoDZ::TempBarrierCount > 0 && isInteger(%num) && %num < $CoDZ::TempBarrierCount && %num >= 0)
		{
			%base = $CoDZ::TempBarrier[%num,"Base"];
			%parts = $CoDZ::TempBarrier[%num,"Parts"];
			
			for(%i = 0; %i < getWordCount(%base); %i++)
			{
				if(!isObject(getWord(%base,%i)))
					%r = 0;
				else
					%r = 1;
			}
			
			for(%i = 0; %i < getWordCount(%parts); %i++)
			{
				if(!isObject(getWord(%parts,%i)))
					%r = trim(%r SPC 0);
				else
					%r = trim(%r SPC 1);
			}
		}
		else
			%r = 0;
		
		if(getWord(%r,0) == 0 || getWord(%r,1) == 0)
			%r = 0;
		
		return %r;
	}
	
	
	
	//gets the amount of 'parts' in a registered barrier
	function CoDZ_getBarrierPartsCount(%grpNum)
	{
		for(%i = 1; $CoDZ::Barrier[%grpNum,%i] !$= ""; %i++)
		{
			//nothing
		}
		
		//talk("Barrier" SPC %grpNum SPC "Parts" SPC %i - 1);
		return %i - 1;
	}
	
	function CoDZ_deleteBarriers()
	{
		deleteVariables("$CoDZ::Bar*");
		$CoDZ::BarriersAreRegistered = false;
	}
};
activatePackage("CoDZ_BarriersMainPackage");