# RayTracedContactShadows
 Ray Traced Contact Shadows (DXR) for Unity's Built in render pipeline.

![Screenshot](https://cdn.discordapp.com/attachments/755854193054449665/779404497062723614/unknown.png)


# Usage
Add the RTXCS component to your main camera, assign the main directional light in the scene to the Sun parameter,
and hit Play Mode to see the effect.

# Known issues
Disabling the component wont disable the effect, but stop updating it and showing the frozen frame of the shadow texture.
Currently the only way to disable it after enabling it is setting the Ray Length to 0.
Shadow texture slightly displaced on fast camera movement. This is noticeable when the framerate is low and it may be related to the way that i read and modify the
shadow buffer with the contact shadows. 

# Requirements
As this works with DXR, it wont work on graphic cards that dont support DirectX Ray Tracing, such as Nvidia´s 2000 series or newer or AMD´s 6000 series.
I developed this using a RTX 2060, using Unity version 2020.1.2f1.

# Extra note
Yes, this can be used as a regular hard shadow solution as the ray length can be extended to infinity. Feel free to use it in any way you like!
To disable regular shadows and not break the ray traced ones, enable shadows in the light but set Shadow Intensity to 0.
