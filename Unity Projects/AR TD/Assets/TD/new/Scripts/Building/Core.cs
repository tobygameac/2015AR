using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CharacterStats))]
public class Core : MonoBehaviour {

  private CharacterStats characterStats;

  private static Game game;

  public GameObject particleSystemObject;
  private ParticleSystem damagedParticleSystem;

  public AudioClip damagedSound;

  private bool hasHealingTechnology;

  void Start() {
    characterStats = GetComponent<CharacterStats>();

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    if (particleSystemObject != null) {
      damagedParticleSystem = particleSystemObject.GetComponent<ParticleSystem>();
    }

    hasHealingTechnology = false;
  }

  void Update() {
    if (!hasHealingTechnology) {
      hasHealingTechnology = game.HasTechnology(GameConstants.TechnologyID.SELF_HEALING);
    }

    if (hasHealingTechnology) {
      characterStats.CurrentHP += characterStats.MaxHP * GameConstants.HP_PERCENT_REGENERATING_PER_SECOND_OF_SELF_HEALING * Time.deltaTime;
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      //CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      //targetCharacterStats.CurrentHP = 0;
      Destroy(collider.gameObject);
      MessageManager.AddMessage("寶箱受到攻擊");

      AudioManager.PlayAudioClip(damagedSound);

      if (damagedParticleSystem != null/* && !damagedParticleSystem.isPlaying */) {
        damagedParticleSystem.Play();
      }

      --characterStats.CurrentHP;
    }
  }
}
