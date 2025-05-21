using UnityEngine;

namespace Excercise1
{
    public class Character : MonoBehaviour, ICharacter
    {
        [SerializeField] protected string id;

        protected virtual void OnEnable()
        {
            //TODO: Add to CharacterService. The id should be the given serialized field. 
            if (CharacterService.Instance == null)
            {
                Debug.LogError($"[Character] CharacterService singleton no existe en escena!");
                return;
            }
            if (!CharacterService.Instance.TryAddCharacter(id, this))
            {
                Debug.LogWarning($"[Character] Ya existía un personaje con id='{id}'.");
            }
        }
        

        protected virtual void OnDisable()
        {
            //TODO: Remove from CharacterService.
            if (CharacterService.Instance != null)
            {
                CharacterService.Instance.TryRemoveCharacter(id);
            }
        }
    }
}
