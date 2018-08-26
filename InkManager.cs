using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

public class InkManager : MonoBehaviour {

    public GameObject spawnManager;

    public TextAsset theStoryFile;

    Story inkStory;

    string currentLine;
    List<string> tags;
    string[] currentLineParts;

    public GameObject storyTextPanel;
    public TextMeshProUGUI storyText;
    [Space(5)]
    public GameObject characterPanel;
    public TextMeshProUGUI characterName;




    void Start () {
        inkStory = new Story(theStoryFile.text);
        storyTextPanel.SetActive(false);
	}
	

	void Update () {

	}


    public void Progress()
    {
        StopCoroutine("DisplayText");

        if (inkStory.canContinue)
        {
            currentLine = inkStory.Continue();
            tags = inkStory.currentTags;
            if (currentLine.Contains(":")) {
                currentLineParts = currentLine.Split(':');
                //Debug.Log("it contains " + currentLineParts.Length + " elements!");
                for (int i = 0; i < currentLineParts.Length; i++)
                {
                    currentLineParts[i] = currentLineParts[i].Trim();
                    Debug.Log(currentLineParts[i]);
                }
            } else {

                currentLineParts[0] = "";
                currentLineParts[1] = currentLine;
                Debug.Log(currentLineParts[1]);
            }

            StartCoroutine("DisplayText");

            if (tags.Count != 0)
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    Debug.Log("tag " + i + " is: " + tags[i]);
                    if ( tags[i].Contains("moving blocker"))
                    {
                        spawnManager.GetComponent<SpawnManager>().Spawn(1);
                    } else if (tags[i].Contains("blocker"))
                    {
                        spawnManager.GetComponent<SpawnManager>().Spawn(0);
                    }
                }


            }

        } else if(inkStory.currentChoices.Count > 0)
        {
            for (int i = 0; i < inkStory.currentChoices.Count; ++i)
            {
                Choice choice = inkStory.currentChoices[i];
                Debug.Log("Choice " + (i + 1) + ". " + choice.text);
            }
        }
    }

    public void MakeChoice(int index)
    {
        inkStory.ChooseChoiceIndex(index);
        Progress();
    }

    IEnumerator DisplayText()
    {
        storyTextPanel.SetActive(true);
        characterName.text = currentLineParts[0];
        if(currentLineParts[0] == "Pudding")
        {
            characterPanel.SetActive(true);
        } else
        {
            characterPanel.SetActive(false);
        }
        storyText.text = currentLineParts[1];
        yield return new WaitForSeconds(3);
        storyTextPanel.SetActive(false);
    }
}

