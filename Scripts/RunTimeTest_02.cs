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
    /// Não exede o máximo de verticies permitido por malha.
    /// </summary>
    public class RunTimeTest_02 : MonoBehaviour
    {
        public int maxVertexForMesh = 3000;
        public bool combineInative = false;

        Dictionary<string, MeshMaterial> meshes = new Dictionary<string, MeshMaterial>();

        void Start()
        {
            //Categoriza todos os mesh renderer filhos deste objeto por material.
            MeshRenderer[] meshRendererChildren = GetComponentsInChildren<MeshRenderer>(combineInative);

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

            /*
            //Agrupa todos os Mesh combinados gerados em um novo MESH final. Este novo Mesh vai conter um MeshRenderer com todos os materiais usados
            //pelos combinados anteriores, e isto vai permitir que o novo mesh mantenha todas os materiais usados anteriormente
            MeshMaterial lastMeshMaterial = new MeshMaterial("Combined", Vector3.zero);


            //Adiciona todos os materiais que farão parte deste mesh e também faz os cobinados individuais serem filhos do novo objeto final
            foreach (string material in meshes.Keys)
            {
                MeshMaterial meshMaterial = meshes[material];
                lastMeshMaterial.AddMeshRender(meshMaterial.meshRenderer);
            }

            //Combina todos as malhas nesta nova
            lastMeshMaterial.CombineMeshes();
            */

        }


    }

}