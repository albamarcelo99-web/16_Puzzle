using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _16_Puzzle
{
    public static class CLAlgoritmosDeBusqueda
    {
        private const int TAMANO = 4;
        private const int ENCONTRADO = -1;

        /// <summary>
        /// Resuelve el puzzle 4x4 mediante IDA*.
        ///
        /// La solución se devuelve en el mismo orden que tu algoritmo anterior:
        /// estado final -> ... -> estado inicial.
        /// </summary>
        public static List<CLEstado> AlgoritmoIDAEstrella(
            CLEstado Inicial)
        {
            List<CLEstado> SinSolucion =
                new List<CLEstado>();

            if (Inicial == null)
                return SinSolucion;

            /*
             * Evita ejecutar el algoritmo cuando el puzzle
             * matemáticamente no tiene solución.
             */
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

                if (resultado == ENCONTRADO)
                {
                    /*
                     * Camino está en orden:
                     * inicial -> ... -> final.
                     *
                     * Se invierte para conservar el comportamiento
                     * de tu algoritmo anterior:
                     * final -> ... -> inicial.
                     */
                    List<CLEstado> Solucion =
                        new List<CLEstado>(Camino);

                    Solucion.Reverse();

                    return Solucion;
                }

                /*
                 * No existen más estados que explorar.
                 */
                if (resultado == int.MaxValue)
                    return SinSolucion;

                /*
                 * El próximo límite es el menor valor f(n)
                 * que excedió el límite anterior.
                 */
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

            /*
             * Si g(n) + h(n) supera el límite actual,
             * se detiene esta rama.
             */
            if (funcionEvaluacion > limite)
                return funcionEvaluacion;

            if (Actual.EsFinal())
                return ENCONTRADO;

            int siguienteLimite = int.MaxValue;

            List<CLEstado> Hijos =
                Actual.GenerarHijos();

            /*
             * Se calcula la heurística una sola vez por hijo.
             */
            List<HijoEvaluado> HijosEvaluados =
                new List<HijoEvaluado>();

            foreach (CLEstado Hijo in Hijos)
            {
                ulong claveHijo = CrearClave(Hijo);

                /*
                 * Evitar ciclos.
                 *
                 * Por ejemplo:
                 * A -> B -> A
                 */
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

            /*
             * Primero se exploran los hijos con menor heurística.
             * Esto no cambia IDA*, pero generalmente acelera
             * bastante la búsqueda.
             */
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

                /*
                 * Cuando se encuentra la solución no se elimina
                 * el hijo del camino, porque ese camino completo
                 * será devuelto.
                 */
                if (resultado == ENCONTRADO)
                    return ENCONTRADO;

                /*
                 * Retroceso de la búsqueda en profundidad.
                 */
                Camino.RemoveAt(Camino.Count - 1);
                EstadosDelCamino.Remove(Evaluado.Clave);

                if (resultado < siguienteLimite)
                    siguienteLimite = resultado;
            }

            return siguienteLimite;
        }

        #region Heurística

        /// <summary>
        /// H = distancia Manhattan + conflicto lineal.
        /// </summary>
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

            for (int fila = 0; fila < TAMANO; fila++)
            {
                for (int columna = 0;
                     columna < TAMANO;
                     columna++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    /*
                     * El espacio vacío no se toma en cuenta.
                     */
                    if (valor == 0)
                        continue;

                    /*
                     * Estado meta utilizado:
                     *
                     *  1  2  3  4
                     *  5  6  7  8
                     *  9 10 11 12
                     * 13 14 15  0
                     */
                    int filaMeta =
                        (valor - 1) / TAMANO;

                    int columnaMeta =
                        (valor - 1) % TAMANO;

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

            /*
             * Conflictos en las filas.
             */
            for (int fila = 0; fila < TAMANO; fila++)
            {
                List<int> columnasMeta =
                    new List<int>();

                for (int columna = 0;
                     columna < TAMANO;
                     columna++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    if (valor == 0)
                        continue;

                    int filaMeta =
                        (valor - 1) / TAMANO;

                    int columnaMeta =
                        (valor - 1) % TAMANO;

                    /*
                     * Solo interesan fichas que ya están
                     * en su fila final.
                     */
                    if (filaMeta == fila)
                    {
                        columnasMeta.Add(columnaMeta);
                    }
                }

                /*
                 * Las fichas fuera de la subsecuencia creciente
                 * necesitan abandonar temporalmente la fila.
                 *
                 * Cada una requiere como mínimo dos movimientos
                 * adicionales: salir y volver a entrar.
                 */
                penalizacion += 2 *
                    (columnasMeta.Count -
                     LongitudSubsecuenciaCreciente(
                         columnasMeta
                     ));
            }

            /*
             * Conflictos en las columnas.
             */
            for (int columna = 0;
                 columna < TAMANO;
                 columna++)
            {
                List<int> filasMeta =
                    new List<int>();

                for (int fila = 0;
                     fila < TAMANO;
                     fila++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    if (valor == 0)
                        continue;

                    int filaMeta =
                        (valor - 1) / TAMANO;

                    int columnaMeta =
                        (valor - 1) % TAMANO;

                    /*
                     * Solo interesan fichas que ya están
                     * en su columna final.
                     */
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

            for (int fila = 0; fila < TAMANO; fila++)
            {
                for (int columna = 0;
                     columna < TAMANO;
                     columna++)
                {
                    int valor =
                        Estado.tablero[fila, columna];

                    if (valor == 0)
                    {
                        /*
                         * Fila 3 -> 1 desde abajo.
                         * Fila 2 -> 2 desde abajo.
                         * Fila 1 -> 3 desde abajo.
                         * Fila 0 -> 4 desde abajo.
                         */
                        filaCeroDesdeAbajo =
                            TAMANO - fila;
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

            /*
             * Para una matriz 4x4 con meta:
             *
             *  1  2  3  4
             *  5  6  7  8
             *  9 10 11 12
             * 13 14 15  0
             *
             * El puzzle es solucionable cuando la suma
             * de inversiones y fila del cero desde abajo
             * es impar.
             */
            return (inversiones + filaCeroDesdeAbajo) % 2 == 1;
        }

        #endregion

        #region Clave del tablero

        private static ulong CrearClave(
            CLEstado Estado)
        {
            ulong clave = 0;
            int desplazamiento = 0;

            /*
             * Cada ficha utiliza 4 bits porque sus valores
             * se encuentran entre 0 y 15.
             *
             * 16 fichas x 4 bits = 64 bits.
             */
            for (int fila = 0; fila < TAMANO; fila++)
            {
                for (int columna = 0;
                     columna < TAMANO;
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