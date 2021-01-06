using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace CombineMesh
{
    /// <summary>
    /// Utilize isto para criar <see cref="Mesh"/> combinados.
    /// Com este objeto é possivel combinar todas as Mesh que utilizam um mesmo material.
    /// Com este objeto também é possivel combinar todas as combinacoes de materiais iguais feitas anteriormente em uma nova malha.
    /// Neste caso os materiais diferentes serão preservados.
    /// </summary>
    public class MeshMaterial
    {
        private bool differentMaterials = false;
        public bool AllowDifferentMaterials { get { return differentMaterials; } }
        /// <summary>
        /// <seealso cref="GameObject"/> criado para receber o novo Mesh combinado.
        /// </summary>
        public GameObject gameObject;
        /// <summary>
        /// Este MeshFilter possui o novo <see cref="Mesh"/>combinado
        /// </summary>
        public MeshFilter meshFilter;
        /// <summary>
        /// Possui o material origem, compatilhado por todos os <see cref="MeshRenderer"/> da lista <see cref="meshRenderers"/>.
        /// 
        /// O SharedMaterial deste MeshRender será o mesmo material do <see cref="sharedMaterial"/>.
        /// 
        /// A Lista de SharedMaterial deste MeshRender vai conter todos os Materiais diferentes caso haja 
        /// sharedMaterial diferente entre os MeshRender da lista.
        /// </summary>
        public MeshRenderer meshRenderer;
        /// <summary>
        /// Este é o material compartilhado por todos os <see cref="MeshRenderer"/> da lista <see cref="meshRenderers"/>.
        /// 
        /// Este material sera o mesmo do sharedMaterial do <see cref="meshRenderer"/> se a lista permitir somente materiais iguais
        /// 
        /// O Material pode ser nulo caso este objeto tenha sido criado por <see cref="MeshRenderer"/> com sharedMaterial diferentes.
        /// </summary>
        public Material sharedMaterial;
        /// <summary>
        /// Lista de todos os MeshRenderer que compartilham o mesmo material.
        /// Esta lista pode conter apenas MeshRender com sharedMaterial iguais e ou apenas materiais diferente.
        /// O Material compartilhado pode ser verificado em <see cref="sharedMaterial"/> se esta for uma lista de sharedMaterial iguais.
        /// </summary>
        public List<MeshRenderer> meshRenderers;
        /// <summary>
        /// Lista de todos os materiais existentes em <see cref="meshRenderers"/>. 
        /// Esta lista pode conter todos os materiais iguais e ou todos diferentes.
        /// </summary>
        public List<Material> sharedMaterials
        {
            get
            {
                if (meshRenderers.Count == 0)
                    return new List<Material>();
                if (meshRenderers == null)
                    return new List<Material>();

                return meshRenderers.Where(r => r.sharedMaterial != null).Select(r => r.sharedMaterial).ToList();
            }
        }
        /// <summary>
        /// Esta é malha criada que possui os todos os <see cref="meshRenderers"/> combinados.
        /// </summary>
        public Mesh combinedMesh
        {
            get
            {

                if (meshFilter.mesh == null)
                    meshFilter.mesh = new Mesh();

                return meshFilter.mesh;

            }
        }

        /// <summary>
        /// Cria um novo MeshMaterial que permite apenas <see cref="MeshRenderer"/> que possuem sharedMaterial iguais.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="fromMeshRenderer"></param>
        public MeshMaterial(Material material, MeshRenderer fromMeshRenderer)
        {
            gameObject = new GameObject(fromMeshRenderer.gameObject.name);
            gameObject.transform.position = fromMeshRenderer.gameObject.transform.position;
            fromMeshRenderer.gameObject.transform.parent = gameObject.transform;

            this.meshFilter = gameObject.AddComponent<MeshFilter>();
            //this.meshFilter.mesh = new Mesh();


            this.meshRenderer = gameObject.AddComponent<MeshRenderer>();
            this.meshRenderer.sharedMaterial = fromMeshRenderer.sharedMaterial;

            this.meshRenderers = new List<MeshRenderer>();
            this.sharedMaterial = material;

            this.meshRenderers.Add(fromMeshRenderer);
        }
        /// <summary>
        /// Cria um novo MeshMaterial que permite apenas <see cref="MeshRenderer"/> que possuem sharedMaterial diferente.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        public MeshMaterial(string name, Vector3 position)
        {
            differentMaterials = true;
            gameObject = new GameObject(name);
            gameObject.transform.position = position;

            this.meshFilter = gameObject.AddComponent<MeshFilter>();
            //this.meshFilter.mesh = new Mesh();

            this.meshRenderer = gameObject.AddComponent<MeshRenderer>();
            this.meshRenderers = new List<MeshRenderer>();
            this.sharedMaterial = null;
        }
        /// <summary>
        /// Adiciona um novo MeshRender à lista. Observe <see cref="AllowDifferentMaterials"/> para saber que tipo de <see cref="MeshRenderer"/>
        /// é permitido.
        /// 
        /// Esta lista será usada para criar um novo <see cref="Mesh"/> combinado.
        /// 
        /// O Objeto será adicionado como filho do <seealso cref="gameObject"/>.
        /// 
        /// Se a lista <see cref="MeshRenderer"/> for de sharedMaterial Diferentes, a lista de material de <see cref="meshRenderer"/> será atualiada
        /// para conter o sharedMaterial deste meshRenderer adicionado.
        /// </summary>
        /// <param name="meshRenderer"></param>
        public void AddMeshRender(MeshRenderer meshRenderer)
        {

            if (sharedMaterials.Contains(meshRenderer.sharedMaterial) == true && differentMaterials == true)
            {
                Debug.LogError(meshRenderer.name + " couldnt added to list because this MeshMaterial support only different sharedMaterial");
                return;
            }
            else if (sharedMaterials.Contains(meshRenderer.sharedMaterial) == false && differentMaterials == false)
            {
                Debug.LogError(meshRenderer.name + " couldnt added to list because this MeshMaterial support only same sharedMaterial");
                return;
            }

            this.meshRenderers.Add(meshRenderer);
            meshRenderer.gameObject.transform.parent = gameObject.transform;

            if (differentMaterials)
            {
                this.meshRenderer.sharedMaterials = sharedMaterials.ToArray();
            }
        }


        public void CombineMeshes()
        {
            if (differentMaterials)
                CombineDifferentMaterials();
            else
                CombineSameMaterials();
        }
        private void CombineDifferentMaterials()
        {
            //Create the array that will form the combined mesh
            CombineInstance[] totalMesh = new CombineInstance[meshRenderers.Count];

            for (int i = 0; i < totalMesh.Length; i++)
            {
                //Add the submeshes in the same order as the material is set in the combined mesh
                totalMesh[i].mesh = meshRenderers[i].GetComponent<MeshFilter>().mesh;
                totalMesh[i].transform = meshRenderers[i].gameObject.transform.localToWorldMatrix;
                meshRenderers[i].gameObject.SetActive(false);
            }
            //Create the final combined mesh
            // Mesh combinedAllMesh = new Mesh();
            //Make sure it's set to false to get 2 separate meshes
            //lestMashMaterial.meshFilter.mesh = new Mesh();
            meshFilter.mesh.CombineMeshes(totalMesh, false, true);

            gameObject.AddComponent<MeshCollider>();
        }
        private void CombineSameMaterials()
        {
            Vector3 position = gameObject.transform.position;
            gameObject.transform.position = Vector3.zero;

            //Get all mesh filters and combine
            List<MeshFilter> meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true).ToList();
            meshFilters.RemoveAll(r => r.sharedMesh == null);

            CombineInstance[] combine = new CombineInstance[meshFilters.Count];

            int i = 0;
            while (i < meshFilters.Count)
            {

                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);

                i++;
            }

            //meshFilter.mesh = new Mesh();
            meshFilter.mesh.CombineMeshes(combine, true, true);

            //Return to original position
            gameObject.transform.position = position;
            gameObject.SetActive(true);

            //Add collider to mesh (if needed)
            gameObject.AddComponent<MeshCollider>();
        }
    }
}