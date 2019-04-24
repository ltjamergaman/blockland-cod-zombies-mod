// todo: reduce the amount of raycast checks

// returns true if it needs to overrite normal movement to avoid obstacle
function AiPlayer::handleObstacle(%this)
{
	%scale = getWord(%this.getScale(), 2);
	
	if(vectorLen(getWords(%this.getVelocity(), 0, 1)) < 1 * %scale)
		%this.stuckCount += $SceneryRP::TickTime / 66;
	else
		%this.stuckCount = 0;
	
	// figuring out of the bot is stuck or not
	if(%this.stuckCount > 15)
		%this.isStuck = true;
	else
		%this.isStuck = false;
	
	// checks to see if there's a player in our way
	if(!%this.isSideStepping)
		%this.playerSideStep();
	else
		%this.stopSideStep();
	
	%start = vectorAdd(%this.getHackPosition(), "0.2");
	%raycast = containerRaycast(%start, vectorAdd(%start, vectorScale(%this.getForwardVector(), 1 * %scale)), $TypeMasks::fxBrickObjectType | $TypeMasks::PlayerObjectType, %this);
	
	if(isObject(%col = getWord(%raycast, 0)) && %col != %this.getMountedObject(0))
	{
		// seeing if we can jump over the obstacle
		// if top of the box is a certain distance away from the root of the bot then we can jump over it
		if(%col.getType() & $TypeMasks::fxBrickObjectType && %this.stuckCount > 5)
		{
			%topBrick = %col.getTopBrick();
			%worldBox = %topBrick.getWorldBox();
			%height = getWord(%worldBox, 5);
			%zPosition = getWord(%this.getPosition(), 2);
			
			// if we can jump over a wall then jump over the wall
			if(%height - %zPosition < 3.5 * %scale && %height - %zPosition > 0)
			{
				%this.jump();
				return false;
			}
		}
		else if(%this.isStuck)
			%this.jump();
		
		// shoving players
		if(%col.getClassName() $= "Player" && %this.isStuck)
		{
			%this.shovePlayer(%col);
			return false;
		}
		else if(%this.stuckCount > 30) // no clue what this check is for
		{
			%this.shovePlayer(%col);
			return false;
		}
	}
	// if we're stuck then just jump in desperation
	else if(%this.stuckCount > 25)
		%this.jump();
	
	return %this.avoidWall() || %this.isSideStepping;
}

