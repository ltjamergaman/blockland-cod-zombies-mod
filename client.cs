//exec("./client/CoDZMenu.gui");
if(!isObject(CoDZMenu))
{
	exec("./client/Profiles/CoDZMenu_CheckProfile.cs");
	exec("./client/Profiles/CoDZMenu_CheckSmallProfile.cs");
	exec("./client/CoDZMenu.gui");
}

function CoDZMenu::setActiveTab(%this,%tab)
{
	if(!isObject(%this.activeTab))
		%this.activeTab = nameToID("CoDZMenu_Tab_MiniGame");
	if(!isObject(%this.activePage))
		%this.activePage = nameToID("CoDZMenu_Page_MiniGame");
	
	CoDZMenu.activeTab.mColor = "255 255 255 255";
	%this.activePage.setVisible(0);
	
	switch(%tab)
	{
		case 0: //minigame
			%this.activeTab = nameToID("CoDZMenu_Tab_MiniGame");
			%this.activePage = nameToID("CoDZMenu_Page_MiniGame");
		case 1: //admin
			%this.activeTab = nameToID("CoDZMenu_Tab_Admin");
			%this.activePage = nameToID("CoDZMenu_Page_Admin");
		case 2: //help
			%this.activeTab = nameToID("CoDZMenu_Tab_Help");
			%this.activePage = nameToID("CoDZMenu_Page_Help");
		case 3: //about
			%this.activeTab = nameToID("CoDZMenu_Tab_About");
			%this.activePage = nameToID("CoDZMenu_Page_About");
	}
	
	%this.activeTab.mColor = "200 200 200 255";
	%this.activePage.setVisible(1);
	warn("CoDZMenu -+ active tab set to " @ %this.activeTab);
}

function CoDZMenu::AddPlayer(%this)
{
	if(%this.numPlayers < 1)
		%this.numPlayers = 0;
	
	exec("./client/temp/tempPlayer.cs");
	CoDZMenu_Page_Minigame_Back.add(CoDZMenu_TempPlayer);
	CoDZMenu_TempPlayer.setName("CoDZMenu_Player" @ %this.numPlayers);
	CoDZMenu_TempPlayer_Top.setName("CoDZMenu_Player" @ %this.numPlayers @ "_Top");
	CoDZMenu_TempPlayer_Bottom.setName("CoDZMenu_Player" @ %this.numPlayers @ "_Bottom");
	%item = nameToID("CoDZMenu_Player" @ %this.numPlayers);
	%itemTop = nameToID("CoDZMenu_Player" @ %this.numPlayers @ "_Top");
	%itemBottom = nameToID("CoDZMenu_Player" @ %this.numPlayers @ "_Bottom");
	%pos = %item.getPosition();
	%posX = getWord(%pos,0);
	%posY = getWord(%pos,1);
	%extent = %item.getExtent();
	%extentX = getWord(%extent,0);
	%extentY = getWord(%extent,1);
	%back = CoDZMenu_Page_Minigame_Back;
	%backEx = %back.getExtent();
	%backExX = getWord(%backEx,0);
	%backExY = getWord(%backEx,1);
	
	if(%this.numPlayers > 0)
		%item.setMargin(%posY + (50 * %this.numPlayers),%posX);
	
	%newPosY = (%posY + (50 * %this.numPlayers) + %extentY);
	
	if(%newPosY > %backExY)
		%back.resize(0,0,%backExX,%newPosY + 10);
	
	//echo(%newPosY);
	CoDZ_Client_RequestPlayerInfo(%this.numPlayers);
	%info = $CoDZ::Client::TempPlayerInfo;
	
	%name = getField(%info,0);
	%score = getField(%info,1);
	%kills = getField(%info,2);
	
	if(%score $= "")
		%score = 0;
	if(%kills $= "")
		%kills = 0;
	
	if(strLen(%name) > 22)
		%name = getSubStr(%name,0,18) @ "...";
	
	%top = %itemTop.getText();
	%top = strReplace(%top,"[]",%name);
	
	%bottom = %itemBottom.getText();
	%bottom = strReplace(%bottom,"[]",%score);
	%bottom = strReplace(%bottom,"{}",%kills);
	
	%itemTop.setText(%top);
	%itemBottom.setText(%bottom);
	%this.numPlayers++;
}

function CoDZ_Client_RequestPlayerInfo(%val)
{
	//echo("requesting...");
	commandToServer('CoDZ_SendPlayerInfo',%val);
}

function clientCmdCoDZ_ReceivePlayerInfo(%info)
{
	//echo("received...");
	//echo(%info SPC getField(%info,0) SPC getField(%info,1) SPC getField(%info,2));
	$CoDZ::Client::TempPlayerInfo = %info;
}