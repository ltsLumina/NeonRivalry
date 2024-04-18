#region
using System.Collections;
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
                
                showGUI = true;
                
                AudioManager.StopAllMusic(1.75f);
                
                // Do something here
                var audioSequence = new Sequence(this);
                audioSequence.WaitThenExecute(1.5f, () =>
                {
                    var house = new Music(Track.House);
                    house.Play();
                });
            }
        }
    }
    
    bool showGUI;

    void OnGUI()
    {
        if (showGUI) StartCoroutine(HouseGUI());
    }

    IEnumerator HouseGUI()
    {
        GUI.Label(new (Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50), "Konami Code Entered!");
        
        yield return new WaitForSeconds(2.5f);

        showGUI = false;
    }
}
