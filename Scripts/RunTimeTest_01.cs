using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using CombineMesh;

namespace CombineMesh.Tests
{
    /// <summary>
    /// Combina todos malhas de objetos Filhos que contenham o mesmo material.
    /// Não verifica o limite máximo de verticies por malha    /// 
    /// </summary>
    public class RunTimeTest_01 : MonoBehaviour
    {


    
        Dictionary<string, MeshMaterial> meshes = new Dictionary<string, MeshMaterial>();

        void Start()
        {
            //Categoriza todos os mesh renderer filhos deste objeto por material.
            MeshRenderer[] meshRendererChildren = GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer meshRenderer in meshRendererChildren)
            {
                MeshMaterial meshMaterial = null;


                if (meshes.TryGetValue(meshRenderer.sharedMaterial.name, out meshMaterial))
                {

                    meshMaterial.AddMeshRender(meshRenderer);
                }
                else
                {
                    meshes.Add(meshRenderer.sharedMaterial.name, new MeshMaterial(meshRenderer.sharedMaterial, meshRenderer));
                }
            }

            //Cria combinação de mesh para cada material diferente
            foreach (string material in meshes.Keys)
            {
                meshes[material].CombineMeshes();
            }



        }


    }

}