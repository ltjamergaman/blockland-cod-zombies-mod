datablock TriggerData(CoDZ_TriggerData)
{
	tickPeriodMS = 150; //this is how often Slayer_CPTriggerData::onTickTrigger will be called when an object is in the trigger
};

function fxDTSBrick::createTriggerOG(%this,%data)
{
	//credits to Space Guy for showing how to create triggers

	%t = new Trigger()
	{
		datablock = %data;
		polyhedron = "0 0 0 1 0 0 0 -1 0 0 0 1"; //this determines the shape of the trigger
		//x1 y1 z1 x2 y2 z2 x3 y3 z3 x4 y4 z4
	};
	missionCleanup.add(%t);
	
	%boxMax = getWords(%this.getWorldBox(), 3, 5);
	%boxMin = getWords(%this.getWorldBox(), 0, 2);
	%boxDiff = vectorSub(%boxMax,%boxMin);
	%boxDiff = vectorAdd(%boxDiff,"0 0 0.2"); 
	%t.setScale(%boxDiff);
	%posA = %this.getWorldBoxCenter();
	%posB = %t.getWorldBoxCenter();
	%posDiff = vectorSub(%posA, %posB);
	%posDiff = vectorAdd(%posDiff, "0 0 0.1");
	%t.setTransform(%posDiff);

	%this.trigger = %t;
	%t.brick = %this;

	return %t;
}

function fxDtsBrick::createTrigger(%this,%data,%type,%polyhedron)
{
	if(%polyhedron $= "")
		%polyhedron = "0 0 0 1 0 0 0 -1 0 0 0 1";

	if(isObject(%this.trigger))
		%this.trigger.delete();
	
	if(%type $= "")
		return

	%t = new Trigger()
	{
		datablock = %data;
		type = %type;
		polyhedron = %polyhedron;
	};
	missionCleanup.add(%t);

	%boxMax = getWords(%this.getWorldBox(), 3, 5);
	%boxMin = getWords(%this.getWorldBox(), 0, 2);
	%boxDiff = vectorSub(%boxMax,%boxMin);
	%boxDiff = vectorAdd(%boxDiff,"0 0 0.2"); 
	%t.setScale(%boxDiff);
	%posA = %this.getWorldBoxCenter();
	%posB = %t.getWorldBoxCenter();
	%posDiff = vectorSub(%posA, %posB);
	%posDiff = vectorAdd(%posDiff, "0 0 0.1");
	%t.setTransform(%posDiff);

	%this.trigger = %t;
	%t.brick = %this;

	return %t;
}

package CoDZ_deleteBrickTriggers
{
	function fxDTSBrick::onRemove(%this) //make sure to clean up your triggers
	{
		if(isObject(%this.trigger))
			%this.trigger.delete();

		parent::onRemove(%this);
	}
};
activatePackage(CoDZ_deleteBrickTriggers);

function CoDZ_TriggerData::onEnterTrigger(%this,%trigger,%obj)
{
	talk("now gay");
}

function CoDZ_TriggerData::onLeaveTrigger(%this,%trigger,%obj)
{
	talk("not gay");
}

function CoDZ_TriggerData::onTickTrigger(%this,%trigger,%obj)
{
	talk("gay");
}

function fxDTSBrick::CoDZ_ScaleTrigger(%this,%scaleX,%y,%z)
{
	if(!isObject(%trigger = %this.trigger))
		return;
	if(getWordCount(%scaleX) == 3)
		%trigger.setScale(%scaleX);
	else
		%trigger.setScale(%scaleX SPC %y SPC %z);
	
	//%this.CodZ_CenterTrigger();
}

function fxDTSBrick::CoDZ_CenterTrigger(%this)
{
	if(!isObject(%t = %this.trigger))
		return;
	%posA = %this.getWorldBoxCenter();
	%posB = %t.getWorldBoxCenter();
	%posDiff = vectorSub(%posA, %posB);
	%posDiff = vectorAdd(%posDiff, "0 0 0.1");
	%t.setTransform(%posDiff);
}

function CoDZ_loadSaveZones()
{
}