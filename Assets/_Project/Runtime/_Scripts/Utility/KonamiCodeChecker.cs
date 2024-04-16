#region
using System.Collections.Generic;
using System.Linq;
using Lumina.Essentials.Sequencer;
using MelenitasDev.SoundsGood;
using UnityEngine;
#endregion

public class KonamiCodeChecker : MonoBehaviour
{
    List<KeyCode> konamiCode = new()
    { KeyCode.UpArrow,
      KeyCode.UpArrow,
      KeyCode.DownArrow,
      KeyCode.DownArrow,
      KeyCode.LeftArrow,
      KeyCode.RightArrow,
      KeyCode.LeftArrow,
      KeyCode.RightArrow,
      KeyCode.B,
      KeyCode.A };

    [SerializeField] List<KeyCode> inputKeys = new ();

    void Update() => CheckKonamiCode();

    void CheckKonamiCode()
    {
        if (Input.anyKeyDown)
        {
            KeyCode[] possibleKeys =
            { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A };

            foreach (KeyCode key in possibleKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    inputKeys.Add(key);
                    break;
                }
            }

            if (inputKeys.Count > konamiCode.Count) inputKeys.RemoveAt(0);

            if (inputKeys.Count == konamiCode.Count && inputKeys.SequenceEqual(konamiCode))
            {
                Debug.Log("Konami Code Entered!");
                inputKeys.Clear();

                AudioManager.StopAllMusic(1.75f);
                
                // Do something here
                var sequence = new Sequence(this);
                sequence.WaitThenExecute(1.5f, () =>
                {
                    var house = new Music(Track.House);
                    house.Play();

                    foreach (PlayerController player in PlayerManager.Players)
                    {
                        player.Character.moveSpeed = 15;
                    }
                });
            }
        }
    }
}
