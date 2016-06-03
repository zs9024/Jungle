using UnityEngine;
using System.Collections;

public abstract class Hero : Creature 
{
    protected string heroName;
    protected GameObject heroGo;
    protected GameObject camGo;
    protected GameObject weaponTrailGo;

    public static Camera thirdCam { get;private set; }
    protected WeaponTrail weaponTrail;
    protected Material matWeaponTrail;

    protected override void Init()
    {
        base.Init();
        Debug.Log("Hero Init()...");
    }

    protected override void OnShow()
    {
        base.OnShow();

        heroGo = trans.Find(heroName).gameObject;
        camGo = trans.Find("Camera").gameObject;
        weaponTrailGo = heroGo.transform.Find2("weapon_trail", false).gameObject;

        initCamera();
        initWeaponTrail();
    }

    private void initCamera()
    {
        if(camGo != null)
        {
            var camFollow = camGo.AddComponent<CameraFollow>();
            camFollow.Target = heroGo;
            camFollow.RotateX = 45f;
            camFollow.RotateY = 180f;
            camFollow.Distance = 5f;
            camFollow.TargetOffset = new Vector3(0, 1.5f, 0);

            thirdCam = camGo.GetComponent<Camera>();
        }
    }

    private void initWeaponTrail()
    {
        if(weaponTrailGo != null)
        {
            weaponTrail = weaponTrailGo.AddComponent<WeaponTrail>();
            weaponTrail.height = 0.2f;
            weaponTrail.time = 0.6f;
            weaponTrail.minDistance = 0.1f;
            weaponTrail.timeTransitionSpeed = 1f;
            weaponTrail.desiredTime = 0.5f;

            loadWeaponTrailMat();
        }
    }

    private void loadWeaponTrailMat()
    {
        ResourceManager.LoadMaterial(ABConfig.MAT_WEAPONTRAIL,(Material mat)=> {
            weaponTrailGo.GetComponent<MeshRenderer>().material = mat;
        });
    }
}
