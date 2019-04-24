package CoDZ_AI
{
	function AIPlayer::onBotTakeDownBarrier(%this,%grpNum)
	{
		%this.oldPos = %this.getTransform();
		%this.clearAim();
		%this.hCrouch();
		%this.setMoveY(5);
		%this.hJump();
		if(%this.isThroughBarrier)
		{
			%this.schedule(500,"setBotPowered",1);
			%this.schedule(500,"setSearchRadius","AlwaysFindPlayer",128);
		}
		else
		{
			if($CoDZ::Barrier[%grpNum,"Occupied","Count"] >= 3)
			{
				switch($CoDZ::Barrier[%grpNum,0].angleID)
				{
					case 3: //north xyz x = back y = right
					%this.setVelocity("10 0 0");
					
					case 2: //west x = left y = back
					%this.setVelocity("0 10 -");
					
					case 1: //south x = forwards y = left
					%this.setVelocity("-10 0 0");
					
					case 0: //east x = right y = forwards
					%this.setVelocity("0 -10 0");
				}
			}
			else
			{
				%this.setTransform(%this.oldPos);
				%this.TakeDownBarrier(%grpNum);
			}
		}
		//return error("This function does nothing. RETURNED - " @ %this SPC %grpNum);
	}
	
	//function AIPlayer::TakeDownBarrierb(%this,%grpNum)
	//{
	//	cancel(%this.CoDZ_BTSched);
	//	if($CoDZ::BarriersAreRegistered == true)
	//	{
	//		if($CoDZ::Barrier[%grpNum] !$= "")
	//		{
	//			%totalcount = getWordCount($CoDZ::Barrier[%grpNum]);
	//			%totalPartsCount = $CoDZ::Barrier[%grpNum,"PartsCount"];
	//			for(%i = %totalPartsCount; %i > 0; %i--)
	//			{
	//				%partsCount[%i] = getWordCount($CoDZ::Barrier[%grpNum,%i]);
	//				for(%j = 0; %j < %partsCount[%i]; %j++)
	//				{
	//					%obj = getWord($CoDZ::Barrier[%grpNum,%i],%j);
	//					if(%obj.isColliding() == true && %obj.isRendering() == true)//|| %obj.CoDZ_enabled == true)
	//					{
	//						CoDZ_setBrickProperties($CoDZ::Barrier[%grpNum,%i],0,1);
	//						%this.activateStuff();
	//					}
	//					if(getWord($CoDZ::Barrier[%grpNum,1],0).isColliding() == false && getWord($CoDZ::Barrier[%grpNum,1],0).isRendering() == false)
	//					{
	//						%this.clearAim();
	//						%this.setMoveY(3);
	//						%this.hCrouch();
	//						%this.hJump();
	//						%this.schedule(500,"setBotPowered",1);
	//						%this.schedule(500,"setSearchRadius","AlwaysFindPlayer",128);
	//						return;
	//					}
	//				}
	//			}
	//		}
	//	}
	//	%this.CoDZ_BTSched = %this.schedule(2000,"TakeDownBarrier",%grpNum);
	//}
	
	function AIPlayer::TakeDownBarrier(%this,%grpNum,%check)
	{
		cancel(%this.CoDZ_Sched);
		if($CoDZ::BarriersAreRegistered == true)// && isObject(nameToID("CoDZMiniGame")) == true)
		{
			if($CoDZ::Barrier[%grpNum] !$= "")
			{
				//if(getWord($CoDZ::Barrier[%grpNum,1],0).isRendering() == false && getWord($CoDZ::Barrier[%grpNum,1],0).isColliding() == false || $CoDZ::Barrier[%grpNum,"TakenDown"] == true)
				//{
				//	if(isObject(%this.getAimObject()))
				//	{
				//		$CoDZ::Barrier[%grpNum,"TakenDown"] = true;
				//		%this.clearAim();
				//		%this.hCrouch();
				//		%this.setMoveY(5);
				//		%this.hJump();
				//		%this.schedule(500,"setBotPowered",1);
				//		%this.schedule(500,"setSearchRadius","AlwaysFindPlayer",128);
				//		return;
				//	}
				//}
				$CoDZ::Barrier[%grpNum,"Occupied","Count"] += 1;
				%totalParts = CoDZ_getBarrierPartsCount(%grpNum);
				%totalBricks = getWordCount($CoDZ::Barrier[%grpNum]);
				for(%i = %totalParts; %i > 0; %i--)
				{
					//talk(%i);
					//talk(%prtWordCnt);
					%prtWordCnt = getWordCount($CoDZ::Barrier[%grpNum,%i]);
					if(%i == 2)
						%continue = true;
					if(%check == true && %i == 2)
					{
						%continue = false;
						%i = %i - 1;
						%prtWordCnt = getWordCount($CoDZ::Barrier[%grpNum,%i]);
					}
					if(%prtWordCnt > 1 && %prtWordCnt <= $CoDZ::MaxBricksPerPart)
					{
						if(getWord($CoDZ::Barrier[%grpNum,%i],0).isRendering() == true && getWord($CoDZ::Barrier[%grpNum,%i],0).isColliding() == true)// || getWord($CoDZ::Barrier[%grpNum,%i],0).CoDZ_enabled == true)
						{
							//talk(getWordCount(%obj.part[%i]));
							CoDZ_setBrickProperties($CoDZ::Barrier[%grpNum,%i],0,1);
							%this.activateStuff(); //Make them do activation thread so it looks like they're taking it down
							$CoDZ::Barrier[%grpNum,"TakenDown"] = true;
							if(%i == 1)
							{
								//%this.clearAim();
								//%this.hCrouch();
								//%this.setMoveY(5);
								//%this.hJump();
								//%this.schedule(500,"setBotPowered",1);
								//%this.schedule(500,"setSearchRadius","AlwaysFindPlayer",128);
								//return;
								return %this.schedule(1250,"onBotTakeDownBarrier");
							}
							break;
						}
						//break;
					}
					else if(%prtWordCnt == 1)
					{
						if($CoDZ::Barrier[%grpNum,%i].isRendering() == true && $CoDZ::Barrier[%grpNum,%i].isColliding() == true || $CoDZ::Barrier[%grpNum,%i].CoDZ_enabled == true)
						{
							%part = $CoDZ::Barrier[%grpNum,%i];
							CoDZ_setBrickProperties(%part,0,1);
							%this.activateStuff(); //Make them do activation thread so it looks like they're taking it down
							$CoDZ::Barrier[%grpNum,"TakenDown"] = true;
							if(%i == 1)
							{
								//%this.clearAim();
								//%this.hCrouch();
								//%this.setMoveY(5);
								//%this.hJump();
								//%this.schedule(500,"setBotPowered",1);
								//%this.schedule(500,"setSearchRadius","AlwaysFindPlayer",128);
								//return;
								return %this.schedule(1250,"onBotTakeDownBarrier");
							}
							break;
						}
					}
				}
			}
		}
		if(%continue == true)
			%this.CoDZ_Sched = %this.schedule(2000,"TakeDownBarrier",%grpNum,1);
		else
			%this.CoDZ_Sched = %this.schedule(2000,"TakeDownBarrier",%grpNum,0);
	}
	
	function AIPlayer::setPathToBarrier(%obj,%bar)
	{
		if(isObject(getCoDZGame()))
		{
			if(!$CoDZ::BarriersAreRegistered)
			{
				%obj.delete();
				return;
			}
				
			if(CoDZ_CheckBarrier(%bar) == 0)
			{
				%obj.GoToBrick(%name = "CoDZ_TDB" @ %bar);
				%obj.setWanderDistance("Off",8);
				%obj.setSearchRadius("Off",0);
				%obj.setIdleBehavior("Off");
				talk("AIPlayer::setPathToBarrier - check 3");
				//%obj.setBotPowered(0);
				
				//$CoDZ::Barrier[%bar,"Occupied","Count"] += 1;
				
				if($CoDZ::Barrier[%bar,"Occupied","Count"] > 5 || !$CoDZ::Barrier[%bar,"Occupied"])
					$CoDZ::Barrier[%bar,"Occupied"] = true;
			}
			else
			{
				talk("AIPlayer::setPathToBarrier - else check");
				if(!$CoDZ::ZonesCreated)
				{
					%obj.delete();
					return;
				}
					
				%count = $CoDZ::ZonesCount;
				for(%i = 0; %i < %count; %i++)
				{
					if($CoDZ::Zone[%i].isOccupied)
					{
						%barsInZone = $CoDZ::Zone[%i,"Barriers"];
						for(%j = 0; %j < getWordCount(%barsInZone); %j++)
						{
							continue;
						}
					}
				}
			}
		}
	}
	
	
	//zombie spawning optimizing algorithm
	//1: check last path node for barrier info
	//2: check barrier occupancy
	//2a: if occupancy is < 3, spawn new zombie
	//2aA: get pos of first zombie at barrier or get pos of path node + 0.1 to z axis
	//2b: if occupancy is >= 3, spawn  new zombie, but don't move it
	//3: if 2b, then move zombie when occupancy is < 3
	//4: if 2a & if occupancy < 2, move zombie to right when behind other zombie
	//5: if 2a & occupancy > 1, move zombie to left when behind other zombies
	
	//function fxDTSBrick::SpawnZombie(%this)
	//{
		//if(!%this.hasPath)
		//	return error("No path!");
		
		//if(%this.pathNode[%this.pathNodeCount-1].barrier
	
	function CoDZMiniGame::onZombieSpawn(%this,%botObj,%check,%brick)
	{
		//if(!$CoDZ::BarriersAreRegistered)
		//	return %botObj.delete();
		
		%this.addZombie(%botObj);
		
		if(%check)
		{
			if(isObject(%brick))
			{
				//if(%brick.CoDZ_Enabled)
				//{
					%grpNum = %brick.CoDZ_GrpNum;
					%botObj.barrier = %grpNum;
					%botObj.setPathToBarrier(%botObj.barrier);
					talk("CoDZMiniGame::onZombieSpawn - check 2");
				//}
			}
		}
	}
};
activatePackage("CoDZ_AI");