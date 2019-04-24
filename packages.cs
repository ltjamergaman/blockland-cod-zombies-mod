exec("./packages/WeaponImage_onFire.cs");

function ServerLoadSaveFile_Tick()
{
	if(isObject(ServerConnection))
	{
		if(!ServerConnection.isLocal())
			return;
	}
	%line = $Server_LoadFileObj.readLine();
	if (trim(%line) $= "")
	{
		return;
	}
	
	%firstWord = getWord(%line,0);
	
	if(%firstWord $= "+-EVENT")
	{
		if (isObject($LastLoadedBrick))
		{
			%idx = getField(%line, 1);
			%enabled = getField(%line, 2);
			%inputName = getField(%line, 3);
			%delay = getField(%line, 4);
			%targetName = getField(%line, 5);
			%NT = getField(%line, 6);
			%outputName = getField(%line, 7);
			%par1 = getField(%line, 8);
			%par2 = getField(%line, 9);
			%par3 = getField(%line, 10);
			%par4 = getField(%line, 11);
			%inputEventIdx = inputEvent_GetInputEventIdx(%inputName);
			%targetIdx = inputEvent_GetTargetIndex("fxDTSBrick",%inputEventIdx,%targetName);
			
			if(%targetName == -1)
				%targetClass = "fxDTSBrick";
			else
			{
				%field = getField($InputEvent_TargetList["fxDTSBrick",%inputEventIdx],%targetIdx);
				%targetClass = getWord(%field,1);
			}
			
			%outputEventIdx = outputEvent_GetOutputEventIdx(%targetClass,%outputName);
			%NTNameIdx = -1;
			
			while($LoadingBricks_Client == $LoadingBricks_BrickGroup)
			{
				%j = 0;
				if(%j < 4)
				{
					%field = getField($OutputEvent_parameterList[%targetClass,%outputEventIdx], %j);
					%dataType = getWord(%field, 0);
					
					if(%dataType $= "datablock")
					{
						if(%par[%j + 1.0] != -1.0 && !isObject(%par[%j + 1.0]))
						{
							warn("WARNING: could not find datablock for event " @ %outputName @ " -> " @ %par[%j + 1.0]);
						}
					}
					%j = %j + 1.0;
				}
			}
			"$LoadingBricks_Client".wrenchBrick = $LastLoadedBrick;
			serverCmdAddEvent($LoadingBricks_Client, %enabled, %inputEventIdx, %delay, %targetIdx, %NTNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4);
			"$LastLoadedBrick".eventNT[$LastLoadedBrick.numEvents - 1.0] = %NT;
		}
	}
	else
	{
		if(%firstWord $= "+-NTOBJECTNAME")
		{
			if(isObject($LastLoadedBrick))
			{
				%name = getWord(%line,1);
				$LastLoadedBrick.setNTObjectName(%name);
			}
		}
		else
		{
			if(%firstWord $= "+-LIGHT")
			{
				if(isObject($LastLoadedBrick))
				{
					%line = getSubStr(%line,8,strlen(%line) - 8);
					%pos = strpos(%line,"");
					%dbName = getSubStr(%line,0,%pos);
					%db = $uiNameTable_Lights[%dbName];
					
					if($LoadingBricks_Client == $LoadingBricks_BrickGroup)
					{
						if(!isObject(%db))
							warn("WARNING: could not find light datablock for uiname " @ %dbName @ "");
					}
					if(!isObject(%db))
						%db = $uiNameTable_Lights["Player\'s Light"];
					if(strlen(%line) - %pos - 2 >= 0)
					{
						%line = getSubStr(%line,%pos + 2,strlen(%line) - %pos - 2);
						%enabled = %line;
						
						if(%enabled $= "")
							%enabled = 1;
					}
					else
						%enabled = 1;
					
					%quotaObject = getQuotaObjectFromBrick($LastLoadedBrick);
					setCurrentQuotaObject(%quotaObject);
					$LastLoadedBrick.setLight(%db);
					
					if(isObject($LastLoadedBrick.light))
						$LastLoadedBrick.light.setEnable(%enabled);

					clearCurrentQuotaObject();
				}
			}
			else
			{
				if(%firstWord $= "+-EMITTER")
				{
					if(isObject($LastLoadedBrick))
					{
						%line = getSubStr(%line,10,strlen(%line) - 10);
						%pos = strpos(%line,"");
						%dbName = getSubStr(%line,0,%pos);
						
						if(%dbName $= "NONE")
							%db = 0;
						else
							%db = $uiNameTable_Emitters[%dbName];
						if($LoadingBricks_Client == $LoadingBricks_BrickGroup)
						{
							if(%db $= "")
								warn("WARNING: could not find emitter datablock for uiname " @ %dbName @ "");
						}
						
						%line = getSubStr(%line,%pos + 2,strlen(%line) - %pos - 2);
						%dir = getWord(%line,0);
						%quotaObject = getQuotaObjectFromBrick($LastLoadedBrick);
						setCurrentQuotaObject(%quotaObject);
						
						if(isObject(%db))
							$LastLoadedBrick.setEmitter(%db);

						$LastLoadedBrick.setEmitterDirection(%dir);
						clearCurrentQuotaObject();
					}
				}
				else
				{
					if(%firstWord $= "+-ITEM")
					{
						if(isObject($LastLoadedBrick))
						{
							%line = getSubStr(%line,7,strlen(%line) - 7);
							%pos = strpos(%line,"");
							%dbName = getSubStr(%line,0,%pos);
							
							if(%dbName $= "NONE")
								%db = 0;
							else
								%db = $uiNameTable_Items[%dbName];
							if($LoadingBricks_Client == $LoadingBricks_BrickGroup)
							{
								if(%dbName !$= "NONE" && !isObject(%db))
									warn("WARNING: could not find item datablock for uiname " @ %dbName @ "");
							}
							
							%line = getSubStr(%line,%pos + 2,strlen(%line) - %pos - 2);
							%pos = getWord(%line,0);
							%dir = getWord(%line,1);
							%respawnTime = getWord(%line,2);
							%quotaObject = getQuotaObjectFromBrick($LastLoadedBrick);
							setCurrentQuotaObject(%quotaObject);
							
							if(isObject(%db))
								$LastLoadedBrick.setItem(%db);

							$LastLoadedBrick.setItemDirection(%dir);
							$LastLoadedBrick.setItemPosition(%pos);
							$LastLoadedBrick.setItemRespawntime(%respawnTime);
							clearCurrentQuotaObject();
						}
					}
					else
					{
						if(%firstWord $= "+-AUDIOEMITTER")
						{
							if(isObject($LastLoadedBrick))
							{
								%line = getSubStr(%line,15,strlen(%line) - 15);
								%pos = strpos(%line,"");
								%dbName = getSubStr(%line,0,%pos);
								%db = $uiNameTable_Music[%dbName];
								
								if($LoadingBricks_Client == $LoadingBricks_BrickGroup)
								{
									if(!isObject(%db))
										warn("WARNING: could not find music datablock for uiname " @ %dbName @ "");
								}
								
								%quotaObject = getQuotaObjectFromBrick($LastLoadedBrick);
								setCurrentQuotaObject(%quotaObject);
								$LastLoadedBrick.setSound(%db);
								clearCurrentQuotaObject();
							}
						}
						else
						{
							if(%firstWord $= "+-VEHICLE")
							{
								if(isObject($LastLoadedBrick))
								{
									%line = getSubStr(%line,10,strlen(%line) - 10);
									%pos = strpos(%line,"");
									%dbName = getSubStr(%line,0,%pos);
									%db = $uiNameTable_Vehicle[%dbName];
									
									if($LoadingBricks_Client == $LoadingBricks_BrickGroup)
									{
										if(!isObject(%db))
											warn("WARNING: could not find vehicle datablock for uiname " @ %dbName @ "");
									}
									
									%line = getSubStr(%line,%pos + 2,strlen(%line) - %pos - 2);
									%recolorVehicle = getWord(%line,0);
									%quotaObject = getQuotaObjectFromBrick($LastLoadedBrick);
									setCurrentQuotaObject(%quotaObject);
									$LastLoadedBrick.setVehicle(%db);
									$LastLoadedBrick.setReColorVehicle(%recolorVehicle);
									clearCurrentQuotaObject();
								}
							}
							else
							{
								if(%firstWord $= "Linecount")
								{
									if(isObject(ProgressGui))
									{
										"Progress_Bar".total = getWord(%line,1);
										Progress_Bar.setValue(0);
										"Progress_Bar".count = 0;
										Canvas.popDialog(ProgressGui);
										Progress_Window.setText("Loading Progress");
										Progress_Text.setText("Loading...");
									}
								}
								else
								{
									if(%firstWord $= "+-OWNER")
									{
										if(isObject($LastLoadedBrick))
										{
											if($LoadingBricks_Ownership == 1)
											{
												%ownerBLID = mAbs(mFloor(getWord(%line,1)));
												%oldGroup = $LastLoadedBrick.getGroup();
												
												if($Server::LAN)
													"$LastLoadedBrick".bl_id = %ownerBLID;
												else
												{
													if (%ownerBLID == 999999)
													{
													}
													else
													{
														%ownerBrickGroup = "BrickGroup_" @ %ownerBLID;
														
														if(isObject(%ownerBrickGroup))
															%ownerBrickGroup = %ownerBrickGroup.getId();
														else
														{
															%ownerBrickGroup = new SimGroup("BrickGroup_" @ %ownerBLID){};
															
															%ownerBrickGroup.client = 0;
															%ownerBrickGroup.name = "\c2BL_ID: " @ %ownerBLID @ "\c2\c1";
															%ownerBrickGroup.bl_id = %ownerBLID;
															mainBrickGroup.add(%ownerBrickGroup);
														}
														if(isObject(%ownerBrickGroup))
														{
															%ownerBrickGroup.add($LastLoadedBrick);
															
															if(isObject(brickSpawnPointData))
															{
																if($LastLoadedBrick.getDataBlock().getId() == brickSpawnPointData.getId())
																{
																	if(%ownerBrickGroup != %oldGroup)
																	{
																		%oldGroup.removeSpawnBrick($LastLoadedBrick);
																		%ownerBrickGroup.addSpawnBrick($LastLoadedBrick);
																	}
																}
															}
														}
													}
												}
											}
										}
									}
									else
									{
										if(getBrickCount() >= getBrickLimit())
										{
											MessageAll('','Brick limit reached (%1)',getBrickLimit());
											ServerLoadSaveFile_End();
											return;
										}
										
										%quotePos = strstr(%line,"\"");
										
										if(%quotePos <= 0)
										{
											error("ERROR: ServerLoadSaveFile_Tick() - Bad line " @ %line @ " - expected brick line but found no uiname");
											ServerLoadSaveFile_End();
											return;
										}
										
										%uiName = getSubStr(%line,0,%quotePos);
										%db = $uiNameTable[%uiName];
										%line = getSubStr(%line,%quotePos + 2, 9999);
										%pos = getWords(%line,0,2);
										%angId = getWord(%line,3);
										%isBaseplate = getWord(%line,4);
										%colorId = $colorTranslation[mFloor(getWord(%line,5))];
										%printName = getWord(%line,6);
										
										if(strpos(%printName,"/") != -1)
										{
											%printName = fileBase(%printName);
											%aspectRatio = %db.printAspectRatio;
											%printIDName = %aspectRatio @ "/" @ %printName;
											%printId = $printNameTable[%printIDName];
											
											if(%printId $= "")
											{
												%printIDName = "Letters/" @ %printName;
												%printId = $printNameTable[%printIDName];
											}
											if(%printId $= "")
												%printId = $printNameTable["Letters/-space"];
										}
										else
											%printId = $printNameTable[%printName];

										%colorFX = getWord(%line,7);
										%shapeFX = getWord(%line,8);
										%rayCasting = getWord(%line,9);
										%collision = getWord(%line,10);
										%rendering = getWord(%line,11);
										%pos = VectorAdd(%pos,$LoadingBricks_PositionOffset);
										
										if(%db)
										{
											%trans = %pos;
											
											if(%angId == 0)
												%trans = %trans SPC " 1 0 0 0";
											else
											{
												if (%angId == 1.0)
												{
													%trans = %trans SPC " 0 0 1" SPC $piOver2;
												}
												else
												{
													if (%angId == 2.0)
													{
														%trans = %trans SPC " 0 0 1" SPC $pi;
													}
													else
													{
														if (%angId == 3.0)
														{
															%trans = %trans SPC " 0 0 -1" SPC $piOver2;
														}
													}
												}
											}
											
											%b = new fxDTSBrick(""){
												dataBlock = %db;
												angleID = %angId;
												isBasePlate = %isBaseplate;
												colorID = %colorId;
												printID = %printId;
												colorFxID = %colorFX;
												shapeFxID = %shapeFX;
												isPlanted = 1;
											};
											
											if(isObject($LoadingBricks_BrickGroup))
												$LoadingBricks_BrickGroup.add(%b);
											else
											{
												error("ERROR: ServerLoadSaveFile_Tick() - $LoadingBricks_BrickGroup does not exist!");
												MessageAll('', "ERROR: ServerLoadSaveFile_Tick() - $LoadingBricks_BrickGroup does not exist!");
												%b.delete();
												ServerLoadSaveFile_End();
												return;
											}
											
											%b.setTransform(%trans);
											%b.trustCheckFinished();
											$LastLoadedBrick = %b;
											%err = %b.plant();
											
											if(%err == 1 || %err == 3 || %err == 5)
											{
												$Load_failureCount = $Load_failureCount + 1;
												%b.delete();
												$LastLoadedBrick = 0;
											}
											else
											{
												//ADDING SOME INFO FOR CODZ LOADING
												%type = %b.getDatablock().CoDZDataType;
												
												if(%type $= "BarrierBase")
												{
													talk(%b SPC "is a barrier base");
													CoDZ_Load_AddTempBase(%b);
												}
												if(%rayCasting !$= "")
													%b.setRayCasting(%rayCasting);
												if(%collision !$= "")
													%b.setColliding(%collision);
												if(%rendering !$= "")
													%b.setRendering(%rendering);
												if($LoadingBricks_Ownership && !$Server::LAN)
												{
													%oldGroup = %b.getGroup();
													%ownerGroup = "";
													
													if(%b.getNumDownBricks())
													{
														%ownerGroup = %b.getDownBrick(0).getGroup();
														%ownerGroup.add(%b);
													}
													else
													{
														if(%b.getNumUpBricks())
														{
															%ownerGroup = %b.getUpBrick(0).getGroup();
															%ownerGroup.add(%b);
														}
													}
													if(isObject(brickSpawnPointData))
													{
														if(%b.getDataBlock().getId() == brickSpawnPointData.getId())
														{
															if(%ownerGroup > 0 && %ownerGroup != %oldGroup)
															{
																%oldGroup.removeSpawnBrick(%b);
																%ownerGroup.addSpawnBrick(%b);
															}
														}
													}
												}
												else
													"$LastLoadedBrick".client = $LoadingBricks_Client;
											}
										}
										else
										{
											if(!$Load_MissingBrickWarned[%uiName])
											{
												warn("WARNING: loadBricks() - DataBlock not found for brick named ", %uiName, "");
												$Load_MissingBrickWarned[%uiName] = 1;
											}
											
											$LastLoadedBrick = 0;
											$Load_failureCount = $Load_failureCount + 1;
										}
										
										$Load_brickCount = $Load_brickCount + 1;
										
										if(isObject(ProgressGui))
										{
											"Progress_Bar".count = "Progress_Bar".count + 1;
											Progress_Bar.setValue("Progress_Bar".count / "Progress_Bar".total);
											
											if("Progress_Bar".count + 1 == "Progress_Bar".total)
												Canvas.popDialog(ProgressGui);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
	if(!$Server_LoadFileObj.isEOF())
	{
		if($Server::ServerType $= "SinglePlayer")
			$LoadSaveFile_Tick_Schedule = schedule(0,0,ServerLoadSaveFile_Tick);
		else
			$LoadSaveFile_Tick_Schedule = schedule(0,0,ServerLoadSaveFile_Tick);
	}
	else
		ServerLoadSaveFile_End();
}

function serverCmdPlantBrick(%client)
{
	if($Game::MissionCleaningUp)
		return 0;

	%player = %client.Player;
	%tempBrick = %player.tempBrick;
	
	if(!isObject(%player))
		return;
	
	%player.playThread(3,plant);
	%mg = %client.miniGame;
	
	if(isObject(%mg))
	{
		if(!%mg.EnableBuilding)
			return 0;
	}
	if(getBrickCount() >= getBrickLimit())
	{
		messageClient(%client,'MsgPlantError_Limit');
		return 0;
	}
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{
		if($Server::MaxBricksPerSecond > 0)
		{
			%currTime = getSimTime();
			
			if(%client.bpsTime + 1000 < %currTime)
			{
				%client.bpsCount = 0;
				%client.bpsTime = %currTime;
			}
			if(%client.bpsCount >= $Server::MaxBricksPerSecond)
				return 0;
		}
	}
	if(!isObject(%tempBrick))
		return 0;
	
	%tempBrickTrans = %tempBrick.getTransform();
	%tempBrickPos = getWords(%tempBrickTrans,0,2);
	%brickData = %tempBrick.getDataBlock();
	
	if(%brickData.CoDZDataType $= "SpawnZombie")
	{
		if($CoDZ::Recording[%client.getBLID()])
		{
			CoDZ_deleteRecording(%client.getBLID());
			CoDZ_EraseRecording(%client.getBLID());
			$CoDZ::Recording[%client.getBLID()] = 1;
			messageClient(%client,'',"\c6Recording \c2on\c6.");
		}
		else
		{
			CoDZ_EraseRecording(%client.getBLID());
			$CoDZ::Recording[%client.getBLID()] = 1;
			messageClient(%client,'',"\c6Recording \c2on\c6.");
		}
	}
	if(%brickData.CoDZDataType $= "SpawnRally")
	{
		if(!$CoDZ::Recording[%client.getBLID()])
		{
			messageClient(%client,'',"\c6You cannot place a \c3Rally Point \c6unless you are recording.");
			return 0;
		}
		else
		{
			if(!isObject($CoDZ::Recording[%client.getBLID(),"Brick",0]))
				return 0;
			else if($CoDZ::Recording[%client.getBLID(),"Brick",0].CoDZDataType $= "SpawnZombie")
			{
				if($CoDZ::Recording[%client.getBLID(),"Brick",$CoDZ::Recording[%client.getBLID(),"Brick","Count"] - 1].CoDZDataType !$= "SpawnZombie")
				{
					if($CoDZ::Recording[%client.getBLID(),"Brick",$CoDZ::Recording[%client.getBLID(),"Brick","Count"] - 1].CoDZDataType !$= "SpawnRally")
						return 0;
				}
				else
					return 0;
			}
		}
	}
	if(%brickData.CoDZDataType $= "BarrierBase" | %brickData.CoDZDataType $= "BarrierPartH" | %brickData $= "BarrierPartV")
	{
		if(!$CoDZ::Recording[%client.getBLID()])
		{
			messageClient(%client,'',"\c6You cannot place a \c3Barrier Brick \c6unless you are recording.");
			return 0;
		}
	}
	if(%brickData.CoDZDataType $= "BarrierBaseAuto")
	{
		if($CoDZ::Recording[%client.getBLID(),"Loaded"])
		{
			CoDZ_PlaceBarrierSet(%client);
			return 0;
		}
		else
		{
			messageClient(%client,'',"\c6You cannot place a \c3Barrier Base Auto \c6brick.");
			%client.schedule(500,"chatMessage","\c6Use \c3/loadBarrierSet \c0[name] \c6(use \"\c0@\c6\" to replace spaces) to load a \c3Barrier Set\c6.");
			return 0;
		}
	}
	
	if(%brickData.brickSizeX > %brickData.brickSizeY)
		%brickRadius = %brickData.brickSizeX;
	else
		%brickRadius = %brickData.brickSizeY;

	%brickRadius = %brickRadius * 0.5 / 2;
	
	if($Pref::Server::TooFarDistance == 0 || $Pref::Server::TooFarDistance $= "")
		$Pref::Server::TooFarDistance = 50;
	
	$Pref::Server::TooFarDistance = mClampF($Pref::Server::TooFarDistance,20,99999);
	
	if(VectorDist(%tempBrickPos,%client.Player.getPosition()) > $Pref::Server::TooFarDistance + %brickRadius)
	{
		messageClient(%client,'MsgPlantError_TooFar');
		return 0;
	}
	
	%plantBrick = new fxDTSBrick("")
	{
		dataBlock = %tempBrick.getDataBlock();
		position = %tempBrickTrans;
		isPlanted = 1;
	};
	
	%client.brickGroup.add(%plantBrick);
	%plantBrick.setTransform(%tempBrickTrans);
	%plantBrick.setColor(%tempBrick.getColorID());
	%plantBrick.setPrint(%tempBrick.getPrintID());
	%plantBrick.client = %client;
	%plantErrorCode = %plantBrick.plant();
	
	if(!%plantBrick.isColliding())
		%plantBrick.dontCollideAfterTrust = 1;
	
	%plantBrick.setColliding(0);
	
	if(%plantErrorCode == 0)
	{
		if(!$Server::LAN)
		{
			if(%plantBrick.getNumDownBricks())
				%plantBrick.stackBL_ID = %plantBrick.getDownBrick(0).stackBL_ID;
			else
			{
				if(%plantBrick.getNumUpBricks())
					%plantBrick.stackBL_ID = %plantBrick.getUpBrick(0).stackBL_ID;
				else
					%plantBrick.stackBL_ID = %client.getBLID();
			}
			if(%plantBrick.stackBL_ID <= 0)
				%plant.stackBL_ID = %client.getBLID();
		}
		
		%client.undoStack.push(%plantBrick TAB "PLANT");
		
		if($Server::LAN)
			%plantBrick.trustCheckFinished();
		else
			%plantBrick.PlantedTrustCheck();

		ServerPlay3D(brickPlantSound,%plantBrick.getTransform());
		%player.tempBrick.setColor(%client.currentColor);
		%client.bpsCount = %client.bpsCount + 1;
	}
	else
	{
		if(%plantErrorCode > 0 && %plantErrorCode < 6)
		{
			switch(%plantErrorCode)
			{
				case 1:
					messageClient(%client,'MsgPlantError_Overlap');
				case 2:
					messageClient(%client,'MsgPlantError_Float');
				case 3:
					messageClient(%client,'MsgPlantError_Stuck');
				case 4:
					messageClient(%client,'MsgPlantError_Unstable');
				case 5:
					messageClient(%client,'MsgPlantError_Buried');
			}
			
			%plantBrick.delete();
		}
		else
		{
			%plantBrick.delete();
			messageClient(%client,'MsgPlantError_Forbidden');
		}
	}
	if(getBrickCount() <= 100 && getRayTracerProgress() <= -1 && getRayTracerProgress() < 0 && $Server::LAN == 0 && doesAllowConnections())
		startRaytracer();
	
	if($CoDZ::Recording[%client.getBLID()] && isObject(%plantBrick))
	{
		if(!CoDZ_CheckRecording(%client))
			$CoDZ::Recording[%client.getBLID(),"Brick","Count"] = 0;

		$CoDZ::Recording[%client.getBLID(),"Brick",$CoDZ::Recording[%client.getBLID(),"Brick","Count"]] = %plantBrick;
		$CoDZ::Recording[%client.getBLID(),"Brick","Count"] += 1;
		
		if(%brickData.CoDZDataType $= "SpawnZombie")
			brick2x2fCoDZRallyPointData.onUse(%player);
		if(%brickData.CoDZDataType $= "SpawnRally")
		{
			%plantBrick.setColliding(0);
			%plantBrick.setRayCasting(0);
			%plantBrick.setRendering(0);
			%plantBrick.setColor(2);
		}
	}
	
	return %plantBrick;
}

package CoDZ_MainPackage
{
	function serverCmdCancelBrick(%client)
	{
		parent::serverCmdCancelBrick(%client);
		
		if($CoDZ::Recording[%client.getBLID()])
		{
			CoDZ_deleteRecording(%client.getBLID());
			messageClient(%client,'',"\c6Recording \c0off\c6.");
		}
		
		$CoDZ::Recording[%client.getBLID()] = 0;
	}
	
	function fxDTSBrickData::onUse(%this,%player,%InvSlot)
	{
		if(!isObject(%player))
			return;
		
		%client = %player.client;
		
		if(%this.CoDZDataType $= "RallyPoint")
		{
			if($CoDZ::Recording[%client.getBLID()] && $CoDZ::Recording[%client.getBLID(),"Brick",0].CoDZDataType $= "SpawnZombie")
				parent::onUse(%this,%player,%InvSlot);
			else
				return;
		}
		else
			parent::onUse(%this,%player,%InvSlot);
	}
	function armor::onTrigger(%datablock,%obj,%slot,%enabled)
	{
		//%slot
		//0 = left-click
		//1 = unused by blockland
		//2 = jump
		//3 = crouch
		//4 = right-click
		//talk(%datablock);
		//talk(%obj);
		//talk(%slot);
		//talk(%enabled);
		if(%obj.getClassName() $= "Player" && isObject(PlayerCoDZDefault) && isObject(PlayerCoDZStaminUp) && isObject(PlayerCoDZMuleKick) && isObject(PlayerCoDZSUMK))
		{
			if(%obj.getDatablock() == PlayerCoDZDefault.getID() || %obj.getDatablock() == PlayerCoDZStaminUp.getID() || %obj.getDatablock == PlayerCoDZMuleKick.getID() || %obj.getDatablock() == PlayerCoDZSUMK.getID())
			{
				if(%slot == 3)
				{
					%obj.togSprint(0);
					if(%obj.client.inBarZone)
					{
						if(%enabled == true)
							%obj.client.rebuildBarrier(%obj.client.CoDZ_Bar);
					}
				}
				if(%slot == 4)
				{
					if(%obj.isCrouched() == true)
						return;
				
					if(%enabled == true)
					{
						if(%obj.isCrouched() == false)
						{
							%obj.togSprint(1);
							cancel(%obj.CoDZ_SprintRegenTick);
							%obj.CoDZ_degenSprintEnergy();
						}
					}
					else
					{
						if(%obj.isCrouched() == false)
						{
							%obj.togSprint(0);
							cancel(%obj.CoDZ_SprintDegenTick);
							%obj.schedule(500,"CoDZ_regenSprintEnergy");
						}
					}
				}
			}
		}
		parent::onTrigger(%datablock,%obj,%slot,%enabled);
	}
	
	function fxDTSBrick::onPlant(%this)
	{
		%client = %this.client;
		if(isObject(%client))
		{
			if(!$CoDZ::Recording[%client.getBLID()])
				parent::onPlant(%this);
		}
		if(%this.getDatablock().CoDZDataType $= "BarrierPartV" || %this.getDatablock().CoDZDataType $= "BarrierPartH")
		{
			%downCount = %this.getNumDownBricks();
			if(%downCount > 0)
			{
				%downBrick = %this.getDownBrick(0);
				%downData = %downBrick.getDatablock();
				%downType = %downData.CoDZDataType;
				
				if(%downType !$= "BarrierBase")
				{
					if(%downType !$= "BarrierPartV")
					{
						if(%downType !$= "BarrierPartH")
						{
							if(isObject(%client))
							{
								messageClient(%client,'',"\c6You can only place a \c3Barrier Part \c6on top of another \c3Barrier Part \c6or \c3Barrier Base\c6.");
								$CoDZ::Recording[%client.getBLID()] = 0;
								messageClient(%client,'',"\c3Barrier Set \c6recording \c0off");
								CoDZ_deleteRecording(%client);
							}
							
							%this.killBrick();
						}
					}
					
				}
				else
					parent::onPlant(%this);
			}
		}
		else
			parent::onPlant(%this);
	}
	
	//function fzDTSBrick
	
	function ProjectileData::impactImpulse(%this, %obj, %col, %vector)
	{
		return;
	}
	
	function ProjectileData::radiusImpulse(%this, %obj, %col, %distanceFactor, %pos, %impulseAmt, %verticalAmt)
	{
		return;
	}
	
	function ServerLoadSaveFile_End()
	{
		parent::ServerLoadSaveFile_End();
		//CoDZ_Load_CorrectTempBases();
		CoDZ_CreateBarriersFromTemp();
	}
	
	function GameConnection::autoAdminCheck(%this)
	{
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%name = ClientGroup.getObject(%i).getPlayerName();
			%blid = ClientGroup.getObject(%i).getBLID();
			
			if(%this.getPlayerName() $= %name || %this.getBLID() $= %blid)
				%count++;
		}

		if(%count - 1 > 0)
		{
			%this.delete("CONSOLE:<br>You are not allowed to Multi-Client on this server.");
			talk("\c3" @ %this.getPlayerName() SPC "\c6has been \c3kicked \c6for \c3Multi-Clienting\c6.");
			return;
		}
		
		return parent::autoAdminCheck(%this);
	}
};
activatePackage("CoDZ_MainPackage");

function serverCmdMessageSent(%client,%text)
{
	%trimText = trim(%text);
	
	if(%client.lastChatText $= %trimText && !%client.isAdmin)
	{
		%chatDelta = (getSimTime() - %client.lastChatTime) / getTimeScale();
		if (%chatDelta < 15000)
		{
			%client.spamMessageCount = $SPAM_MESSAGE_THRESHOLD;
			messageClient(%client,'','\c6Do not repeat yourself.');
		}
	}
	
	%client.lastChatTime = getSimTime();
	%client.lastChatText = %trimText;
	%player = %client.Player;
	
	if(isObject(%player))
	{
		%player.playThread(3,talk);
		%player.schedule(strlen(%text) * 50,playThread,3,root);
	}
	
	%text = chatWhiteListFilter(%text);
	%text = StripMLControlChars(%text);
	%text = trim(%text);
	
	if (strlen(%text) <= 0)
		return;
	if($Pref::Server::MaxChatLen > 0)
	{
		if(strlen(%text) >= $Pref::Server::MaxChatLen)
			%text = getSubStr(%text,0,$Pref::Server::MaxChatLen);
	}
	
	%protocol = "http://";
	%protocolLen = strlen(%protocol);
	%urlStart = strpos(%text,%protocol);
	
	if(%urlStart == -1)
	{
		%protocol = "https://";
		%protocolLen = strlen(%protocol);
		%urlStart = strpos(%text,%protocol);
	}
	if(%urlStart == -1)
	{
		%protocol = "ftp://";
		%protocolLen = strlen(%protocol);
		%urlStart = strpos(%text,%protocol);
	}
	if(%urlStart != -1)
		return messageClient(%client,'',"\c5URLs are not allowed on this server.");
	
	%protocol = ".com";
	%protocolLen = strLen(%protocol);
	%urlEnd = strPos(%text,%protocol);
	
	if(%urlEnd == -1)
	{
		%protocol = ".net";
		%protocolLen = strLen(%protocol);
		%urlEnd = strPos(%text,%protocol);
	}
	if(%urlEnd == -1)
	{
		%protocol = ".org";
		%protocolLen = strLen(%protocol);
		%urlEnd = strPos(%text,%protocol);
	}
	if(%urlEnd == -1)
	{
		%protocol = ".gov";
		%protocolLen = strLen(%protocol);
		%urlEnd = strPos(%text,%protocol);
	}
	if(%urlEnd != -1)
		return messageClient(%client,'',"\c5URLs are not allowed on this server.");

	if(isObject(getCoDZGame()))
	{
		if(getCoDZGame().isMember(%client))
		{
			//if(%client.CoDZ_Color $= "")
			//	CoDZMiniGame.SetPlayerColor(%client);
			if($CoDZ::State[%client.getBLID()] $= "Dead")
				chatMessageAll(%client,"\c0DEAD" SPC trim(%client.CoDZ_State SPC "<color:" @ $CoDZ::Color[$CoDZ::Player[%client.getBLID(),"Color"]] @ ">") @ %client.getPlayerName() @ "\c6:" SPC %text);
			else
				chatMessageAll(%client,trim(%client.CoDZ_State SPC "<color:" @ $CoDZ::Color[$CoDZ::Player[%client.getBLID(),"Color"]] @ ">") @ %client.getPlayerName() @ "\c6:" SPC %text);
		}
		else
		{
			for(%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				if(!getCoDZGame().isMember(ClientGroup.getObject(%i)))
					messageClient(ClientGroup.getObject(%i),'',"\c0SPECTATOR \c7" @ %client.getPlayerName() @ "\c4:" SPC %text);
			}
		}
	}
	else
		chatMessageAll(%client,"\c7" @ %client.clanPrefix @ "\c2" @ %client.getPlayerName() @ "\c6:" SPC %text);
	//echo(%client.getSimpleName(), ": ", %text);
}

function serverCmdTeamMessageSent(%client,%text)
{
	messageClient(%client,'',"\c5Team chat is disabled on this server.");
}


function CoDZMiniGame::addMember(%this,%client)
{
	if(!isObject(%client))
		return;
	if(%client.getClassName() !$= "GameConnection")
		return;
	if(%this.isMember(%client))
		return;
	if(isObject(%client.miniGame))
		%client.miniGame.removeMember(%client);
	
	%this.member[%this.numMembers] = %client;
	%this.numMembers++;
	%client.miniGame = %this;
	
	
	
	commandToClient(%client,'SetPlayingMiniGame',1);
	commandToClient(%client,'SetBuildingDisabled',!%this.EnableBuilding);
	commandToClient(%client,'SetPaintingDisabled',!%this.EnablePainting);
	
	$CoDZ::Player[%client.getBLID(),"Kills"] = 0;
	$CoDZ::Player[%client.getBLID(),"Revives"] = 0;
	$CoDZ::Player[%client.getBLID(),"Downs"] = 0;
	$CoDZ::Player[%client.getBLID(),"Score"] = 0;
	$CoDZ::Player[%client.getBLID(),"Headshots"] = 0;
}

function Armor::onDamage(%this,%obj,%delta)
{
	if(%delta > 0 && %obj.getState() !$= "Dead")
	{
		%flash = %obj.getDamageFlash() + %delta / %this.maxDamage * 2;
		
		if (%flash > 0.75)
			%flash = 0.75;

		%obj.setDamageFlash(%flash);
		%painThreshold = 7;
		
		if(%this.painThreshold !$= "")
			%painThreshold = %this.painThreshold;
		if(%delta > %painThreshold)
			%obj.playPain();
	}
}

function Armor::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType)
{
	if(%obj.getState() $= "Dead")
		return;
	if(getSimTime() - %obj.spawnTime < $Game::PlayerInvulnerabilityTime && !%obj.hasShotOnce)
		return;
	if(%obj.invulnerable)
		return;
	if(%obj.isMounted() && %damageType != $DamageType::Suicide && %this.rideAble == 0)
	{
		%mountData = %obj.getObjectMount().getDataBlock();
		
		if($Damage::Direct[%damageType])
		{
			if(%mountData.protectPassengersDirect)
				return;
		}
		else
		{
			if(%mountData.protectPassengersRadius)
				return;
		}
	}
	if($Damage::Direct[%damageType] == 1)
	{
		%obj.lastDirectDamageType = %damageType;
		%obj.lastDirectDamageTime = getSimTime();
	}
	
	%obj.lastDamageType = %damageType;
	
	if(getSimTime() - %obj.lastPainTime > 300)
		%obj.painLevel = %damage;
	else
		%obj.painLevel = %obj.painLevel + %damage;

	%obj.lastPainTime = getSimTime();
	
	if(%obj.isCrouched())
	{
		if($Damage::Direct[%damageType])
			%damage = %damage * 2.1;
		else
			%damage = %damage * 0.75;
	}
	
	%scale = getWord(%obj.getScale(),2);
	%damage = %damage / %scale;
	%obj.applyDamage(%damage);
	%location = "Body";
	%client = %obj.client;
	
	if(isObject(%sourceObject))
	{
		if(%sourceObject.getClassName() $= "GameConnection")
			%sourceClient = %sourceObject;
		else
			%sourceClient = %sourceObject.client;
	}
	else
		%sourceClient = 0;
	if(isObject(%sourceObject))
	{
		if(%sourceObject.getType() & $TypeMasks::VehicleObjectType)
		{
			if(%sourceObject.getControllingClient())
				%sourceClient = %sourceObject.getControllingClient();
		}
	}
	if(%obj.getState() $= "Dead")
	{
		if(isObject(%client))
			%client.onDeath(%sourceObject,%sourceClient,%damageType,%location);
		else
		{
			if (isObject(%obj.spawnBrick))
			{
				%mg = getMiniGameFromObject(%sourceObject);
				if (isObject(%mg))
				{
					%obj.spawnBrick.spawnVehicle(%mg.VehicleRespawnTime);
				}
				else
				{
					%obj.spawnBrick.spawnVehicle(5000);
				}
			}
		}
	}
	else
	{
		if (%this.useCustomPainEffects == 1.0)
		{
			if (%obj.painLevel >= 40.0)
			{
				if (isObject(%this.PainHighImage))
				{
					%obj.emote(%this.PainHighImage, 1);
				}
			}
			else
			{
				if (%obj.painLevel >= 25.0)
				{
					if (isObject(%this.PainMidImage))
					{
						%obj.emote(%this.PainMidImage, 1);
					}
				}
				else
				{
					if (isObject(%this.PainLowImage))
					{
						%obj.emote(%this.PainLowImage, 1);
					}
				}
			}
		}
		else
		{
			if (%obj.painLevel >= 40.0)
			{
				%obj.emote(PainHighImage, 1);
			}
			else
			{
				if (%obj.painLevel >= 25.0)
				{
					%obj.emote(PainMidImage, 1);
				}
				else
				{
					%obj.emote(PainLowImage, 1);
				}
			}
		}
	}
}

function GameConnection::onDeath(%client, %sourceObject, %sourceClient, %damageType, %damLoc)
{
	if (%sourceObject.sourceObject.isBot)
	{
		%sourceClientIsBot = 1;
		%sourceClient = %sourceObject.sourceObject;
	}
	%player = %client.Player;
	if (isObject(%player))
	{
		%player.setShapeName("", 8564862);
		if (isObject(%player.tempBrick))
		{
			%player.tempBrick.delete();
			%player.tempBrick = 0;
		}
		%player.client = 0;
	}
	else
	{
		warn("WARNING: No player object in GameConnection::onDeath() for client \'" @ %client @ "\'");
	}
	if (isObject(%client.Camera) && isObject(%client.Player))
	{
		if (%client.getControlObject() == %client.Camera && %client.Camera.getControlObject() > 0.0)
		{
			%client.Camera.setControlObject(%client.dummyCamera);
		}
		else
		{
			%client.Camera.setMode("Corpse", %client.Player);
			%client.setControlObject(%client.Camera);
			%client.Camera.setControlObject(0);
		}
	}
	%client.Player = 0;
	if ($Damage::Direct[%damageType] != 1.0)
	{
		if (getSimTime() - %player.lastDirectDamageTime < 100.0)
		{
			if (%player.lastDirectDamageType !$= "")
			{
				%damageType = %player.lastDirectDamageType;
			}
		}
	}
	if (%damageType == $DamageType::Impact)
	{
		if (isObject(%player.lastPusher))
		{
			if (getSimTime() - %player.lastPushTime <= 1000.0)
			{
				%sourceClient = %player.lastPusher;
			}
		}
	}
	%message = "%2 killed %1";
	if (%sourceClient == %client || %sourceClient == 0.0)
	{
		%message = $DeathMessage_Suicide[%damageType];
	}
	else
	{
		%message = $DeathMessage_Murder[%damageType];
	}
	if ($Damage::Direct[%damageType] == 1.0 && %player.getWaterCoverage() < 0.05)
	{
		if (%sourceClient && isObject(%sourceClient.Player))
		{
			%playerVelocity = VectorLen(VectorSub(%player.preHitVelocity, %sourceClient.Player.getVelocity())) / 2.64 * 6.0 * 3600.0 / 5280.0;
		}
		else
		{
			%playerVelocity = VectorLen(%player.preHitVelocity) / 2.64 * 6.0 * 3600.0 / 5280.0;
		}
		%playerPos = %player.getPosition();
		%mask = $TypeMasks::StaticShapeObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
		%res0 = containerRayCast(VectorAdd(%playerPos, "0 0 2"), VectorAdd(%playerPos, "0 0  -6.8"), %mask);
		%res1 = containerRayCast(VectorAdd(%playerPos, "0 0 2"), VectorAdd(%playerPos, "0 -1 -6.8"), %mask);
		%res2 = containerRayCast(VectorAdd(%playerPos, "0 0 2"), VectorAdd(%playerPos, "1 1  -6.8"), %mask);
		%res3 = containerRayCast(VectorAdd(%playerPos, "0 0 2"), VectorAdd(%playerPos, "-1 1 -6.8"), %mask);
		if (!isObject(getWord(%res0, 0)) && !isObject(getWord(%res1, 0)) && !isObject(getWord(%res2, 0)) && !isObject(getWord(%res3, 0)))
		{
			%range = round(VectorLen(VectorSub(%playerPos, %sourceObject.originPoint)) / 2.65 * 6.0);
			if (isObject(%sourceClient.Player))
			{
				%sourceClient.Player.emote(winStarProjectile, 1);
			}
			if (!%sourceClientIsBot)
			{
				%sourceClient.play2D(rewardSound);
				commandToClient(%sourceClient, 'BottomPrint', "<bitmap:base/client/ui/ci/star>\c4 MID AIR KILL - " @ %client.getPlayerName() @ " " @ round(%playerVelocity) @ "MPH, " @ %range @ "ft!", 3);
			}
			commandToClient(%client, 'BottomPrint', "\c6 MID AIR\'d by " @ %sourceClient.getPlayerName() @ " - " @ round(%playerVelocity) @ "MPH, " @ %range @ "ft!", 3);
		}
	}
	if (isObject(%client.miniGame))
	{
		if (%sourceClient == %client)
		{
			%client.incScore(%client.miniGame.Points_KillSelf);
		}
		else
		{
			if (%sourceClient == 0.0)
			{
				%client.incScore(%client.miniGame.Points_Die);
			}
			else
			{
				if (!%sourceClientIsBot)
				{
					%sourceClient.incScore(%client.miniGame.Points_KillPlayer);
				}
				%client.incScore(%client.miniGame.Points_Die);
			}
		}
	}
	%clientName = %client.getPlayerName();
	if (isObject(%sourceClient))
	{
		%sourceClientName = %sourceClient.getPlayerName();
	}
	else
	{
		if (isObject(%sourceObject.sourceObject) && %sourceObject.sourceObject.getClassName() $= "AIPlayer")
		{
			%sourceClientName = %sourceObject.sourceObject.name;
		}
		else
		{
			%sourceClientName = "";
		}
	}
	%mg = %client.miniGame;
	if (isObject(%mg))
	{
		%mg.messageAllExcept(%client, 'MsgClientKilled', %message, %client.getPlayerName(), %sourceClientName);
		messageClient(%client, 'MsgYourDeath', %message, %client.getPlayerName(), %sourceClientName, %mg.RespawnTime);
		if (%mg.RespawnTime < 0.0)
		{
			commandToClient(%client, 'centerPrint', "", 1);
		}
		%mg.checkLastManStanding();
	}
	else
	{
		messageAllExcept(%client, -1.0, 'MsgClientKilled', %message, %client.getPlayerName(), %sourceClientName);
		messageClient(%client, 'MsgYourDeath', %message, %client.getPlayerName(), %sourceClientName, $Game::MinRespawnTime);
	}
}