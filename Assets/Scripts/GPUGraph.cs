using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField, Range(10, 1000)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;
    public enum TransitionMode { Cycle, Random, None }

    float duration;
    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;

    [SerializeField]
    ComputeShader computeShader;
    ComputeBuffer positionsBuffer;
    static readonly int 
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        functionId = Shader.PropertyToID("_Function");
    
    [SerializeField]
    Material material;
    [SerializeField]
    Mesh mesh;

    void OnEnable() {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * sizeof(float));
    }

    void OnDisable() {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning) {
            if (duration >= transitionDuration) {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (transitionMode != TransitionMode.None && duration >= functionDuration) {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        UpdateFunctionOnGPU();
    }

    void UpdateFunctionOnGPU () {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetInt(functionId, (int)function);

        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector2.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }

    void PickNextFunction() {
        function = (transitionMode == TransitionMode.Cycle) ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}