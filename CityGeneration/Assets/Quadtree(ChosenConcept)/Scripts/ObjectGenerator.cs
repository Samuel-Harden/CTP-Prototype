using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] GameObject building_root;
    [SerializeField] CuboidMesh cube_mesh;

    [SerializeField] Texture grass_texture;

    [SerializeField] List<Texture> roof_textures;
    [SerializeField] List<Texture> wall_textures;
    [SerializeField] List<Texture> slant_textures;


    // Currently just creating two cubes - Will be split out so that it creates one at a time,
    // and following a ruleset will combine them in natural ways
    public GameObject GenerateBuilding(Vector3 _parent_pos, float _size_x, float _size_z)
    {
        bool single_component = false;

        List<GameObject> components = new List<GameObject>();

        // Single block building or double...
        if (Random.Range(0,2) == 0)
            single_component = true;

        // The Base building object
        var object_base = Instantiate(building_root, Vector3.zero, Quaternion.identity);
        components.Add(object_base);

        object_base.AddComponent<BoxCollider>();
        object_base.GetComponent<BoxCollider>().size = new Vector3(_size_x, 1, _size_z);

        object_base.transform.position = new Vector3(_parent_pos.x, 0, _parent_pos.z);

        float width = Random.Range(_size_x / 2, _size_x);
        float length = Random.Range(_size_z / 2, _size_z);
        float height = Random.Range((_size_x + _size_z), ((_size_x * 2) + (_size_z * 2)));

        // Create first Component and Base component....
        var component_1 = cube_mesh.GenerateCuboid(width, height, length);
        components.Add(component_1);

        float height_comp_1 = component_1.GetComponent<Renderer>().bounds.size.y / 2;

        component_1.transform.position = new Vector3(_parent_pos.x, height_comp_1, _parent_pos.z);

        component_1.transform.parent = object_base.transform;

        // If building is two components, create second component
        if (!single_component)
        {
            width = Random.Range(width / 2, width);
            length = Random.Range(length / 2, length);
            height = Random.Range(0, height / 2);

            var component_2 = cube_mesh.GenerateCuboid(width, height, length);
            components.Add(component_2);

            float height_comp_2 = component_1.GetComponent<Renderer>().bounds.size.y +
                component_2.GetComponent<Renderer>().bounds.size.y / 2;

            component_2.transform.position = new Vector3(_parent_pos.x, height_comp_2, _parent_pos.z);

            component_2.transform.parent = object_base.transform;
        }

        // Could just do this for parent, 
        // but may be useful to do all at a later stage (Debugging etc)
        SetTexture(components);

        // Merge components into parents mesh
        object_base.GetComponent<MeshCombine>().CombineMeshes();

        return object_base;
    }


    // Will split out the generation of components, when I have a system in place
    // to manually combine certain types, to create different looking buildings
    /*
    private GameObject GenerateComponent()
    {


        return;
    }*/

    
    // Will eventually just use a plane, rather than a cuboid,
    // as its just a flat surface and doesnt need to be 3D! 
    public GameObject GeneratePark(Vector3 _parent_pos, float _size_x, float _size_z)
    {
        float width = _size_x;
        float length = _size_z;
        float height = 1.0f;

        var component_1 = cube_mesh.GenerateCuboid(width, height, length);

        // The Base building object
        var object_base = Instantiate(building_root, Vector3.zero, Quaternion.identity);

        object_base.AddComponent<BoxCollider>();
        object_base.GetComponent<BoxCollider>().size = new Vector3(_size_x, 1, _size_z);

        float height_comp_1 = component_1.GetComponent<Renderer>().bounds.size.y / 2;

        component_1.transform.position = new Vector3(_parent_pos.x, height_comp_1, _parent_pos.z);

        object_base.transform.position = new Vector3(_parent_pos.x, 0, _parent_pos.z);

        component_1.transform.parent = object_base.transform;

        component_1.GetComponent<Renderer>().material.SetTexture("_RoofTex", grass_texture);
        object_base.GetComponent<Renderer>().material.SetTexture("_RoofTex", grass_texture);

        object_base.GetComponent<MeshCombine>().CombineMeshes();

        return object_base;
    }



    private void SetTexture(List<GameObject> _components)
    {
        int roof = Random.Range(0, roof_textures.Count);
        int wall = Random.Range(0, wall_textures.Count);

        foreach (GameObject obj in _components)
        {
            obj.GetComponent<Renderer>().material.SetTexture("_RoofTex", roof_textures[roof]);
            obj.GetComponent<Renderer>().material.SetTexture("_WallTex", wall_textures[wall]);
        }
    }
}
