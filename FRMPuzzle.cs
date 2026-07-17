using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _16_Puzzle
{
    public partial class FRMPuzzle : Form
    {
        private int contador = 0;
        private String pos0;
        private String[,] posiciones;
        private int contadorAnchura = 0;
        private List<CLEstado> Resultado = new List<CLEstado>();
        bool bandera = false;
        Random rn = new Random();
        public FRMPuzzle()
        {
            InitializeComponent();
        }

        private void TMRReloj_Tick(object sender, EventArgs e)
        {
            posiciones = new string[4, 4];

            posiciones[0, 0] = LBL00.Text;
            posiciones[0, 1] = LBL01.Text;
            posiciones[0, 2] = LBL02.Text;
            posiciones[0, 3] = LBL03.Text;

            posiciones[1, 0] = LBL10.Text;
            posiciones[1, 1] = LBL11.Text;
            posiciones[1, 2] = LBL12.Text;
            posiciones[1, 3] = LBL13.Text;

            posiciones[2, 0] = LBL20.Text;
            posiciones[2, 1] = LBL21.Text;
            posiciones[2, 2] = LBL22.Text;
            posiciones[2, 3] = LBL23.Text;

            posiciones[3, 0] = LBL30.Text;
            posiciones[3, 1] = LBL31.Text;
            posiciones[3, 2] = LBL32.Text;
            posiciones[3, 3] = LBL33.Text;

            Label[,] labels =
            {
        { LBL00, LBL01, LBL02, LBL03 },
        { LBL10, LBL11, LBL12, LBL13 },
        { LBL20, LBL21, LBL22, LBL23 },
        { LBL30, LBL31, LBL32, LBL33 }
    };

            if (contador < 3)
            {
                contador++;
                LBLContador.Text = contador.ToString();

                int fila0 = -1;
                int columna0 = -1;

                // Buscar la posición del espacio vacío.
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (posiciones[i, j] == "0")
                        {
                            fila0 = i;
                            columna0 = j;
                            pos0 = i.ToString() + j.ToString();
                            break;
                        }
                    }

                    if (fila0 != -1)
                        break;
                }

                // Posibles movimientos:
                // arriba, abajo, izquierda y derecha.
                int[,] movimientos =
                {
            { -1,  0 },
            {  1,  0 },
            {  0, -1 },
            {  0,  1 }
        };

                List<int[]> movimientosValidos = new List<int[]>();

                for (int i = 0; i < 4; i++)
                {
                    int nuevaFila = fila0 + movimientos[i, 0];
                    int nuevaColumna = columna0 + movimientos[i, 1];

                    if (nuevaFila >= 0 && nuevaFila < 4 &&
                        nuevaColumna >= 0 && nuevaColumna < 4)
                    {
                        movimientosValidos.Add(
                            new int[] { nuevaFila, nuevaColumna }
                        );
                    }
                }

                // Elegir uno de los movimientos válidos.
                int aleatorio = rn.Next(0, movimientosValidos.Count);

                int filaDestino = movimientosValidos[aleatorio][0];
                int columnaDestino = movimientosValidos[aleatorio][1];

                // Intercambiar el cero con la ficha vecina.
                labels[fila0, columna0].Text =
                    labels[filaDestino, columnaDestino].Text;

                labels[filaDestino, columnaDestino].Text = "0";
            }
            else
            {
                TMRReloj.Enabled = false;
                MessageBox.Show("Reloj apagado");

                LBLContador.Text = "";
                contador = 0;
            }
        }

        private void BTNDesordenar_Click(object sender, EventArgs e)
        {
            TMRReloj.Enabled = true;
        }

        private void BTNGenerarHijos_Click(object sender, EventArgs e)
        {
            CLEstado Inicial = new CLEstado(Convert.ToInt32(LBL00.Text),
                                            Convert.ToInt32(LBL01.Text),
                                            Convert.ToInt32(LBL02.Text),
                                            Convert.ToInt32(LBL03.Text),
                                            Convert.ToInt32(LBL10.Text),
                                            Convert.ToInt32(LBL11.Text),
                                            Convert.ToInt32(LBL12.Text),
                                            Convert.ToInt32(LBL13.Text),
                                            Convert.ToInt32(LBL20.Text),
                                            Convert.ToInt32(LBL21.Text),
                                            Convert.ToInt32(LBL22.Text),
                                            Convert.ToInt32(LBL23.Text),
                                            Convert.ToInt32(LBL30.Text),
                                            Convert.ToInt32(LBL31.Text),
                                            Convert.ToInt32(LBL32.Text),
                                            Convert.ToInt32(LBL33.Text)
                                            );
            List<CLEstado> Hijos = Inicial.GenerarHijos();
            FRMHijos A = new FRMHijos();
            A.Hijos = Hijos;
            A.ShowDialog();
        }

        private void BTNEsFinal_Click(object sender, EventArgs e)
        {
            CLEstado Inicial = new CLEstado(Convert.ToInt32(LBL00.Text),
                                            Convert.ToInt32(LBL01.Text),
                                            Convert.ToInt32(LBL02.Text),
                                            Convert.ToInt32(LBL03.Text),
                                            Convert.ToInt32(LBL10.Text),
                                            Convert.ToInt32(LBL11.Text),
                                            Convert.ToInt32(LBL12.Text),
                                            Convert.ToInt32(LBL13.Text),
                                            Convert.ToInt32(LBL20.Text),
                                            Convert.ToInt32(LBL21.Text),
                                            Convert.ToInt32(LBL22.Text),
                                            Convert.ToInt32(LBL23.Text),
                                            Convert.ToInt32(LBL30.Text),
                                            Convert.ToInt32(LBL31.Text),
                                            Convert.ToInt32(LBL32.Text),
                                            Convert.ToInt32(LBL33.Text)
                                            );
            if (Inicial.EsFinal())
            {
                MessageBox.Show("ES el estado FINAL");
            }
            else
            {
                MessageBox.Show("NO ES el estado FINAL");
            }
        }

        private void BTNEuristico_Click(object sender, EventArgs e)
        {
            CLEstado Inicial = new CLEstado(Convert.ToInt32(LBL00.Text),
                                            Convert.ToInt32(LBL01.Text),
                                            Convert.ToInt32(LBL02.Text),
                                            Convert.ToInt32(LBL03.Text),
                                            Convert.ToInt32(LBL10.Text),
                                            Convert.ToInt32(LBL11.Text),
                                            Convert.ToInt32(LBL12.Text),
                                            Convert.ToInt32(LBL13.Text),
                                            Convert.ToInt32(LBL20.Text),
                                            Convert.ToInt32(LBL21.Text),
                                            Convert.ToInt32(LBL22.Text),
                                            Convert.ToInt32(LBL23.Text),
                                            Convert.ToInt32(LBL30.Text),
                                            Convert.ToInt32(LBL31.Text),
                                            Convert.ToInt32(LBL32.Text),
                                            Convert.ToInt32(LBL33.Text)
                                            );
            Resultado =
    CLAlgoritmosDeBusqueda.AlgoritmoIDAEstrella(Inicial);

            if (Resultado.Count == 0)
            {
                MessageBox.Show(
                    "No se encontró solución o el tablero no es solucionable."
                );

                return;
            }

            MessageBox.Show(
                "Solución encontrada en " +
                (Resultado.Count - 1) +
                " movimientos."
            );
            TMRRelojAnchuraPrioritaria.Enabled = true;
        }

        private void EstadoATablero(CLEstado Estado)
        {
            LBL00.Text = Estado.tablero[0, 0].ToString();
            LBL01.Text = Estado.tablero[0, 1].ToString();
            LBL02.Text = Estado.tablero[0, 2].ToString();
            LBL03.Text = Estado.tablero[0, 3].ToString();
            LBL10.Text = Estado.tablero[1, 0].ToString();
            LBL11.Text = Estado.tablero[1, 1].ToString();
            LBL12.Text = Estado.tablero[1, 2].ToString();
            LBL13.Text = Estado.tablero[1, 3].ToString();
            LBL20.Text = Estado.tablero[2, 0].ToString();
            LBL21.Text = Estado.tablero[2, 1].ToString();
            LBL22.Text = Estado.tablero[2, 2].ToString();
            LBL23.Text = Estado.tablero[2, 3].ToString();
            LBL30.Text = Estado.tablero[3, 0].ToString();
            LBL31.Text = Estado.tablero[3, 1].ToString();
            LBL32.Text = Estado.tablero[3, 2].ToString();
            LBL33.Text = Estado.tablero[3, 3].ToString();
        }

        private void TMRRelojAnchuraPrioritaria_Tick(object sender, EventArgs e)
        {
            if ((contadorAnchura < Resultado.Count) && (!bandera))
            {
                EstadoATablero(Resultado[Resultado.Count - contadorAnchura - 1]);
                contadorAnchura++;
            }
            if ((contadorAnchura == Resultado.Count) && (!bandera))
            {
                bandera = true;
                contadorAnchura--;
                TMRRelojAnchuraPrioritaria.Enabled = false;
                if (MessageBox.Show("Listo") == DialogResult.OK)
                {
                    TMRRelojAnchuraPrioritaria.Enabled = true;
                }

            }
            if ((contadorAnchura >= 0) && (bandera))
            {
                EstadoATablero(Resultado[Resultado.Count - contadorAnchura - 1]);
                contadorAnchura--;
            }
            if ((contadorAnchura == -1) && (bandera))
            {
                TMRRelojAnchuraPrioritaria.Enabled = false;
                MessageBox.Show("Otra vez desordenado");
            }
        }

        #region eventos_labels
        private void LBL00_Click(object sender, EventArgs e)
        {
            if (LBL10.Text == "0")
            {
                LBL10.Text = LBL00.Text;
                LBL00.Text = "0";
            }
            else if (LBL01.Text == "0")
            {
                LBL01.Text = LBL00.Text;
                LBL00.Text = "0";
            }
        }

        private void LBL01_Click(object sender, EventArgs e)
        {
            if (LBL00.Text == "0")
            {
                LBL00.Text = LBL01.Text;
                LBL01.Text = "0";
            }
            else if (LBL11.Text == "0")
            {
                LBL11.Text = LBL01.Text;
                LBL01.Text = "0";
            }
            else if (LBL02.Text == "0")
            {
                LBL02.Text = LBL01.Text;
                LBL01.Text = "0";
            }
        }

        private void LBL02_Click(object sender, EventArgs e)
        {
            if (LBL01.Text == "0")
            {
                LBL01.Text = LBL02.Text;
                LBL02.Text = "0";
            }
            else if (LBL12.Text == "0")
            {
                LBL12.Text = LBL02.Text;
                LBL02.Text = "0";
            }
            else if (LBL03.Text == "0")
            {
                LBL03.Text = LBL02.Text;
                LBL02.Text = "0";
            }
        }

        private void LBL03_Click(object sender, EventArgs e)
        {
            if (LBL02.Text == "0")
            {
                LBL02.Text = LBL03.Text;
                LBL03.Text = "0";
            }
            else if (LBL13.Text == "0")
            {
                LBL13.Text = LBL03.Text;
                LBL03.Text = "0";
            }
        }


        private void LBL10_Click(object sender, EventArgs e)
        {
            if (LBL00.Text == "0")
            {
                LBL00.Text = LBL10.Text;
                LBL10.Text = "0";
            }
            else if (LBL11.Text == "0")
            {
                LBL11.Text = LBL10.Text;
                LBL10.Text = "0";
            }
            else if (LBL20.Text == "0")
            {
                LBL20.Text = LBL10.Text;
                LBL10.Text = "0";
            }
        }

        private void LBL11_Click(object sender, EventArgs e)
        {
            if (LBL01.Text == "0")
            {
                LBL01.Text = LBL11.Text;
                LBL11.Text = "0";
            }
            else if (LBL10.Text == "0")
            {
                LBL10.Text = LBL11.Text;
                LBL11.Text = "0";
            }
            else if (LBL12.Text == "0")
            {
                LBL12.Text = LBL11.Text;
                LBL11.Text = "0";
            }
            else if (LBL21.Text == "0")
            {
                LBL21.Text = LBL11.Text;
                LBL11.Text = "0";
            }
        }

        private void LBL12_Click(object sender, EventArgs e)
        {
            if (LBL02.Text == "0")
            {
                LBL02.Text = LBL12.Text;
                LBL12.Text = "0";
            }
            else if (LBL11.Text == "0")
            {
                LBL11.Text = LBL12.Text;
                LBL12.Text = "0";
            }
            else if (LBL13.Text == "0")
            {
                LBL13.Text = LBL12.Text;
                LBL12.Text = "0";
            }
            else if (LBL22.Text == "0")
            {
                LBL22.Text = LBL12.Text;
                LBL12.Text = "0";
            }
        }

        private void LBL13_Click(object sender, EventArgs e)
        {
            if (LBL03.Text == "0")
            {
                LBL03.Text = LBL13.Text;
                LBL13.Text = "0";
            }
            else if (LBL12.Text == "0")
            {
                LBL12.Text = LBL13.Text;
                LBL13.Text = "0";
            }
            else if (LBL23.Text == "0")
            {
                LBL23.Text = LBL13.Text;
                LBL13.Text = "0";
            }
        }


        private void LBL20_Click(object sender, EventArgs e)
        {
            if (LBL10.Text == "0")
            {
                LBL10.Text = LBL20.Text;
                LBL20.Text = "0";
            }
            else if (LBL21.Text == "0")
            {
                LBL21.Text = LBL20.Text;
                LBL20.Text = "0";
            }
            else if (LBL30.Text == "0")
            {
                LBL30.Text = LBL20.Text;
                LBL20.Text = "0";
            }
        }

        private void LBL21_Click(object sender, EventArgs e)
        {
            if (LBL11.Text == "0")
            {
                LBL11.Text = LBL21.Text;
                LBL21.Text = "0";
            }
            else if (LBL20.Text == "0")
            {
                LBL20.Text = LBL21.Text;
                LBL21.Text = "0";
            }
            else if (LBL22.Text == "0")
            {
                LBL22.Text = LBL21.Text;
                LBL21.Text = "0";
            }
            else if (LBL31.Text == "0")
            {
                LBL31.Text = LBL21.Text;
                LBL21.Text = "0";
            }
        }

        private void LBL22_Click(object sender, EventArgs e)
        {
            if (LBL12.Text == "0")
            {
                LBL12.Text = LBL22.Text;
                LBL22.Text = "0";
            }
            else if (LBL21.Text == "0")
            {
                LBL21.Text = LBL22.Text;
                LBL22.Text = "0";
            }
            else if (LBL23.Text == "0")
            {
                LBL23.Text = LBL22.Text;
                LBL22.Text = "0";
            }
            else if (LBL32.Text == "0")
            {
                LBL32.Text = LBL22.Text;
                LBL22.Text = "0";
            }
        }

        private void LBL23_Click(object sender, EventArgs e)
        {
            if (LBL13.Text == "0")
            {
                LBL13.Text = LBL23.Text;
                LBL23.Text = "0";
            }
            else if (LBL22.Text == "0")
            {
                LBL22.Text = LBL23.Text;
                LBL23.Text = "0";
            }
            else if (LBL33.Text == "0")
            {
                LBL33.Text = LBL23.Text;
                LBL23.Text = "0";
            }
        }


        private void LBL30_Click(object sender, EventArgs e)
        {
            if (LBL20.Text == "0")
            {
                LBL20.Text = LBL30.Text;
                LBL30.Text = "0";
            }
            else if (LBL31.Text == "0")
            {
                LBL31.Text = LBL30.Text;
                LBL30.Text = "0";
            }
        }

        private void LBL31_Click(object sender, EventArgs e)
        {
            if (LBL30.Text == "0")
            {
                LBL30.Text = LBL31.Text;
                LBL31.Text = "0";
            }
            else if (LBL21.Text == "0")
            {
                LBL21.Text = LBL31.Text;
                LBL31.Text = "0";
            }
            else if (LBL32.Text == "0")
            {
                LBL32.Text = LBL31.Text;
                LBL31.Text = "0";
            }
        }

        private void LBL32_Click(object sender, EventArgs e)
        {
            if (LBL31.Text == "0")
            {
                LBL31.Text = LBL32.Text;
                LBL32.Text = "0";
            }
            else if (LBL22.Text == "0")
            {
                LBL22.Text = LBL32.Text;
                LBL32.Text = "0";
            }
            else if (LBL33.Text == "0")
            {
                LBL33.Text = LBL32.Text;
                LBL32.Text = "0";
            }
        }

        private void LBL33_Click(object sender, EventArgs e)
        {
            if (LBL23.Text == "0")
            {
                LBL23.Text = LBL33.Text;
                LBL33.Text = "0";
            }
            else if (LBL32.Text == "0")
            {
                LBL32.Text = LBL33.Text;
                LBL33.Text = "0";
            }
        }
        #endregion
    }
}
