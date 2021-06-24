using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace ProjetosXbim
{
    class RelatorioDeEspacoExcel
    {
        public static void Create()
        {
            // inicializar a pasta de trabalho NPOI a partir do modelo
            //esse modelo precisa existir para que o relatório seja gerado.
            var workbook = new XSSFWorkbook("C:\\Users\\JVFS\\source\\repos\\ProjetosXbimEstudos\\ProjetosXbim\\IfcFiles\\template.xlsx");
            var sheet = workbook.GetSheet("Spaces");


            // Crie formatos numéricos agradáveis ​​com unidades. As unidades precisariam de MUITO MAIS cuidado no mundo real.
            // Nós apenas sabemos que nosso modelo atual tem áreas espaciais em metros quadrados e volumes espaciais em metros cúbicos
            // Observe que os dados originais exportados do Revit estavam errados porque os volumes eram 1000 vezes maiores do que deveriam ser.
            // Os dados foram corrigidos usando xbim para este exemplo.

            var areaFormat = workbook.CreateDataFormat();
            var areaFormatId = areaFormat.GetFormat("# ##0.00 [$m²]");
            var areaStyle = workbook.CreateCellStyle();
            areaStyle.DataFormat = areaFormatId;

            var volumeFormat = workbook.CreateDataFormat();
            var volumeFormatId = volumeFormat.GetFormat("# ##0.00 [$m³]");
            var volumeStyle = workbook.CreateCellStyle();
            volumeStyle.DataFormat = volumeFormatId;


            // Abra o modelo IFC. Não vamos mudar nada no modelo, portanto, podemos deixar de fora as credenciais do editor.
            using (var model = IfcStore.Open("C:\\Users\\JVFS\\source\\repos\\ProjetosXbimEstudos\\ProjetosXbim\\IfcFiles\\SampleHouse.ifc"))
            {
                // Pega todos os espaços do modelo.
                // Usamos ToList () aqui para evitar enumeração múltipla com Count () e foreach () {}
                var spaces = model.Instances.OfType<IIfcSpace>().ToList();
                
                // Definir o conteúdo do cabeçalho
                sheet.GetRow(0).GetCell(0)
                    .SetCellValue($"Space Report ({spaces.Count} spaces)");
                foreach (var space in spaces)
                {
                    // escrever dados de relatório
                    WriteSpaceRow(space, sheet, areaStyle, volumeStyle);
                }
            }

            //Salvar o relatorio
            using (var stream = File.Create("spaces.xlsx"))
            {
                workbook.Write(stream);
                stream.Close();
            }

            // veja o resultado se você tiver algum SW associado ao * .xlsx
            //  Process.Start("spaces.xlsx");   //essa linha seria executada no program.cs  e abriria o arquivo gerado automaticamente.
            //O arquivo foi gerado no diretório bin -> Debug da aplicação
        }

        private static void WriteSpaceRow(IIfcSpace space, ISheet sheet, ICellStyle areaStyle, ICellStyle volumeStyle)
        {
            var row = sheet.CreateRow(sheet.LastRowNum + 1);

            var name = space.Name;
            row.CreateCell(0).SetCellValue(name);

            var floor = GetFloor(space);
            row.CreateCell(1).SetCellValue(floor?.Name);

            var area = GetArea(space);
            if (area != null)
            {
                var cell = row.CreateCell(2);
                cell.CellStyle = areaStyle;

                //there is no guarantee it is a number if it came from property and not from a quantity
                if (area.UnderlyingSystemType == typeof(double))
                    cell.SetCellValue((double)(area.Value));
                else
                    cell.SetCellValue(area.ToString());
            }

            var volume = GetVolume(space);
            if (volume != null)
            {
                var cell = row.CreateCell(3);
                cell.CellStyle = volumeStyle;

                //there is no guarantee it is a number if it came from property and not from a quantity
                if (volume.UnderlyingSystemType == typeof(double))
                    cell.SetCellValue((double)(volume.Value));
                else
                    cell.SetCellValue(volume.ToString());
            }
        }

        private static IIfcBuildingStorey GetFloor(IIfcSpace space)
        {
            return
                //get all objectified relations which model decomposition by this space
                space.Decomposes

                //select decomposed objects (these might be either other space or building storey)
                .Select(r => r.RelatingObject)

                //get only storeys
                .OfType<IIfcBuildingStorey>()

                //get the first one
                .FirstOrDefault();
        }

        private static IIfcValue GetArea(IIfcProduct product)
        {
            //try to get the value from quantities first
            var area =
                //get all relations which can define property and quantity sets
                product.IsDefinedBy

                //Search across all property and quantity sets. 
                //You might also want to search in a specific quantity set by name
                .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)

                //Only consider quantity sets in this case.
                .OfType<IIfcElementQuantity>()

                //Get all quantities from all quantity sets
                .SelectMany(qset => qset.Quantities)

                //We are only interested in areas 
                .OfType<IIfcQuantityArea>()

                //We will take the first one. There might obviously be more than one area properties
                //so you might want to check the name. But we will keep it simple for this example.
                .FirstOrDefault()?
                .AreaValue;

            if (area != null)
                return area;

            //try to get the value from properties
            return GetProperty(product, "Area");
        }

        private static IIfcValue GetVolume(IIfcProduct product)
        {
            var volume = product.IsDefinedBy
                .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)
                .OfType<IIfcElementQuantity>()
                .SelectMany(qset => qset.Quantities)
                .OfType<IIfcQuantityVolume>()
                .FirstOrDefault()?.VolumeValue;
            if (volume != null)
                return volume;
            return GetProperty(product, "Volume");
        }

        private static IIfcValue GetProperty(IIfcProduct product, string name)
        {
            return
                //get all relations which can define property and quantity sets
                product.IsDefinedBy

                //Search across all property and quantity sets. You might also want to search in a specific property set
                .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)

                //Only consider property sets in this case.
                .OfType<IIfcPropertySet>()

                //Get all properties from all property sets
                .SelectMany(pset => pset.HasProperties)

                //lets only consider single value properties. There are also enumerated properties, 
                //table properties, reference properties, complex properties and other
                .OfType<IIfcPropertySingleValue>()

                //lets make the name comparison more fuzzy. This might not be the best practise
                .Where(p =>
                    string.Equals(p.Name, name, System.StringComparison.OrdinalIgnoreCase) ||
                    p.Name.ToString().ToLower().Contains(name.ToLower()))

                //only take the first. In reality you should handle this more carefully.
                .FirstOrDefault()?.NominalValue;
        }
    }
}
