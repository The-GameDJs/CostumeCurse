using Assets.Scripts.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Abilities
{
    public class FireBall : Ability
    {
        private int CurrentDamage;
        
        private enum FireballPhase { Growth, Inactive }
        private FireballPhase CurrentFireballPhase = FireballPhase.Inactive;
        private enum FireballCyclePhase { Normal, UnstableWarning, Unstable };
        private FireballCyclePhase CurrentFireballCyclePhase = FireballCyclePhase.Normal;

        [SerializeField]
        private GameObject Fireball;
        private Timer Timer;
        private const float FireballGrowthMinDuration = 3.0f;
        private const float FireballGrowthMaxDuration = 5.0f;
        private const int FireballCycles = 3;
        private int CurrentFireballCycle = 0;
        private const float FireballUnstablingWarningDuration = 1.0f;
        private const float FireballUnstableDuration = 2.0f;

        private float FireballSize = 1.0f;
        private const float FireballGrowth = 0.025f;
        private const float FireballShrinkNormal = 0.005f;
        private const float FireballShrinkUnstable = 0.15f;
        private const float FireballScalingSmoothness = 2f;
        private float FireballScalingElapsedTime = 0;

        private enum ExpectedDirection { Up, Down, Right, Left};
        private List<ExpectedDirection> ExpectedDirections = new List<ExpectedDirection>();


        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();
            Fireball.SetActive(false);

            TargetSchema = new TargetSchema(
                1,
                CombatantType.Enemy,
                SelectorType.Number);
        }

        private void Update()
        {
            if (CurrentFireballPhase == FireballPhase.Growth)
                FireballUpdate();
        }

        private void FireballUpdate()
        {
            float t = 1 / (Mathf.PI / 2) * Mathf.Atan(FireballScalingElapsedTime / FireballScalingSmoothness);
            Fireball.transform.localScale = Vector3.Lerp(
                Fireball.transform.localScale, 
                Vector3.one * FireballSize,
                t);
            FireballScalingElapsedTime += Time.deltaTime;

            if (Timer.IsInProgress())
            {
                float progress = Timer.GetProgress();

                if(IsFireballKeyDown())
                {
                    if(CurrentFireballCyclePhase == FireballCyclePhase.Unstable)
                    {
                        ShrinkFireball();
                        return;
                    }


                    var currentKey = ExpectedDirections[0];

                    switch (currentKey)
                    {
                        case ExpectedDirection.Up:
                            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A))
                                ShrinkFireball();
                            else
                                GrowFireball();
                            break;
                        case ExpectedDirection.Down:
                            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D))
                                ShrinkFireball();
                            else
                                GrowFireball();
                            break;
                        case ExpectedDirection.Right:
                            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W))
                                ShrinkFireball();
                            else
                                GrowFireball();
                            break;
                        case ExpectedDirection.Left:
                            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
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
                    && CurrentFireballCyclePhase == FireballCyclePhase.Normal)
                {
                    CurrentDamage += (int)EvaluateFireballDamage();
                    EndAbility();
                    return;
                }

                if (CurrentFireballCyclePhase == FireballCyclePhase.Normal)
                {
                    CurrentFireballCyclePhase = FireballCyclePhase.UnstableWarning;
                    Timer.StartTimer(FireballUnstablingWarningDuration);
                    Fireball.GetComponent<Renderer>().material.color = Color.yellow;
                    return;

                }

                if (CurrentFireballCyclePhase == FireballCyclePhase.UnstableWarning)
                {
                    CurrentFireballCyclePhase = FireballCyclePhase.Unstable;
                    Timer.StartTimer(FireballUnstableDuration);
                    Fireball.GetComponent<Renderer>().material.color = Color.white;
                    return;

                }

                if (CurrentFireballCyclePhase == FireballCyclePhase.Unstable)
                {
                    CurrentFireballCycle += 1;
                    CurrentFireballCyclePhase = FireballCyclePhase.Normal;
                    Fireball.GetComponent<Renderer>().material.color = Color.red;

                    Timer.StartTimer(Random.Range(FireballGrowthMinDuration, FireballGrowthMaxDuration));
                    return;

                }

            }
        }

        private float EvaluateFireballDamage()
        {
            //// Using variables from https://www.desmos.com/calculator/km7jlgm5ws
            float M = 50;
            float m = 10;
            float d = 64;
            float s = FireballSize > 1f ? FireballSize * 100 - 100 : 0;

            //// Please refer to https://www.desmos.com/calculator/km7jlgm5ws for curve
            float fireballDamage = (M - m) / (Mathf.PI / 2) * Mathf.Atan(s / d) + m;

            return fireballDamage;
        }

        protected override void EndAbility()
        {
            Debug.Log($"Fireball Damage total: {CurrentDamage}");

            // Deal damage to defender, wait
            GameObject victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];
            
            StartCoroutine(LaunchFireball(victim));
            
            Attack attack = new Attack(CurrentDamage);
            victim.GetComponent<Combatant>()
                    .Defend(attack);

            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);

            CurrentFireballPhase = FireballPhase.Inactive;
        }

        private IEnumerator LaunchFireball(GameObject target)
        {
            float launchDuration = 1f;
            float elapsedTime = 0;
            Vector3 origin = Fireball.transform.position;

            while (elapsedTime < launchDuration)
            {
                Fireball.transform.position = Vector3.Lerp(
                    origin, 
                    target.transform.position,
                    elapsedTime / launchDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Fireball.SetActive(false);
        }

        private void GrowFireball()
        {
            Debug.Log("GrowFireball");
            var currentKey = ExpectedDirections[0];
            ExpectedDirections.Remove(currentKey);
            ExpectedDirections.Add(currentKey);

            FireballSize += FireballGrowth;
            FireballScalingElapsedTime = 0;
        }

        private void ShrinkFireball()
        {
            Debug.Log("ShrinkFireball");

            FireballSize -= CurrentFireballCyclePhase == FireballCyclePhase.Unstable ? FireballShrinkUnstable : FireballShrinkNormal;
            if (FireballSize < 0)
                FireballSize = 0f;
            FireballScalingElapsedTime = 0;
        }

        private bool IsFireballKeyDown()
        {
            return Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.A);
        }

        public new void StartAbility()
        {
            Debug.Log("Starting fireball ability");

            CurrentDamage = 0;
            FireballSize = 1f;

            base.StartAbility();
        }
        protected override void ContinueAbilityAfterTargeting()
        {
            StartFireballPhase();
        }

        private void StartFireballPhase()
        {
            this.CurrentFireballPhase = FireballPhase.Growth;

            Fireball.SetActive(true);
            Fireball.transform.position = transform.position + 5f * Vector3.up;
            Fireball.transform.localScale = Vector3.one;
            Fireball.GetComponent<Renderer>().material.color = Color.red;

            ExpectedDirections = new List<ExpectedDirection>
            {
                ExpectedDirection.Up,
                ExpectedDirection.Right,
                ExpectedDirection.Down,
                ExpectedDirection.Left
            };

            CurrentFireballCyclePhase = FireballCyclePhase.Normal;

            Timer.StartTimer(Random.Range(FireballGrowthMinDuration, FireballGrowthMaxDuration));
        }
    }
}
