using Assets.Scripts.Combat;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Combat.Abilities
{
    public class Bonk : Ability
    {
        private enum Phase { Inactive, Approaching, Bonking, Disengaging }
        private Phase CurrentPhase = Phase.Inactive;

        private Timer Timer;
        [SerializeField] private float ApproachingDuration = 1.25f;
        [SerializeField] private float DisengagingDuration = 1.0f;
        
        [SerializeField]
        private const float ActionCommandMultiplier = 1.2f;

        [SerializeField] private float Damage;

        private Vector3 InitialPosition;
        private GameObject Victim;
        private Vector3 AttackingPosition;
        [SerializeField] private float SpaceBetweenBonk = 2.5f;

        private bool BelongsToAlly;
        private bool HasTimedPress;

        [SerializeField] private AudioSource BonkSound;

        [SerializeField] private GameObject Model;

        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();

            BelongsToAlly = GetComponentsInParent<AllyCombatant>().Length == 1;

            TargetSchema = new TargetSchema(
                1,
                BelongsToAlly ? CombatantType.Enemy : CombatantType.Ally,
                SelectorType.Number);
        }

        public void Update()
        {
            if (CurrentPhase == Phase.Inactive)
                return;

            if (CurrentPhase == Phase.Approaching)
                ApproachingUpdate();

            if (CurrentPhase == Phase.Bonking)
                BonkingUpdate();

            if (CurrentPhase == Phase.Disengaging)
                DisengagingUpdate();
        }

        private void ApproachingUpdate()
        {
            if (Timer.IsInProgress())
            {
                float t = Timer.GetProgress() / ApproachingDuration;
                transform.parent.gameObject.transform.position = Vector3.Lerp(
                    InitialPosition,
                    AttackingPosition,
                    t);
            }

            else if (Timer.IsFinished())
            {
                StartBonkPhase();
            }
        }

        private void DisengagingUpdate()
        {
            if (Timer.IsInProgress())
            {
                float t = Timer.GetProgress() / DisengagingDuration;
                transform.parent.gameObject.transform.position = Vector3.Lerp(
                    AttackingPosition,
                    InitialPosition,
                    t);
            }

            else if (Timer.IsFinished())
            {
                EndAbility();
            }
        }

        public void DealBonkDamage()
        {
            if (CurrentPhase == Phase.Bonking)
            {
                int damage = (int) EvaluateBonkDamage();
                Attack attack = new Attack(damage, Element, Style);

                BonkSound.Play();

                Victim.GetComponent<Combatant>().Defend(attack);
            }
        }

        private void BonkingUpdate()
        {
            if (Timer.IsInProgress())
            {
                var timerProgress = Timer.GetProgress() / Animator.GetCurrentAnimatorStateInfo(0).length;
                
                if (!HasTimedPress && (timerProgress >= 0.4f && timerProgress <= 0.56f) 
                                   && Input.GetButtonDown("Action Command"))
                {
                    Debug.Log("Perfectly timed bonk!");
                    PerfectActionCommandSound.Play();
                    HasTimedPress = true;
                }
            }

            else if (Timer.IsFinished())
            {
                StartDisengagingPhase();
            }
        }

        private void StartBonkPhase()
        {
            CurrentPhase = Phase.Bonking;

            if (!Animator)
            {
                // See Ability script for more info!
                Animator = GetComponentInParent<Animator>();
            }

            // Fix animation issue for models with rotated animations, specifically the Boss
            if (Combatant is EnemyCombatant enemy)
            {
                enemy.RotateModel();
            }
            Animator.Play($"Base Layer.Bonk");

            Timer.StartTimer(Animator.GetCurrentAnimatorStateInfo(0).length);
        }

        private void StartDisengagingPhase()
        {
            CurrentPhase = Phase.Disengaging;

            Timer.StartTimer(DisengagingDuration);
        }

        private float EvaluateBonkDamage()
        {
            var damage = HasTimedPress switch
            {
                true when BelongsToAlly => Damage * ActionCommandMultiplier,
                true when !BelongsToAlly => Damage / ActionCommandMultiplier,
                _ => Damage
            };

            return damage;
        }

        protected override void EndAbility()
        {
            Debug.Log($"Bonk Damage total: {Damage}");

            CurrentPhase = Phase.Inactive;
            
            HasTimedPress = false;

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting BONK ability");

            base.StartAbility();
        }
        protected override void ContinueAbilityAfterTargeting()
        {
            Victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];
            FaceAllyInCombat(Victim);
            InitialPosition = transform.position;

            Vector3 selfToVictimVector = Victim.gameObject.transform.position - InitialPosition;
            AttackingPosition = InitialPosition
                + selfToVictimVector.normalized * (selfToVictimVector.magnitude - SpaceBetweenBonk);

            StartApproachingPhase();
        }

        private void StartApproachingPhase()
        {
            if (Combatant.CombatType == Combatant.CombatantType.Flying)
            {
                Animator.Play($"Base Layer.BonkStart");
            }
            else
            {
                CurrentPhase = Phase.Approaching;
                Timer.StartTimer(ApproachingDuration);
            }
        }

        public void ActivateFlyingBonk()
        {
            CurrentPhase = Phase.Approaching;
            Timer.StartTimer(ApproachingDuration);
        }
    }
}
