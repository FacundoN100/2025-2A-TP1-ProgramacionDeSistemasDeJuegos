using System;
using UnityEngine;

namespace Excercise1
{
    public class Enemy : Character
    {
        [SerializeField] private float speed = 5;
        [SerializeField] private string playerId = "Player";
        private ICharacter _player;
        private string _logTag;

        private void Reset()
            => id = nameof(Enemy);

        private void Awake()
            => _logTag = $"{name}({nameof(Enemy).Colored("#555555")}):";


        private void Update()
        {
            if (_player == null) return;

            // Calcula dirección hacia el player
            Vector3 direction = _player.transform.position - transform.position;
            // Mueve al Enemy en esa dirección usando tu speed
            transform.position += direction.normalized * (speed * Time.deltaTime);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //TODO: Get the reference to the player.

            if (CharacterService.Instance == null)
            {
                Debug.LogError($"{_logTag} CharacterService singleton no existe en escena!");
                return;
            }

            if (!CharacterService.Instance.TryGetCharacter(playerId, out _player)) // aca use el service locator para localizar la instancia Player y gusrdarla en el _player
            {
                Debug.LogError($"{_logTag} Player con id '{playerId}' no encontrado!");
            }
        }
    }
}
