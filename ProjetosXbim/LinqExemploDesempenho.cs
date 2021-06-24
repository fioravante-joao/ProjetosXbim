using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace ProjetosXbim
{
    class LinqExemploDesempenho
    {
        public static void SelectionWithLinq()
        {
            const string ifcFilename = @"C:\Users\JVFS\source\repos\ProjetosXbimEstudos\ProjetosXbim\IfcFiles\SampleHouse.ifc";
            var model = IfcStore.Open(ifcFilename);
            using (var txn = model.BeginTransaction())
            {
                var requiredProducts = new IIfcProduct[0]
                    .Concat(model.Instances.OfType<IIfcWallStandardCase>())
                    .Concat(model.Instances.OfType<IIfcDoor>())
                    .Concat(model.Instances.OfType<IIfcWindow>());

                //expression using LINQ
                var ids =
                    from wall in model.Instances.OfType<IIfcWall>()
                    where wall.HasOpenings.Any()
                    select wall.GlobalId;

                foreach (var id in ids)
                {
                    Console.WriteLine(id);
                }

                // expressão equivalente usando extensões encadeadas de expressões IEnumerable e lambda
                //var ids =
                //    model.Instances
                //    .Where<IIfcWall>(wall => wall.HasOpenings.Any())
                //    .Select(wall => wall.GlobalId);

                //foreach (var id in ids)
                //{
                //    Console.WriteLine(id);
                //}

                txn.Commit();
            }
        }
    }
}
