using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace ProjetosXbim
{
    public class BasicModelOperations
    {
        public static void GetDadosModel()
        {
            //Documento Utilizado está acoplado à Solution
            const string fileName = @"C:\Users\JVFS\source\repos\ProjetosXbimEstudos\ProjetosXbim\IfcFiles\SampleHouse.ifc";

            using (var model = IfcStore.Open(fileName))
            {
                // obter todas as portas no modelo (usando a interface IFC4 de IfcDoor, isso funcionará tanto para IFC2x3 quanto para IFC4)
                var allDoors = model.Instances.OfType<IIfcDoor>();

                // obtém apenas portas com IIfcTypeObject definido
                var someDoors = model.Instances.Where<IIfcDoor>(d => d.IsTypedBy.Any());

                // pega uma única porta
                var id = "1amqZU13q0JRR$Gsa$X8X$";
                var theDoor = model.Instances.FirstOrDefault<IIfcDoor>(d => d.GlobalId == id);
                Console.WriteLine($"Door ID: {theDoor.GlobalId}, Name: {theDoor.Name}");

                // obtém todas as propriedades de valor único da porta
                var properties = theDoor.IsDefinedBy
                    .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                    .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                    .OfType<IIfcPropertySingleValue>();
                foreach (var property in properties)
                    Console.WriteLine($"Property: {property.Name}, Value: {property.NominalValue}");
            }

        }
    }
}
