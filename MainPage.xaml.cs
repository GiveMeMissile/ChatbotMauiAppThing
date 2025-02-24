using Python.Runtime;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace ChatbotThingyWow;

public partial class MainPage : ContentPage
{
    string aiInput = "";
	bool generating = false;
	dynamic aiManager;
	string endGeneration = "<|EndGenerationNowUWU|>";


    private async Task GenerateResponce()
    {
        // Set up a new label to output the AI's response
        Label label = new Label();
        label.Text = "";
        AIUserChat.Add(label);

        // Change the User's editor so they cannot input anything (:)
        //UserInputEditor.IsReadOnly = true;
        //UserInputEditor.Text = "";
        //UserInputEditor.Placeholder = "Generating...";
        await MainThread.InvokeOnMainThreadAsync(() => {
            AIUserChat.Add(label);
            UserInputEditor.IsReadOnly = true;
            UserInputEditor.Text = "";
            UserInputEditor.Placeholder = "Generating...";
        });


        string text;
            
        while (generating)
        {
            using (Py.GIL())
            {
                //text = (string)aiManager.forward(aiInput);
                text = "UWU";
                Random random = new Random();
                if (random.Next(1, 100) == 42)
                {
                    text = endGeneration;
                }
            }

            if (text == endGeneration)
            {
                await MainThread.InvokeOnMainThreadAsync(() => {
                    UserInputEditor.IsReadOnly = false;
                    UserInputEditor.Placeholder = "Ask the AI assistant anything you want!";
                });
                //UserInputEditor.IsReadOnly = false;
                //UserInputEditor.Placeholder = "Ask the AI assistant anything you want!";
                break;
            }
            await MainThread.InvokeOnMainThreadAsync(() => {
                label.Text += text;
            });
            //label.Text += text;
            aiInput += text;
                
            await Task.Delay(10);
        }
    }

    private void InputText(object sender, EventArgs e)
    {
        Label label = new Label();
        label.Text = "\nUser:\n" + UserInputEditor.Text;
        aiInput += label.Text;
        AIUserChat.Add(label);

        Task.Run(async () => await GenerateResponce());
    }

    public MainPage()
    {
        InitializeComponent();
        Runtime.PythonDLL = @"C:\Users\dylan\anaconda3\python312.dll";
        PythonEngine.Initialize();
        using (Py.GIL())
        {
            dynamic sys = Py.Import("sys");
            sys.path.append(@"ChatbotThingyWow");  // Fixed path.append

            aiManager = Py.Import("LLM_Manager");
			aiManager = aiManager.LLMManager();
        }
        
        // Task.Run(async () => await GenerateResponce());
    }
}