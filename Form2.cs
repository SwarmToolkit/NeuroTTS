using Microsoft.CognitiveServices.Speech;
using NeuroTTS.Properties;
using System;
using System.Windows.Forms;

namespace NeuroTTS
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = Settings.Default.REGION;
            textBox2.Text = Settings.Default.API_KEY;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please fill in both fields!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string ssml = "<speak version='1.0' xml:lang='en-US'><voice name='en-US-AshleyNeural'><express-as style='chat'><prosody pitch='+25%'>Hello! I'm Neuro. Someone tell vedal there is a problem with my AI.</prosody></express-as></voice></speak>";
            SpeechConfig speechConfig = SpeechConfig.FromSubscription(textBox2.Text, textBox1.Text);
            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig))
            {
                SpeechSynthesisResult result = speechSynthesizer.SpeakSsmlAsync(ssml).Result;
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    MessageBox.Show("Test successful!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Settings.Default.REGION = textBox1.Text;
                    Settings.Default.API_KEY = textBox2.Text;
                    Settings.Default.Save();
                    MessageBox.Show("Setup complete!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Test failed: " + result.Reason.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
