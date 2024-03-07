using UnityEngine;
using System;

public class PlayerCar : Car
{
    [SerializeField] public float invincibleTime;
    [SerializeField] private PlayInvincibleAnimation invincibleAnimation;
    [SerializeField] public LightTrailGeneration lt;
    [SerializeField] private PlayerAudio playerAudio;
    public bool invincible;
    public static Vector3 PreviousPosition;
    public static Action OnPlayerDamage;
    
    private const int Health = 3;
    private float currentHealth;
    public bool TrailChargeFull => lt.FullCharge;
    [Header("Debug")]
    [SerializeField] private bool noGameOver;
    private Vector3 resetVector;
    protected override void OnEnable()
    {
        base.OnEnable();
        currentHealth = Health;
        gameManager = GameObject.FindWithTag("manager").GetComponent<GameManager>();
    }
    


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    private void LateUpdate() {
        PreviousPosition = transform.position;
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("LightTrail"))
        {
            //  CalculateCollision(other, true);
            TakeDamage();
        }

    }

    protected override void TakeDamage()
    {
        if(noGameOver) { return; }
        if (invincible)
        {
            return;
        }
        currentHealth--;
        if (currentHealth == 0)
        {
            Die();
        }
        else
        {
            if(OnPlayerDamage != null){
                OnPlayerDamage.Invoke();
            }
            GameEvents.TriggerEvent("PlayerHit");
            TurnToInvincible();
        }
    }
    protected override void Die()
    {
        Spawner.Instance.SpawnFx(GlobalGameItemsData.Item.ExplosionBlueFx.ToString(), transform.position, Quaternion.identity);
        gameManager.GameLost(this);
    }

    public void SwitchLightTrailState()
    {
        // spawn particle and play sound and other stuff
        /*if (!lt.Active) 
        {*/ 
            lt.SwitchTrailState();
            if (lt.Active)
            {
                GameEvents.TriggerEvent("LightTrailSwitch", 1);
            }
            else
            {
                playerAudio.PlayNoChargeTrailSFX();
                GameEvents.TriggerEvent("LightTrailSwitch", 0);
            }
        //}
    }
    private void TurnToInvincible()
    {
        invincibleAnimation.PlayAnimationOfActiveChild(invincibleTime);
        invincible = true;
        Timer.Register(invincibleTime + 0.5f, () =>
        {
            invincible = false;
        });
    }
    

   

    public Vector3 movingDirection => (transform.forward);


}
