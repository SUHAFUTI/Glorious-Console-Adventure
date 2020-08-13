using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GloriousConsoleAdventureCore.Helpers
{
    public class SoundPlayer
    {
        Dictionary<string, string> _audioFiles;
        public SoundPlayer(Dictionary<string, string> audioFiles)
        {
            _audioFiles = audioFiles;
        }
        private string GetFilePath(string soundName)
        {
            if (!_audioFiles.TryGetValue(soundName, out var name))
                throw new Exception("Unable to find sound {}");
            return name;
        }
        public void PlaySound(string soundName)
        {
            Task.Run(() =>
            {
                using (var audioFile = new AudioFileReader(GetFilePath(soundName)))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
        }
    }
}