function AiPlayer::avoidWall(%this)
{
	%scale = getWord(%this.getScale(), 2);
	
	%endPosition = %this.moveGoal;
	
	// trying to see if there's an object in front of the bot
	
	if(vectorLen(%this.getVelocity()) < 1 * %scale)
	 	%distance = 0.1;
	else
		%distance = 1.5;
	
	%col = %this.boxSearch(vectorAdd(%this.getHackPosition(), vectorScale(%this.getForwardVector(), %distance)));
	%ceilingCheck = containerRaycast(%this.getPosition(), vectorAdd(%this.getPosition(), "0 0 15"), $TypeMasks::fxBrickObjectType, false);
	
	// if there's a ceiling then abandon the wall avoidance
	if(isObject(%door = %this.areDoorsNear()) || isObject(getWord(%ceilingCheck, 0)) || getWord(%this.getScale(), 2) < 0.5)
	{
		// if there's a door in our way then open it
		if(isObject(%door))
			%this.openDoorInPath();
		
		return %this.interiorAvoidance();
	}
	// if we're doing interior obstacle avoidance but aren't in an interior anymore then restore normal movement
	else if(%this.avoidingInterior)
	{
		%this.avoidingInterior = false;
		%this.setMoveX(0);
		%this.setMoveY(1);
	}
	
	// getting normal of the wall so we can walk along it
	if(isObject(%col))
		%normal = %this.getNormal(%col);
	
	// if its a wall then walk along it
	if(isObject(%col) && mAbs(getWord(vectorToEuler(%normal), 0)) < 5 && %normal != 0)
	{
		// getting the direction we should walk along
		%botRightVector = vectorCross(%this.getForwardVector(), "0 0 1");
		%normalRightVector = vectorCross(%normal, "0 0 1");
		%relativePosition = vectorSub(%this.getPosition(), %endPosition);
		
		if(%this.isPositionToRight $= "")
		{
			%isPositionToRight = %this.isPositionToRight2(%endPosition);
			%this.isPositionToRight = %isPositionToRight;
		}
		
		// if the position is to the right and the normal is to the left, flip the normal
		if(%this.isPositionToRight && vectorDot(%botRightVector, %normalRightVector) > 0)
		{
			%normalRightVector = vectorScale(%normalRightVector, -1);
			%this.obstacleSide = 1; // means the wall is on the right hand side of the bot
		}
		// if the position is to the left and the normal is to the right, flip the normal
		else if(!%this.isPositionToRight && vectorDot(%botRightVector, %normalRightVector) < 0)
		{
			%normalRightVector = vectorScale(%normalRightVector, -1);
			%this.obstacleSide = 0; // means the wall is on the left hand side of the bot
		}
		else
			%this.obstacleSide = 1; // means the wall is on the right hand side of the bot
		
		// if there's shit to the left and we are going left then switch direction
		if(%this.isWallToSide(0, true) && %this.obstacleSide == 1)
		{
			%normalRightVector = vectorScale(%normalRightVector, -1);
			%this.obstacleSide = 0;
		}
		// if there's shit to the right and we're going to the right then switch direction
		else if(%this.isWallToSide(1, true) && %this.obstacleSide == 0)
		{
			%normalRightVector = vectorScale(%normalRightVector, -1);
			%this.obstacleSide = 1;
		}
		
		// used to set the direction the bot should be going in to avoid the obstacle
		%this.obstacleDirection = %normalRightVector;
		%aimPosition = vectorAdd(%this.getEyePoint(), vectorScale(%this.obstacleDirection, 500));
		%this.setAimLocation(%aimPosition);
		%this.clearMoveGoal();
		
		return true;
	}
	// if we're following a wall but we can now walk towards the movegoal 
	else if(%this.obstacleDirection !$= "" && ((%beeline = %this.canBeeLine(%endPosition)) || !%this.areWallsAround()))
	{
		%this.obstacleDirection = "";
		%this.isPositionToRight = "";
		%this.obstacleSide = -1;
		return false;
	}
	// following a wall if we can't beeline. below is for handling walking along wall corners
	else if(%this.obstacleDirection !$= "" && !%beeline)
	{
		// checking to see if we can walk around a wall's corner
		if(!isObject(%this.isWallToSide()) && getSimTime() - %this.lastFlip > 500)
		{
			if(%this.obstacleSide)
			{
				%position = vectorAdd(%this.getEyePoint(), vectorScale(vectorCross(%this.obstacleDirection, "0 0 1"), 500));
				%this.obstacleSide = 1;
				%this.obstacleDirection = vectorCross(%this.obstacleDirection, "0 0 1");
			}
			else
			{
				%position = vectorAdd(%this.getEyePoint(), vectorScale(vectorCross(%this.obstacleDirection, "0 0 1"), -500));
				%this.obstacleSide = 0;
				%this.obstacleDirection = vectorScale(vectorCross(%this.obstacleDirection, "0 0 1"), -1);
			}
			
			%this.lastFlip = getSimTime();
			%this.setAimLocation(vectorAdd(%position, vectorScale("0 0 2.1", %scale)));
			%this.clearMoveGoal();
		}
		
		return true;
	}
	// we aren't following a wall to begin with and we found nothing to walk along
	else
	{
		%this.obstacleDirection = "";
		%this.isPositionToRight = "";
		%this.obstacleSide = -1;
		return false;
	}
}

// much simpler obstacle algorithm for inside spaces, just set x move to 1 or -1 depending on if movegoal is to right/left
function AiPlayer::interiorAvoidance(%this)
{
	%scale = getWord(%this.getScale(), 2);
	
	if(vectorLen(%this.getVelocity()) < 0.5 * %scale)
	{
		%position = vectorAdd(%this.getPosition(), "0 0 0.1");
		%rightVector = vectorCross(%this.getForwardVector(), "0 0 1");
		
		for(%i = 0; %i < 3; %i++)
		{
			%start = vectorAdd(%position, vectorScale(%rightVector, 0.9 * (%i - 1) * %scale));
			%endPosition = vectorAdd(%start, vectorScale(%this.getForwardVector(), 2));
			
			if(isObject(%col = getWord(containerRaycast(%start, %endPosition, $TypeMasks::fxBrickObjectType, false), 0)))
				%foundObject = true;
		}
		
		if(%foundObject)
		{
			%this.avoidingInterior = true;
			
			if(%this.isPositionToRight2(%this.moveGoal))
			{
				%this.setMoveX(1);
				%this.setMoveY(0);
				return true;
			}
			else
			{
				%this.setMoveX(-1);
				%this.setMoveY(0);
				return true;
			}
		}
		else
		{
			%this.avoidingInterior = false;
			%this.setMoveX(0);
			%this.setMoveY(1);
			return false;
		}
	}
	else if(%this.avoidingInterior)
	{
		%this.avoidingInterior = false;
		%this.setMoveX(0);
		%this.setMoveY(1);
		return false;
	}
	
	return false;
}

