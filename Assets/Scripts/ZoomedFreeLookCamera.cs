using UnityEngine;
using Cinemachine;

public class ZoomedFreeLookCamera : MonoBehaviour
{
    [SerializeField] float duration; //0.2f
    [SerializeField] float radiusShrinkageFactor; //2f
    [SerializeField] float heightUpperShrinkageFactor; //1.9f
    [SerializeField] float heightMidShrinkageFactor; //1.7f
    [SerializeField] float heightLowerShrinkageFactor; //1.5f
    [SerializeField] Vector3 zoomedInLateralAimOffset;
    [SerializeField] Vector3 zoomedOutLateralAimOffset;

    public Vector3 ZoomedInLateralAimOffset { get => zoomedInLateralAimOffset; set => zoomedInLateralAimOffset = value; }

    CinemachineFreeLook vcam;

    float maxUpperRadius; //The upper radius is assumed to start at this value.
    float maxMidRadius; //The mid radius is assumed to start at this value.
    float maxLowerRadius; //The lower radius is assumed to start at this value.
    
    float minUpperRadius;
    float minMidRadius;
    float minLowerRadius;

    float maxUpperHeight; //The upper height is assumed to start at this value.
    float maxMidHeight; //The mid height is assumed to start at this value.
    float maxLowerHeight; //The lower height is assumed to start at this value.

    float minUpperHeight;
    float minMidHeight;
    float minLowerHeight;

    float startUpperRadius;
    float startMidRadius;
    float startLowerRadius;

    float targetUpperRadius;
    float targetMidRadius;
    float targetLowerRadius;

    float startUpperHeight;
    float startMidHeight;
    float startLowerHeight;

    float targetUpperHeight;
    float targetMidHeight;
    float targetLowerHeight;

    CinemachineLateralAim lateralAim;
    
    Vector3 startLateralAimOffset;
    Vector3 targetLateralAimOffset;

    float timeElapsed = 0;

    enum ZoomDirection
    {
        ZOOM_IN  = -1,
        NO_ZOOM  =  0,
        ZOOM_OUT = +1,
    }
    ZoomDirection zoomDir = ZoomDirection.NO_ZOOM;

    void Awake()
    {
        vcam = GetComponent<CinemachineFreeLook>();

        maxUpperRadius = vcam.m_Orbits[0].m_Radius; //The initial upper radius is assumed to be at the max radius.
        maxMidRadius = vcam.m_Orbits[1].m_Radius; //The initial mid radius is assumed to be at the max radius.
        maxLowerRadius = vcam.m_Orbits[2].m_Radius; //The initial lower radius is assumed to be at the max radius.

        minUpperRadius = maxUpperRadius / radiusShrinkageFactor;
        minMidRadius = maxMidRadius / radiusShrinkageFactor;
        minLowerRadius = maxLowerRadius / radiusShrinkageFactor;

        maxUpperHeight = vcam.m_Orbits[0].m_Height; //The initial upper height is assumed to be at the max height.
        maxMidHeight = vcam.m_Orbits[1].m_Height; //The initial mid height is assumed to be at the max height.
        maxLowerHeight = vcam.m_Orbits[2].m_Height; //The initial lower height is assumed to be at the max height.

        minUpperHeight = maxUpperHeight / heightUpperShrinkageFactor;
        minMidHeight = maxMidHeight / heightMidShrinkageFactor;
        minLowerHeight = maxLowerHeight / heightLowerShrinkageFactor;

        lateralAim = GetComponent<CinemachineLateralAim>();
        lateralAim.offset = zoomedOutLateralAimOffset;
    }

    public void Zoom(bool isAimingPressed)
    {
        if (isAimingPressed)
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
    }

    void ZoomIn()
    {
        if (zoomDir != ZoomDirection.ZOOM_IN && vcam.m_Orbits[1].m_Radius > minMidRadius)
        {
            zoomDir = ZoomDirection.ZOOM_IN;
            timeElapsed = (maxMidRadius - vcam.m_Orbits[1].m_Radius) / (maxMidRadius - minMidRadius) * duration;

            startUpperRadius = vcam.m_Orbits[0].m_Radius;
            startMidRadius = vcam.m_Orbits[1].m_Radius;
            startLowerRadius = vcam.m_Orbits[2].m_Radius;
            
            targetUpperRadius = minUpperRadius;
            targetMidRadius = minMidRadius;
            targetLowerRadius = minLowerRadius;

            startUpperHeight = vcam.m_Orbits[0].m_Height;
            startMidHeight = vcam.m_Orbits[1].m_Height;
            startLowerHeight = vcam.m_Orbits[2].m_Height;

            targetUpperHeight = minUpperHeight;
            targetMidHeight = minMidHeight;
            targetLowerHeight = minLowerHeight;

            startLateralAimOffset = lateralAim.offset;
            targetLateralAimOffset = zoomedInLateralAimOffset;
        }
    }

