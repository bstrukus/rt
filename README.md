# rt
A backward ray tracer that takes in JSON scene files and outputs images.

## Motivation
Ever since I took a ray tracing course in college, I wanted to build one outside of the constraints of the class. I enjoy the images that ray tracers produce almost as much as I enjoy the way they're produced (as opposed to more traditional graphics engines). This project is largely a learning exercise in architecture, unit testing, and class design. I've chosen C# since it's the language I'm currently most familiar with, but I could see this slowly converting over to C++ libraries as I develop more of the project.


Thanks for stopping by!

## How To Run
### Requirements
I used Microsoft Visual Studio Community 2019 to create this project, though if you're code-savvy you might be able to compile it with whatever tools you prefer.

### Steps
1. Clone the project
2. Open up `rt.sln`
3. Press `F5`
- Optional
  - Tweak `dev_config.json` to change how detailed you want the image, and to specify which scene you want to render.
  - Keep reading if you want to understand the specifics.

## Data Files
### Paths
`rt` is nearly completely data-driven, it reads in files from the following locations in the given priority order.
1. The same directory as the `exe`
2. The root of the development directory, called `<src_root>`
3. The `<src_root>\scenes` directory
4. The `<src_root>\scenes\unit_tests` directory
5. The `<src_root>\scenes\unconverted` directory

Two types of files are supported, one for the config that `rt` takes as a command line argument, and within that file there's the specified scenes.

### Config
 
  Config files contain information on how to generate the image. In this file you specify:
  - _See `<src_root>\dev_config.json` as an example._

  - `width` - The number of pixels for the width of the image. The height is calculated from the aspect ratio of the projection plane in the scene file.
  - `renderDepth` - The max number of bounces each ray can make, the higher this number the more light detail is calculated for each pixel at the cost of render speed.
  - `output` - What you want the generated file to be called, unused currently.
  - `sceneFile` - A list of scenes that you want to render, all of which use the same `width` and `renderDepth` values.
  
  ### Scene
  Scene files contain information about how the scene is constructed, specifically:
  - _See any file in `<src_root>\scenes\` as an example._

  - `camera` - Represents the viewpoint through which we observe the scene.
    - `eyeDirection` - Direction vector specifying the camera's eye position relative to the projection plane's center.
    - `projectionPlane` - The surface through which we view the scene, used in conjunction with the camera's eye position to generate the initial rays that we cast.
      - `center` - Center of the projection plane, used for per-pixel location calculations.
      - `uAxis` - Horizontal axis of the projection plane, length is half the width of the projection plane.
      - `vAxis` - Vertical axis of the projection plane, length is half the height of the projection plane.

 - `shapes`
   - `material` - All shapes contain this set of properties that describe how they appear, basic explainations are given but correct terms are used if you want to search online for more info.
        - `transmissionAttenuation` - The type of color produced by light that goes through the object, RGB values in the range of `[0, 1]`
        - `diffuse` - The surface color of the object, RGB values in the range of `[0, 1]`
        - `specularCoefficient` - How shiny the object appears, values in the range of `[0, 1]`
        - `specularExponent` - How big the shiny dot appears, values usually `>1`
        - `electricPermittivity` - Used to calculate the air's index of refraction.
        - `magneticPermeability` - Used to calculate the air's index of refraction and reflection coefficients.
   - `spheres` - Point and radius
   - `boxes` - Corner point and 3 scaled direction vectors. Vectors don't need to be orthogonal, which will result in parallelpipeds.
   - `polygons` - List of vertices specified in a fan format (first point is the anchor, every consecutive set of 2 points after the first are used to create individual triangles with the first).
   - `ellipsoids` - Point and 3 scaled direction vectors. Vectors are assumed to be orthogonal.
 
 - `lights`
   - `points`
     - `transform\position` - Outdated format of specifying position of the light, will be `center` soon.
     - `color` - Color emitted by the light, RGB values in the range of `[0, 1]`
     - `radius` - Size of the light, will be used in future shadow test feature.
 
 - `ambient` - Ambient light diffuse color, represented by 3 floating point values in the RBG format.
 - `air` - The medium that light travels that isn't a shape.
    - `attenuation` - The "color" of the air, energy is lost as light travels through a medium and air is no different.
   - `electricPermittivity` - Used to calculate the air's index of refraction.
   - `magneticPermeability` - Used to calculate the air's index of refraction and reflection coefficients.