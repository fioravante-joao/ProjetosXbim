using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace ProjetosXbim
{
    class StepToXmlExample
    {
        public static void Convert()
        {
            const string patch = @"C:\Users\JVFS\source\repos\ProjetosXbimEstudos\ProjetosXbim\IfcFiles\SampleHouse.ifc";
            //open STEP21 file
            using (var stepModel = IfcStore.Open(patch))
            {
                //save as XML
                stepModel.SaveAs(@"C:\Users\JVFS\source\repos\ProjetosXbimEstudos\ProjetosXbim\IfcFiles\SampleHouse.ifcxml");

                //open XML file
                using (var xmlModel = IfcStore.Open(@"C:\Users\JVFS\source\repos\ProjetosXbimEstudos\ProjetosXbim\IfcFiles\SampleHouse.ifcxml"))
                {
                    //just have a look that it contains the same number of entities and walls.
                    var stepCount = stepModel.Instances.Count;
                    var xmlCount = xmlModel.Instances.Count;

                    var stepWallsCount = stepModel.Instances.CountOf<IIfcWall>();
                    var xmlWallsCount = xmlModel.Instances.CountOf<IIfcWall>();

                    Console.WriteLine($"STEP21 file has {stepCount} entities. XML file has {xmlCount} entities.");
                    Console.WriteLine($"STEP21 file has {stepWallsCount} walls. XML file has {xmlWallsCount} walls.");
                }
            }
        }
    }
}
