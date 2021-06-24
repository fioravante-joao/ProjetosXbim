using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using BasicExamples;

namespace ProjetosXbim
{
    class Program
    {
        static void Main(string[] args)
        {
            //SpatialStructureExample.Show(); //pega a estrutura do modelo espacial.

            //BasicModelOperations.GetDadosModel();  // pega dados do modelo.

            //LinqExemploDesempenho.SelectionWithLinq();

            RelatorioDeEspacoExcel.Create();

            Console.ReadLine();
        }
    }
}
