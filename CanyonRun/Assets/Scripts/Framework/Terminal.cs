using UnityEngine;
using System.ComponentModel;

/// <summary>
/// Terminal Controller
/// </summary>
public class Terminal : MonoBehaviour
{
    private System.Exception programException = null;

    //If the application is stopped while we're waiting on input, we can error on exit. This is
    //set to false when the instance is destroyed. We can't check the instance from another thread
    //bacause unity doesn't let you use == in another thread (override equals is dumb)
    //This is a threadsafe way of knowing when the instance has been destroyed
    private static bool terminated = false;

    /// <summary>
    /// The UI component for displaying the text area of the terminal
    /// </summary>
    [SerializeField]
    private UI_TextArea textArea;

    /// <summary>
    /// The background worker responsible for running the program
    /// </summary>
    private BackgroundWorker programWorker;

    /// <summary>
    /// If the boot message should be displayed on start
    /// </summary>
    public bool playBootMessage = false;

    /// <summary>
    /// Print the boot message
    /// </summary>
    void BootMessage()
    {
        Terminal.WriteLine("-= SAEos v6.0.4a =-");
        Terminal.WriteLine();
        Terminal.WriteLine("COM1:         at 0375f");
        Terminal.WriteLine("SER1:         at 1476a");
        Terminal.WriteLine("RAM : Checking 640 KBytes ................................ OK.");
        Terminal.WriteLine("Date: " + System.DateTime.Now.AddYears(30).ToShortDateString());
        Terminal.WriteLine();
        Terminal.WriteLine("Boot complete. All OK.");
        Terminal.WriteLine();
    }

    /// <summary>
    /// The terminal singleton instance
    /// </summary>
    private static Terminal Instance { get; set; }

    /// <summary>
    /// This is called once, and only once, before all other game objects have Start() invoked
    /// </summary>
    void Awake()
    {
        //This is an implementation of the Singleton pattern
        //We hold on to a single static instance of Terminal
        //If there is antoher instance, we need to automatically destroy it
        if (Instance != null)
        { //We have an existing instance
            GameObject.Destroy(this); //Destroy the new instance (this)
            return; //Abort the awake, this game object is about to be destroyed
        }

        //If we get here, we are the only instance - so set this game object to the static Instance variable
        Instance = this;
        terminated = false;

        //Make sure there is no text in the termianl text component
        textArea.Clear();
    }

    /// <summary>
    /// This is called when the game object has been destroyed
    /// </summary>
    void OnDestroy()
    {
        //We want to make sure we clean up when destroying singletons
        //If this game object is the active singleton, clear the static variable holding the instance of the singleton
        if (Instance == this)
        {
            terminated = true;
            Instance = null;
        }
    }

    /// <summary>
    /// This is called after every game object in the scene has had Awake() invoked.
    /// This will only ever be called once per GameObject
    /// </summary>
    void Start()
    {
        if (playBootMessage)
        {
            BootMessage();
        }

        programWorker = new BackgroundWorker();
        programWorker.DoWork += programWorker_DoWork;
        programWorker.RunWorkerCompleted += programWorker_RunWorkerCompleted;
        programWorker.RunWorkerAsync();
    }
    
    void Update() 
    {
        if (!textArea.isPrinting) {
            UIController.EnableLaunchButton();
        }    
    }

    void programWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (programException != null)
        {
            Debug.LogError(programException);
        }
    }

    void programWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        try
        {
            Terminal.Write(""); //game dev fix! Prime the text area with an empty value. FIXED FOREVER!

            GameCoreInterface.TestInterfaces();

            WriteLine();
            WriteLine("You are ready to launch Commander.");
        }
        catch (System.Exception ex)
        {
            programException = ex;
        }
    }

    /// <summary>
    /// A static method to write a message to the terminal
    /// </summary>
    /// <param name="message">The message to write to the terminal</param>
    public static void Write(string message)
    {
        if (terminated) return;

        //Write the message
        Instance.textArea.Write(message);
    }

    /// <summary>
    /// Write a message to the terminal and add a new line
    /// </summary>
    /// <param name="message">The message to write to the terminal</param>
    public static void WriteLine(string message)
    {
        if (terminated) return;

        //Write the message
        Instance.textArea.WriteLine(message);
    }

    /// <summary>
    /// Write a blank line to the terminal
    /// </summary>
    public static void WriteLine()
    {
        if (terminated) return;

        //Write the message
        Instance.textArea.WriteLine(string.Empty);
    }

    /// <summary>
    /// Wait for a certain number of seconds
    /// </summary>
    /// <param name="seconds">The time to wait, in seconds</param>
    public static void Wait(float seconds)
    {
        System.Threading.Thread.Sleep((int)(seconds * 1000f));
    }
}
