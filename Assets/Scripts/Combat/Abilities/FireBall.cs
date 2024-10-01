using Assets.Scripts.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Combat.Abilities
{
    public class FireBall : Ability
    {
        private const float EndOfTurnDelay = 2.0f;
        private int CurrentDamage;
        
        private enum FireballPhase { Growth, Inactive }
        private FireballPhase CurrentPhase = FireballPhase.Inactive;
        private enum FireballCyclePhase { Normal, UnstableWarning, Unstable };
        private FireballCyclePhase CurrentCyclePhase = FireballCyclePhase.Normal;

        [Header("Scaling")]
        private static GameObject Fireball;
        private const float FireballGrowthMinDuration = 1.5f;
        private const float FireballGrowthMaxDuration = 3.4f;
        private const int FireballCycles = 1;
        private int CurrentFireballCycle = 0;
        private const float FireballUnstablingWarningDuration = 1.0f;
        private const float FireballUnstableDuration = 2.0f;

        [SerializeField] private AudioSource FireballGrowSound;
        [SerializeField] private AudioSource FireballShrinkSound;

        private const float FireballHeight = 7f;
        private const float FireballScale = 0.08f;
        private float TargetFireballSize = 0.15f;
        private const float FireballGrowth = 0.015f;
        private const float FireballShrinkNormal = 0.005f;
        private const float FireballShrinkUnstable = 0.4f;
        private const float FireballScalingSmoothness = 2f;
        private const float FireballParticleSystemAdjustmentFactor = 0.15f;
        private const float FireballLightSourceAdjustmentFactor = 7.5f;

        [FormerlySerializedAs("BulletTargetHeightOffset")]
        [Header("Properties")] 
        [SerializeField] private float FireballTargetHeightOffset;
        private const float FireballMaximumDamage = 150;
        private const float FireballMinimumDamage = 85;
        private const float FireballDifficultyCurve = 100;

        [Header("Components")]
        private Timer Timer;
        private ParticleSystem ParticleComponent;
        private ParticleSystem.MainModule MainModule;

        private enum ExpectedDirection { Up, Down, Right, Left};
        private List<ExpectedDirection> ExpectedDirections = new List<ExpectedDirection>();
        private Light LightSource;


        public new void Start()
        {
            base.Start();
            
            Fireball = GameObject.Find("Fireball");
            Timer = GetComponent<Timer>();
            ParticleComponent = Fireball.GetComponent<ParticleSystem>();
            MainModule = ParticleComponent.main;
            Fireball.SetActive(false);
            LightSource = Fireball.transform.GetChild(0).GetComponent<Light>();

            TargetSchema = new TargetSchema(
                1,
                CombatantType.Enemy,
                SelectorType.Number);
        }

        private void Update()
        {
            if (CurrentPhase == FireballPhase.Growth)
                FireballUpdate();
        }

        private void FireballUpdate()
        {
            float t = 1 / (Mathf.PI / 2) * Mathf.Atan(0.02f / FireballScalingSmoothness);
            Fireball.transform.localScale = Vector3.Lerp(
                Fireball.transform.localScale, 
                Vector3.one * TargetFireballSize,
                t);

            if (Timer.IsInProgress())
            {
                if(IsFireballKeyDown())
                {
                    if(CurrentCyclePhase == FireballCyclePhase.Unstable)
                    {
                        ShrinkFireball();
                        return;
                    }
                    
                    var currentKey = ExpectedDirections[0];

                    switch (currentKey)
                    {
                        case ExpectedDirection.Up:
                            if (InputManager.InputDirection.x > 0.88f // Right
                                || InputManager.InputDirection.z < -0.88f // Down
                                || InputManager.InputDirection.x < -0.88f) // Left
                                ShrinkFireball();
                            else
                                GrowFireball();
                            break;
                        case ExpectedDirection.Down:
                            if (InputManager.InputDirection.x < -0.88f // Left
                                || InputManager.InputDirection.x > 0.88f // Right
                                || InputManager.InputDirection.z > 0.88f) // Up
                                ShrinkFireball();
                            else
                                GrowFireball();
                            break;
                        case ExpectedDirection.Right:
                            if (InputManager.InputDirection.z < -0.88f // Down
                                || InputManager.InputDirection.x < -0.88f // Left
                                || InputManager.InputDirection.z > 0.88f) // Up
                                ShrinkFireball();
                            else
                                GrowFireball();
                            break;
                        case ExpectedDirection.Left:
                            if (InputManager.InputDirection.z > 0.88f // Up
                                || InputManager.InputDirection.x > 0.88f // Right
                                || InputManager.InputDirection.z < -0.88f) // Down
                                ShrinkFireball();
                            else
                                GrowFireball();
                            break;
                        default:
                            break;
                    }
                }
            }

            else if (Timer.IsFinished())
            {
                if (CurrentFireballCycle == FireballCycles 
                    && CurrentCyclePhase == FireballCyclePhase.Normal)
                {
                    CurrentDamage += (int)EvaluateFireballDamage();
                    Debug.Log(CurrentDamage);
                    EndAbility();
                    return;
                }

                if (CurrentCyclePhase == FireballCyclePhase.Normal)
                {
                    CurrentCyclePhase = FireballCyclePhase.UnstableWarning;
                    Timer.StartTimer(FireballUnstablingWarningDuration);
                    MainModule.startColor = Color.yellow;
                    LightSource.color = Color.yellow;
                    return;
                }

                if (CurrentCyclePhase == FireballCyclePhase.UnstableWarning)
                {
                    CurrentCyclePhase = FireballCyclePhase.Unstable;
                    Timer.StartTimer(FireballUnstableDuration);
                    MainModule.startColor = Color.white;
                    LightSource.color = Color.white;
                    return;
                }

                if (CurrentCyclePhase == FireballCyclePhase.Unstable)
                {
                    CurrentFireballCycle += 1;
                    CurrentCyclePhase = FireballCyclePhase.Normal;
                    MainModule.startColor = Color.red;
                    LightSource.color = Color.red;

                    Timer.StartTimer(Random.Range(FireballGrowthMinDuration, FireballGrowthMaxDuration));
                    return;
                }
            }
        }

        private float EvaluateFireballDamage()
        {
            //// Using variables from https://www.desmos.com/calculator/ca9cqhpsto
            float M = FireballMaximumDamage;
            float m = FireballMinimumDamage;
            float d = FireballDifficultyCurve;
            float s = TargetFireballSize > 0.5f ? TargetFireballSize * 100 : 0;

            //// Please refer to https://www.desmos.com/calculator/ca9cqhpsto for curve
            float fireballDamage = (M - m) / (Mathf.PI / 2) * Mathf.Atan(s / d) + m;

            return fireballDamage;
        }

        protected override void EndAbility()
        {
            Debug.Log($"Fireball Damage total: {CurrentDamage}");

            // Deal damage to defender, wait
            GameObject victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];

            FireballGrowSound.Stop();

            StartCoroutine(LaunchFireball(victim));

            Animator.SetBool("IsFinishedCasting", true);

            CurrentPhase = FireballPhase.Inactive;
        }

        private IEnumerator LaunchFireball(GameObject target)
        {
            float launchDuration = 1f;
            float elapsedTime = 0;
            Vector3 origin = Fireball.transform.position;
            var offset = TargetedCombatants[0].GetComponent<Combatant>().isBoss
                ? FireballTargetHeightOffset * 4
                : FireballTargetHeightOffset;
            CinemachineCameraRig.Instance.SetCinemachineCameraTarget(target.transform);

            while (elapsedTime < launchDuration)
            {
                Fireball.transform.position = Vector3.Lerp(
                    origin, 
                    target.transform.position + new Vector3(0.0f, offset, 0.0f),
                    elapsedTime / launchDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Fireball.SetActive(false);

            Attack attack = new Attack(CurrentDamage, Element, Style);
            var combatant = target.GetComponent<Combatant>();
            combatant.Defend(attack);
            combatant.SetFire(true, Combatant.FireType.eRedFire);
            yield return new WaitForSeconds(EndOfTurnDelay);
            combatant.SetFire(false, Combatant.FireType.eRedFire);

            CombatSystem.EndTurn();
        }

        private void GrowFireball()
        {
            Debug.Log("GrowFireball");

            Debug.Log($"Target size is {TargetFireballSize}");
            FireballGrowSound.volume = Mathf.Clamp(TargetFireballSize, 0.01f, 1f);

            if (!FireballGrowSound.isPlaying)
                FireballGrowSound.Play();
            
            var currentKey = ExpectedDirections[0];
            ExpectedDirections.Remove(currentKey);
            ExpectedDirections.Add(currentKey);

            TargetFireballSize += FireballGrowth * FireballParticleSystemAdjustmentFactor;
            TargetFireballSize = Mathf.Clamp(TargetFireballSize, 0.15f, 0.6f);
            LightSource.intensity += FireballGrowth * FireballLightSourceAdjustmentFactor;
        }

        private void ShrinkFireball()
        {
            Debug.Log("ShrinkFireball");

            if (FireballGrowSound.isPlaying)
                FireballGrowSound.Stop();

            MissedActionCommandSound.Play();

            TargetFireballSize -= CurrentCyclePhase == FireballCyclePhase.Unstable ? 
                FireballShrinkUnstable * FireballParticleSystemAdjustmentFactor : 
                FireballShrinkNormal * FireballParticleSystemAdjustmentFactor;
            LightSource.intensity -= CurrentCyclePhase == FireballCyclePhase.Unstable ?
                FireballShrinkUnstable * FireballLightSourceAdjustmentFactor :
                FireballShrinkNormal * FireballLightSourceAdjustmentFactor;
            
            TargetFireballSize = Mathf.Max(TargetFireballSize, 0.05f);
        }

        private bool IsFireballKeyDown()
        {
            return InputManager.InputDirection.x != 0 || InputManager.InputDirection.z != 0;
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting fireball ability");

            CurrentDamage = 0;
            TargetFireballSize = FireballScale;
            Fireball.transform.localScale = Vector3.one * TargetFireballSize;
            CurrentFireballCycle = 0;

            base.StartAbility();
        }
        protected override void ContinueAbilityAfterTargeting()
        {
            StartFireballPhase();
        }

        private void StartFireballPhase()
        {
            this.CurrentPhase = FireballPhase.Growth;

            Fireball.SetActive(true);
            Animator.SetBool("IsFinishedCasting", false);
            Animator.Play("Base Layer.Casting");
            Fireball.transform.position = transform.position + FireballHeight * Vector3.up;
            Fireball.transform.localScale = Vector3.one * FireballScale;
            MainModule.startColor = Color.red;
            LightSource.color = Color.red;
            LightSource.intensity = 15.0f;

            ExpectedDirections = new List<ExpectedDirection>
            {
                ExpectedDirection.Up,
                ExpectedDirection.Right,
                ExpectedDirection.Down,
                ExpectedDirection.Left
            };

            CurrentCyclePhase = FireballCyclePhase.Normal;

            Timer.StartTimer(Random.Range(FireballGrowthMinDuration, FireballGrowthMaxDuration));
        }
    }
}
