/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
#if UNITY_EDITOR
using System;
using System.IO;

namespace MelenitasDev.SoundsGood.Editor
{
    public class EnumGenerator : IDisposable
    {
        public void GenerateEnum (string enumName, string[] tags)
        {
            string ident = "	";
            string filePathAndName = "Assets/Plugins/Melenitas Dev/Sounds Good/Scripts/Application/" + enumName + ".cs";
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("namespace MelenitasDev.SoundsGood");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine(ident + "public enum " + enumName);
                streamWriter.WriteLine(ident + "{");
                for (int i = 0; i < tags.Length; i++)
                {
                    streamWriter.WriteLine(ident + ident + tags[i] + (i == tags.Length - 1 ? "" : ","));
                }
                streamWriter.WriteLine(ident + "}");
                streamWriter.WriteLine("}");
            }
        }

        public void Dispose ()
        {
            
        }
    }
}
#endif