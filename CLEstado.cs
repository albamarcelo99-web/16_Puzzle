using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _16_Puzzle
{
    public class CLEstado
    {
        #region Campos
        private int[,] _tablero;
        private int _nivel;
        private CLEstado _padre;
        private int _h3;
        #endregion

        #region Propiedades
        public int[,] tablero
        {
            get => _tablero;
            set => _tablero = value;
        }

        public int nivel
        {
            get => _nivel;
            set => _nivel = value;
        }

        public int h3
        {
            get => _h3;
            set => _h3 = value;
        }

        public CLEstado padre
        {
            get => _padre;
            set => _padre = value;
        }
        #endregion

        #region Constructor
        public CLEstado()
        {
            this._tablero = new int[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    this._tablero[i, j] = 0;

            this._nivel = 0;
            this._padre = null;
            this._h3 = H3();
        }

        public CLEstado(
            int p00, int p01, int p02, int p03,
            int p10, int p11, int p12, int p13,
            int p20, int p21, int p22, int p23,
            int p30, int p31, int p32, int p33)
        {
            this._tablero = new int[4, 4];

            this._tablero[0, 0] = p00;
            this._tablero[1, 0] = p10;
            this._tablero[2, 0] = p20;
            this._tablero[3, 0] = p30;

            this._tablero[0, 1] = p01;
            this._tablero[1, 1] = p11;
            this._tablero[2, 1] = p21;
            this._tablero[3, 1] = p31;

            this._tablero[0, 2] = p02;
            this._tablero[1, 2] = p12;
            this._tablero[2, 2] = p22;
            this._tablero[3, 2] = p32;

            this._tablero[0, 3] = p03;
            this._tablero[1, 3] = p13;
            this._tablero[2, 3] = p23;
            this._tablero[3, 3] = p33;

            this._nivel = 0;
            this._padre = null;
            this._h3 = H3();
        }
        #endregion

        #region Métodos

        public List<CLEstado> GenerarHijos()
        {
            List<CLEstado> Respuesta = new List<CLEstado>();

            int fila0 = -1;
            int col0 = -1;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (this._tablero[i, j] == 0)
                    {
                        fila0 = i;
                        col0 = j;
                    }
                }
            }

            int[,] movimientos =
            {
                { -1,  0 }, // arriba
                {  1,  0 }, // abajo
                {  0, -1 }, // izquierda
                {  0,  1 }  // derecha
            };

            for (int k = 0; k < 4; k++)
            {
                int nuevaFila = fila0 + movimientos[k, 0];
                int nuevaCol = col0 + movimientos[k, 1];

                if (nuevaFila >= 0 && nuevaFila < 4 &&
                    nuevaCol >= 0 && nuevaCol < 4)
                {
                    int[,] aux = new int[4, 4];

                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < 4; j++)
                            aux[i, j] = this._tablero[i, j];

                    aux[fila0, col0] = aux[nuevaFila, nuevaCol];
                    aux[nuevaFila, nuevaCol] = 0;

                    CLEstado A = new CLEstado(
                        aux[0, 0], aux[0, 1], aux[0, 2], aux[0, 3],
                        aux[1, 0], aux[1, 1], aux[1, 2], aux[1, 3],
                        aux[2, 0], aux[2, 1], aux[2, 2], aux[2, 3],
                        aux[3, 0], aux[3, 1], aux[3, 2], aux[3, 3]
                    );

                    A.nivel = this.nivel + 1;
                    A.padre = this;
                    Respuesta.Add(A);
                }
            }

            return Respuesta;
        }

        public bool EsFinal()
        {
            bool res = false;

            if (_tablero[0, 0] == 1 &&
                _tablero[0, 1] == 2 &&
                _tablero[0, 2] == 3 &&
                _tablero[0, 3] == 4 &&
                _tablero[1, 0] == 5 &&
                _tablero[1, 1] == 6 &&
                _tablero[1, 2] == 7 &&
                _tablero[1, 3] == 8 &&
                _tablero[2, 0] == 9 &&
                _tablero[2, 1] == 10 &&
                _tablero[2, 2] == 11 &&
                _tablero[2, 3] == 12 &&
                _tablero[3, 0] == 13 &&
                _tablero[3, 1] == 14 &&
                _tablero[3, 2] == 15 &&
                _tablero[3, 3] == 0)
            {
                res = true;
            }

            return res;
        }

        public bool EsIgual(CLEstado a)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (a.tablero[i, j] != this.tablero[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int H1()
        {
            int piezasFueraDeLugar = 0;

            int[,] estadoMeta =
            {
                {  1,  2,  3,  4 },
                { 12, 13, 14,  5 },
                { 11,  0, 15,  6 },
                { 10,  9,  8,  7 }
            };

            for (int fila = 0; fila < 4; fila++)
            {
                for (int columna = 0; columna < 4; columna++)
                {
                    if (_tablero[fila, columna] != 0 &&
                        _tablero[fila, columna] != estadoMeta[fila, columna])
                    {
                        piezasFueraDeLugar++;
                    }
                }
            }

            return piezasFueraDeLugar;
        }

        public int H2()
        {
            int distanciaTotal = 0;

            int[,] estadoMeta =
            {
                {  1,  2,  3,  4 },
                { 12, 13, 14,  5 },
                { 11,  0, 15,  6 },
                { 10,  9,  8,  7 }
            };

            for (int fila = 0; fila < 4; fila++)
            {
                for (int columna = 0; columna < 4; columna++)
                {
                    int valor = _tablero[fila, columna];

                    if (valor == 0)
                        continue;

                    int filaMeta = 0;
                    int columnaMeta = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (estadoMeta[i, j] == valor)
                            {
                                filaMeta = i;
                                columnaMeta = j;
                            }
                        }
                    }

                    distanciaTotal +=
                        Math.Abs(fila - filaMeta) +
                        Math.Abs(columna - columnaMeta);
                }
            }

            return distanciaTotal;
        }

        private int H3()
        {
            int sumaS = 0;

            int[] borde =
            {
                _tablero[0,0],
                _tablero[0,1],
                _tablero[0,2],
                _tablero[0,3],
                _tablero[1,3],
                _tablero[2,3],
                _tablero[3,3],
                _tablero[3,2],
                _tablero[3,1],
                _tablero[3,0],
                _tablero[2,0],
                _tablero[1,0]
            };

            for (int i = 0; i < 12; i++)
            {
                int actual = borde[i];

                if (actual == 0)
                    continue;

                int siguiente = borde[(i + 1) % 12];
                int sucesorCorrecto = (actual == 12) ? 1 : actual + 1;

                if (siguiente == sucesorCorrecto)
                    sumaS += 0;
                else
                    sumaS += 2;
            }

            // Penalización de las posiciones centrales.
            if (_tablero[1, 1] != 13)
                sumaS += 1;
            if (_tablero[1, 2] != 14)
                sumaS += 1;
            if (_tablero[2, 1] != 0)
                sumaS += 1;
            if (_tablero[2, 2] != 15)
                sumaS += 1;

            return H2() + sumaS;
        }

        #endregion
    }
}
