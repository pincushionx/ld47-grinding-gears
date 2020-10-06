using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    public class SoundController : MonoBehaviour
    {
        public AudioSource rotate;
        public AudioSource walking;
        public AudioSource running;

        public void playSound(string soundName)
        {
            if (soundName == "rotate")
            {
                if (!rotate.isPlaying)
                {
                    stopSound("rotate");
                    rotate.Play();
                }
            }
            else if (soundName == "walking")
            {
                if (!walking.isPlaying)
                {
                    stopSound("running");
                    walking.Play();
                }
            }
            else if (soundName == "running")
            {
                if (!running.isPlaying)
                {
                    stopSound("walking");
                    running.Play();
                }
            }
        }
        public void stopSound(string soundName)
        {
            if (soundName == "rotate")
            {
                if (rotate.isPlaying)
                {
                    rotate.Stop();
                }
            }
            else if (soundName == "walking")
            {
                if (walking.isPlaying)
                {
                    walking.Stop();
                }
            }
            else if (soundName == "running")
            {
                if (running.isPlaying)
                {
                    running.Stop();
                }
            }
        }
    }
}