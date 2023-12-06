using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Player playerScript;
    public int health = 3;
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    public Transform player;
    public Transform house;
    public LayerMask whatIsPlayer;
    public float sightRange;
    public float shootRange;
    public bool playerInHouseRange;
    public bool playerInshootRange;
    [SerializeField]
    private float timer = 5;
    private float bulletTime;
    public GameObject Bullet;
    public Transform Enemyspawn;
    public float enemyspeed;
    //public AudioSource damageSound;
    //public AudioSource blowUpSound;
    void Start()
    {
        //PlayerScript = GameObject.Find("KCO").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        agent.autoBraking = false;
        GotoNextPoint();
    }
    /*void Explode() 
    {
        ParticleSystem exp = GetComponent<ParticleSystem>();
        exp.Play();
    }*/
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
		{
			Destroy(other.gameObject);
            //damageSound.Play();
            //Explode();
            TakeBullet();
		}
    }
    void TakeBullet()
	{
        health -= 3;
		if(health < 1)
        {
            playerScript.AddKill();
            Destroy(gameObject);
        }
	}
    void GotoNextPoint()
    {
        if (points.Length == 0)
            return;
        agent.destination = points[destPoint].position;
        int newDestPoint = 0;
        do
        {
            newDestPoint = Random.Range(0, points.Length);
        } while (destPoint == newDestPoint);


        destPoint = newDestPoint;
    }
    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    void ShootAtPlayer()
    {
        bulletTime -= Time.deltaTime;

        if(bulletTime > 0) return;

        bulletTime = timer;

        GameObject bulletObj = Instantiate(Bullet, Enemyspawn.transform.position, Enemyspawn.transform.rotation) as GameObject;
        Rigidbody bulletRig = bulletObj.GetComponent<Rigidbody>();
        bulletRig.AddForce(bulletRig.transform.forward * enemyspeed);
        Destroy(bulletObj, 5f);
    }
    void Update()
    {
        playerInHouseRange = Physics.CheckSphere(house.position, sightRange, whatIsPlayer); //Checks radius around house
        playerInshootRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); //Checks radius around enemy so they know when player is near to run
        if(playerInHouseRange)
            ChasePlayer();
          
        if(!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
        if(playerInshootRange && playerInHouseRange)
            ShootAtPlayer();
    }
}
