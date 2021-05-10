The models included in the Environment/Dynamic/Frame folder are designed to be as modular as possible, and
easy to implement programming-wise.

All frame types come with two models:

	- 1 x Corner piece
	- 1 x Straight piece


When it comes to designing custom frames, the frame pieces can be as wide or as thick as you want, the only
restriction is that the straight piece MUST BE 1 UNITY UNIT LONG (1cm in Maya).

This makes scaling the straight sections precisely much easier, as the X scale of the frame piece equals the
length or width of the planar surface used to show the image/painting (e.g. if an image is 1.85 Unity Units
long, both length-wise frames would need an X scale of 1.85).



Also important to keep in mind when designing frames: the pivot point of the modules MUST be on the inside
edge/corner of the object (For corners, this would be the inside corner, and for straight pieces, this would
be on the right-most inner corner). Doing this makes it soo much easier to build the frame in-game by
snapping the modules to each corner of the image/painting.


-----------------------------------------------------------------------------------------
BUILDING THE FRAME IN-ENGINE:

Before you can start this, you need an object for the image/painting to be textured/cast onto. Preferrably a
single-sided plane, but a cube would work too (if a bit pointless, considering 5 of it's 6 faces would never
be seen). For the sake of simplicity, I'll be referring to this object as a "canvas" for this guide.



	1. Create/spawn 4 corner pieces, and snap them to the corners of the canvas.

	   - If you have the coordinates for the corners of the canvas, you can probably manage this
	     through code.

	   - Make sure they're rotated correctly. The inside corners/ediges of the frame pieces should
	     line up with the OUTSIDE edge of the canvas, NOT clipping into the canvas.




	2. Create/spawn 4 straight pieces, and snap them to the same corners of the canvas.

	   - Because the straight pieces have the same local pivot point and orientation as corner pieces,
	     you can copy the position and rotation of each corner piece, apply them to each straight piece,
	     and they'll line up in the correct places.




	3. Scale the straight pieces based on the length and width of the canvas.

	   - I'm not 100% sure how this could work through code, as you'd need a way to define which axis
	     of the canvas' size applies to which two straight pieces, but once you get over that, it's as
	     simple as copying the scale of the canvas along one axis and pasting it into the X axis for
	     the straight pieces that follow the axis of the canvas.


	   - If the above description made no sense, imagine that you have...
			- Two straight frame pieces that are parallel to the canvas' X axis (We'll call them
			  X1 and X2)
			- ...and two straight frame pieces that are parallel to the canvas' Y axis (We'll
			  call them Y1 and Y2).

	     You need to apply the local X and Y scale of the canvas to their respective frame pieces, so...
			- Canvas local X scale = X1 and X2's local X scale
			- Canvas local Y scale = Y1 and Y2's local X scale


	     If this still doesn't make any sense, I'm sorry. It's such a basic step, but it's difficult
	     to describe without making it sound overcomplicated.

	     If you need visual aid, I have included an image in the same folder as this file (named
	     "FrameSetup_readmeDEMO1.jpg") which should help (it also helps with visualising how the
	     corner and straight pieces snap onto the canvas).