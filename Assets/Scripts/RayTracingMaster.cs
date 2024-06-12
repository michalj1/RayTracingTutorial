using System;
using UnityEngine;

public class RayTracingMaster : MonoBehaviour {
    public ComputeShader RayTracingShader;
    public Texture SkyboxTexture;
    public Light DirectionalLight;

    private RenderTexture _target;
    private Camera _camera;
    private Material _addMaterial;
    private uint _currentSample = 0;

    /* Rendering functions */

    /*
    Calls custom pipeline after Unity pipeline is finished rendering
    */
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        SetShaderParameters();
        Render(destination);
    }

    /*
    Uses shader to render to the screen
    */
    private void Render(RenderTexture destination) {
        // Creates target and informs shader
        InitRenderTexture();
        RayTracingShader.SetTexture(0, "Result", _target);

        // Execute shader with one thread per 8x8 pixel area on the screen
        int numThreadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int numThreadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, numThreadGroupsX, numThreadGroupsY, 1);

        // Display on screen
        if (_addMaterial == null) {
            _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
        }
        _addMaterial.SetFloat("_Sample", _currentSample);
        Graphics.Blit(_target, destination, _addMaterial);
        _currentSample += 1;
    }

    /*
    Creates a render target with the correct dimensions
    */
    private void InitRenderTexture() {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height) {
            if (_target != null) _target.Release();
            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }

    /* 
    Calculates current sample every frame
    */
    private void Update() {
        if (transform.hasChanged) {
            _currentSample = 0;
            transform.hasChanged = false;
        }
    }

    /* Camera functions */

    /*
    Gets camera when scene is loaded
    */
    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    /*
    Sets matrices in the shader
    */
    private void SetShaderParameters() {
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        Vector2 offset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
        RayTracingShader.SetVector("_PixelOffset", offset);
        Vector3 light = DirectionalLight.transform.forward;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(light.x, light.y, light.z, DirectionalLight.intensity));
    }
}