    void ZoomOut()
    {
        if (zoomDir != ZoomDirection.ZOOM_OUT && vcam.m_Orbits[1].m_Radius < maxMidRadius)
        {
            zoomDir = ZoomDirection.ZOOM_OUT;
            timeElapsed = (vcam.m_Orbits[1].m_Radius - minMidRadius) / (maxMidRadius - minMidRadius) * duration;

            startUpperRadius = vcam.m_Orbits[0].m_Radius;
            startMidRadius = vcam.m_Orbits[1].m_Radius;
            startLowerRadius = vcam.m_Orbits[2].m_Radius;
            
            targetUpperRadius = maxUpperRadius;
            targetMidRadius = maxMidRadius;
            targetLowerRadius = maxLowerRadius;

            startUpperHeight = vcam.m_Orbits[0].m_Height;
            startMidHeight = vcam.m_Orbits[1].m_Height;
            startLowerHeight = vcam.m_Orbits[2].m_Height;

            targetUpperHeight = maxUpperHeight;
            targetMidHeight = maxMidHeight;
            targetLowerHeight = maxLowerHeight;

            startLateralAimOffset = lateralAim.offset;
            targetLateralAimOffset = zoomedOutLateralAimOffset;
        }
    }

    void LateUpdate()
    {
        if (zoomDir != 0)
        {
            vcam.m_Orbits[0].m_Radius = Mathf.Lerp(startUpperRadius, targetUpperRadius, timeElapsed / duration);
            vcam.m_Orbits[1].m_Radius = Mathf.Lerp(startMidRadius, targetMidRadius, timeElapsed / duration);
            vcam.m_Orbits[2].m_Radius = Mathf.Lerp(startLowerRadius, targetLowerRadius, timeElapsed / duration);
            vcam.m_Orbits[0].m_Height = Mathf.Lerp(startUpperHeight, targetUpperHeight, timeElapsed / duration);
            vcam.m_Orbits[1].m_Height = Mathf.Lerp(startMidHeight, targetMidHeight, timeElapsed / duration);
            vcam.m_Orbits[2].m_Height = Mathf.Lerp(startLowerHeight, targetLowerHeight, timeElapsed / duration);
            lateralAim.offset.x = Mathf.Lerp(startLateralAimOffset.x, targetLateralAimOffset.x, timeElapsed / duration);
            lateralAim.offset.y = Mathf.Lerp(startLateralAimOffset.y, targetLateralAimOffset.y, timeElapsed / duration);
            lateralAim.offset.z = Mathf.Lerp(startLateralAimOffset.z, targetLateralAimOffset.z, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            if (zoomDir == ZoomDirection.ZOOM_IN && vcam.m_Orbits[1].m_Radius <= 1.01f * minMidRadius)
            {
                zoomDir = ZoomDirection.NO_ZOOM;
                vcam.m_Orbits[0].m_Radius = minUpperRadius;
                vcam.m_Orbits[1].m_Radius = minMidRadius;
                vcam.m_Orbits[2].m_Radius = minLowerRadius;
                vcam.m_Orbits[0].m_Height = minUpperHeight;
                vcam.m_Orbits[1].m_Height = minMidHeight;
                vcam.m_Orbits[2].m_Height = minLowerHeight;
                lateralAim.offset = zoomedInLateralAimOffset;
            }
            else if (zoomDir == ZoomDirection.ZOOM_OUT && vcam.m_Orbits[1].m_Radius >= 0.99f * maxMidRadius)
            {
                zoomDir = ZoomDirection.NO_ZOOM;
                vcam.m_Orbits[0].m_Radius = maxUpperRadius;
                vcam.m_Orbits[1].m_Radius = maxMidRadius;
                vcam.m_Orbits[2].m_Radius = maxLowerRadius;
                vcam.m_Orbits[0].m_Height = maxUpperHeight;
                vcam.m_Orbits[1].m_Height = maxMidHeight;
                vcam.m_Orbits[2].m_Height = maxLowerHeight;
                lateralAim.offset = zoomedOutLateralAimOffset;
            }
        }
    }
}