function eulerToAxis(%euler)
{
	%euler = VectorScale(%euler,$pi / 180);
	%matrix = MatrixCreateFromEuler(%euler);
	return getWords(%matrix,3,6);
}

function axisToEuler(%axis)
{
	%angleOver2 = getWord(%axis,3) * 0.5;
	%angleOver2 = -%angleOver2;
	%sinThetaOver2 = mSin(%angleOver2);
	%cosThetaOver2 = mCos(%angleOver2);
	%q0 = %cosThetaOver2;
	%q1 = getWord(%axis,0) * %sinThetaOver2;
	%q2 = getWord(%axis,1) * %sinThetaOver2;
	%q3 = getWord(%axis,2) * %sinThetaOver2;
	%q0q0 = %q0 * %q0;
	%q1q2 = %q1 * %q2;
	%q0q3 = %q0 * %q3;
	%q1q3 = %q1 * %q3;
	%q0q2 = %q0 * %q2;
	%q2q2 = %q2 * %q2;
	%q2q3 = %q2 * %q3;
	%q0q1 = %q0 * %q1;
	%q3q3 = %q3 * %q3;
	%m13 = 2.0 * (%q1q3 - %q0q2);
	%m21 = 2.0 * (%q1q2 - %q0q3);
	%m22 = 2.0 * %q0q0 - 1.0 + 2.0 * %q2q2;
	%m23 = 2.0 * (%q2q3 + %q0q1);
	%m33 = 2.0 * %q0q0 - 1.0 + 2.0 * %q3q3;
	return mRadToDeg(mAsin(%m23)) SPC mRadToDeg(mAtan(-%m13, %m33)) SPC mRadToDeg(mAtan(-%m21, %m22));
}

