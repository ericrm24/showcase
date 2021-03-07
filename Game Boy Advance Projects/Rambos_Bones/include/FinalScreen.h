
//{{BLOCK(FinalScreen)

//======================================================================
//
//	FinalScreen, 256x256@8, 
//	+ palette 256 entries, not compressed
//	+ 219 tiles (t|f|p reduced) not compressed
//	+ regular map (in SBBs), not compressed, 32x32 
//	Total size: 512 + 14016 + 2048 = 16576
//
//	Time-stamp: 2020-10-03, 17:18:35
//	Exported by Cearn's GBA Image Transmogrifier, v0.8.3
//	( http://www.coranac.com/projects/#grit )
//
//======================================================================

#ifndef GRIT_FINALSCREEN_H
#define GRIT_FINALSCREEN_H

#define FinalScreenTilesLen 14016
extern const unsigned int FinalScreenTiles[3504];

#define FinalScreenMapLen 2048
extern const unsigned int FinalScreenMap[512];

#define FinalScreenPalLen 512
extern const unsigned int FinalScreenPal[128];

#endif // GRIT_FINALSCREEN_H

//}}BLOCK(FinalScreen)
