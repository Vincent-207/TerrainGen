# Types of terrain generation:

## Procedural
- Generated using mathematical equations
- Can be generated once, or at runtime.
## Authored
- Created by hand by environment artists. Often by using tools to modify procedurally generated terrain.
- loaded from memory

Terrain generation is most often used to create as realistic, lifelike terrain as possible. But it can also be used to create heavily stylized worlds.

# Algorithm Criteria

I will focus on investigating procedural terrain generation algorithms that meet the following criteria:

## Independently generated 
Some forms of terrain generation like erosion, in addition to being performance costly, need to reference neighboring terrain. This makes them very performance heavy and limits you to not generating new terrain after initially generating.
## Able to be generated at runtime
The terrain should be able to be generated at runtime. Significant performance cost will be acceptable. I will focus on implementing unoptimized versions of several algorithms. Optimization is often one of the hardest parts of terrain generation.
## Realism focused
The terrain generation will be focused on generating ‘realistic’ looking terrain, rather than heavily stylized terrain. 
## Retain artistic control
In addition to being able to manually author terrain using tools. The base generation should be some what easily be able to be customized by artists


Algorithms I won't cover:

- Errosion (Can't be generated independently. Can't be generate at runtime.)