function AiPlayer::playerSideStep(%this)
{
	%scale = getWord(%this.getScale(), 2);
	
	if(vectorLen(%this.getVelocity()) < 0.5 * %scale)
	{
		%col = %this.isObjectInWay($TypeMasks::PlayerObjectType | $TypeMasks::StaticShapeObjectType, %this);
		
		if(isObject(%col) && !%this.isSideStepping && %col != %this.getMountedObject(0) && %col.getDatablock() != nameToId(MedievalStandardBot))
		{
			%this.isSideStepping = true;
			%this.setMoveX(getRandom(0, 1) ? -1 : 1);
			
			%this.schedule(1500, stopSideStep);
		}
	}
}

function AiPlayer::stopSideStep(%this)
{
	%this.isSideStepping = false;
	%this.setMoveX(0);
}

function AiPlayer::shovePlayer(%this, %col)
{
	if(getSimTime() - %col.lastShove < 1000)
		return;
	
	%scale = getWord(%this.getScale(), 2);
	
	%this.playThread(0, activate2);
	%col.addVelocity(vectorAdd(vectorScale(%this.getForwardVector(), 7), vectorScale("0 0 5", %scale)));
	%col.lastShove = getSimTime();
}

function AiPlayer::isWallToSide(%this, %customSide, %raycast)
{
	%rightVector = vectorCross(%this.getForwardVector(), "0 0 1");
	
	if(%customSide $= "")
		%customSide = %this.obstacleSide;
	
	if(%customSide)
		%position = vectorAdd(%this.getEyePoint(), vectorScale(%rightVector, 2));
	else
		%position = vectorAdd(%this.getEyePoint(), vectorScale(%rightVector, -2));
	
	if(%raycast)
	{
		%raycast = containerRaycast(%this.getEyePoint(), %position, $TypeMasks::fxBrickObjectType, false);
		return isObject(getWord(%raycast, 0));
	}
	else if(isObject(%col = %this.boxSearch(%position)))
		return %col;
	else
		return 0;
}

function AiPlayer::areWallsAround(%this)
{
	%rightVector = vectorCross(%this.getForwardVector(), "0 0 1");
	
	%scale = getWord(%this.getScale(), 2);
	
	%col |= %this.boxSearch(vectorAdd(%this.getEyePoint(), vectorScale(%this.getForwardVector(), 2 * %scale)));
	%col |= %this.boxSearch(vectorAdd(%this.getEyePoint(), vectorScale(%this.getForwardVector(), -2 * %scale)));
	%col |= %this.boxSearch(vectorAdd(%this.getEyePoint(), vectorScale(%rightVector, 2 * %scale)));
	%col |= %this.boxSearch(vectorAdd(%this.getEyePoint(), vectorScale(%rightVector, -2 * %scale)));
	
	return %col;
}

// much better way of calculating angle than whatever shitty ass function i was using before. i don't use it in the pathfinding shit but i'm keeping it anyway because its so fucking good
// its a month later context: i had some old get angle from position code i wrote a few years ago where it did trig instead of linear algebra and linear algebra is more fashionable than trig which is why this method is good
function Player::getAngleFromPosition(%this, %position)
{
	%forwardVector = vectorNormalize(getWords(%this.getForwardVector(), 0, 1));
	%vectorToPosition = vectorNormalize(getWords(vectorSub(%position, %this.getPosition()), 0, 1));
	%dot = vectorDot(%forwardVector, %vectorToPosition);
	%theta = mACos(%dot);
	return %theta * 180 / $PI;
}

// see is point on one side of the plane or not code from the eggine
function Player::isPositionToRight(%this, %position)
{
	return (vectorDot(vectorCross(%this.getForwardVector(), "0 0 1"), vectorSub(%this.getPosition(), %position)) < 0);
}

