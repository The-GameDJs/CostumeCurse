using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Combat.Enemy_Abilities
{
    public class CandyStorm : Ability
    {
        private enum CandyStormPhase { Cloud, Inactive }
        [SerializeField] private GameObject LightningStrike;
        [SerializeField] private Vector3 ThunderCloudOffsetPosition;
        private Timer Timer;

        private CandyStormPhase CurrentPhase = CandyStormPhase.Inactive;
        private GameObject Target;

        [Header("Cloud Components")]
        [SerializeField] private CandyStormVfx ThunderCandyClusterVfx;

        [Header("Collecting Phase")]
        [SerializeField] private Canvas CollectionCanvas;

        [SerializeField] private RectTransform CandyCornSpawnerAnchor;
        [SerializeField] private GameObject[] CandyCornSpawners;
        [SerializeField] private GameObject SieldHatObject;
        [SerializeField] private Transform SieldHatReset;
        [SerializeField] private GameObject CandyCornPrefab;
        [SerializeField] private float CatchingDuration;
        [SerializeField] private float HatMovementSpeed;
        [SerializeField] private Text CandyCornCollectedCounterText;
        [SerializeField] private Text CollectingTimerText;
        private readonly int BaseDamage = 60;
        private int TotalCandiesCollected = 0;
        private float CandyCornDropCountdown;

        [Header("Audio Components")]
        [SerializeField] private AudioSource LightningStrikeSound;
        [SerializeField] private AudioSource ThunderCloudSound;
        
        
        // Start is called before the first frame update
        void Start()
        {
            Timer = GetComponent<Timer>();
            base.Start();

            TargetSchema = new TargetSchema(
                0,
                CombatantType.Ally,
                SelectorType.All);

            BossAnimationHelper.DealCandyStormDamageAction += OnDealCandyStormDamage;
        }
        
        public new void StartAbility(bool userTargeting = false)
        {
            Animator.SetBool("IsFinishedCasting", false);

            base.StartAbility();
        }
        
        // Update is called once per frame
        void Update()
        {
            if (CurrentPhase == CandyStormPhase.Cloud)
                CandyCloudUpdate();
            
        }

        private void CandyCloudUpdate()
        {
            if (Timer.IsInProgress())
            {
                CandyCornDropCountdown += Time.deltaTime;
                if (CandyCornDropCountdown > 0.2f)
                {
                    var randomInt = Random.Range(0, CandyCornSpawners.Length);
                    var go = Instantiate(CandyCornPrefab, CandyCornSpawners[randomInt].transform.position,
                        Quaternion.identity,
                        CandyCornSpawnerAnchor);
                    var candyCornDropComponent = go.GetComponent<CandyCornStormDrop>();
                    candyCornDropComponent.SetCandyStormComponents(this, SieldHatObject.GetComponent<RectTransform>());
                    CandyCornDropCountdown = 0.0f;
                }

                // Move Sield's Hat to catch candy
                if (Input.GetButton("Left"))
                {
                    SieldHatObject.transform.Translate(new Vector3(Time.deltaTime * HatMovementSpeed, 0.0f, 0.0f));
                }
                if (Input.GetButton("Right"))
                {
                    SieldHatObject.transform.Translate(new Vector3(Time.deltaTime * -HatMovementSpeed, 0.0f, 0.0f));
                }
                
                // Set Catching Timer text field
                float timeRemaining = CatchingDuration - Timer.GetProgress();
                CollectingTimerText.text = Mathf.RoundToInt(timeRemaining) + "";
            }

            if (Timer.IsFinished())
            {
                EndCloudPhase();
            }
        }

        public void CatchCandy()
        {
            TotalCandiesCollected++;
            CandyCornCollectedCounterText.text = TotalCandiesCollected.ToString();
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            Debug.Log("Starting Cloud Summoning Phase");
            ThunderCandyClusterVfx.gameObject.SetActive(true);
            ThunderCandyClusterVfx.transform.position = transform.position + ThunderCloudOffsetPosition;
            
            
            Animator.Play("Base Layer.Summon");
            
            ThunderCandyClusterVfx.SwitchVfxActivation();
            ThunderCandyClusterVfx.SwitchCloudStormParticleSystemsState();

            StartCoroutine(PrepareCollectionUI());
        }

        private IEnumerator PrepareCollectionUI()
        {
            CurrentPhase = CandyStormPhase.Cloud;
            
            yield return new WaitForSeconds(2.0f);
            
            Debug.Log("Activating Candy Collection UI");
            CollectionCanvas.gameObject.SetActive(true);
            CollectionCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            TotalCandiesCollected = 0;

            Timer.StartTimer(CatchingDuration);
        }

        private void EndCloudPhase()
        {
            Debug.Log("Ending Cloud Phase");
            Timer.ResetTimer();
            CandyCornManager.AddCandyCorn(TotalCandiesCollected);
            //ResetCloudPhaseValues();
            
            CollectionCanvas.gameObject.SetActive(false);

            StartCoroutine(StartStrikingPhase());
        }

        private void ResetCloudPhaseValues()
        {
            ThunderCandyClusterVfx.ResetVfx();
            SieldHatObject.transform.position = SieldHatReset.position;
            TotalCandiesCollected = 0;
            CandyCornCollectedCounterText.text = "0";
            CollectingTimerText.text = "0";
        }

        private IEnumerator StartStrikingPhase()
        {
            Debug.Log("Starting Striking Phase");
            yield return new WaitForSeconds(1.0f);
            CurrentPhase = CandyStormPhase.Inactive;
            EndAbility();
        }

        private void OnDealCandyStormDamage()
        {
            ThunderCandyClusterVfx.StartMoving();
            CameraRigSystem.SetTargetGO(Target);
            CameraRigSystem.MoveCameraRelative(CameraRigSystem.DefaultOffset, CameraRigSystem.DefaultRotation);
        }

        public IEnumerator DealCandyStormDamage()
        {
            var damage = BaseDamage - TotalCandiesCollected;
            Debug.Log("Damage after Collecting " + damage);
            Attack attack = new Attack(damage);
            Target.GetComponent<Combatant>().Defend(attack);
            ThunderCandyClusterVfx.ExplodeCandyStormMix();
            
            yield return new WaitForSeconds(2.0f);
            
            ResetCloudPhaseValues();
            CombatSystem.EndTurn(GetComponentInParent<Combatant>().gameObject);
        }

        protected override void EndAbility()
        {
            //ThunderCloudParticles.Stop();
            //ThunderCloud.transform.position = Vector3.zero;
            //ThunderCloud.SetActive(false);
            
            Target = TargetedCombatants[0];
            ThunderCandyClusterVfx.SetComponents(Target);
            Animator.Play("Base Layer.Throw");
            //Animator.SetBool("IsFinishedCasting", true);

            //CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        private void OnDestroy()
        {
            BossAnimationHelper.DealCandyStormDamageAction -= OnDealCandyStormDamage;
        }
    }
}
