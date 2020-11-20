using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class RTXCS : MonoBehaviour
{

    RenderTexture cs;
    RenderTexture blend;
    RayTracingShader shader;
    Material combine;
    Camera cam;
    RayTracingAccelerationStructure _RTAS;
    Renderer[] renderers;

    public float rayLength = 1.0f;

    public Light sun;

    CommandBuffer cmd;

    public enum Resolution
    {
        Eight = 8,
        Quarter = 4,
        Half = 2,
        Full = 1
    }
    public Resolution resolution = Resolution.Half;
    void Start()
    {
        shader = Resources.Load<RayTracingShader>("RTXCS");
        combine = new Material(Shader.Find("Hidden/Combine"));
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
        CheckNulls();
        CheckRenderTexture();
        CreateRTAS();
        shader.SetShaderPass("DxrPass");
     
     
        

    }
    void CheckRenderTexture()
    {   
        if (cs == null)
        {
            cs = new RenderTexture(Screen.width/(int)resolution, Screen.height/ (int)resolution, 16);
            cs.enableRandomWrite = true;
            cs.Create();
            blend = new RenderTexture(Screen.width, Screen.height , 16);
            blend.Create();
            shader.SetTexture("RenderTarget", cs);
            cmd = new CommandBuffer();
            sun.AddCommandBuffer(LightEvent.AfterScreenspaceMask, cmd);
            cmd.CopyTexture(BuiltinRenderTextureType.CurrentActive, blend);
            combine.SetTexture("_SSS", blend);
            cmd.Blit(cs, BuiltinRenderTextureType.CurrentActive, combine);
        }
        else if (cs.width != Screen.width/(int)resolution || cs.height != Screen.height/(int)resolution)
        {
           
            cs.Release();
            cs = new RenderTexture(Screen.width/(int)resolution, Screen.height/(int)resolution, 16);
            cs.enableRandomWrite = true;
            cs.Create();
            blend.Release();
            blend = new RenderTexture(Screen.width , Screen.height , 16);
            blend.Create();
            shader.SetTexture("RenderTarget", cs);
            cmd.Clear();
            sun.RemoveAllCommandBuffers();
            sun.AddCommandBuffer(LightEvent.AfterScreenspaceMask, cmd);
            cmd.CopyTexture(BuiltinRenderTextureType.CurrentActive, blend);
            combine.SetTexture("_SSS", blend);
            cmd.Blit(cs, BuiltinRenderTextureType.CurrentActive, combine);
        }

    }

    void CheckNulls()
    {
        if (shader == null)
        {
            shader = Resources.Load<RayTracingShader>("RTXAO");
        }
        if (combine == null) {
            combine = new Material(Shader.Find("Hidden/Combine"));
        }
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
    }
    void CreateRTAS()
    {
        RayTracingAccelerationStructure.RASSettings settings = new RayTracingAccelerationStructure.RASSettings();
      
        settings.layerMask = ~0;
        
        settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Automatic;
     
        settings.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;

        

        _RTAS = new RayTracingAccelerationStructure(settings);
       
        renderers = FindObjectsOfType<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            _RTAS.AddInstance(renderers[i]);
        }
        _RTAS.Build();


       



    }
   
    void SetShaderProperties()
    {
    
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 100000)).normalized;
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, 100000)).normalized;
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, 100000)).normalized;
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 100000)).normalized;

        shader.SetVector("_TopLeftFrustumDir", topLeft);
        shader.SetVector("_TopRightFrustumDir", topRight);
        shader.SetVector("_BottomLeftFrustumDir", bottomLeft);
        shader.SetVector("_BottomRightFrustumDir", bottomRight);
        shader.SetVector("_Sun", sun.transform.forward);
        shader.SetVector("_WSCP", transform.position);
        shader.SetFloat("_RayLength", rayLength);

        combine.SetTexture("_CS", cs);

        shader.SetAccelerationStructure("_RTAS", _RTAS);
        combine.SetColor("_Ambient", RenderSettings.ambientLight);
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        
      
        CheckNulls();
        CheckRenderTexture();
        SetShaderProperties();

        _RTAS.Update();

        shader.Dispatch("RayGen", cam.pixelWidth/ (int)resolution, cam.pixelHeight/ (int)resolution, 1, cam);
     
        Graphics.Blit(source, destination);
    
    }

}
