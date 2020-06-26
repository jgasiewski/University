using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    Parser parser;
    DialogueObject dObj;
    Block currBlock;

    public DialogueTest()
    {
        parser = new Parser();
        dObj = null;
        currBlock = null;
    }

	void Start()
    {
        dObj = parser.Compile("Assets/Texts/Parser.txt");

        if(dObj != null)
        {
            currBlock = dObj.Start("Test");
            Debug.Log(currBlock);

            while (currBlock != null)
            {
                currBlock = dObj.Next();
                Debug.Log(currBlock);
            }

            currBlock = dObj.Start("Test");
            Debug.Log(currBlock);

            while (currBlock != null)
            {
                currBlock = dObj.Next();
                Debug.Log(currBlock);
            }

            currBlock = dObj.Start("Branch");
            Debug.Log(currBlock);

            while (currBlock != null)
            {
                currBlock = dObj.Next();
                Debug.Log(currBlock);
            }

            currBlock = dObj.Start("Branch");
            Debug.Log(currBlock);

            while (currBlock != null)
            {
                currBlock = dObj.Next();
                Debug.Log(currBlock);
            }
        }   
    }
}
