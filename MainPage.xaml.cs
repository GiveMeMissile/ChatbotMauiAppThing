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

    private async Task ManageEditorWhileGenerate()
    {
        UserInputEditor.IsReadOnly = true;
        UserInputEditor.Text = "";
        UserInputEditor.Placeholder = "Generating...";

        int dot = -1;

        while(generating)
        {
            switch (dot)
            {
                case 1:
                    UserInputEditor.Placeholder = "Generating.";
                    dot = 2;
                    break;
                case 2:
                    UserInputEditor.Placeholder = "Generating..";
                    dot = 3;
                    break;
                case 3:
                    UserInputEditor.Placeholder = "Generating...";
                    dot = -1;
                    break;
                default:
                    UserInputEditor.Placeholder = "Generating";
                    dot=1;
                    break;
            }
            await Task.Delay(500);
        }
        UserInputEditor.IsReadOnly = false;
        UserInputEditor.Placeholder = "Ask the AI assistant anything you want!";
    }

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
                generating = false;
                break;
            }
            label.Text += text;
            aiInput += text;
            tokens++;
            await Task.Delay(1);
        }
    }

    private void InputChange(object sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue;
        string newText = e.NewTextValue;
        string myText = UserInputEditor.Text;
    }

    private void InputComplete(object sender, EventArgs e)
    {
        string text = ((Editor)sender).Text;
    }

    private async void InputText(object sender, EventArgs e)
    {
        if (generating || UserInputEditor.Text == "")
        {
            return;
        }
        Label label = new Label();
        label.Text = "\nUser:\n" + UserInputEditor.Text;
        aiInput += label.Text + "\nAssistant\n";
        AIUserChat.Add(label);
        generating = true;
        Task manageTask = ManageEditorWhileGenerate();
        Task generateTask = GenerateResponce();
        await manageTask;
        await generateTask;
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

    }
}
