using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using CombineMesh;

namespace CombineMesh.Tests
{
    /// <summary>
    /// Combina todas malhas de objetos Filhos que contenham o mesmo material.
    /// Não exede o máximo de verticies permitido por malha.
    /// </summary>
    public class RunTimeTest_01 : MonoBehaviour
    {
        public int maxVertexForMesh = 3000;
        public bool combineInative = false;

        Dictionary<string, MeshMaterial> meshes = new Dictionary<string, MeshMaterial>();

        void Start()
        {
            CombineSameMaterial();
        }

        //Categoriza todas as malhas filho por material
        //Combina todos os de mesmo material
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

            //Cria combinação de mesh para cada material diferente
            foreach (string material in meshes.Keys)
            {
                meshes[material].CombineMeshes();
            }

        }
        


    }

}