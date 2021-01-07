# CombineMesh
Unity MeshCombine combines meshes to decrease the Batches.

[Oficial Doc:](https://docs.unity3d.com/Manual/DrawCallBatching.html)

# What exactly does this do?

OneClick: Mesh combiner that works at run time. It allows deep combinations and maintains multiple materials.

1 - Combines mesh that use the same shared material.

2 - Combines the previous combinations to create new meshes with the maximum possible verticies.

### Features

- Combines meshes of the same material.

![](https://i.gyazo.com/ed7bdc02632e99e34102a6f1fc19dbbc.png)

- Make combinations of combinations keeping the different materials of each sub-combination;


![](https://i.gyazo.com/6f21424e46d737d7ea9b209491642267.png)


- Ensures that the maximum number of vertices stipulated by the user is not extrapolated, creating new meshes automatically to guarantee the limit.

- Works at run time.

![](https://media.giphy.com/media/qwZHLYDWcEV8bVyzNc/giphy.gif)

### HOW TO USE?

All work is done by the **MeshMaterial object.**

*In short, this creates a list of MeshRenderer for each unique type of material. After that, the mesh matching work is started.*

All application logic can be created by the user, but in this repository there are 3 components of practical examples to assist in the first steps.

> 1 - RunTimeTest01.cs: 

This example creates combinations of meshes with the same material.

> 2 - RunTimeTest02.cs:

This example creates depth combinations, combining all previously combined meshes with others. The System gives preference to meshes with the same material, and when it is not possible it will make the combination with meshes that use other materials.

> 3 - CombineMeshChildren.cs

This is the official component, it has the same functionality as example2, but with more configuration parameters.

#TIPS:
For all examples, you need to follow these steps:

- Add the component to a parent object. This object must contain all the children that will be grouped.

![](https://i.gyazo.com/eef742f55c0c3da033ed1b1879f0bc04.png)

You do not have to worry about preventing children from using the same material. The examples will cause the group to be grouped on its own.

####Combine Mesh with same materials　

```c#
    // Combines all meshes of Child objects that contain the same material.
    public class RunTimeTest_01 : MonoBehaviour
    {
        public int maxVertexForMesh = 3000;
        public bool combineInative = false;

        Dictionary<string, MeshMaterial> meshes = new Dictionary<string, MeshMaterial>();

        void Start()
        {
            CombineSameMaterial();
        }

        //Categorizes all child meshes by material
        //Combine all same material
        void CombineSameMaterial()
        {
            MeshRenderer[] meshRendererChildren = GetComponentsInChildren<MeshRenderer>(combineInative);

            foreach (MeshRenderer meshRenderer in meshRendererChildren)
            {
                MeshMaterial meshMaterial = null;

                if (meshes.TryGetValue(meshRenderer.sharedMaterial.name, out meshMaterial))
                    meshMaterial.AddMeshRender(meshRenderer);
                else
                    meshes.Add(meshRenderer.sharedMaterial.name, new MeshMaterial(meshRenderer.sharedMaterial, meshRenderer, maxVertexForMesh));
            }

            //Create combination
            foreach (string material in meshes.Keys)
            {
                meshes[material].CombineMeshes();
            }
        }      
    }
```


