using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using CombineMesh;

namespace CombineMesh
{
    /// <summary>
    /// Combina todas malhas de objetos Filhos que contenham o mesmo material.
    /// N�o exede o m�ximo de verticies permitido por malha.
    /// Combina malhas com outros combinados para manter o m�ximo de vertices possiveis na mesma malha.
    /// Permite malhas de materiais diferentes
    /// </summary>
    public class CombineMeshChildren : MonoBehaviour
    {
        public int maxVertexForMesh = 3000;
        public bool combineInative = false;
        public bool combineDeep = false;

        //lista de material contendo todos as malhas que a utilizam.
        Dictionary<string, MeshMaterial> meshes = new Dictionary<string, MeshMaterial>();

        IEnumerator corotineCombine = null;

        public void StartCombine()
        {
            if (corotineCombine != null)
            {
                Debug.LogWarning("StartCombine Denied, wait the last process has finishied");
                return;
            }

            corotineCombine = IECombineSameMaterial();
            StartCoroutine(corotineCombine);
        }

        //Categoriza todas as malhas filho por material
        //Combina todos os de mesmo material
        IEnumerator IECombineSameMaterial()
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

            //Cria combina��o de mesh para cada material diferente
            foreach (string material in meshes.Keys)
            {
                meshes[material].CombineMeshes();
            }


            if (!combineDeep)
                yield break;

            //DEEP COMBINATION

            //Agrupa todos os Mesh combinados gerados em um novo MESH final. Este novo Mesh vai conter um MeshRenderer com todos os materiais usados
            //pelos combinados anteriores, e isto vai permitir que o novo mesh mantenha todas os materiais usados anteriormente
            MeshMaterial lastMeshMaterial = new MeshMaterial(maxVertexForMesh);

            //Adiciona todos os materiais que far�o parte deste mesh e tamb�m faz os cobinados individuais serem filhos do novo objeto final
            foreach (string material in meshes.Keys)
            {
                MeshMaterial meshMaterial = meshes[material];
                lastMeshMaterial.AddMeshRender(meshMaterial.partitions.Last().meshRenderer);
            }

            //Combina todos as malhas nesta nova
            lastMeshMaterial.CombineMeshes();
        }


    }

}