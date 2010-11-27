using System;
using System.Speech.Synthesis;
using Phoenix;

namespace Phoenix.Speech
{
    /// <summary>
    /// Wrapper around <see cref="SpeechSynthesizer"/>.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Rate of speech.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Volume of speech.
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            Volume = 100;
        }

        /// <summary>
        /// Says the text using windows synthesizer (that means aloud using sound card).
        /// </summary>
        /// <param name="text">Text to say.</param>
        [Command]
        public void Speak(string text)
        {
            if (text == null || text.Length == 0)
                return;

            using (SpeechSynthesizer synth = new SpeechSynthesizer()) {
                synth.Volume = Volume;
                synth.Rate = Speed;

                synth.Speak(text);
            }            
        }
    }
}
