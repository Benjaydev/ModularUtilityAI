using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// There is a problem with shaders that use transparency where they often overlap other transparent objects with inproper layering.
// It seems like this problem stops if you add a solid material to an object as well.
// This script solves this problem by adding a solid material when the camera is far enough away, but removing it during the fade process when overlapping is not a problem.
public class TransparencyOverlapFightingSolver : MonoBehaviour
{
    [System.Serializable]
    public struct RendererInfo
    {
        public MeshRenderer renderer;
        public float distance;

        public Material[] doubleMats;
        public Material[] singleMat;
    }

    [SerializeField]
    private RendererInfo[] renderers;

    [SerializeField]
    private Material solidMaterial;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < renderers.Length; i++)
        {
            // Get the distance at which the whole wall is solid
            // Get base distance
            renderers[i].distance = renderers[i].renderer.materials[0].GetFloat("_FadeDistance");
            // Get the furthers distance when wall is fading that it becomes solid
            renderers[i].distance *= (1f + (1f * (renderers[i].renderer.materials[0].GetFloat("_FadeSpeed")+1f)));

            // Allow for two materials
            renderers[i].doubleMats = new Material[2] { renderers[i].renderer.materials[0], solidMaterial };
            renderers[i].singleMat = new Material[1] { renderers[i].renderer.materials[0] };

            renderers[i].renderer.materials = renderers[i].doubleMats;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (transform.position.y >= renderers[i].distance)
            {
                renderers[i].renderer.materials = renderers[i].doubleMats;
            }
            else
            {
                renderers[i].renderer.materials = renderers[i].singleMat;
            }
        }
    }

}
