using System.Collections.Generic;
using UnityEngine;

// CharacterService explicado:
// - Aplica el patron Singleton para garantizar una unica instancia global accesible desde cualquier script.
// - Actua como Service Locator: mantiene un registro  de ICharacter por su ID y permite resolver referencias.

namespace Excercise1 //aca use Sigleton para garantisar un CharacterService acesible en todo el codigo
{
    public class CharacterService : MonoBehaviour
    {
        private readonly Dictionary<string, ICharacter> _charactersById = new();
        public static CharacterService Instance { get; private set; } //aca se estaria ejecutando el sigleton y en el Awake
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public bool TryAddCharacter(string id, ICharacter character) // aca use el service locator ya q registra el ID 
            => _charactersById.TryAdd(id, character);
        public bool TryRemoveCharacter(string id)
            => _charactersById.Remove(id);
        public bool TryGetCharacter(string id, out ICharacter character) // aca tambien usaria Service Locator por el ID
            => _charactersById.TryGetValue(id, out character);
    }
}
