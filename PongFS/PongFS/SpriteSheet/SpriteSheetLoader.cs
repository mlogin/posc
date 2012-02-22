using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using PongFS.Config;
using PongFS.Core;

namespace PongFS.SpriteSheet
{
    public class SpriteSheetLoader
    {
        public Color TransparencyColor{get;set;}
        private Dictionary<string, List<Frame>> animations = new Dictionary<string,List<Frame>>();
        private Frame currentFrame;
        private string currentAnimation;
        private int currentFrameIndex;
        private int currentCount;
        public event EventHandler OnAnimComplete;

        public SpriteSheetLoader(string xmlDescriptor)
        {
            LoadFromXmlStream(xmlDescriptor + ".xml");
        }

        public Frame GetCurrentAnimationFrame()
        {
            return currentFrame;
        }

        public bool SetCurrentAnimation(string animKey)
        {
            List<Frame> frames;
            if (animations.TryGetValue(animKey, out frames))
            {
                currentAnimation = animKey;
                currentFrameIndex = 0;
                currentCount = 0;
                currentFrame = frames[currentFrameIndex];
                return true;
            }
            else
            {
                return false;
            }

        }

        public Frame GetNextAnimationFrame()
        {
            List<Frame> frames;
            animations.TryGetValue(currentAnimation, out frames);
            if (currentCount < frames[currentFrameIndex].duration)
            {
                currentCount++;
            }
            else
            {
                if (currentFrameIndex == frames.Count - 1 && frames.Count > 1)
                {
                    OnAnimComplete(currentAnimation, null);
                }
                currentCount = 0;
                currentFrameIndex = (currentFrameIndex + 1) % frames.Count;
            }
            currentFrame = frames[currentFrameIndex];
            return GetCurrentAnimationFrame();
        }

        private void LoadFromXmlStream(string xml)
        {
            string result = string.Empty;

            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("PongFS.Config.Animations." + xml))
            {
                XmlDocument mappingFile = new XmlDocument();
                mappingFile.Load(stream);
                XmlNodeList anims = mappingFile.GetElementsByTagName("animations");
                foreach(XmlNode anim in anims[0]){
                    List<Frame> listFrames = new List<Frame>();
                    foreach(XmlNode animSpec in anim.ChildNodes){
                        if (animSpec.Name == "transparentColour")
                        {
                            TransparencyColor = Helper.ToColor(animSpec.InnerText);
                        }
                        else if (animSpec.HasChildNodes)
                        {
                            foreach (XmlNode frame in animSpec.ChildNodes)
                            {
                                Frame f = new Frame();
                                string strRect = frame.SelectSingleNode("sourceRect").InnerText;
                                int[] rectParams = Helper.ToInt(strRect.Replace("rect(", "").Replace(" ", "").Replace(")", "").Split(','));
                                f.rect = new Rectangle(rectParams[0], rectParams[1], rectParams[2] - rectParams[0], rectParams[3] - rectParams[1]);
                                f.duration = Int32.Parse(frame.SelectSingleNode("duration").InnerText);
                                string strOffset = frame.SelectSingleNode("offset").InnerText;
                                int[] offsetParams = Helper.ToInt(strOffset.Replace("point(", "").Replace(")", "").Replace(" ", "").Split(','));
                                f.offset = new Point(offsetParams[0], offsetParams[1]);
                                listFrames.Add(f);
                            }
                        }
                    }
                    animations.Add(anim.Name, listFrames);
                }

                stream.Close();
            }

        }
    }
}
