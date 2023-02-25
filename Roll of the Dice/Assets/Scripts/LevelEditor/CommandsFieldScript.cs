using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsFieldScript : MonoBehaviour
{
    private TMPro.TMP_InputField inputField;

    private Grid tilesPlacer;

    void Start()
    {
        inputField = GetComponent<TMPro.TMP_InputField>();
        tilesPlacer = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1) /*&& !inputField.IsInteractable()*/)
        {
            inputField.interactable = true;
            inputField.Select();
        }
    }

    public void OnWriting(string ciao)
    {
        var dice = GameObject.FindGameObjectWithTag("Dice");
        if (dice) dice.GetComponent<Dice>().SetGameStatus(false);
    }

    public void OnEnteringCommand(string finalCommand)
    {
        finalCommand = finalCommand.ToLower();

        string command = finalCommand.Split(" ")[0];
        
        var dice = GameObject.FindGameObjectWithTag("Dice");
        if(dice) dice.GetComponent<Dice>().SetGameStatus(true);
        
        switch(command)
        {
            case "setdice":
                string commandParameters = finalCommand.Split(" ")[1];

                bool parse = int.TryParse(commandParameters, out int value);

                if(parse) 
                {
                    tilesPlacer.LevelData.DiceValue = value;

                    if(dice) dice.GetComponent<Dice>().DiceValue = value;

                    ErrorsPanelScript.SendError.Invoke("<color=green>Dice Value Setted To <color=red>" + commandParameters);
                }
                else ErrorsPanelScript.SendError.Invoke("Invalid parameters: " + commandParameters);

                break;

            case "godmode" or "god":
                tilesPlacer.godMode = !tilesPlacer.godMode;
                if(dice) dice.GetComponent<Dice>().GodMode = tilesPlacer.godMode;

                if(tilesPlacer.godMode) ErrorsPanelScript.SendError.Invoke("<color=green>Godmode enabled");
                else ErrorsPanelScript.SendError.Invoke("<color=red>Godmode disabled");
                break;

            case "?" or "help":
                ErrorsPanelScript.SendError.Invoke("<color=green>? // help // setdice <int> // godmode // reset");

                break;

            case "reset":
                break;


            default:
                ErrorsPanelScript.SendError.Invoke("Invalid command: " + command);
                break;
            
        }

        inputField.interactable = false;
        inputField.text = "";
    }
}
