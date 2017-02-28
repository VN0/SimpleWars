---------------------------------------------------------------------------------------------------------------------------

Color Palette Extractor - © 2014, 2015 Wasabimole http://wasabimole.com

---------------------------------------------------------------------------------------------------------------------------

HOW TO - INSTALLING COLOR PALETTE EXTRACTOR:

- Unpack the Asset Store package into your project
- [Or alternatively] Copy ColorPaletteExtractor.dll into Assets/Wasabimole/ColorPaletteExtractor/Editor/

---------------------------------------------------------------------------------------------------------------------------

HOW TO - USING COLOR PALETTE EXTRACTOR:

1 - Open Color Palette Extractor window by selecting Window > Color Palette Extractor from the Unity 3D menu.
2 - Press Load Image … to load any PNG or JPG image you want to use.
3 - Press Analyze … to compute the color palette for the image.
4 - [Optional] Tweak any of the tool settings, and analyze again until you are happy with the results.
5 - Press any of the Export … buttons to save the color palette.

---------------------------------------------------------------------------------------------------------------------------

COLOR PICKER

After analyzing the image, clicking on the color gradients will show info on the individual colors.

---------------------------------------------------------------------------------------------------------------------------

CONTROLS

Max. num gradients - Select the maximum number of detected gradients. This is just an upper limit. 
	For example, selecting 3 will try to detect the 3 main color gradients from the image.

Detection sensitivity - This controls how sensitive the detection algorithm will be. Increasing 
	this value will find minor gradients. Decrease to find only major gradients.

Gradient smoothness - The resulting gradients are smoothed out more or less according to this 
	value. Smoothing produces more constant increments in the color changes.

Gradient luminosity - This allows picking between different luminosity algorithms. The gradients 
	are scaled accordingly so that their luminosity matches the selected one.

Gradient steps - This chooses how many colors are included in every gradient.

Saturation - By the default the gradients take the maximum saturation from the image. Here you can 
	choose any gradient saturation output from average to maximum.

Gradient gaps - Gaps in color data are filled in by default, but you can choose to leave them empty.

Gap saturation - You can choose if the color gaps are filled in with fully saturated colors, or if 
	you prefer them filled in with desaturated ones.

Gradient Order - Choose between ordering the gradients by Hue, or by use percent (percent of that 
	color in the source image).
	
---------------------------------------------------------------------------------------------------------------------------

EXPORT OPTIONS

All the export options will save the color gradients as currently displayed. Gradients will be 
ordered by Hue or Percent as selected.

Export Big PNG … the color palette is saved as an image designed as an artist digital color palette. 
This image is compact, yet big enough for easy color picking in any drawing tool.

Export Tiny PNG … the color palette is saved as a very small image. Each gradient is just 1 pixel 
height, and each color step is represented as a single pixel. This format is intended to be used as 
a reference texture in your code. Can also be used inside a shader.

Export C# script … the color palette is saved as a Unity C# script, containing a class with an array 
of gradients, each gradient being an array of colors. In the color picker details you can change 
between 2 different color types; Color(float, float, float, float) or Color32(byte, byte, byte, byte). 
Alpha is filled in as 1f or 255.
	
---------------------------------------------------------------------------------------------------------------------------

Color Palette Extractor was tested on Unity 4.3 and above, if you have any issues with this plugin,
please email contact@wasabimole.com

---------------------------------------------------------------------------------------------------------------------------
