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
        private FireballPhase Phase = FireballPhase.Inactive;
        [SerializeField]
        private GameObject Fireball;
        private Timer Timer;
        private float FireballGrowthDuration = 8.0f;
        private float FireballSize = 1.0f;

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
            if (Phase == FireballPhase.Growth)
                FireballUpdate();
        }

        private void FireballUpdate()
        {
            if (Timer.IsInProgress())
            {
                float progress = Timer.GetProgress();

                if(IsFireballKeyDown())
                {
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
                CurrentDamage += (int) EvaluateFireballDamage();
                
                EndAbility();
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

            Phase = FireballPhase.Inactive;
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

            FireballSize += 0.05f;

            Fireball.transform.localScale = Vector3.one * FireballSize;
        }

        private void ShrinkFireball()
        {
            Debug.Log("ShrinkFireball");

            FireballSize -= 0.15f;
            if (FireballSize < 0)
                FireballSize = 0f;

            Fireball.transform.localScale = Vector3.one * FireballSize;
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
            this.Phase = FireballPhase.Growth;

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

            Timer.StartTimer(FireballGrowthDuration);
        }
    }
}
