using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _16_Puzzle
{
    public static class CLAlgoritmosDeBusqueda
    {
        private const int tamaño = 4;
        private const int detectado = -1;

        
        public static List<CLEstado> AlgoritmoIDAEstrella(CLEstado Inicial)
        {
            List<CLEstado> SinSolucion =
                new List<CLEstado>();

            if (Inicial == null)
                return SinSolucion;

            
            if (!EsSolucionable(Inicial))
                return SinSolucion;

            Inicial.padre = null;
            Inicial.nivel = 0;

            int heuristicaInicial =
                CalcularHeuristica(Inicial);

            int limite = heuristicaInicial;

            List<CLEstado> Camino =
                new List<CLEstado>();

            HashSet<ulong> EstadosDelCamino =
                new HashSet<ulong>();

            Camino.Add(Inicial);
            EstadosDelCamino.Add(CrearClave(Inicial));

            while (true)
            {
                int resultado = Buscar(
                    Camino,
                    EstadosDelCamino,
                    0,
                    limite,
                    heuristicaInicial
                );

                if (resultado == detectado)
                {
                    
                    List<CLEstado> Solucion =
                        new List<CLEstado>(Camino);

                    Solucion.Reverse();

                    return Solucion;
                }

                
                if (resultado == int.MaxValue)
                    return SinSolucion;

                
                limite = resultado;
            }
        }

        private static int Buscar(
            List<CLEstado> Camino,
            HashSet<ulong> EstadosDelCamino,
            int costoActual,
            int limite,
            int heuristicaActual)
        {
            CLEstado Actual =
                Camino[Camino.Count - 1];

            int funcionEvaluacion =
                costoActual + heuristicaActual;

           
            if (funcionEvaluacion > limite)
                return funcionEvaluacion;

            if (Actual.EsFinal())
                return detectado;

            int siguienteLimite = int.MaxValue;

            List<CLEstado> Hijos =
                Actual.GenerarHijos();

            
            List<HijoEvaluado> HijosEvaluados =
                new List<HijoEvaluado>();

            foreach (CLEstado Hijo in Hijos)
            {
                ulong claveHijo = CrearClave(Hijo);

               
                if (EstadosDelCamino.Contains(claveHijo))
                    continue;

                int heuristicaHijo =
                    CalcularHeuristica(Hijo);

                HijosEvaluados.Add(
                    new HijoEvaluado(
                        Hijo,
                        claveHijo,
                        heuristicaHijo
                    )
                );
            }

            
            HijosEvaluados.Sort(
                delegate (
                    HijoEvaluado Primero,
                    HijoEvaluado Segundo)
                {
                    return Primero.Heuristica.CompareTo(
                        Segundo.Heuristica
                    );
                }
            );

            foreach (HijoEvaluado Evaluado in HijosEvaluados)
            {
                CLEstado Hijo = Evaluado.Estado;

                Hijo.padre = Actual;
                Hijo.nivel = costoActual + 1;

                Camino.Add(Hijo);
                EstadosDelCamino.Add(Evaluado.Clave);

                int resultado = Buscar(
                    Camino,
                    EstadosDelCamino,
                    costoActual + 1,
                    limite,
                    Evaluado.Heuristica
                );

               
                if (resultado == detectado)
                    return detectado;

                
                Camino.RemoveAt(Camino.Count - 1);
                EstadosDelCamino.Remove(Evaluado.Clave);

                if (resultado < siguienteLimite)
                    siguienteLimite = resultado;
            }

            return siguienteLimite;
        }

        #region Heurística

   
        public static int CalcularHeuristica(
            CLEstado Estado)
        {
            return CalcularDistanciaManhattan(Estado) +
                   CalcularConflictoLineal(Estado);
        }

        private static int CalcularDistanciaManhattan(
            CLEstado Estado)
        {
            int distanciaTotal = 0;

            for (int fila = 0; fila < tamaño; fila++)
            {
                for (int columna = 0;
                     columna < tamaño;
                     columna++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    
                    if (valor == 0)
                        continue;

                    
                    int filaMeta =
                        (valor - 1) / tamaño;

                    int columnaMeta =
                        (valor - 1) % tamaño;

                    distanciaTotal +=
                        Math.Abs(fila - filaMeta) +
                        Math.Abs(columna - columnaMeta);
                }
            }

            return distanciaTotal;
        }

        private static int CalcularConflictoLineal(
            CLEstado Estado)
        {
            int penalizacion = 0;

            
            for (int fila = 0; fila < tamaño; fila++)
            {
                List<int> columnasMeta =
                    new List<int>();

                for (int columna = 0;
                     columna < tamaño;
                     columna++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    if (valor == 0)
                        continue;

                    int filaMeta =
                        (valor - 1) / tamaño;

                    int columnaMeta =
                        (valor - 1) % tamaño;

                    
                    if (filaMeta == fila)
                    {
                        columnasMeta.Add(columnaMeta);
                    }
                }

              
                penalizacion += 2 *
                    (columnasMeta.Count -
                     LongitudSubsecuenciaCreciente(
                         columnasMeta
                     ));
            }

            for (int columna = 0;
                 columna < tamaño;
                 columna++)
            {
                List<int> filasMeta =
                    new List<int>();

                for (int fila = 0;
                     fila < tamaño;
                     fila++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    if (valor == 0)
                        continue;

                    int filaMeta =
                        (valor - 1) / tamaño;

                    int columnaMeta =
                        (valor - 1) % tamaño;

                    
                    if (columnaMeta == columna)
                    {
                        filasMeta.Add(filaMeta);
                    }
                }

                penalizacion += 2 *
                    (filasMeta.Count -
                     LongitudSubsecuenciaCreciente(
                         filasMeta
                     ));
            }

            return penalizacion;
        }

        private static int LongitudSubsecuenciaCreciente(
            List<int> Valores)
        {
            if (Valores.Count == 0)
                return 0;

            int[] longitud =
                new int[Valores.Count];

            int mejorLongitud = 1;

            for (int i = 0; i < Valores.Count; i++)
            {
                longitud[i] = 1;

                for (int j = 0; j < i; j++)
                {
                    if (Valores[j] < Valores[i] &&
                        longitud[j] + 1 > longitud[i])
                    {
                        longitud[i] =
                            longitud[j] + 1;
                    }
                }

                if (longitud[i] > mejorLongitud)
                {
                    mejorLongitud = longitud[i];
                }
            }

            return mejorLongitud;
        }

        #endregion

        #region Solubilidad

        private static bool EsSolucionable(
            CLEstado Estado)
        {
            List<int> valores =
                new List<int>();

            int filaCeroDesdeAbajo = 0;

            for (int fila = 0; fila < tamaño; fila++)
            {
                for (int columna = 0;
                     columna < tamaño;
                     columna++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    if (valor == 0)
                    {
                        
                        filaCeroDesdeAbajo =
                            tamaño - fila;
                    }
                    else
                    {
                        valores.Add(valor);
                    }
                }
            }

            int inversiones = 0;

            for (int i = 0;
                 i < valores.Count - 1;
                 i++)
            {
                for (int j = i + 1;
                     j < valores.Count;
                     j++)
                {
                    if (valores[i] > valores[j])
                        inversiones++;
                }
            }

            
            return (inversiones + filaCeroDesdeAbajo) % 2 == 1;
        }

        #endregion

        #region Clave del tablero

        private static ulong CrearClave(
            CLEstado Estado)
        {
            ulong clave = 0;
            int desplazamiento = 0;

            
            for (int fila = 0; fila < tamaño; fila++)
            {
                for (int columna = 0;
                     columna < tamaño;
                     columna++)
                {
                    ulong valor =
                        (ulong)Estado.tablero[fila, columna];

                    clave |= valor << desplazamiento;
                    desplazamiento += 4;
                }
            }

            return clave;
        }

        #endregion

        #region Clase auxiliar

        private sealed class HijoEvaluado
        {
            public CLEstado Estado { get; private set; }

            public ulong Clave { get; private set; }

            public int Heuristica { get; private set; }

            public HijoEvaluado(
                CLEstado estado,
                ulong clave,
                int heuristica)
            {
                Estado = estado;
                Clave = clave;
                Heuristica = heuristica;
            }
        }

        #endregion
    }
}