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

        private readonly float BaseDamage = 40f;
        private float Damage;
        
        private GameObject MusicalNotesObject;
        private MusicalNotes MusicalNotesVfx;

        [SerializeField] private AudioSource SkelemusicSound;
        [SerializeField] private float MusicalNotesVfxVerticalOffset;
        
        public new void Start()
        {
            base.Start();
            Timer = GetComponent<Timer>();
            Animator = GetComponentInParent<Animator>();
            MusicalNotesObject = GameObject.Find("MusicalNotesPowerMove");
            MusicalNotesVfx = MusicalNotesObject.GetComponent<MusicalNotes>();

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
            CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
        }

        private float CalculateDamage()
        {
            return Random.Range(BaseDamage, BaseDamage + 10);
        }

        public void ThrowMusicalNotesAtTarget()
        {
            MusicalNotesVfx.StartMoving();
            CameraRigSystem.MoveCameraToSelectedTarget(Victim);
        }

        public void DealSkelemusicDamage()
        {
            if (CurrentPhase == Phase.Skelemusic)
            {
                Damage = CalculateDamage();
                Attack attack = new Attack((int)Damage);

                Victim.GetComponent<Combatant>().Defend(attack);
                StartCoroutine(DelayEndOfTurn());
            }
        }

        IEnumerator DelayEndOfTurn()
        {
            yield return new WaitForSeconds(EndOfTurnDelay);
            EndAbility();
        }
    }