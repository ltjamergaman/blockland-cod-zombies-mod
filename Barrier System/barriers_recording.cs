package CoDZBarriersAutoPackage
{
	function serverCmdrecBarrierSet(%client,%string)
	{
		if($CoDZ::Recording[%client.getBLID()] || %string $= "off")
		{
			$CoDZ::Recording[%client.getBLID()] = false;
			messageClient(%client,'',"\c3Barrier Set \c6recording \c0disabled\c6.");
		}
		else if(!$CoDZ::Recording[%client.getBLID()] || %string $= "on")
		{
			$CoDZ::Recording[%client.getBLID()] = true;
			messageClient(%client,'',"\c3Barrier Set \c6recording \c2enabled\c6.");
			%client.schedule(1000,chatMessage,"\c6Start building using \c3CoDZ Bricks\c6.");
			%client.schedule(1500,chatMessage,"\c3Barrier Sets \c6are saved in \"\c0add-ons/Gamemode_CoD_Zombies/barrierSets/*.bar\c6\"");
			%client.schedule(2000,chatMessage,"\c6Say \c3/saveBarrierSet \c0[name] \c6to save a \c3Barrier Set \c6when you're done.");
			%client.schedule(2500,chatMessage,"\c6When choosing a name, you can substitute \"\c0@\c6\"\'s for spaces. Example \c3/saveBarrierSet \c0temp@barrier \c6-> \c3/saveBarrierSet \c0temp barrier\c6.");
		}
		
		CoDZ_deleteRecording(%client);
		CoDZ_eraseRecording(%client);
	}
	
	function serverCmdUndoBarrierSet(%client)
	{
		CoDZ_UndoBarrierSet(%client);
	}
	
	function CoDZ_UndoBarrierSet()
	{
		//if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
		//	return;
		
		
		if($CoDZ::TempBarrierCount > 0)
		{
			%i = $CoDZ::TempBarrierCount - 1;
			%base = $CoDZ::TempBarrier[%i,"Base"];
			%parts = $CoDZ::TempBarrier[%i,"Parts"];
				
				//if(%base.client != %client)
				//	continue;
				
			for(%z = 0; %z < getWordCount(%base); %z++)
			{
				if(isObject(getWord(%base,%z)))
					getWord(%base,%z).killBrick();
			}	
			for(%x = 0; %x < getWordCount(%parts); %x++)
			{
				if(isObject(getWord(%parts,%x)))
					getWord(%parts,%x).killBrick();
			}
			
			talk("Cleared \c3Barrier Set " @ $CoDZ::TempBarrierCount @ "\c6.");
			$CoDZ::TempBarrierCount--;
			$CoDZ::TempBarrier[%i,"Base"] = "";
			$CoDZ::TempBarrier[%i,"Parts"] = "";
		}
		//if($CoDZ::TempBarrierCount > 0)
		//{
		//	if(!isObject($CoDZ::TempBarrier[$CoDZ::TempBarrierCount]))
		//		return;
			
		//	%brick = $CoDZ::TempBarrier[$CoDZ::TempBarrierCount];
			
		//	%parts = %brick.parts;
			
		//	for(%i = 0; %i < getWordCount(%parts); %i++)
		//	{
		//		if(!isObject(getWord(%parts,%i)))
		//			continue;
				
		//		getWord(%parts,%i).delete();
		//	}
		//}
	}
	
	function CoDZ_PlaceBarrierSet(%client)
	{
		if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
			return;
		
		if($CoDZ::Recording[%client.getBLID(),"Loaded"] && CoDZ_CheckRecording(%client))
		{
			for(%i = 0; %i < $CoDZ::Recording[%client.getBLID(),"Brick","Count"]; %i++)
			{
				%brick = $CoDZ::Recording[%client.getBLID(),"Brick",%i];
				%plantErrorCode = %brick.plant();
				
				if(%plantErrorCode == 0)
				{
					if(!$Server::LAN)
					{
						if(%brick.getNumDownBricks())
							%brick.stackBL_ID = %brick.getDownBrick(0).stackBL_ID;
						else
						{
							if(%brick.getNumUpBricks())
								%brick.stackBL_ID = %brick.getUpBrick(0).stackBL_ID;
							else
								%brick.stackBL_ID = %client.getBLID();
						}
						if(%brick.stackBL_ID <= 0.0)
						%brick.stackBL_ID = %client.getBLID();
					}
					
					if($Server::LAN)
						%brick.trustCheckFinished();
					else
						%brick.PlantedTrustCheck();
		
					ServerPlay3D(brickPlantSound,%brick.getTransform());
					if(%i == 0)
					{	
						if($CoDZ::TempBarrierCount < 1)
							$CoDZ::TempBarrierCount = 1;
						else
							$CoDZ::TempBarrierCount++;
					}
					
					if(%brick.getDatablock().CoDZDataType $= "BarrierBase")
					{
						%brick.CoDZ_BarNum = $CoDZ::TempBarrierCount - 1;
						$CoDZ::TempBarrier[$CoDZ::TempBarrierCount - 1,"Base"] = trim($CoDZ::TempBarrier[$CoDZ::TempBarrierCount - 1,"Base"] SPC %brick);
					}
					else
						$CoDZ::TempBarrier[$CoDZ::TempBarrierCount - 1,"Parts"] = trim($CoDZ::TempBarrier[$CoDZ::TempBarrierCount - 1,"Parts"] SPC %brick);
					
					$CoDZ::TempBarrier[$CoDZ::TempBarrierCount - 1,"Part",%i,"DifferencePos"] = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"DifferencePos"];
				}
				else
				{
					if(%plantErrorCode == 1.0)
					{
						//%brick.delete();
						CoDZ_DeleteBarrierSet(%client);
						messageClient(%client,'MsgPlantError_Overlap');
					}
					else
					{
						if(%plantErrorCode == 2.0)
						{
							//%brick.delete();
							CoDZ_DeleteBarrierSet(%client);
							messageClient(%client,'MsgPlantError_Float');
						}
						else
						{
							if(%plantErrorCode == 3.0)
							{
								//%brick.delete();
								CoDZ_DeleteBarrierSet(%client);
								messageClient(%client,'MsgPlantError_Stuck');
							}
							else
							{
								if(%plantErrorCode == 4.0)
								{
									//%brick.delete();
									CoDZ_DeleteBarrierSet(%client);
									messageClient(%client,'MsgPlantError_Unstable');
								}
								else
								{
									if(%plantErrorCode == 5.0)
									{
										//%brick.delete();
										CoDZ_DeleteBarrierSet(%client);
										messageClient(%client,'MsgPlantError_Buried');
									}
									else
									{
										//%brick.delete();
										CoDZ_DeleteBarrierSet(%client);
										messageClient(%client,'MsgPlantError_Forbidden');
									}
								}
							}
						}
					}
				}
				if(getBrickCount() <= 100.0 && getRayTracerProgress() <= -1.0 && getRayTracerProgress() < 0.0 && $Server::LAN == 0.0 && doesAllowConnections())
					startRaytracer();
		
				//	%client.undoStack.push(%plantBrick TAB "PLANT");
					//use these for undo brick
				//	$CoDZ::Recording[%client.getBLID(),"Brick",$CoDZ::Recording[%client.getBLID(),"Brick","Count"] - 1] = "";
				//	$CoDZ::Recording[%client.getBLID(),"Brick","Count"] -= 1;
			}
		}
		$CoDZ::Recording[%client.getBLID(),"Loaded"] = false;
		CoDZ_EraseRecording(%client);
		//CoDZ_DeleteBarrierSet(%client);
	}
	
	function CoDZ_DeleteBarrierSet(%client)
	{
		if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
			return;
		
		if($CodZ::Recording[%client.getBLID(),"Loaded"] && CoDZ_CheckRecording(%client))
		{
			for(%i = 0; %i < $CoDZ::Recording[%client.getBLID(),"Brick","Count"]; %i++)
			{
				%brick = $CoDZ::Recording[%client.getBLID(),"Brick",%i];
				%datablock = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"Datablock"];
				%differencePos = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"DifferencePos"];
				%rotation = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"Rotation"];
				%angleID = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"AngleID"];
				%colorID = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"ColorID"];
				%colorFXID = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"ColorFXID"];
				%shapeFXID = $CoDZ::Recording[%client.getBLID(),"Brick",%i,"ShapeFXID"];
				
				//%angleID = getCorrectBrickAngleID("",%rotation);
				//%rotation = getCorrectBrickRotation("",%angleID);
				
				%brick.killBrick();
				
				(%newBrick = new fxDTSBrick()
				{
					datablock = %datablock;
					rotation = %rot;
					client = %client;
					position = %differencePos;
					isPlanted = 0;
					//colorID = %colorID;
					//colorFXID = %colorFXID;
					//shapefxid = %shapeFXID;
					//printid = "";
					//stackbl_id = %client.getBLID();
				}).angleID = %angleID;
				
				%newBrick.setTransform(%differencePos);
				%client.brickGroup.add(%newBrick);
				
				$CoDZ::Recording[%client.getBLID(),"Brick",%i] = %newBrick;
			}
		}
	}					
	
	function serverCmdLoadBarrierSet(%client,%filename)
	{
		CoDZ_loadBarrierSet(%client,%filename);
	}
	
	function CoDZ_loadBarrierSet(%client,%filename)
	{
		%filepath = $CoDZ::RecordingFolder @ replaceConcsSpaces(%filename) @ ".bar";
		
		if(!isFile(%filepath) || !isObject(%client) || %client.getClassName() !$= "GameConnection")
			return error("NO FILE OR NO CLIENT");
	
		if(!isObject(%client.player))
			return error("NO PLAYER");
	
		if(!isObject(%client.player.tempbrick))
			return error("NO TEMP BRICK");
	
		%tempBrick = %client.player.tempbrick;
		%tempDatablock = %tempbrick.getDatablock().getName();
		%tempTransform = %tempbrick.getTransform();
		%tempAngleID = %tempbrick.angleID;
		%tempRot = %tempbrick.rotation;
		%tempColorID = %tempbrick.colorID;
		%tempColorFXID = %tempbrick.colorFXID;
		%tempShapeFXID = %tempbrick.ShapeFXID;
	
		//%tempAngleID = getCorrectBrickAngleID("",%tempRot);
		//%tempRot = getCorrectBrickRotation("",%tempAngleID);
	
		if(%tempDatablock.CoDZDataType !$= "BarrierBaseAuto")
			return error("NO AUTO BARRIER");
		%file = new FileObject();
		%file.openForRead(%filepath);
	
		%i = 0;
		%x = 0;
		while(!%file.isEOF())// || %file.readLine() !$= "")
		{
			%i++;
		
			if(%i == 1) //datablock and angleid
			{
				if(%x == 0)
				{
					%recordedAngleID = %file.readLine();
					if(%recordedAngleID $= "" || !isInteger(%recordedAngleID))
					{
						messageClient(%client,'',"\c6The \c3Barrier Set \c6\"\c0" @ %filepath @ "\c6\" cannot be loaded because it was not recorded properly.");
						return error("NO ANGLE ID HAS BEEN RECORDED");
					}
				}
				
				%line = %file.readLine();
				//echo(%line);
			
				if(isObject(%line))
				{
					%datablock = %line;
					%i++;
				}
				else
					return error("NO DATABLOCK");
			}
			if(%i == 2) //difpos
			{
				%line = %file.readLine();
				//echo(%line);
			
				if(%line !$= "")
				{
					%difPos = %line;
					%i++;
				}
				else
					return error("NO DIF POS");
			}
			if(%i == 3) //colorID
			{
				%line = %file.readLine();
				//echo(%line);
				
				if(%line !$= "")
				{
					%colorID = %line;
					%i++;
				}
				else
					return error("NO COLOR ID");
			}
			if(%i == 4) //colorFXID
			{
				%line = %file.readLine();
				//echo(%line);
				
				if(%line !$= "")
				{
					%colorFXID = %line;
					%i++;
				}
				else
					return error("NO COLOR FX ID");
			}
			if(%i == 5) //shapeFXID
			{
				%line = %file.readLine();
				//echo(%line);
				
				if(%line !$= "")
				{
					%shapeFXID = %line;
					%i = 0;
					%x++;
				}
				else
					return error("NO SHAPE FX ID");
			}
		
			//%angleID = %tempAngleID;
			%basePos = getWords(%tempTransform,0,2);
			//%rot = %tempRot;
			
			//make the first brick go on the transform and rotation of the temp brick
			if(%tempBrick.getDatablock().CoDZDataType $= "BarrierBaseAuto")
			{
				//echo("1");
				if(%x == 1)
					%newPos = %basePos;
				else
					%newPos = vectorAdd(%basePos,CoDZ_CorrectPosFromAngleIDs(%tempAngleID,%recordedAngleID,%difPos));
			}
			
			//echo(%basePos);
			//echo(%difPos);
			
			if(%newPos == -1 || %x > 1 && %basePos $= %newPos)
				return error("DIFPOS ERROR");
			
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"Datablock"] = %datablock;
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"DifferencePos"] = %difPos;
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"NewPos"] = %newPos;
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"Rotation"] = %tempRot;
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"AngleID"] = %tempAngleID;
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"ColorID"] = %colorID;
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"ColorFXID"] = %colorFXID;
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"ShapeFXID"] = %shapeFXID;
		
			(%brick = new fxDTSBrick()
			{
				datablock = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"Datablock"];
				rotation = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"Rotation"];
				client = %client;
				position = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"NewPos"];
				difPosition = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"DifferencePos"];
				isPlanted = 0;
				colorID = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"ColorID"];
				colorFXID = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"ColorFXID"];
				//shapefxid = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"ShapeFXID"];
				//printid = "";
				stackbl_id = %client.getBLID();
			}).angleID = $CoDZ::Recording[%client.getBLID(),"Brick",%x - 1,"AngleID"];
			
			//%brick.setTransform(%newPos);
			
			$CoDZ::Recording[%client.getBLID(),"Brick",%x - 1] = %brick;
			$CoDZ::Recording[%client.getBLID(),"Brick","Count"] = %x;
		
			//%brick.plant();
			%client.brickgroup.add(%brick);
			//%brick.setTrusted(1);
			//missionCleanup.add(%brick);
		
			//%brick.stackBL_ID = %client.getBLID();
			//%brick.trustCheckFinished();
		
			//%brick.scopeToClient(%client);
			//%brick.clearScopeToClient(%client);			
		}
	
		$CoDZ::Recording[%client.getBLID(),"Loaded"] = true;
		%file.close();
		%file.delete();
		messageClient(%client,'',"\c6Loaded\c0" SPC %filepath);
	}
	
	function serverCmdsaveBarrierSet(%client,%string)
	{
		if($CoDZ::Recording[%client.getBLID()])
		{
			//CoDZ_loadBarrierSets();
			%check = CoDZ_CheckRecording(%client);
	
			if(!%check)
			{
				serverCmdrecBarrierSet(%client,"off");
				error("Recording cannot be saved. ERR0 Recording check came back false");
			}
			else
			{
				CoDZ_LoadRecordingData(%client);
				
				%file = new FileObject();
				%file.openForWrite($CoDZ::RecordingFolder @ replaceConcSpaces(%string) @ ".bar");
				
				for(%i = 0; %i < $CoDZ::Recording[%client.getBLID(),"Brick","Count"]; %i++)
				{
					%brick = $CoDZ::Recording[%client.getBLID(),"Brick",%i];
					if(!isObject(%brick))
					{
						serverCmdrecBarrierSet(%client,"off");
						error("Recording cannot be saved. ERR1 First brick does not exist in recording");
						%file.close();
						%file.delete();
						fileDelete($CoDZ::RecordingFolder @ replaceConcSpaces(%string) @ ".bar");
					}
					else
					{
						if(%i == 0)
							%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"AngleID"]);
						
						%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"Datablock"]);
						%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"DifferencePos"]);
						//%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"Rotation"]);
						//%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"AngleID"]);
						%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"ColorID"]);
						%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"ColorFXID"]);
						%file.writeLine($CoDZ::Recording[%client.getBLID(),"Brick",%i,"ShapeFXID"]);
					}
				}
				
				%file.close();
				%file.delete();
				messageClient(%client,'',"\c3Barrier set saved.");
				serverCmdrecBarrierSet(%client,"off");
			}
		}
		else
			return;
	}
	
	//ONLY USE THIS FOR NON-LOADED RECORDINGS
	function CoDZ_LoadRecordingData(%client)
	{
		if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
			return;
		
		if($CoDZ::Recording[%client.getBLID(),"Loaded"])
			return;
		
		if(!CoDZ_CheckRecording(%client))
			return;
		
		for(%i = 0; %i < $CoDZ::Recording[%client.getBLID(),"Brick","Count"]; %i++)
		{
			%brick = $CoDZ::Recording[%client.getBLID(),"Brick",%i];
			
			if(%i > 0)
			{
				%pos = getWords(%brick.getTransform(),0,2);
				%basePos = getWords($CoDZ::Recording[%client.getBLID(),"Brick",0].getTransform(),0,2);
				%differencePos = vectorSub(%pos,%basePos);
				//%newPos = vectorAdd(%basePos,%differencePos);
			}
			else
				%differencePos = "-1";
			
			$CoDZ::Recording[%client.getBLID(),"Brick",%i,"Datablock"] = %brick.getDatablock().getName();
			$CoDZ::Recording[%client.getBLID(),"Brick",%i,"DifferencePos"] = %differencePos;
			$CoDZ::Recording[%client.getBLID(),"Brick",%i,"Rotation"] = %brick.rotation;
			$CoDZ::Recording[%client.getBLID(),"Brick",%i,"AngleID"] = %brick.angleID;
			$CoDZ::Recording[%client.getBLID(),"Brick",%i,"ColorID"] = %brick.colorID;
			$CoDZ::Recording[%client.getBLID(),"Brick",%i,"ColorFXID"] = %brick.colorFXID;
			$CoDZ::Recording[%client.getBLID(),"Brick",%i,"ShapeFXID"] = %brick.shapeFXID;
			
			echo("Data set " @ %i @ " of " @ $CoDZ::Recording[%client.getBLID(),"Brick","Count"] - 1 @ " loaded.");
		}
		
		echo("All data sets for " @ %client.getBLID() @ "\'s recording has been loaded.");
	}
	
	function CoDZ_checkRecording(%client)
	{
		if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
			return 0;
		
		if($CoDZ::Recording[%client.getBLID(),"Brick",0] $= "" || !isObject($CoDZ::Recording[%client.getBLID(),"Brick",0]))
		{
			//CoDZ_deleteRecording(%client);
			return 0;
		}
		
		else
			return 1;
	}
	
	function CoDZ_EraseRecording(%client)
	{
		if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
			return;
		
		echo("Erasing Barrier Set recording of client " @ %client.getBLID() @ "...");
		//deleteVariables() wouldn't work with concatenation
		//the string needs to be a whole set of characters e.g. "$CoDZ::Recording*" or $CoDZ::Recording[23108*
		//not $CoDZ::Recording @ %blid*
		
		for(%i = 0; %i < $CoDZ::RecordingMaxBricks; %i++)
			$CoDZ::Recording[%client.getBLID(),"Brick",%i] = "";
		if(!$CoDZ::Recording[%client.getBLID()])
			return;
		
		$CoDZ::Recording[%client.getBLID(),"Loaded"] = false;
		$CoDZ::Recording[%client.getBLID(),"Brick","Count"] = 0;
		echo("Recording erased.");
	}
	
	function CoDZ_deleteRecording(%client)
	{
		if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
			return;
		
		echo("Deleting Barrier Set recording of client " @ %client.getBLID() @ "...");
		//deleteVariables() wouldn't work with concatenation
		//the string needs to be a whole set of characters e.g. "$CoDZ::Recording*" or $CoDZ::Recording[23108*
		//not $CoDZ::Recording @ %blid*
			
		if(CoDZ_CheckRecording(%client))
		{
			for(%i = 0; %i < $CoDZ::Recording[%client.getBLID(),"Brick","Count"]; %i++)
				$CoDZ::Recording[%client.getBLID(),"Brick",%i].delete();
		}
		
		for(%i = 0; %i < $CoDZ::RecordingMaxBricks; %i++)
			$CoDZ::Recording[%client.getBLID(),"Brick",%i] = "";
		if(!$CoDZ::Recording[%client.getBLID()])
			return;
		
		$CoDZ::Recording[%client.getBLID(),"Loaded"] = false;
		$CoDZ::Recording[%client.getBLID(),"Brick","Count"] = 0;
		echo("Recording deleted.");
	}
	
	function serverCmdChooseBar(%client,%val)
	{
		if(!isObject(%pl = %client.player))
			return;
		
		if(!isInteger(%val))
		{
			if(!isFile(%val))
				return;
			
			if(getWordCount(%val) == 1)
				%val = replaceConcSpaces(%val);
			
			%recorded = true;
			%filepath = %val;
		}
		else if(%val < 1 || %val > 8)
			return;
		else
			%recorded = false;
		
		if(%recorded)
		{
			//barrier set format
			//first brick | must be a barrier base
			//##brick datablock##
			//##colorID##
			//##nothing because it uses the transform of the tempbrick##
			//second brick | must be a barrier base
			//##brick datablock##
			//##colorID
			//##difference of transform from first brick##
			//bricks after second
			//##brick datablock##
			//##colorID##
			//##difference of transform from first brick##
			
			%file = new FileObject();
			%file.openForRead(%filepath);
			%file.close();
			%file.delete();
		}
	}
	
	function serverCmdChooseBarHelp(%client)
	{
		messageClient(%client,'',"-\c6Every default \c3Barrier Set \c6is either \c34 \c6or \c36 \c6bricks wide, \c31 \c6brick long, and \c37 \c6bricks tall.");
		messageClient(%client,'',"-\c6There are \c38 \c6default \c3Barrier Sets \c6to choose from.");
		schedule(2000,0,messageClient,%client,'',"-+\c7[\c61\c7]\c31x4x7 \c6with \c35 \c6parts. Variation \c31\c6.");
		schedule(2500,0,messageClient,%client,'',"-+\c7[\c62\c7]\c31x4x7 \c6with \c35 \c6parts. Variation \c32\c6.");
		schedule(3000,0,messageClient,%client,'',"-+\c7[\c63\c7]\c31x4x7 \c6with \c36 \c6parts. Variation \c31\c6.");
		schedule(3500,0,messageClient,%client,'',"-+\c7[\c64\c7]\c31x4x7 \c6with \c36 \c6parts. Variation \c32\c6.");
		schedule(4000,0,messageClient,%client,'',"-+\c7[\c65\c7]\c31x6x7 \c6with \c35 \c6parts. Variation \c31\c6.");
		schedule(4500,0,messageClient,%client,'',"-+\c7[\c66\c7]\c31x6x7 \c6with \c35 \c6parts. Variation \c32\c6.");
		schedule(5000,0,messageClient,%client,'',"-+\c7[\c67\c7]\c31x6x7 \c6with \c36 \c6parts. Variation \c31\c6.");
		schedule(5500,0,messageClient,%client,'',"-+\c7[\c68\c7]\c31x6x7 \c6with \c36 \c6parts. Variation \c32\c6.");
		schedule(6500,0,messageClient,%client,'',"-++\c6Additionally, you can record a \c3Barrier Set \c6by typing \c3/recBarrierSet \c0[off \c6or \c0on] (optional) \c6and planting \c3CoDZ \c6bricks.");
		schedule(7000,0,messageClient,%client,'',"-++\c6Disclaimer: You cannot build a barrier on top of, below, in front of, in back of, or on the side of another barrier.");
		schedule(7500,0,messageClient,%client,'',"-++\c6If you want to load a recorded \c3Barrier Set\c6, you will need to say \c3/chooseBar \c0[filepath]\c6.");
		schedule(8000,0,messageClient,%client,'',"-++\c6You can substitute \"\c0@\c6\"\'s for spaces when typing a filepath. Example: \c3saves/zombie\c0@\c3town\c0@\c32.bls\c6.");
		schedule(8000,0,messageClient,%client,'',"-++\c6PageUp to see all info.");
	}
};
activatePackage("CoDZBarriersAutoPackage");
