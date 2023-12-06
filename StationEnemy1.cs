using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationEnemy1 : MonoBehaviour
{
    //Player playerScript;
    public Transform player;
    public LayerMask whatIsPlayer;
    public float shootRange;
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
        player = GameObject.Find("Player").transform;
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
        playerInshootRange = Physics.CheckSphere(transform.position, shootRange, whatIsPlayer); //Checks radius around enemy so they know when player is near to run
        if(playerInshootRange)
            ShootAtPlayer();
    }
}
