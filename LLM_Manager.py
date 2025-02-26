import torch
from transformers import AutoTokenizer, AutoModelForCausalLM, BitsAndBytesConfig  # MUHAHAHA UwU


TOKENIZER = "Qwen/Qwen2.5-1.5B-Instruct"
MODEL = "Qwen/Qwen2.5-1.5B-Instruct"
MAX_TOKENS = 512
CONTEXT = "An AI assistant is assisting a user. The AI model is Qwen2.5 1.5b. The user is the user of an app called 'The AI helpline'. It is a very good app. The AI Assistant will not repeat itself at all. And the AI will decide when it ends its generation by outputting the EoS token. The conversation between the AI and User is seen below."
device = "cuda" if torch.cuda.is_available() else "cpu"


class LLMManager:
    def __init__(self):
        config = BitsAndBytesConfig(
            load_in_4bit=True,
            bnb_4bit_compute_dtype=torch.float16
        )
        self.tokenizer = AutoTokenizer.from_pretrained(
            TOKENIZER
        )
        self.model = AutoModelForCausalLM.from_pretrained(
            MODEL,
            quantization_config=config,
            device_map="auto"
        )
    
    def forward(self, text):
        input_text = CONTEXT + text
        tokens = self.tokenizer(input_text, return_tensors="pt")["input_ids"].to(device)
        logits = self.model(tokens, attention_mask=None).logits[:, -1, :]
        prob = torch.argmax(logits, dim=-1).type(torch.int64)
        output = self.tokenizer.decode(prob.item())
        if self.tokenizer.eos_token_id == prob.item() or output == "User" or output == "<|endoftext|>":

            # Best return statement ever.
            return "<|EndGenerationNowUWU|>"
        else:
            return output
