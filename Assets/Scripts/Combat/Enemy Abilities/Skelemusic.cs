using System.Collections;
using Assets.Scripts.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

    public class Skelemusic : Ability
    {
        Timer Timer;
        private GameObject Victim;
        private readonly float SkelemusicDuration = 2.8f;
        private readonly float EndOfTurnDelay = 2.0f;
        private enum Phase { Inactive, Skelemusic }
        private Phase CurrentPhase = Phase.Inactive;

        [SerializeField] private float BaseDamage = 40f;
        [SerializeField] private float OffsetDamage = 10f;
        private float Damage;
        
        private GameObject MusicalNotesObject;
        private MusicalNotes MusicalNotesVfx;
        private ElementType type;

        [SerializeField] private AudioSource SkelemusicSound;
        [SerializeField] private float MusicalNotesVfxVerticalOffset;
        
        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();
            Animator = GetComponentInParent<Animator>();
            if (Combatant.ElementResistances.Contains(ElementType.Normal))
            {
                MusicalNotesObject = GameObject.Find("MusicalNotesPowerMove");
                type = ElementType.Normal;
            }
            else if (Combatant.ElementResistances.Contains(ElementType.Ice))
            {
                MusicalNotesObject = GameObject.Find("IceNotePowerMove");
                type = ElementType.Ice;
            }
            else if (Combatant.ElementResistances.Contains(ElementType.Fire))
            {
                MusicalNotesObject = GameObject.Find("FireNotePowerMove");
                type = ElementType.Fire;
            }
            else
            {
                MusicalNotesObject = GameObject.Find("MusicalNotesPowerMove");
                type = ElementType.Normal;
            }

            MusicalNotesVfx = MusicalNotesObject.GetComponent<MusicalNotes>();
            MusicalNotesVfx.SetType(type);

            TargetSchema = new TargetSchema(
                1,
                CombatantType.Ally,
                SelectorType.Number);
        }

        public new void StartAbility(bool userTargeting = false)
        {
            Debug.Log("Starting Skelemusic ability");

            base.StartAbility();
        }

        protected override void ContinueAbilityAfterTargeting()
        {
            Victim = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];
            FaceAllyInCombat(Victim);
            StartSkelemusic();
        }

        private void StartSkelemusic()
        {
            CurrentPhase = Phase.Skelemusic;
            Timer.ResetTimer();
            SetMusicalNotesOnEnemy();
            Animator.Play("Base Layer.Skelemusic");
            SkelemusicSound.Play();
        }

        private void SetMusicalNotesOnEnemy()
        {
            MusicalNotesVfx.SetTarget(Victim);
            MusicalNotesVfx.SetAbility(this);
            MusicalNotesVfx.SetLight();
            MusicalNotesObject.transform.position = Combatant.transform.position + new Vector3(0.0f, MusicalNotesVfxVerticalOffset, 0.0f);
            MusicalNotesVfx.SwitchMusicalNotesParticleSystemsState();
        }

        protected override void EndAbility()
        {
            StopAllCoroutines();
            Timer.StopTimer();

            Debug.Log($"Skelemusic Damage total: {Damage}");
            CurrentPhase = Phase.Inactive;

            MusicalNotesVfx.ResetVfx();
            CombatSystem.EndTurn();
        }

        private float CalculateDamage()
        {
            return Random.Range(BaseDamage, BaseDamage + OffsetDamage);
        }

        public void ThrowMusicalNotesAtTarget()
        {
            MusicalNotesVfx.StartMoving();
            if (Victim != null)
            {
                CinemachineCameraRig.Instance.SetCinemachineCameraTarget(Victim.transform);
            }
        }

        public void DealSkelemusicDamage()
        {
            if (CurrentPhase == Phase.Skelemusic)
            {
                Damage = CalculateDamage();
                Attack attack = new Attack((int)Damage, Element, Style);

                var victimCombatant = Victim.GetComponent<AllyCombatant>();
                PlayExplosionParticles(victimCombatant.HasParriedCorrectly, victimCombatant);
                victimCombatant.Defend(attack);
                if (type == ElementType.Fire)
                {
                    victimCombatant.SetFire(true, Combatant.FireType.eOrangeFire);
                }
                StartCoroutine(DelayEndOfTurn());
            }
        }

        IEnumerator DelayEndOfTurn()
        {
            // Delay a tiny bit of time so that it doesn't check for action command input immediately after the first hit happens
            // (for fire skeleton)
            yield return new WaitForSeconds(0.02f);
            
            var currentTime = 0f;
            while (currentTime <= EndOfTurnDelay / 2)
            {
                currentTime += Time.deltaTime;

                if (currentTime >= 0.8f && currentTime <= 1.8f)
                    MusicalNotesVfx.CheckIfParriedSecondTime();
                
                if (InputManager.HasPressedActionCommand)
                {
                    Victim.TryGetComponent<AllyCombatant>(out var victim);
                    victim.HasParried = true;
                }

                yield return null;
            }
            
            if (Victim.TryGetComponent<AllyCombatant>(out var victimCombatant) && type == ElementType.Fire && victimCombatant.IsCombatantStillAlive())
            {
                PlayExplosionParticles(victimCombatant.HasParriedCorrectly, victimCombatant);
                var attack = new Attack((int)Damage / 2, Element, Style);
                victimCombatant.Defend(attack);
                victimCombatant.SetFire(false, Combatant.FireType.eOrangeFire);
            }

            yield return new WaitForSeconds(EndOfTurnDelay / 2);
            
            EndAbility();
        }
    }