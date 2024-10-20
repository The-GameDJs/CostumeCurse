using System.Collections;
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

        [SerializeField] private bool IsJumpingBonk;
        
        [SerializeField]
        private const float ActionCommandMultiplier = 1.2f;

        [SerializeField] private float Damage;

        private Vector3 InitialPosition;
        private GameObject Victim;
        private Vector3 AttackingPosition;
        [SerializeField] private float SpaceBetweenBonk = 2.5f;

        private bool BelongsToAlly;
        private bool HasParried;
        private bool HasCorrectlyParried;

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
                var progress = Timer.GetProgress() / ApproachingDuration;
                var startPos = InitialPosition;
                var targetPos = AttackingPosition;
                
                if (Timer.GetProgress() >= 0.2f && !HasParried && InputManager.HasPressedActionCommand)
                {
                    HasParried = true;
                    Debug.Log("Missed the timed bonk!");
                }
                
                if (IsJumpingBonk)
                {
                    var currentPos = Vector3.Lerp(startPos, targetPos, progress);
                    
                    var jumpHeight = CombatSystem.MovementAnimCurve.Evaluate(progress);
                    currentPos.y = Mathf.Lerp(startPos.y, targetPos.y, progress) + jumpHeight;
                    
                    transform.parent.gameObject.transform.position = currentPos;
                }
                else
                {
                    transform.parent.gameObject.transform.position = Vector3.Lerp(
                        startPos,
                        targetPos,
                        progress);
                }
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
                var progress = Timer.GetProgress() / ApproachingDuration;
                var startPos = AttackingPosition;
                var targetPos = InitialPosition;
                
                if (IsJumpingBonk)
                {
                    
                    var currentPos = Vector3.Lerp(startPos, targetPos, progress);
                    
                    var jumpHeight = CombatSystem.MovementAnimCurve.Evaluate(progress);
                    currentPos.y = Mathf.Lerp(startPos.y, targetPos.y, progress) + jumpHeight;
                    
                    transform.parent.gameObject.transform.position = currentPos;
                }
                else
                {
                    transform.parent.gameObject.transform.position = Vector3.Lerp(
                        startPos,
                        targetPos,
                        progress);
                }
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
                
                
                if (!HasParried && Victim.TryGetComponent<AllyCombatant>(out var allyCombatant))
                {
                    allyCombatant.ExplosionParticles.gameObject.SetActive(true);
                    allyCombatant.ExplosionParticles.Play();
                }
                else if (HasParried && Victim.TryGetComponent<EnemyCombatant>(out var enemyCombatant))
                {
                    enemyCombatant.ExplosionParticles.gameObject.SetActive(true);
                    enemyCombatant.ExplosionParticles.Play();
                }

                Victim.GetComponent<Combatant>().Defend(attack);
            }
        }

        private void BonkingUpdate()
        {
            if (Timer.IsInProgress())
            {
                var timerProgress = Timer.GetProgress() / Animator.GetCurrentAnimatorStateInfo(0).length;
                
                if (!HasParried && (timerProgress >= 0.4f && timerProgress <= 0.56f) 
                                   && InputManager.HasPressedActionCommand)
                {
                    Debug.Log("Perfectly timed bonk!");
                    PerfectActionCommandSound.Play();
                    HasParried = true;
                    HasCorrectlyParried = true;
                }
                else if (!HasParried && InputManager.HasPressedActionCommand)
                {
                    HasParried = true;
                    Debug.Log("Missed the timed bonk!");
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

            Timer.StartTimer(Animator.GetCurrentAnimatorStateInfo(0).length / 1.2f);
        }

        private void StartDisengagingPhase()
        {
            CurrentPhase = Phase.Disengaging;

            Timer.StartTimer(DisengagingDuration);
        }

        private float EvaluateBonkDamage()
        {
            float damage = HasCorrectlyParried switch
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

            if (Victim.TryGetComponent<Combatant>(out var victimCombatant) && victimCombatant.ExplosionParticles.gameObject.activeSelf)
            {
                victimCombatant.ExplosionParticles.gameObject.SetActive(false);
            }
            
            HasParried = false;
            
            HasCorrectlyParried = false;

            StartCoroutine(DelayEndAbility());
        }

        private IEnumerator DelayEndAbility()
        {
            yield return new WaitForSeconds(0.3f);
            CombatSystem.EndTurn();
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
