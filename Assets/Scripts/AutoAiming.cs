using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UI;

public class AutoAiming : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] Image crosshairIdle;
    [SerializeField] Image crosshairNoHit;
    [SerializeField] Image crosshairRedHit;
    [SerializeField] Image crosshairWhiteHit;
    [SerializeField] Image avatarCrosshairX;
    [SerializeField] Image avatarCrosshairO;
    [SerializeField] TrailRenderer bulletTracer;
    [SerializeField] FloatReference pistolCurrentEnergy = null;
    [SerializeField] FloatReference pistolEnergyConsumption = null;

    const float debugDrawLineDuration = 0.1f;
    //FIXME: ideally, there would be a programmatic way to know
    //  what the diameter of a crosshair is, and set this value to it.
    //FIXME: right now, the distance is a 3D distance in the game world,
    //  whereas it should be the distance of the two icons on the 2D screen space.
    const float minCrosshairDistance = 0.01f;

    Ray ray1;
    Ray ray2;
    RaycastHit hitInfo1;
    RaycastHit hitInfo2;
    Camera cam;
    Target target1;
    Target target2;

    private void Awake()
    {
        target1 = null;
        target2 = null;
        crosshairIdle.transform.gameObject.SetActive(false);
        crosshairNoHit.transform.gameObject.SetActive(false);
        crosshairRedHit.transform.gameObject.SetActive(false);
        crosshairWhiteHit.transform.gameObject.SetActive(false);
        avatarCrosshairX.transform.gameObject.SetActive(false);
        avatarCrosshairO.transform.gameObject.SetActive(false);
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public Target StartAiming()
    {
        target1 = null;
        target2 = null;
        crosshairIdle.transform.gameObject.SetActive(true);
        crosshairNoHit.transform.gameObject.SetActive(false);
        crosshairRedHit.transform.gameObject.SetActive(false);
        crosshairWhiteHit.transform.gameObject.SetActive(false);
        avatarCrosshairX.transform.gameObject.SetActive(false);
        avatarCrosshairO.transform.gameObject.SetActive(false);

        ray1.origin = transform.position;
        ray1.direction = transform.forward;
        if (Physics.Raycast(ray1, out hitInfo1))//, Mathf.Infinity, layerMask))
        {
            //Debug.DrawLine(ray1.origin, hitInfo1.point, Color.white, debugDrawLineDuration);

            target1 = hitInfo1.transform.gameObject.GetComponent<Target>();
            if (target1 == null)
            {
                //Perform another raycast parallel to the first raycast starting from the muzzle of the gun
                ray2.origin = muzzle.position;
                ray2.direction = hitInfo1.point - muzzle.position;
                if (Physics.Raycast(ray2, out hitInfo2))
                {
                    //Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red, debugDrawLineDuration);

                    crosshairIdle.transform.gameObject.SetActive(false);
                    crosshairNoHit.transform.gameObject.SetActive(true);
                    target2 = hitInfo2.transform.gameObject.GetComponent<Target>();
                    if (target2 != null)
                    {
                        avatarCrosshairO.transform.gameObject.SetActive(true);
                        Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                        avatarCrosshairO.transform.position = screenPos;
                    }
                }
            }
            else // target1 != null
            {
                //Perform another raycast from the muzzle of the gun to the hitInfo.point
                ray2.origin = muzzle.position;
                ray2.direction = hitInfo1.point - muzzle.position;
                if (Physics.Raycast(ray2, out hitInfo2))
                {
                    if (Vector3.Distance(hitInfo1.point, hitInfo2.point) < minCrosshairDistance)
                    {
                        //Debug.DrawLine(ray2.origin, hitInfo2.point, Color.green, debugDrawLineDuration);
                    }
                    else
                    {
                        //Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red, debugDrawLineDuration);
                    }

                    crosshairIdle.transform.gameObject.SetActive(false);
                    target2 = hitInfo2.transform.gameObject.GetComponent<Target>();
                    if (target2 != null) // and target1 != null
                    {
                        //Since both target1 and target2 are not null, the camera crosshair is a hit.
                        if (Vector3.Distance(hitInfo1.point, hitInfo2.point) < minCrosshairDistance)
                        {
                            crosshairRedHit.transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            crosshairWhiteHit.transform.gameObject.SetActive(true);
                            avatarCrosshairO.transform.gameObject.SetActive(true);
                            Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                            avatarCrosshairO.transform.position = screenPos;
                        }
                    }
                    else // target2 == null (and target1 != null)
                    {
                        if (Vector3.Distance(hitInfo1.point, hitInfo2.point) < minCrosshairDistance)
                        {
                            //The idea here, is to have a white circle with a red cross inside (and under) it.
                            crosshairIdle.transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            crosshairWhiteHit.transform.gameObject.SetActive(true);
                        }
                        avatarCrosshairX.transform.gameObject.SetActive(true);
                        Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                        avatarCrosshairX.transform.position = screenPos;
                    }
                }
            }
        }
        else
        {
            target1 = null;
            //Perform another raycast parallel to the first raycast starting from the muzzle of the gun
            ray2.origin = muzzle.position;
            ray2.direction = ray1.direction;
            if (Physics.Raycast(ray2, out hitInfo2))
            {
                target2 = hitInfo2.transform.gameObject.GetComponent<Target>();
                if (target2 != null)
                {
                    //Debug.DrawLine(ray2.origin, hitInfo2.point, Color.green, debugDrawLineDuration);

                    avatarCrosshairO.transform.gameObject.SetActive(true);
                    Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                    avatarCrosshairO.transform.position = screenPos;
                }
                else
                {
                    //Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red, debugDrawLineDuration);

                    avatarCrosshairX.transform.gameObject.SetActive(true);
                    Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                    avatarCrosshairX.transform.position = screenPos;
                }
            }
        }

        return target2;
    }

    public void StopAiming()
    {
        target1 = null;
        target2 = null;
        crosshairIdle.transform.gameObject.SetActive(false);
        crosshairNoHit.transform.gameObject.SetActive(false);
        crosshairRedHit.transform.gameObject.SetActive(false);
        crosshairWhiteHit.transform.gameObject.SetActive(false);
        avatarCrosshairX.transform.gameObject.SetActive(false);
        avatarCrosshairO.transform.gameObject.SetActive(false);
    }

    const int N = 4;
    float distanceN;
    float initialOffset = 0;
    float offsetSpeedChange = 20f;
    TrailRenderer tracer;
    public void StartFiring(Target target)
    {
        pistolCurrentEnergy.Value -= pistolEnergyConsumption.Value;
        if (pistolCurrentEnergy.Value < 0)
            pistolCurrentEnergy.Value = 0;

        distanceN = Vector3.Distance(muzzle.position, hitInfo2.point) / N;
        initialOffset += offsetSpeedChange * Time.deltaTime;
        initialOffset %= distanceN;

        tracer = Instantiate(bulletTracer, muzzle.position, muzzle.rotation);
        tracer.AddPosition(muzzle.position);
        for (var index = 0; index < N; index++)
        {
            tracer.AddPosition(ray2.GetPoint(initialOffset + index * distanceN));
        }
        tracer.AddPosition(hitInfo2.point);
        tracer.transform.position = hitInfo2.point;

        //FIXME: it should really be the weapon used to shoot that determines the intensity
        //  of the hit, but for the time being this hardcoded value will do.
        target.Hit(1);
    }
}
