//--- OBJECT WRITE BEGIN ---
new GuiSwatchCtrl(CoDZMenu_TempPlayer) {
   profile = "GuiDefaultProfile";
   horizSizing = "right";
   vertSizing = "bottom";
   position = "303 41";
   extent = "246 43";
   minExtent = "8 2";
   enabled = "1";
   visible = "1";
   clipToParent = "1";
   color = "125 125 125 75";

   new GuiMLTextCtrl(CoDZMenu_TempPlayer_Top) {
      profile = "GuiMLTextProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "5 0";
      extent = "230 26";
      minExtent = "8 2";
      enabled = "1";
      visible = "1";
      clipToParent = "1";
      lineSpacing = "2";
      allowColorChars = "0";
      maxChars = "-1";
      text = "<font:impact:26><color:FFFFFF>[]";
      maxBitmapHeight = "-1";
      selectable = "1";
      autoResize = "1";
   };
   new GuiMLTextCtrl(CoDZMenu_TempPlayer_Bottom) {
      profile = "GuiMLTextProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "5 21";
      extent = "233 20";
      minExtent = "8 2";
      enabled = "1";
      visible = "1";
      clipToParent = "1";
      lineSpacing = "2";
      allowColorChars = "0";
      maxChars = "-1";
      text = "<font:impact:20><color:FFFFFF>Score: <color:FFFF00>[] \t <color:FFFFFF>Kills: <color:FFFF00>{}";
      maxBitmapHeight = "-1";
      selectable = "1";
      autoResize = "1";
   };
};
//--- OBJECT WRITE END ---
