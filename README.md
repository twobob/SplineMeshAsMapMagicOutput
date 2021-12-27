# SplineMeshAsMapMagicOutput
Makes SplineMesh be available as an output in MapMagic with integrated UI.

SplineMesh is https://github.com/benoit-dumas/SplineMesh

In essence it is just a fancy wrapper for the ExampleSower script.

Contains a few bits from the MeshUtils class by zulfajuniadi also. 

This is not carefully put together, simply ripped wholesale from the TownGenerator project for those who might want it.  It could be very easily improved upon and certainly needs a cleanup.

The splines upon which the splineMesh is acting

![image](https://user-images.githubusercontent.com/915232/147429917-636e1311-b599-4bad-8dc1-5e34226db4b0.png)

The resulting scripts which are just wrapped ExampleGrower scripts

![image](https://user-images.githubusercontent.com/915232/147430006-daedf37b-f6ef-42b0-b65d-63d0d2fc7eae.png)


NB: When using infinite mode or unpinned tiles MapMagic simply moves unused tiles to become the new ones, thus one has to clear down the objects.
There are events in MapMagic which you can utilise to do this.
