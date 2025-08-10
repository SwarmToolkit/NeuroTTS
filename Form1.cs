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
                return;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 1)
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
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter some text to synthesize!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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
                    using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig, null as AudioConfig))
                    {
                        SpeechSynthesisResult result = speechSynthesizer.SpeakSsmlAsync(ssml).Result;
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            using (AudioDataStream audioDataStream = AudioDataStream.FromResult(result))
                            {
                                audioDataStream.SaveToWaveFileAsync(fileDialog.FileName).Wait();
                                MessageBox.Show("Generation complete!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Generation cancelled.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
    }
}
