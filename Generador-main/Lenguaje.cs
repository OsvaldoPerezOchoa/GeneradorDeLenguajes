using System.IO;
using System;
namespace Generador
{

    //Requerimiento 1: Modificar la matriz TranD para poder comentar codigo de linea y multilinea
    //Requerimiento 2: La primera prodeccion debe ser publica
    //Requerimiento 3: Para los simbolos ST quitarles la diagonal
    //Requerimiento 4: Cuando es clasificacion invocar el metodo match sobrecargado sin comillas
    //Requerimiento 5: Agregar el CIzquierdo y CDerecho en la matriz TranD
    //Requerimiento 6: Programar el OR en producciones gramaticales
    //Requerimiento 7: Generar el program.cs invocando la primera produccion

    public class Lenguaje:Sintaxis
    {
        int tab;
        public Lenguaje()
        {
            tab = 0;
        }
        private void Genera(string codigo) 
        {
            if(codigo == "}")
            {
                tab--;
            }
            for(int i = 0; i < tab; i++)
            {
                gen.Write("\t");
            }
            gen.WriteLine(codigo);
            if(codigo == "{")
            {
                tab++;
            }
        }
        //Gramatica produce -> Cabecera Producciones
        public void Gramatica()
        {
            Cabecera();
            Producciones();
        }
        //Cabecera -> Lenguaje: SNT
        private void Cabecera()
        {
            Match("Lenguaje");
            Match(":");
            gen.Write(getContenido());
            Match(Tipos.SNT);
            Genera(getContenido());
            Match(Tipos.FinProduccion);
        }
        //producciones -> {ListaProducciones}
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
        //ListaProducciones -> Produccion; ListaProducciones?
        private void ListaProducciones()
        {
            Produccion();
            if(getContenido() != "}")
            {
                ListaProducciones();
            }
        }
        //Produccion -> STN Flechita
        private void Produccion()
        {
            Genera("private void " + getContenido()+ ("()"));
            Genera("{");
            Match(Tipos.SNT);
            Match(Tipos.Flechita);
            ListaSimbolo();
            Match(Tipos.FinProduccion);
            Genera("}");
        }
        // ListaSimbolo -> Simbolo ListaSimbolo?
        private void ListaSimbolo()
        {
            Simbolo();
            if(getClasificacion() == Tipos.ST || getClasificacion() == Tipos.SNT || getClasificacion() == Tipos.PIzquierdo || getClasificacion()== Tipos.CIzquierdo)
            {
                ListaSimbolo();
            }
        }
        // ListaSimbolo -> ST | SNT
        private void Simbolo()
        {
            if(getClasificacion() == Tipos.ST)
            {
                if(EsClasificacion(getContenido()))
                {
                    Genera("Match(Tipos."+ "\""+ getContenido() +"\");");
                }
                else
                {
                    Genera("Match("+ "\""+ getContenido() +"\");");
                }   
                Match(Tipos.ST);
            }
            else if(getClasificacion() == Tipos.SNT)
            {
                Genera(getContenido() + "();");                
                Match(Tipos.SNT);
            }
            else if(getClasificacion() == Tipos.PIzquierdo)
            {
                CerraduraEpsilon();
            }
            else if(getClasificacion() ==Tipos.CIzquierdo)
            {
                Match(Tipos.CIzquierdo);
                ListaOrs();
                Match(Tipos.CDerecho);
            }
        }
        private void CerraduraEpsilon()
        {
            Match(Tipos.PIzquierdo);
            string Simbolo = getContenido();
            Match(Tipos.ST);
            if(EsClasificacion(Simbolo))
            {
                Genera("if(getClasificacion() == Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
                Genera("if(getContenido =="+ "\""+ Simbolo +"\");");
            }   
            Genera("{");
            if(EsClasificacion(Simbolo))
            {
                Genera("Match(Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
               Genera("Match("+ "\""+ Simbolo +"\");");
            } 
            ListaSimbolo();
            Match(Tipos.PDerecho);
            Match(Tipos.Epsilon);
            Genera("}");  
        }
        //LiistaOrs -> ST (|ListaOrs)?
        private void ListaOrs()
        {
            string Simbolo = getContenido();
            Match(Tipos.ST);
            if(EsClasificacion(Simbolo))
            {
                Genera("if(getClasificacion() == Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
                Genera("if(getContenido =="+ "\""+ Simbolo +"\");");
            }
            Genera("{");
            if(EsClasificacion(Simbolo))
            {
                Genera("Match(Tipos."+ "\""+ Simbolo +"\");");
            }
            else
            {
               Genera("Match("+ "\""+ Simbolo +"\");");
            } 
            Genera("}"); 
            if(getClasificacion() == Tipos.Or)
            {
                Match(Tipos.Or);
                ListaOrs();
            }
        }
        private bool EsClasificacion(string ST)
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
                case "opFlujoSalida":
                return true;
            }
            return false;
        } 
    }
}