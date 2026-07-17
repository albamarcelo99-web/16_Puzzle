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
    public partial class FRMHijos : Form
    {
        #region Variables

        public List<CLEstado> Hijos = new List<CLEstado>();
        private int apuntador = 0;

        #endregion
        #region Constructor
        public FRMHijos()
        {
            InitializeComponent();
        }
        #endregion

        #region Eventos
        private void FRMHijos_Load(object sender, EventArgs e)
        {
            if (Hijos.Count > 0)
            {
                apuntador = 0;
                TrasladarEstadoATablero(apuntador);
            }
            else
            {
                MessageBox.Show("Sin hijos");
            }
        }
        #endregion
        #region Métodos
        private void TrasladarEstadoATablero(int apuntador)
        {
            LBL00.Text = Hijos[apuntador].tablero[0, 0].ToString();
            LBL01.Text = Hijos[apuntador].tablero[0, 1].ToString();
            LBL02.Text = Hijos[apuntador].tablero[0, 2].ToString();
            LBL03.Text = Hijos[apuntador].tablero[0, 3].ToString();
            LBL10.Text = Hijos[apuntador].tablero[1, 0].ToString();
            LBL11.Text = Hijos[apuntador].tablero[1, 1].ToString();
            LBL12.Text = Hijos[apuntador].tablero[1, 2].ToString();
            LBL13.Text = Hijos[apuntador].tablero[1, 3].ToString();
            LBL20.Text = Hijos[apuntador].tablero[2, 0].ToString();
            LBL21.Text = Hijos[apuntador].tablero[2, 1].ToString();
            LBL22.Text = Hijos[apuntador].tablero[2, 2].ToString();
            LBL23.Text = Hijos[apuntador].tablero[2, 3].ToString();
            LBL30.Text = Hijos[apuntador].tablero[3, 0].ToString();
            LBL31.Text = Hijos[apuntador].tablero[3, 1].ToString();
            LBL32.Text = Hijos[apuntador].tablero[3, 2].ToString();
            LBL33.Text = Hijos[apuntador].tablero[3, 3].ToString();
            LBLHijo.Text = "Hijo " + (apuntador + 1).ToString();
        }
        #endregion

        private void BTNDerecha_Click(object sender, EventArgs e)
        {
            if ((Hijos.Count - 1) > apuntador)
            {
                apuntador++;
                TrasladarEstadoATablero(apuntador);
            }
        }

        private void BTNIzquierda_Click(object sender, EventArgs e)
        {
            if (apuntador > 0)
            {
                apuntador--;
                TrasladarEstadoATablero(apuntador);
            }
        }
    }
}

