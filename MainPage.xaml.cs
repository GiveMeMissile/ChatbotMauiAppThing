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
    const int MAX_TOKENS = 512;


    private async Task GenerateResponce()
    {
        // Set up a new label to output the AI's response
        Label label = new Label();
        label.Text = "AI: ";
        label.TextColor = Color.FromRgb(0, 0, 255);
        AIUserChat.Add(label);

        // Change the User's editor so they cannot input anything (:
        UserInputEditor.IsReadOnly = true;
        UserInputEditor.Text = "";
        UserInputEditor.Placeholder = "Generating...";


        string text;
        int tokens = 0;
            
        while (generating)
        {
            using (Py.GIL())
            {
                text = (string)aiManager.forward(aiInput);
                Random random = new Random();
            }

            if (text == endGeneration || tokens >+ MAX_TOKENS)
            {
                
                UserInputEditor.IsReadOnly = false;
                UserInputEditor.Placeholder = "Ask the AI assistant anything you want!";
                break;
            }
            label.Text += text;
            aiInput += text;
            tokens++;
            await Task.Delay(5);
            
        }
        generating = false;
    }

    private async void InputText(object sender, EventArgs e)
    {
        if (generating)
        {
            return;
        }
        Label label = new Label();
        label.Text = "\nUser:\n" + UserInputEditor.Text;
        aiInput += label.Text + "\nAssistant\n";
        AIUserChat.Add(label);
        generating = true;

        await GenerateResponce();
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
