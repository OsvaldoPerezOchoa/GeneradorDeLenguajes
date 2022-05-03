using System.IO;
using System;
namespace Generador
{
    public class Lexico:Toke
    {
        protected int linea;
        const int  E=-2;//negativo por si crece el automata 
        const int  F=-1;
        int[,] TranD =
        {
        //   WS  L  -  >  ?	 ;  |  (  )	 \ La  /  *  EL
            { 0, 1, 2,11, 4, 5, 6, 7, 8, 9,11,12,14, 0,}, //Estado 0
            { F, 1, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 1
            { F, F, F, 3, F, F, F, F, F, F, F, F, F, F,}, //Estado 2
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 3
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 4
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 5
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 6
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 7
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 8 
            { F, F, F, F,10,10,10,10,10, F, F, F, F, F,}, //Estado 9
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 10
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F,}, //Estado 11
            { F, F, F, F, F, F, F, F, F, F, F,13,14, F,}, //Estado 12
            {13,13,13,13,13,13,13,13,13,13,13,13,13, 0,}, //Estado 13
            {14,14,14,14,14,14,14,14,14,14,14,14,15, E,}, //Estado 14
            {14,14,14,14,14,14,14,14,14,14,14, 0,15, E,}, //Estado 15
        };     
        private StreamReader archivo;
        protected StreamWriter log;
        protected StreamWriter gen;
        public Lexico()
        {
            linea=1;
            archivo = new StreamReader("C:\\Archivos\\Gramatica2.txt");
            log = new StreamWriter("C:\\Archivos\\Lenguaje.Log");
            gen = new StreamWriter("C:\\Archivos\\Lenguaje.cs");
            log.AutoFlush=true;
            gen.AutoFlush=true;
            log.WriteLine("Instituo Tecnologico de Queretaro");
            log.WriteLine("Osvaldo Pérez Ochoa");
            log.WriteLine("-----------------------------------");
            log.WriteLine("Contenido de Prueba.cs: ");
            string time = DateTime.Now.ToString("hh:mm:ss tt"); // hora
            string Date = DateTime.Now.ToString("dd-MM-yyyy"); //fecha 
            log.WriteLine(time);
            log.WriteLine(Date);
            log.WriteLine("-----------------------------------");
            
            gen.WriteLine("// Instituo Tecnologico de Queretaro");
            gen.WriteLine("// Osvaldo Pérez Ochoa");
            gen.WriteLine("// -----------------------------------");
            gen.WriteLine("// Contenido de la clase Lenguaje.cs: ");
            gen.WriteLine("// " + time);
            gen.WriteLine("// " + Date);
            gen.WriteLine("// -----------------------------------");
            gen.WriteLine("using System;");
            gen.Write("namespace ");

        }
        public void CerrarArchivos()
        { 
            archivo.Close();
            log.Close();
            gen.Close();
        }
        private int Columna(char Trancision)
        {
            if(char.IsWhiteSpace(Trancision))
            {
                return 0;
            }
            else if(char.IsLetter(Trancision))
            {
                return 1;
            }
            else if(Trancision=='-')
            {
                return 2;
            }
            else if(Trancision=='>')
            {
                return 3;
            }
            else if(Trancision=='?')
            {
                return 4;
            }
            else if(Trancision==';')
            {
                return 5;
            }
            else if(Trancision=='|')
            {
                return 6;
            }
            else if(Trancision=='(')
            {
                return 7;
            }
            else if(Trancision==')')
            {
                return 8;
            }
            else if(Trancision=='\\')
            {
                return 9;
            }
            return 10;

        }

        private void Clasifica(int Estado)
        {
            switch(Estado)
            {
               case 1:
               case 2: 
               case 9: 
               case 11:setClasificacion(Tipos.ST); break;
               case 3: setClasificacion(Tipos.Flechita); break;
               case 4: setClasificacion(Tipos.Epsilon); break;
               case 5: setClasificacion(Tipos.FinProduccion); break;
               case 6: setClasificacion(Tipos.Or); break;
               case 7: setClasificacion(Tipos.PIzquierdo); break;
               case 8: setClasificacion(Tipos.PDerecho); break;
            }
        }
        public void NextToken()
        {
            char c;
            string Buffer="";
            int Estado=0;
            while(Estado>=0)
            {
                c=(char)archivo.Peek();
                Estado=TranD[Estado,Columna(c)]; 
                if(Estado>=0)
                {
                    archivo.Read();
                    if(c=='\n')// para contar lineas 
                    { 
                        linea++;
                    }
                    if(Estado>0)
                    { 
                        Clasifica(Estado);
                        Buffer+=c;
                    }
                    else
                    { 
                        Buffer=""; //cadena vacia
                    }
                }
            }
            if(Estado == E)
            {
                   
            }
            if(getClasificacion()==Tipos.ST)
            {
                if(char.IsUpper(Buffer[0]))
                {
                    setClasificacion(Tipos.SNT);
                }
            }
            setContenido(Buffer);
            if(getContenido()=="[")
            {
                setClasificacion(Tipos.CIzquierdo);
            }
            else if(getContenido()=="]")
            {
                setClasificacion(Tipos.CDerecho);
            }
            if(!FinAchivo())
            {
                log.WriteLine(getContenido()+" = "+getClasificacion());
            }
        }
        public bool FinAchivo()
        {
            return archivo.EndOfStream;
        }
    }
}