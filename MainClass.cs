using FlagPFP.Core.Exceptions;
using FlagPFP.Core.Loading;
using FlagPFP.Core.Processing;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FlagPFP.Core.FlagMaking
{
    /// <summary>
    /// The principal attraction of FlagPFP.Core.
    /// This is where you can execute the flag creation or load the flag JSONs.
    /// </summary>
    public class FlagCoreObject
    {
        public Dictionary<string, PrideFlag> FlagDictionary;
        public string FlagsDirectory;

        /// <summary>
        /// Initializes a <c>FlagCoreObject</c> instance.
        /// </summary>
        /// <param name="flagsDir">The folder name of where the flag images are located.</param>
        public FlagCoreObject(string flagsDir)
        {
            FlagsDirectory = flagsDir;
        }

        /// <summary>
        /// Loads all of the JSON Flag definitions from the parameter <c>dir</c>.
        /// </summary>
        /// <param name="dir">The folder name of where the flag JSONs are located.</param>
        /// <exception cref="NoFlagsFoundException">Thrown when no JSON definitions are found inside the directory.</exception>
        public void LoadFlagDefsFromDir(string dir)
        {
            FlagLoader flagLoader = new FlagLoader();
            FlagDictionary = flagLoader.LoadFlags(dir);
        }


        /// <summary>
        /// Generates an output image with the parameters attached.
        /// </summary>
        /// <param name="inputImage">The input image <c>Bitmap</c> for example, your profile picture.</param>
        /// <param name="pixelMargin">The space in pixels between the border of the circle and the border of the image.</param>
        /// <param name="innerSize">The size in pixels of <c>inputImage</c>.</param>
        /// <param name="fullSize">The size in pixels of the output image.</param>
        /// <param name="outputImage">The file name for the output image.</param>
        /// <param name="flags">Can take in any number of parameters. You can pass several flags and the program will divide the
        /// output into segments.</param>
        /// <exception cref="InvalidFlagException">Thrown when one or more elements in <c>flags</c> is not a valid flag name.</exception>
        public void ExecuteProcessing(string inputImage, int pixelMargin, int innerSize,
            int fullSize, string outputImage, params string[] flags)
        {
            BitmapProcessing bitmapProcessor = new BitmapProcessing();
            bitmapProcessor.SetFullImageSize(fullSize);

            List<PrideFlag> chosenFlags = new List<PrideFlag>();
            foreach (string chosenFlag in flags)
            {
                PrideFlag outputFlag;
                if (!FlagDictionary.TryGetValue(chosenFlag, out outputFlag))
                {
                    throw new InvalidFlagException($"Flag type \"{chosenFlag}\" is invalid.");
                }
                chosenFlags.Add(outputFlag);
            }

            Bitmap inputBmp = bitmapProcessor.LoadAndResizeBmp(inputImage, fullSize, fullSize);
            Bitmap primaryFlagBmp = bitmapProcessor.LoadAndResizeBmp(Path.Combine(FlagsDirectory, chosenFlags[0].FlagFile),
                                                                fullSize, fullSize);

            Bitmap croppedPrimaryFlagBmp = bitmapProcessor.CropFlag(ref primaryFlagBmp, pixelMargin);
            // Place primary flag now.
            Bitmap finalBmp = bitmapProcessor.StitchTogether(ref croppedPrimaryFlagBmp, ref inputBmp, innerSize);
            chosenFlags.RemoveAt(0);

            int segmentWidth = finalBmp.Width / flags.Length;
            int currentWidth = segmentWidth;
            foreach (PrideFlag prideFlag in chosenFlags)
            {
                Bitmap secondaryFlagBmp = bitmapProcessor.LoadAndResizeBmp(Path.Combine(FlagsDirectory, prideFlag.FlagFile),
                                                                            fullSize, fullSize);
                Bitmap croppedSecondaryFlagBmp = bitmapProcessor.CropFlag(ref secondaryFlagBmp, pixelMargin);
                croppedSecondaryFlagBmp = bitmapProcessor.ProcessSecondaryFlag(ref croppedSecondaryFlagBmp, currentWidth);

                finalBmp = bitmapProcessor.StitchTogether(ref croppedSecondaryFlagBmp, ref finalBmp, fullSize);

                currentWidth += segmentWidth;
            }

            finalBmp.Save(outputImage, ImageFormat.Png);
        }
    }
}
