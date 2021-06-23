using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace BasicExamples
{
    public class SpatialStructureExample
    {
        public static void Show()
        {
            const string file = @"C:\Bibliotecas\SampleHouse.ifc";

            using (var model = IfcStore.Open(file))
            {
                var project = model.Instances.FirstOrDefault<IIfcProject>();
                PrintHierarchy(project, 0);
            }
        }

        private static void PrintHierarchy(IIfcObjectDefinition o, int level)
        {
            Console.WriteLine(string.Format("{0}{1} [{2}]", GetIndent(level), o.Name, o.GetType().Name));

            //apenas elementos espaciais podem conter elementos de construção
            var spatialElement = o as IIfcSpatialStructureElement;
            if (spatialElement != null)
            {
                //usando IfcRelContainedInSpatialElement para obter os elementos contidos
                var containedElements = spatialElement.ContainsElements.SelectMany(rel => rel.RelatedElements);
                foreach (var element in containedElements)
                    Console.WriteLine(string.Format("{0}    ->{1} [{2}]", GetIndent(level), element.Name, element.GetType().Name));
            }

            //usando IfcRelAggregares para obter decomposição espacial de elementos de estrutura espacial
            foreach (var item in o.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
                PrintHierarchy(item, level + 1);
        }

        private static string GetIndent(int level)
        {
            var indent = "";
            for (int i = 0; i < level; i++)
                indent += "  ";
            return indent;
        }
    }
}