Image analysis: homography: planar homography.
Implementation independent of OpenCV, calculates it directly. Using C# .NET and using MathNet.Numerics.LinearAlgebra;
If images of different sizes are meant to be used, change Coordinate transformations and also the real world sizes. This is an image of chess board, where real world cell size is measured in cm, it is set in the code for the real world size 7 cm * 9.3cm.

Don't forget to change your monitor zoom to 100%. Default value is usually set to 125%, which will spoil the projection - GDI is not able to deal with it correctly, but it is not important, since the only thing we care about is the H matrix at the end, and the technology used to draw the pictures is irrelevant.

We are using calculated H matrix for line following robot, to calculate correct track segment lengths. 

![image_2025-04-11_114032780](https://github.com/user-attachments/assets/a6c1f824-785d-4b6d-8ddb-77b8fee0788c)

