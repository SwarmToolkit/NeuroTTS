using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NeuroTTS.Properties;
using System;
using System.Windows.Forms;

namespace NeuroTTS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            if (Settings.Default.REGION == "" || Settings.Default.API_KEY == "")
            {
                Form2 form2 = new Form2();
                form2.ShowDialog();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label2.Text = "Current words: " + textBox1.Text.Length.ToString();
            label3.Visible = textBox1.Text.Length > 2000 ? true : false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Settings.Default.REGION == "" || Settings.Default.API_KEY == "")
            {
                MessageBox.Show("Please setup your API key first!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Form2 form2 = new Form2();
                form2.ShowDialog();
                return;
            }
            string ssml = "<speak version='1.0' xml:lang='en-US'><voice name='en-US-AshleyNeural'><express-as style='chat'><prosody pitch='+25%'>Hello! I'm Neuro. Someone tell vedal there is a problem with my AI.</prosody></express-as></voice></speak>";
            SpeechConfig speechConfig = SpeechConfig.FromSubscription(Settings.Default.API_KEY, Settings.Default.REGION);
            button1.Enabled = false;
            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig))
            {
                SpeechSynthesisResult result = speechSynthesizer.SpeakSsmlAsync(ssml).Result;
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    button1.Enabled = true;
                    MessageBox.Show("Test successful!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    button1.Enabled = true;
                    MessageBox.Show("Test failed: " + result.Reason.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (MessageBox.Show("It looks like the API configuration didn't take effect. Have you rotated the API key?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Form2 form2 = new Form2();
                        form2.ShowDialog();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Settings.Default.REGION == "" || Settings.Default.API_KEY == "")
            {
                MessageBox.Show("Please setup your API key first!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Form2 form2 = new Form2();
                form2.ShowDialog();
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter some text to synthesize!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox1.Text.Length > 2000)
            {
                if (MessageBox.Show("The word count exceeds 2000. This will cause a lot of API quota and times. Do you want to continue?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }
            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "WAV files (*.wav)|*.wav";
                fileDialog.DefaultExt = "wav";
                fileDialog.AddExtension = true;
                fileDialog.Title = "Save synthesized audio";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string ssml = "<speak version='1.0' xml:lang='en-US'><voice name='en-US-AshleyNeural'><express-as style='chat'><prosody pitch='+25%'>" + textBox1.Text + "</prosody></express-as></voice></speak>";
                    SpeechConfig speechConfig = SpeechConfig.FromSubscription(Settings.Default.API_KEY, Settings.Default.REGION);
                    button1.Enabled = false;
                    using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig, null as AudioConfig))
                    {
                        SpeechSynthesisResult result = speechSynthesizer.SpeakSsmlAsync(ssml).Result;
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            using (AudioDataStream audioDataStream = AudioDataStream.FromResult(result))
                            {
                                audioDataStream.SaveToWaveFileAsync(fileDialog.FileName).Wait();
                                button1.Enabled = true;
                                MessageBox.Show("Generation complete!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            button1.Enabled = true;
                            MessageBox.Show("Generation failed: " + result.Reason.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show("Generation failed, please try again. If this continues to happen, you can click the \"Test\" button to test your API key configuration." + result.Reason.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Generation cancelled.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
