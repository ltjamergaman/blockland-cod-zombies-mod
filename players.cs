
$CoDZ::Datablock::Player::MinFS = 7.25;    //Minimum forward speed
$CoDZ::Datablock::Player::MaxFS = 10.50;   //Maximum forward speed
$CoDZ::Datablock::Player::MinBS = 3.75;    //Minimum backward speed
$CoDZ::Datablock::Player::MaxBS = 3.75;    //Maximum backward speed
$CoDZ::Datablock::Player::MinSS = 3.5;     //Minimum side speed
$CoDZ::Datablock::Player::MaxSS = 3.5;     //Maximum side speed
$CoDZ::Datablock::Player::MinCFS = 1.75;   //Minimum crouched forward speed
$CoDZ::Datablock::Player::MaxCFS = 1.75;   //Maximum crouched forward speed
$CoDZ::Datablock::Player::MinCBS = 1;      //Minimum crouched backward speed
$CoDZ::Datablock::Player::MaxCBS = 1;      //Maximum crouched backward speed
$CoDZ::Datablock::Player::MinCSS = 1.25;   //Minimum crouched side speed
$CoDZ::Datablock::Player::MaxCSS = 1.25;   //Maximum crouched side speed

if($CoDZ::PlayerDataLoaded)
	return;
$CoDZ::PlayerDataLoaded = 1;

datablock PlayerData(PlayerCoDZDefault : PlayerNoJet)
{
	maxForwardSpeed        = $CoDZ::Datablock::Player::MinFS;
	maxBackwardSpeed       = $CoDZ::Datablock::Player::MinBS;
	maxSideSpeed 		   = $CoDZ::Datablock::Player::MinSS;
	maxCrouchForwardSpeed  = $CoDZ::Datablock::Player::MinCFS;
	maxCrouchBackwardSpeed = $CoDZ::Datablock::Player::MinCBS;
	maxCrouchSideSpeed 	   = $CoDZ::Datablock::Player::MinCSS;
	jumpForce = 675;
	canJet = false;
	sprintEnergy = 25;
	
	maxItems = 0;
	maxTools = 2;
	maxWeapons = 2;

	uiName = "CoDZ Player Default";
	showEnergyBar = false;
};

datablock PlayerData(PlayerCoDZMuleKick : PlayerCoDZDefault)
{
	//maxForwardSpeed        = $CoDZ::Datablock::Player::MinFS;
	//maxBackwardSpeed       = $CoDZ::Datablock::Player::MinBS;
	//maxSideSpeed 		   = $CoDZ::Datablock::Player::MinSS;
	//maxCrouchForwardSpeed  = $CoDZ::Datablock::Player::MinCFS;
	//maxCrouchBackwardSpeed = $CoDZ::Datablock::Player::MinCBS = 1;
	//maxCrouchSideSpeed 	   = $CoDZ::Datablock::Player::MinCSS = 1.25;
	//jumpForce = 675;
	//canJet = false;	
	maxTools = 3;
	maxWeapons = 3;

	uiName = "CoDZ Player MuleKick";
	//showEnergyBar = true;
};

datablock PlayerData(PlayerCoDZDowned : PlayerCoDZDefault)
{
	maxForwardSpeed        = 0;
	maxBackwardSpeed       = 0;
	maxSideSpeed 		   = 0;
	maxCrouchForwardSpeed  = 0;
	maxCrouchBackwardSpeed = 0;
	maxCrouchSideSpeed 	   = 0;
	
	uiName = "CoDZ Player Downed";
	//showEnergyBar = true;
};