using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    [System.Serializable]
    public class CharacterAduios
    {
        
        public AudioClip beHurted;
    }
    public class AudioManager :SingletonPersistent<AudioManager>
    {
        private CharacterAduios currentAudios;
        [Header("С���������")]
        
        public CharacterAduios momoiAudios;

        
        [Header("С���������")]
        
        public CharacterAduios midoriAudios;

        [SerializeField] AudioSource sFXPlayer;
        [SerializeField] AudioSource bGMPlayer;
        public void PlaySFX(AudioClip audioClip)
        {
            
            sFXPlayer.PlayOneShot(audioClip);
        }
        public void PlayBeHurted()
        {
            PlaySFX(currentAudios.beHurted);
        }
        public void SetCurrentAudios(string playername)
        {
            if (playername == "Momoi")
            {
                currentAudios = momoiAudios;
            }
            else currentAudios = midoriAudios;
            bGMPlayer.Play();
        }


    }


}
