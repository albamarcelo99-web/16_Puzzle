using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _16_Puzzle
{
    public static class CLAlgoritmosDeBusqueda
    {
        private const int detectado = -1;
        private const int tamaño = 4;

  
        // Implementación del algoritmo IDA*.
        // Recibe un estado inicial y devuelve la lista de estados que forman
        // la solución (si se encuentra), ordenada desde el estado inicial
        // hasta el estado final. Devuelve lista vacía si no hay solución.
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

        // Búsqueda recursiva usada por IDA*.
        // Camino: pila de estados desde la raíz al estado actual.
        // EstadosDelCamino: conjunto de claves para detección rápida de ciclos.
        // costoActual: g(n) (costo desde la raíz hasta el nodo actual).
        // limite: límite f = g + h para la iteración actual.
        // heuristicaActual: h(n) del nodo actual.
        // Devuelve detectado (-1) si encontró la solución, int.MaxValue si no hay camino
        // dentro de los límites, o el siguiente límite mínimo a considerar.
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

        #region MetodoHeurístico
        // Métodos para calcular heurísticas utilizadas por los algoritmos

   
        // Heurística combinada usada en IDA*: distancia Manhattan + conflictos lineales
        public static int CalcularHeuristica(
            CLEstado Estado)
        {
            return CalcularDistanciaManhattan(Estado) +
                   CalcularConflictoLineal(Estado);
        }

        // Calcula la suma de las distancias Manhattan de cada ficha respecto
        // a su posición objetivo. Ignora la ficha 0 (espacio vacío).
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

        // Calcula la penalización por conflictos lineales (linear conflict).
        // Para cada fila y columna, identifica las fichas que pertenecen a
        // esa misma fila/columna objetivo y suma una penalización proporcional
        // a las piezas que están fuera de orden relativo.
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

        // Calcula la longitud de la subsecuencia creciente más larga en la lista
        // (utilizado para medir cuántas piezas están en orden relativo).
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

        // Métodos para determinar si un tablero es solucionable.

        // Comprueba la solvencia del puzzle calculando las inversiones y
        // la fila del cero desde abajo. Retorna true si la configuración
        // es solucionable según la paridad requerida para el 4x4.
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


      
        // Convierte el tablero en una clave de tipo ulong compactando cada
        // ficha en 4 bits. Esto permite comparaciones rápidas y almacenamiento
        // en conjuntos (HashSet) para detección de ciclos.

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

      

        // Clase interna utilizada para mantener información sobre hijos
        // generados durante la expansión: el estado, su clave y su heurística.

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
    }
}