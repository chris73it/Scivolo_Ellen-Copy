using Cinemachine;
using UnityEngine;

public class CinemachineLateralAim : CinemachineExtension
{
    [HideInInspector] public Vector3 offset;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Aim)
        {
            state.PositionCorrection += state.FinalOrientation * offset;
        }
    }
}
