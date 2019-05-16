using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private WeaponManager weapon_Manager;
    public float fireRate = 15f;
    private float nextTimeToFire;
    public float damage = 20f;
    public float attack_Distance = 3.12f;
    private Animator zoomCameraAnim;
    private bool zoomed;
    private Camera mainCam;
    private GameObject crosshair;
    private Transform target;
    private Transform tree;
    private bool is_Aiming;

    [SerializeField]
    private GameObject arrow_Prefab, spear_Prefab;

    [SerializeField]
    private Transform arrow_Bow_StartPosition;

    void Awake()
    {
        weapon_Manager = GetComponent<WeaponManager>();

        zoomCameraAnim = transform.Find(Tags.LOOK_ROOT)
                                  .transform.Find(Tags.ZOOM_CAMERA).GetComponent<Animator>();

        crosshair = GameObject.FindWithTag(Tags.CROSSHAIR);

        target = GameObject.FindWithTag(Tags.PLAYER_TAG).transform;

        tree = GameObject.FindWithTag(Tags.TREE_TAG).transform;

        mainCam = Camera.main;

    }

	void Update ()
    {
        WeaponShoot();
        ZoomInAndOut();
        WeaponReload();
    }

    void WeaponShoot()
    {
        WeaponHandler weapon = weapon_Manager.GetCurrentSelectedWeapon();

        if (weapon.bullets <= 0 && weapon_Manager.GetCurrentSelectedWeapon().name != "Axe")
        {
            return;
        }

        if (weapon.bulletType == WeaponBulletType.NONE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Vector3.Distance(transform.position, target.position) <= attack_Distance)
                {
                    PlayerStats.wood += 1;
                }
            }
        }

        if(weapon.fireType == WeaponFireType.MULTIPLE)
        {
            if(Input.GetMouseButton(0) && Time.time > nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;

                weapon.ShootAnimation();

                 BulletFired();
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(weapon.tag == Tags.AXE_TAG)
                {
                    weapon.ShootAnimation();
                }

                if(weapon.bulletType == WeaponBulletType.BULLET)
                {
                    weapon.ShootAnimation();

                    BulletFired();
                }
                else
                {
                    if(is_Aiming)
                    {
                        weapon.ShootAnimation();

                        if(weapon.bulletType
                           == WeaponBulletType.ARROW)
                        {
                            ThrowArrowOrSpear(true);
                        }
                        else if(weapon.bulletType
                                  == WeaponBulletType.SPEAR)
                        {
                            ThrowArrowOrSpear(false);
                            weapon.gameObject.SetActive(false);
                        }
                    }
                }

                weapon.bullets -= 1;
            }
        }
    }

    void WeaponReload()
    {
        if (Input.GetKeyDown("r"))
        {
            WeaponHandler weapon = weapon_Manager.GetCurrentSelectedWeapon();

            weapon.bullets = weapon.maxBullets;

            if (weapon.bulletType == WeaponBulletType.SPEAR)
            {
                weapon.gameObject.SetActive(true);
            }
        }
    }

    void ZoomInAndOut()
    {
        if(weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.AIM)
        {
            if(Input.GetMouseButtonDown(1))
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_IN_ANIM);

                crosshair.SetActive(false);
            }

            if (Input.GetMouseButtonUp(1))
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM);

                crosshair.SetActive(true);
            }
        }

        if(weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.SELF_AIM)
        {
            if(Input.GetMouseButtonDown(1))
            {
                weapon_Manager.GetCurrentSelectedWeapon().Aim(true);

                is_Aiming = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                weapon_Manager.GetCurrentSelectedWeapon().Aim(false);

                is_Aiming = false;
            }
        }
    }

    void ThrowArrowOrSpear(bool throwArrow)
    {
        if(throwArrow)
        {
            GameObject arrow = Instantiate(arrow_Prefab);
            arrow.transform.position = arrow_Bow_StartPosition.position;

            arrow.GetComponent<ArrowBowScript>().Launch(mainCam);
        }
        else
        {
            GameObject spear = Instantiate(spear_Prefab);
            spear.transform.position = arrow_Bow_StartPosition.position;

            spear.GetComponent<ArrowBowScript>().Launch(mainCam);
        }
    }

    void BulletFired()
    {
        RaycastHit hit;

        if(Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
        {
            if(hit.transform.tag == Tags.ENEMY_TAG)
            {
                hit.transform.GetComponent<HealthScript>().ApplyDamage(damage);
            }
        }

        weapon_Manager.GetCurrentSelectedWeapon().bullets -= 1;
    }
}