//////////
// item //
//////////
datablock ItemData(StuffItem)
{
	category = "Item";  // Mission editor category
	className = "Tool"; // For inventory system

	 // Basic Item Properties
	shapeFile = "base/data/shapes/brickweapon.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Stuff";
	doColorShift = true;
	colorShiftColor = "0 0.25 0 1.000";

	 // Dynamic properties defined by the scripts
	image = StuffImage;
	canDrop = true;
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(StuffImage)
{
   // Basic Item properties
   shapeFile = "base/data/shapes/brickweapon.dts";
   emap = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = StuffItem;

   doColorShift = true;
   colorShiftColor = gunItem.colorShiftColor;//"0.400 0.196 0 1.000";
   
   rotation = eulerToMatrix("-135 0 90");
};

package asdf
{	
	function Player::Pickup(%player,%obj)
	{
		//talk("PLAYER PICKUP - " @ %a SPC %b);
		//talk("OBJ NAME: " @ %obj.getDatablock().getName());
		%item = %obj.getDatablock();
		%name = %item.getName();
		if(%name $= "StuffItem" || %item == nameToID("StuffItem"))
		{
			%obj.delete();
			%player.client.bottomprint("You just picked up a reward.",3,0);
			//MZ_Reward();
		}
		else
			parent::pickup(%player,%obj);
	}
	
	function ItemStuff_spawnRanDrop()
	{		
		//%rwd[1] = "Insta-Kill";
		//%rwd[2] = "Double-Points";
		//%rwd[3] = "Max-Ammo";
		//%rwd[4] = "Carpenter";
		//%rwd[5] = "Nuke";
		
		//%ran = getRandom(1,5);
		//%reward = %rwd[%ran];
		//$MZ::MiniGame::Reward = %reward;
		//$MZ::MiniGame::RwdNum = %ran;
		//%item = StuffItem.getID();
		//if(%reward == 0 || isObject(%item) == false)
			//return;
		
		//switch$(%reward)
		//{
			//case "Insta-Kill":
			//	%item.MZ_reward = %reward;
			//	%item.MZ_rwdNum = 1;
			
			//case "Double-Points":
			//	%item.MZ_reward = %reward;
			//	%item.MZ_rwdNum = 2;
			
			//case "Max-Ammo":
			//	%item.MZ_reward = %reward;
			//	%item.MZ_rwdNum = 3;
			
			//case "Carpenter":
			//	%item.MZ_reward = %reward;
			//	%item.MZ_rwdNum = 4;
			
			//case "Nuke":
			//	%item.MZ_reward = %reward;
			//	%item.MZ_rwdNum = 5;
		//}
		
		if(isObject($ItemStuff_Item))
		{
			$ItemStuff_Item.delete();
			cancel($ItemStuff_Loop);
		}
		
		$ItemStuff_Item = new item()
		{
			datablock = StuffItem.getID();
			canPickup = true;
			scale = "1 1 1";
			position = "0 0 2";
			//name = "StuffItem";
			rotate = false;
			rotation = eulerToMatrix("-135 0 90");
			spawnBrick = -1;
			static = true;
		};
		
		$ItemStuff_Item.setShapeName("REWARD");
		$ItemStuff_Item.setShapeNameColor("0 1 0");
		$ItemStuff_Item.setShapeNameDistance("250");
		
		ItemStuff_PlayFade(1,30000);
	}
	
	function ItemStuff_PlayFade(%check,%timeleft)
	{
		cancel($ItemStuff_Loop);
		if(!isObject($ItemStuff_Item))
			return;
		if(%timeleft <= 0)
		{
			$ItemStuff_Item.delete();
			return;
		}
		if(%check && %timeleft < 15000 && %timeleft >= 10000)
		{
			$ItemStuff_Item.fadeout();
			$ItemStuff_Loop = schedule(1000,0,ItemStuff_PlayFade,0,%timeleft-1000);
		}
		else if(!%check && %timeleft < 15000 && %timeleft >= 10000)
		{
			$ItemStuff_Item.fadein();
			$ItemStuff_Loop = schedule(1000,0,ItemStuff_PlayFade,1,%timeleft-1000);
		}
		else if(%check && %timeleft < 10000 && %timeleft >= 5000)
		{
			$ItemStuff_Item.fadeout();
			$ItemStuff_Loop = schedule(500,0,ItemStuff_PlayFade,0,%timeleft-500);
		}
		else if(!%check && %timeleft < 10000 && %timeleft >= 5000)
		{
			$ItemStuff_Item.fadein();
			$ItemStuff_Loop = schedule(500,0,ItemStuff_PlayFade,1,%timeleft-500);
		}
		else if(%check && %timeleft < 5000 && %timeleft >= 2500)
		{
			$ItemStuff_Item.fadeout();
			$ItemStuff_Loop = schedule(250,0,ItemStuff_PlayFade,0,%timeleft-250);
		}
		else if(!%check && %timeleft < 5000 && %timeleft >= 2500)
		{
			$ItemStuff_Item.fadein();
			$ItemStuff_Loop = schedule(250,0,ItemStuff_PlayFade,1,%timeleft-250);
		}
		else if(%check && %timeleft < 2500)
		{
			$ItemStuff_Item.fadeout();
			$ItemStuff_Loop = schedule(100,0,ItemStuff_PlayFade,0,%timeleft-100);
		}
		else if(!%check && %timeleft < 2500)
		{
			$ItemStuff_Item.fadein();
			$ItemStuff_Loop = schedule(100,0,ItemStuff_PlayFade,1,%timeleft-100);
		}
		else if(%timeleft >= 15000)
			$ItemStuff_Loop = schedule(1000,0,ItemStuff_PlayFade,%check,%timeleft-1000);
		
		$ItemStuff_Item.setShapeName("REWARD");// | TIME:" @ %timeleft / 1000);
	}
	
	function ItemStuff_RandomDrop()
	{
		//if($MZ::MiniState == false)
		//	return;
		
		%ran = getRandom(1,100);
		if(%ran > 90 && %ran <= 100)
		{
			ItemStuff_spawnRanDrop();
		}
		else
			return;
	}
	
	function ItemStuff_Reward()
	{
		//Item used for reward drops
		%item = StuffItem.getID();
		%item.MZ_reward = "";
		%item.MZ_rwdNum = 0;
	}
	
	//function StuffItem::onadd(%this,%obj)
	//{
	//	//trace(1);
	//	//talk("%a" SPC %a);
	//	//talk("%b" SPC %b);
	//	//talk("%c" SPC %c);
	//	//talk("%a NA" SPC %a.getName());
	//	//talk("%b NA" SPC
	//	//%b.dump();
	//	talk("blah");
	//	%obj.rotation = eulerToMatrix("-135 0 90");
	//	//%obj.setTransform(%position SPC eulerToAxis("0 0 90"));
	//	parent::onadd(%a,%b,%c);
	//	//trace(0);
	//}
};
activatepackage(asdf);