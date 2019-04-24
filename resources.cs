function replaceConcsSpaces(%str)
{
	strReplace(%str,"@"," ");
}

function hasItemOnList(%list,%item)
{
	for(%i = 0; %i < getWordCount(%list);%i++)
	{
		if(getWord(%list,%i) $= %item)
			return 1;
	}
		
	return 0;
}

//credit to slicksilver / trinick
function hexToRGB(%hex)
{
   for(%i = 0; %i < 3; %i++)
      eval("%color[%i] = 0x" @ getSubStr(%hex,%i*2,2) @ ";");
   return %color0 SPC %color1 SPC %color2;
}

//credit to xalos
function RBGToHex(%col)   //Converts a 0-255 RGB color to its hex equivalent
{
    for(%i = 0; %i < 3; %i++)
        %sub[%i] = ByteToHex(getWord(%col,%i));
    return %sub0 @ %sub1 @ %sub2;
}

function ByteToHex(%byte)   //Converts a byte into its hex equivalent
{
    %low = %byte % 16;
    %high = %byte - %low;
    return NybbleToChar(%high) @ NybbleToChar(%low);
}

function NybbleToChar(%nybble)   //Converts a nybble into its hex equivalent
{
	return getSubStr("0123456789ABCDEF",%nybble,1); 
}

function isInteger(%string)
{
	%search = "- 0 1 2 3 4 5 6 7 8 9";
	for(%i = 0; %i < getWordCount(%search); %i++)
	{
		%string = strReplace(%string,getWord(%search,%i),"");
	}
	if(%string $= "")
		return true;
	return false;
}

function getClientList()
{
	%clGrp = nameToID("ClientGroup");
	%count = %clGrp.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		if(%list $= "")
			%list = %clGrp.getObject(%i);
		else
			%list = trim(%list SPC %clGrp.getObject(%i));
	}
	
	return %list;
}

function removeItemFromList(%item,%list)
{
	if(%list $= "" || %item $= "")
		return;
	
	%count = getWordCount(trim(%list));
	for(%i = 0; %i < %count; %i++)
	{
		if(getWord(%list,%i) $= %item)
			%list = removeWord(%list,%i);
	}
	
	return %list;
}

function serverCmdGetBarNum(%client)
{
	if(!%client.isAdmin){

		return;
	}
	%player = %client.getControlObject();
	%start = %player.getEyePoint();
	%eyeVec = %player.getEyeVector();
	%vector = VectorScale(%eyeVec,100);
	%end = VectorAdd(%start,%vector);
	%mask = $TypeMasks::FxBrickAlwaysObjectType;
	%scanTarg = containerRayCast(%start,%end,%mask,%player);
	if(%scanTarg)
	{
		%pos = posFromRaycast(%scanTarg);
		%vec = VectorSub(%pos,%start);
		%dist = VectorLen(%vec);
		%scanObj = getWord(%scanTarg,0);
		%num = %scanObj.CoDZ_BarNum;
		if(%scanObj.getClassName() $= "FxDTSBrick")
			messageClient(%client,'',"objectid = " @ %scanObj @ "  Barrier Number - " @ %num);
	}
}

function scaleFromVector(%vector,%newVal)
{
	%a = getWord(%vector,0);
	%b = getWord(%vector,1);
	%c = getWord(%vector,2);
	
	%newA = %newVal - %a;
	%newB = %newVal - %b;
	%newC = %newVal - %c;
	
	if(%newA < %newB && %newA < %newC)
		return %a+%newA SPC %b SPC %c;
	if(%newB < %newA && %newB < %newC)
		return %a SPC %b+%newB SPC %c;
	if(%newC < %newA && %newC < %newB)
		return %a SPC %b SPC %c+%newC;
}