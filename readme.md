# Overview

This project collects multiple compute shader based ray tracing samples for Unity. Because the samples use compute shader for ray tracing, they are not as fast as hardware ray tracing. The main purpose of this project is demonstrating how the ray tracing works. It is very helpfuly for the people who want to know the knowledge behind hardware ray tracing. This project contains following samples:

- Ray Tracing in One Weekend
- Ray Query
- Hybrid Rendering
- Voxel Ray Tracer(upcoming)
- Subsurface Scattering(upcoming)
- Hybrid Ambient Occlusion(upcoming)
- Hybrid reflection and refraction(upcoming)

# Samples

## Ray Tracing in One Weekend 
<img src="./Screenshot.gif" height="333px" width="640px" >
This sample demonstrates how to implement a basic ray tracer using compue shader. The implementation is based on

[Ray Tracing in One Weekend](https://raytracing.github.io/books/RayTracingInOneWeekend.html) article.

### Image Quality Compare

Samples per pixel  : 64

Maximum tracing depth : 64

![](Screenshot_64x64.jpg)


Samples per pixel  : 16

Maximum tracing depth : 6

![](Screenshot.jpg)

## Ray Query
<img src="./RayQuery_Screenshot.gif" height="333px" width="640px" >

This sample demonstrates how to implement ray query using compue shader. This sample uses simple ray tracing for shading and ray query for shadow.

![](RayQuery_Screenshot.jpg)

## Hybrid Rendering
<img src="./HybridRendering_Screenshot.gif" height="333px" width="640px" >

This sample demonstrates how to use ray query in fragment shader to render shadow. This sample uses traditional rasterizing for shading and ray query for shadow. That is why it is called hybrid rendering.

![](HybridRendering_Screenshot.jpg)






 
