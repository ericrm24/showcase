
//{{BLOCK(startScreen)

//======================================================================
//
//	startScreen, 256x256@8, 
//	+ palette 256 entries, not compressed
//	+ 248 tiles (t|f|p reduced) not compressed
//	+ regular map (in SBBs), not compressed, 32x32 
//	Total size: 512 + 15872 + 2048 = 18432
//
//	Time-stamp: 2020-10-03, 05:32:46
//	Exported by Cearn's GBA Image Transmogrifier, v0.8.3
//	( http://www.coranac.com/projects/#grit )
//
//======================================================================

#ifndef GRIT_STARTSCREEN_H
#define GRIT_STARTSCREEN_H

#define startScreenTilesLen 15872
extern const unsigned int startScreenTiles[3968];

#define startScreenMapLen 2048
extern const unsigned int startScreenMap[512];

#define startScreenPalLen 512
extern const unsigned int startScreenPal[128];

#endif // GRIT_STARTSCREEN_H

//}}BLOCK(startScreen)
