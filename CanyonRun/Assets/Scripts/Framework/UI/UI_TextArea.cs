using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The UI component to manage the Termianl text area
/// </summary>
public class UI_TextArea : MonoBehaviour
{
    /// <summary>
    /// The main text ui component
    /// </summary>
    public Text text;

    /// <summary>
    /// How many seconds to wait after each line has been printed
    /// </summary>
    public float lineDelay = 0.1f;

    /// <summary>
    /// How many seconds to wait after each line has been printed, while hurrying
    /// </summary>
    public float hurryLineDelay = 0.05f;

    /// <summary>
    /// How many seconds to wait between each character print
    /// </summary>
    public float characterDelay = 0.1f;

    /// <summary>
    /// A queue to keep track of lines waiting to be printed
    /// </summary>
    private Queue<string> lineQueue = new Queue<string>();

    /// <summary>
    /// An event that is raised when printing starts
    /// </summary>
    public static event System.Action onPrintStart;

    /// <summary>
    /// An event that is raised when printing stops
    /// </summary>
    public static event System.Action onPrintEnd;

    /// <summary>
    /// If the text area is currently printing
    /// </summary>
    public bool isPrinting { get; private set; }

    /// <summary>
    /// If the termianl is currently hurried
    /// </summary>
    private bool isHurried
    {
        get
        {
            return Input.GetKey(KeyCode.Space);
        }
    }

    void Awake()
    {
        isPrinting = true;
    }

    void Start()
    {
        StartCoroutine(DoMessagePrint());
    }

    /// <summary>
    /// This co-routine will managing printing out each character to the terminal
    /// </summary>
    /// <returns></returns>
    IEnumerator DoMessagePrint()
    {
        string currLine = null;

        //We don't want this co-routine to ever end, so it goes in a while(true)
        while (true)
        {
            if (currLine != null)
            { //we have a line we're printing
                foreach (var c in currLine)
                { //move through each character and print it out, one at a time
                    text.text += c; //appent to the text control

                    //We want the user to be able to hurry the text - if it's not hurried, wait for the next character
                    if (!isHurried)
                    {
                        yield return new WaitForSeconds(characterDelay);
                    }
                }

                currLine = null;

                //Check if we have any more lines to render, if not, then raise the appropriate events
                if (lineQueue.Count == 0)
                { //nothing left to display
                    isPrinting = false;

                    //We want to check if an event is null before raising it. If nothing is subscribed to the event, then the event will be null.
                    if (onPrintEnd != null) onPrintEnd();
                }
            }

            //If we don't have a line we're rendering, and there is something in the queue, process it now
            if (currLine == null && lineQueue.Count > 0)
            {
                //Dequeue the next line
                currLine = lineQueue.Dequeue();

                //We want to raise an event when we start printing
                //Using a print flag means that we'll only fire it once, at least until we reset isPrinting to false
                if (!isPrinting)
                {
                    isPrinting = true;
                    if (onPrintStart != null) onPrintStart();
                }

                //Execute a small delay after each line.
                //This line also demonstrates a shortcut for an if (condition) { use variable a } else { use variable b }
                //This is an inline conditional
                yield return new WaitForSeconds(isHurried ? hurryLineDelay : lineDelay);
            }

            //Yielding null in a coroutine will leave the method and wait for the next Update()
            yield return null;
        }
    }

    /// <summary>
    /// Clear the terminal text
    /// </summary>
    public void Clear()
    {
        lock (this)
        {
            //Clear the text
            text.text = "";

            //Empty our queue 
            lineQueue.Clear();
        }
    }

    /// <summary>
    /// Write text to the terminal text component
    /// </summary>
    /// <param name="message">The message to write</param>
    public void Write(string message)
    {
        lock (this)
        {
            isPrinting = true;
            //Queue the line - the corountine will automatically start printing the new message
            lineQueue.Enqueue(message);
        }
    }

    /// <summary>
    /// Write a line to the terminal text component
    /// </summary>
    /// <param name="message">The message to write to a single line</param>
    public void WriteLine(string message)
    {
        lock (this)
        {
            isPrinting = true;
            //Queue the line, but also append the new line characters
            lineQueue.Enqueue(message + "\r\n");
        }
    }
}
