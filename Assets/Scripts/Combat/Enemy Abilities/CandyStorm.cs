using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Combat.Enemy_Abilities
{
    public class CandyStorm : Ability
    {
        private enum CandyStormPhase { Cloud, Strike, Inactive }
        [SerializeField] private GameObject LightningStrike;
        [SerializeField] private Vector3 ThunderCloudOffsetPosition;
        private Timer Timer;
        
        private CandyStormPhase CurrentPhase = CandyStormPhase.Inactive;
        private Combatant Target;

        [Header("Cloud Components")]
        [SerializeField] private GameObject ThunderCloud;
        [SerializeField] private ParticleSystem ThunderCloudParticles;
        [SerializeField] private GameObject ThunderCandyCluster;
        [SerializeField] private ParticleSystem ThunderCandyClusterParticles;
        [SerializeField] private ParticleSystem ThunderSprinkerClusterParticles;
        private ParticleSystem.MainModule MainModule;

        [Header("Collecting Phase")]
        [SerializeField] private Canvas CollectionCanvas;

        [SerializeField] private RectTransform CandyCornSpawnerAnchor;
        [SerializeField] private GameObject[] CandyCornSpawners;
        [SerializeField] private GameObject SieldHatObject;
        [SerializeField] private GameObject CandyCornPrefab;
        [SerializeField] private float CatchingDuration;
        [SerializeField] private Text CandyCornCollectedCounterText;
        [SerializeField] private Text CollectingTimerText;
        private int TotalCandiesCollected = 0;
        private float CandyCornDropCountdown;

        [Header("Audio Components")]
        [SerializeField] private AudioSource LightningStrikeSound;
        [SerializeField] private AudioSource ThunderCloudSound;
        
        
        // Start is called before the first frame update
        void Start()
        {
            Timer = GetComponent<Timer>();
            ThunderCloud.SetActive(false);
            
            TargetSchema = new TargetSchema(
                0,
                CombatantType.Ally,
                SelectorType.All);
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

            if (CurrentPhase == CandyStormPhase.Strike)
                CandyStrikeUpdate();
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
                    CandyCornDropCountdown = 0.0f;
                }
            }

            if (Timer.IsFinished())
            {
                EndCloudPhase();
            }
        }

        private void CandyStrikeUpdate()
        {
            
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            Debug.Log("Starting Cloud Summoning Phase");
            ThunderCloud.SetActive(true);
            ThunderCloud.transform.position = transform.position + ThunderCloudOffsetPosition;
            ThunderCandyCluster.SetActive(true);
            ThunderCandyCluster.transform.position = transform.position + ThunderCloudOffsetPosition;
            
            Animator.Play("Base Layer.Summon");
            
            if (!ThunderCloudParticles.isPlaying)
            {
                ThunderCloudParticles.Play();
            }
            if (!ThunderCandyClusterParticles.isPlaying)
            {
                ThunderCandyClusterParticles.Play();
            }
            if (!ThunderSprinkerClusterParticles.isPlaying)
            {
                ThunderSprinkerClusterParticles.Play();
            }

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
            CollectionCanvas.gameObject.SetActive(false);
        }

        protected override void EndAbility()
        {
            ThunderCloudParticles.Stop();
            ThunderCloud.transform.position = Vector3.zero;
            ThunderCloud.SetActive(false);

            Animator.SetBool("IsFinishedCasting", true);

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }
    }
}
