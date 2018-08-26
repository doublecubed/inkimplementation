using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime; // This is the namespace required to use ink plugin in a script.
using TMPro;
using UnityEngine.UI;

public class InkManager : MonoBehaviour {



    public GameObject spawnManager; 
	// I wrote this script for a platformer type game, so there's a spawn manager that handles the spawning of enemies. 
	// The enemies are spawned by tags in the ink story.

    public TextAsset theStoryFile;
	// This is the raw text file that contains the ink story.

    Story inkStory;
	// This is the Story class that we create, it uses the file we specify above. 
	// This is what the ink plugin uses to progress the story.

    string currentLine;
    List<string> tags;
    string[] currentLineParts;
	// The variables above are used when we chop the current story line into pieces to get any tags.
	// currentLine is the entire line, placed into a string.
	// tags is a List, which lists the tags in order. It's a list so it can dynamically change because we don't know how many tags a line will contain, if any.
	// currentLineParts 
	

    public GameObject storyTextPanel;
    public TextMeshProUGUI storyText;
    [Space(5)]
    public GameObject characterPanel;
    public TextMeshProUGUI characterName;
	// These are the variables required for my game.
	// storyTextPanel is the UI canvas on which all text is displayed.
	// storyText is the text object that displays the actual text.
	// characterPanel is a section that displays the character name and picture.
	// characterName is the text object that displays the speaking character's name.




    void Start () {
        inkStory = new Story(theStoryFile.text);  // This file creates an ink Story from the text asset file.
        storyTextPanel.SetActive(false); // I turn off the text panel because it will only be used during a narrative.
	}
	

	void Update () {

	}

	// This function is the main function for ink story. The game calls it whenever it will display a new piece of story.
    public void Progress()
    {
        StopCoroutine("DisplayText"); // If any other previous text is being displayed, it is stopped first.

		// the following if checks whether the story is able to continue linearly or if there's a choice present.
        if (inkStory.canContinue)
        {
            currentLine = inkStory.Continue();  // currentLine is set to the next line in the ink story.
            tags = inkStory.currentTags;  // tags is a List class, and currentTags command fills it with the tags in the story.
            if (currentLine.Contains(":")) {  // the : character is used to understand if a character is speaking. I my game there was only one character. I didn't use : for the narrator.
                currentLineParts = currentLine.Split(':'); // if there is a ":" the parts are split from that section. The first part is the name and the second part is the message.
                for (int i = 0; i < currentLineParts.Length; i++) // this for loop trims the strings of the spaces.
                {
                    currentLineParts[i] = currentLineParts[i].Trim(); // the trim command.
                    Debug.Log(currentLineParts[i]); // displays the parts in the console to be sure :)
                }
            } else { // if there is no ":"

                currentLineParts[0] = "";  // the zeroeth element (character name element) of the line is blank. This way no name will be displayed.
                currentLineParts[1] = currentLine; // the currentLine (the message) is entirely set to the first element.
                Debug.Log(currentLineParts[1]);
            }

            StartCoroutine("DisplayText");  // after the parsing is done, it's time to display the message. This is a coroutine and its at the bottom.

            if (tags.Count != 0)  // Here the tags are evaluated after the message is displayed. If the tags are more than zero, this code runs.
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    Debug.Log("tag " + i + " is: " + tags[i]);
                    if ( tags[i].Contains("moving blocker")) // these are for spawning my enemies. It checks if the tag has "moving blocker" in it and it spawns a moving blocker.
                    {
                        spawnManager.GetComponent<SpawnManager>().Spawn(1);
                    } else if (tags[i].Contains("blocker")) // or it spawns a blocker.
                    {
                        spawnManager.GetComponent<SpawnManager>().Spawn(0);
                    }
                }


            }

        } else if(inkStory.currentChoices.Count > 0)  // this is the second part of the big if. If there are choices. But this one only displays the choices on the console so I can see them.
        {
            for (int i = 0; i < inkStory.currentChoices.Count; ++i)
            {
                Choice choice = inkStory.currentChoices[i];
                Debug.Log("Choice " + (i + 1) + ". " + choice.text);
            }
        }
    }

    public void MakeChoice(int index)  // This is the method that makes the choice. It's activated by triggers in the game, depending on where the character goes.
    {
        inkStory.ChooseChoiceIndex(index);
        Progress();
    }

    IEnumerator DisplayText()  // the text is displayed through this. 
    {
        storyTextPanel.SetActive(true);
        characterName.text = currentLineParts[0]; // sets the character's name to the zeroeth element.
        if(currentLineParts[0] == "Pudding") // if its pudding the character, display the panel. (because there were no other characters, I just wrote its name up there and switched the panel on and off for convenience)
        {
            characterPanel.SetActive(true);
        } else
        {
            characterPanel.SetActive(false);
        }
        storyText.text = currentLineParts[1]; // sets the message to the screen
        yield return new WaitForSeconds(3); // waits for three seconds
        storyTextPanel.SetActive(false); // closes the message.
    }
}

