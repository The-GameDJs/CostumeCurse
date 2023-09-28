using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
                // do input sequence
            }
            else
            {
                // end candy cloud phase
            }
        }

        private void CandyStrikeUpdate()
        {
            
        }

        protected override void ContinueAbilityAfterTargeting()
        {
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
