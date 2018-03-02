using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Texture texture;

    Renderer renderer;

	// Use this for initialization
	void Start ()
    {
        //material = this.gameObject.GetComponent<Material>();
        renderer = GetComponent<Renderer>();

        Debug.Log(renderer.material.shader.ToString());

        // Pass in name of texture component in Shader & new Texture
        renderer.material.SetTexture("_SlopeTex", texture);

        //material.shader.
	}

}
