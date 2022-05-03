/*
    Requerimiento 1: Modificar la matriz TranD para poder comentar codigo de linea y multilinea **
    Requerimiento 2: La primera produccion debe ser publica 
    Requerimiento 3: Para ST escapados quitar el diagonal(\)
    Requerimiento 4: Cuando es clasificacion invocar el metodo Match sobrecargado sin comillas
    Requerimiento 5: Agregar CIzquierdo y CDerecho en matriz TranD (generar automatas) **
    Requerimiento 6: Programar el OR en producciones gramaticales  
    Requerimiento 7: Generar el Program.cs invocando la primera produccion 
*/
using System.IO;
using System;
namespace Generador
{
    public class Lenguaje:Sintaxis
    {
        int Tab;
        public Lenguaje()
        {
            Tab = 0;
        }
        private void Genera(string codigo) 
        {
            if(codigo=="}")
            {
                Tab--;
            }
            for(int i = 0; i < Tab; i++)
            {
                gen.Write("\t");
            }
            gen.WriteLine(codigo);
            if(codigo=="{")
            {
                Tab++;
            }
        }
        //Gramatica -> Cabecera Producciones
        public void Gramatica()
        {
            Cabecera();
            Producciones();
        }
        // Cabecera -> Lenguaje: SNT;
        private void Cabecera()
        {
            Match("Lenguaje");
            Match(":");
            gen.Write(getContenido());
            Match(Tipos.SNT);
            Genera(getContenido());
            Match(Tipos.FinProduccion);
        }
        // Producciones -> {ListaProducciones}
        private void Producciones()
        {
            Match("{");
            Genera("{");
            Genera("public class Lenguaje:Sintaxis");
            Genera("{");
            ListaProducciones();
            Genera("}");
            Match("}");
            Genera("}");

        }
        // ListaProducciones -> Produccion; ListaProduciones?
        private void ListaProducciones()
        {
            Produccion();
            if(getContenido() != "}")
            {
                ListaProducciones();
            }
        }
        // Produccion -> SNT Flechita 
        private void Produccion()
        {
            Genera("private void " + getContenido()+("()"));
            Genera("{");
            Match(Tipos.SNT);
            Match(Tipos.Flechita);
            ListaSimbolos();
            Match(Tipos.FinProduccion);
            Genera("}");
        }
        // ListaSimbolos -> Simbolo ListaSimbolos?
        private void ListaSimbolos()
        {
            Simbolo();
            if(getClasificacion()== Tipos.ST|| getClasificacion()== Tipos.SNT|| getClasificacion()== Tipos.PIzquierdo|| getClasificacion()==Tipos.CIzquierdo) 
            {
                ListaSimbolos();
            }
        }
        // Simbolo -> ST | SNT 
        private void Simbolo()
        {
            if(getClasificacion() == Tipos.ST)
            {
                if(EsClassificacion(getContenido()))
                {
                    Genera("Match(Tipos."+ "\""+ getContenido() +"\");");
                }
                else
                {
                    Genera("Match("+ "\""+ getContenido() +"\");");
                }
                Match(Tipos.ST);
            }
            else if(getClasificacion()==Tipos.SNT)
            {
                Genera(getContenido()+"();");
                Match(Tipos.SNT);
            }
            else if(getClasificacion()==Tipos.PIzquierdo)
            {
                CerraduraEpsilon();
            }
            else if(getClasificacion()==Tipos.CIzquierdo)
            {
                Match(Tipos.CIzquierdo);
                Console.WriteLine("Lista de ORs");
                ListaORs();
                Match(Tipos.CDerecho);
            }
        }
        private void CerraduraEpsilon()
        {
            Match(Tipos.PIzquierdo);
            string Simbolo=getContenido();
            Match(Tipos.ST);
            if(EsClassificacion(Simbolo))
            {
                Genera("if(getClasificacion()==Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
                Genera("if(getContenido()=="+ "\""+ Simbolo +"\");");
            }
            Genera("{");
            if(EsClassificacion(Simbolo))
            {
                Genera("Match(Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
                Genera("Match("+ "\""+ Simbolo  +"\");");
            }
            ListaSimbolos();
            Match(Tipos.PDerecho);
            Match(Tipos.Epsilon);
            Genera("}");
        }
        //ListaORs -> ST (|ListaORs)?
        private void ListaORs()
        {
            string Simbolo=getContenido();
            Match(Tipos.ST);
            if(EsClassificacion(Simbolo))
            {
                Genera("if(getClasificacion()==Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
                Genera("if(getContenido()=="+ "\""+ Simbolo +"\");");
            }
            Genera("{"); 
            if(EsClassificacion(Simbolo))
            {
                Genera("Match(Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
                Genera("Match("+ "\""+ Simbolo  +"\");");
            }
            Genera("}");
            if(getClasificacion()==Tipos.Or)
            {
                Match(Tipos.Or);
                ListaORs();
            }
        }
        private bool EsClassificacion(string ST)
        {
            switch(ST)
            {
                case "identificador": 
                case "numero": 
                case "caracter": 
                case "asignacion": 
                case "finSentencia": 
                case "opLogico": 
                case "opRelacional": 
                case "opTermino": 
                case "opFactor":
                case "incTermino": 
                case "incFactor": 
                case "Cadena": 
                case "inicializacion": 
                case "tipoDato": 
                case "zona":
                case "condicion":
                case "ciclo":  
                case "ternario": 
                case "opFlujoEntrada":
                case "opFlujoSalida": return true;
            }
            return false;
        }
    }
}