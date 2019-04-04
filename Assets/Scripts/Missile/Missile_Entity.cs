using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Entity : MonoBehaviour {

    private float missileSpeed;
    private int missileDamage;
    private int explosionDamage;

    private float stopDelta = 0f;
    private float explosionDelta = 0f;

    private bool isFire = false;
    private bool isExplode = false;

    private Vector3 dir;
    private Vector3 pos;

    public Collider missileCol;
    public Collider explosionCol;
    public Transform colPos;
    public GameObject projectile;
    public GameObject tail;
    public GameObject explosion;
    public AudioClip shootAudio;
    public AudioClip explosionAudio;

    public Collision_Manager.ColliderData colliderData = new Collision_Manager.ColliderData();

    public void StartMissile(float speed, float stopTime, float explosionTime, int hitDamage, int rangeDamage, string tag)
    {
        isFire = true;
        isExplode = false;

        missileSpeed = speed;
        missileDamage = hitDamage;
        explosionDamage = rangeDamage;

        stopDelta = stopTime;
        explosionDelta = explosionTime;

        missileCol.gameObject.SetActive(true);

        gameObject.tag = tag;
        missileCol.tag = tag;
        explosionCol.tag = tag;

        if (projectile)
        {
            projectile.SetActive(true);
            var m = projectile.GetComponent<ParticleSystem>().main;
            m.startLifetime = stopTime;
            m.startSpeed = speed;
            projectile.GetComponent<ParticleSystem>().Play();
        }

        if (tail)
        {
            tail.SetActive(true);
            var m = tail.GetComponent<ParticleSystem>().main;
            m.startLifetime = stopTime;
            m.startSpeed = speed;
            tail.GetComponent<ParticleSystem>().Play();
        }


        missileCol.transform.position = colPos.position;
        dir = Vector3.forward * speed;
        dir = missileCol.transform.rotation * dir;
        missileCol.GetComponent<Rigidbody>().velocity = dir;

        colliderData.damage = hitDamage;
        Collision_Manager.GetInstance().AddCollider(missileCol, colliderData);

        Audio_Manager.GetInstance().PlayAuio(shootAudio);

    }

    public void MissileCollision()
    {
        isExplode = true;
        isFire = false;

        explosionCol.transform.position = missileCol.transform.position;
        explosion.transform.position = missileCol.transform.position;

        Collision_Manager.GetInstance().RemoveCollider(missileCol);
        missileCol.gameObject.SetActive(false);

        if(projectile)
        {
            projectile.SetActive(false);
        }

        if(tail)
        {
            tail.SetActive(false);
        }

        explosion.SetActive(true);
        explosion.GetComponent<ParticleSystem>().Play();

        explosionCol.gameObject.SetActive(true);

        colliderData.damage = explosionDamage;
        Collision_Manager.GetInstance().AddCollider(explosionCol, colliderData);

        Audio_Manager.GetInstance().PlayAuio(explosionAudio);

    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
        if(isFire)
        {
            if (stopDelta > 0f)
            {
                stopDelta -= Time.deltaTime;
            }
            else
            {                
                Collision_Manager.GetInstance().RemoveCollider(missileCol);
                gameObject.SetActive(false);
            }
        }
        else if(isExplode)
        {
            if (explosionDelta > 0f)
            {
                explosionDelta -= Time.deltaTime;
            }
            else
            {
                explosion.SetActive(false);

                explosionCol.gameObject.SetActive(false);

                Collision_Manager.GetInstance().RemoveCollider(explosionCol);

                gameObject.SetActive(false);
            }
        }
	}
}