// we need to snap the vector to the nearest axis because ???
// this is another situation where i didn't comment this shit when i wrote it and its a month later
function Player::isPositionToRight2(%this, %position)
{
	return (vectorDot(vectorCross(snapVectorToNearestAxis(%this.getForwardVector()), "0 0 1"), vectorSub(%this.getPosition(), %position)) < 0);
}

// ignores z axis
function snapVectorToNearestAxis(%vector)
{
	%x = getWord(%vector, 0);
	%y = getWord(%vector, 1);
	
	if(mAbs(%x) < mAbs(%y))
	{
		if(%y < 0)
			return "0 -1 0";
		else if(%y > 0)
			return "0 1 0";
	}
	else
	{
		if(%x < 0)
			return "-1 0 0";
		else
			return "1 0 0";
	}
}

// its not a box search anymore because boxes aren't too good for what i need them for. why you might ask well i don't have a clue i wrote this code a month ago and didn't comment it
function AiPlayer::boxSearch(%this, %position)
{
	// does box search at given position. box search because a box more accurately represents the player collision box
	// return ContainerFindFirst($TypeMasks::fxBrickObjectType, %position, 0.5, 0.5, 0.5);
	
	%scale = getWord(%this.getScale(), 2);
	
	initContainerRadiusSearch(%position, 0.5 * %scale, $TypeMasks::fxBrickObjectType);
	while(%col = containerSearchNext())
	{
		if(strPos(%col.getDatablock().getName(), "Door") == -1 && %col.isColliding())
			return %col;
	}
	
	return 0;
}

function AiPlayer::areDoorsNear(%this)
{
	initContainerRadiusSearch(%this.getPosition(), 2, $TypeMasks::fxBrickObjectType);
	while(%col = containerSearchNext())
	{
		if(strPos(%col.getDatablock().getName(), "Door") != -1)
			return %col;
	}
}

// if it finds a door then open it
function AiPlayer::openDoorInPath(%this)
{
	%raycast = containerRaycast(%this.getEyePoint(), %this.moveGoal, $TypeMasks::fxBrickObjectType, false);
	%col = getWord(%raycast, 0);
	
	if(isObject(%col) && strPos(%col.getDatablock().getName(), "Door") != -1 && strPos(%col.getDatablock().getName(), "Open") == -1)
	{
		%this.player = %this;
		serverCmdActivateStuff(%this);
		%this.player = 0;
	}
}

// gets normal from a colision brick
function AiPlayer::getNormal(%this, %col)
{
	%raycast = containerRaycast(%this.getHackPosition(), vectorAdd(%col.getPosition(), "0 0 -0.01"), $TypeMasks::fxBrickObjectType, false);
	
	if(isObject(getWord(%raycast, 0)))
		return getWords(%raycast, 4, 6);
}

function AiPlayer::canBeeLine(%this, %endPosition)
{
	%scale = getWord(%this.getScale(), 2);
	
	// checks to see if it can walk straight to the position or not. used in ::avoidWall, used to disengage walking along the wall
	%rightVector = vectorCross(%this.getForwardVector(), "0 0 1");
	
	// doing 3 raycasts, two from the sides of the bot, one from the center. we do 3 so it doesn't get caught on corners its trying to walk around (actually, to see if you only have to do 2)
	for(%i = 0; %i < 3; %i++)
	{
		%start = vectorAdd(%this.getEyePoint(), vectorScale(%rightVector, 0.9 * (%i - 1) * %scale));
		
		if(isObject(getWord(containerRaycast(%start, %endPosition, $TypeMasks::fxBrickObjectType, false), 0)))
			return false;
	}
	
	return true;
}

function AiPlayer::isObjectInWay(%this, %mask, %ignore)
{
	%scale = getWord(%this.getScale(), 2);
	
	// checks to see if it can walk straight to the position or not. used in ::avoidWall, used to disengage walking along the wall
	%rightVector = vectorCross(%this.getForwardVector(), "0 0 1");
	
	// doing 3 raycasts, two from the sides of the bot, one from the center
	for(%i = 0; %i < 3; %i++)
	{
		%start = vectorAdd(%this.getEyePoint(), vectorScale(%rightVector, 1.1 * (%i - 1) * %scale));
		%end = vectorAdd(%start, vectorScale(%this.getForwardVector(), 1 * %scale));
		
		if(isObject(%col = getWord(containerRaycast(%start, %end, %mask, %ignore), 0)))
			return %col;
	}
	
	return false;
}